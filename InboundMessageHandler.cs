namespace MTCAgentTest;

public class InboundMessageHandler : Disruptor.IEventHandler<Message>
{
    private Sink _sink;
    
    public InboundMessageHandler(Sink sink)
    {
        _sink = sink;
    }
    
    public void OnEvent(Message data, long sequence, bool endOfBatch)
    {
        _sink.Inbox.Add(data);
    }
}