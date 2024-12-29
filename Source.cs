using System.Collections.Concurrent;
using MTConnect.Clients;
using Timer = System.Timers.Timer;

namespace MTCAgentTest;

public class Source
{
    private Disruptor.Dsl.Disruptor<Message> _disruptor;
    private ConcurrentBag<KeyValuePair<string, object>> _buffer;
    private MTConnectHttpClient _source;
    private object _lock = new object();
    private Timer _timer;
    private bool _isExecuting;
    
    public Source(Disruptor.Dsl.Disruptor<Message> disruptor)
    {
        _disruptor = disruptor;
        _buffer = new ConcurrentBag<KeyValuePair<string, object>>();
        _source = new MTConnectHttpClient("mtconnect.mazakcorp.com", 5719);
        _timer = new Timer();
        _timer.Elapsed += (_, _) => { Execute(); };
        _timer.Interval = 1000;
        _timer.Enabled = true;
    }

    public void Start()
    {
        _source.ObservationReceived += (s, observation) =>
        {
            lock(_lock)
            {
                _buffer.Add(new KeyValuePair<string, object>(observation.DataItemId, observation.Values.ToList()));
            }
        };

        _source.Start();
    }

    public void Stop()
    {
        _source.Stop();
    }

    private void Execute()
    {
        if (_isExecuting) return;
        _isExecuting = true;

        lock (_lock)
        {
            foreach (var observation in _buffer)
            {
                using (var scope = _disruptor.RingBuffer.PublishEvent())
                {
                    var data = scope.Event();
                    data.Key = observation.Key;
                    data.Value = observation.Value;
                    data.Timestamp = DateTime.Now;
                }
            }
            
            _buffer.Clear();
        }

        _isExecuting = false;
    }
}