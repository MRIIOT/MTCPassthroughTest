using System.Collections.Concurrent;
using MTConnect.Applications;

namespace MTCAgentTest;

public class Sink
{
    public ConcurrentBag<Message> Inbox { get; set; }
    private Disruptor.Dsl.Disruptor<Message> _disruptor;
    private MTConnectAgentApplication _sink = null;
    private Module _module = null;

    public Sink(Disruptor.Dsl.Disruptor<Message> disruptor)
    {
        Inbox = new ConcurrentBag<Message>();
        _disruptor = disruptor;
        _disruptor.HandleEventsWith(new InboundMessageHandler(this));
        _sink = new MTConnectAgentApplication();
    }
    
    public void Start()
    {
        _sink.Run(["run", "MtConnectAgentSink.yaml"], false);
        _module = new Module(_sink.Agent, this);
        _module.Start();
    }

    public void Stop()
    {
        _module.Stop();
    }
}