using MTCAgentTest;
using Disruptor.Dsl;
using Source = MTCAgentTest.Source;

var disruptor = new Disruptor<Message>(() => new Message(), 4096);
var source = new Source(disruptor);
var sink = new Sink(disruptor);

disruptor.Start();
sink.Start();
source.Start();

Console.ReadKey();

disruptor.Shutdown();
source.Stop();
sink.Stop();
