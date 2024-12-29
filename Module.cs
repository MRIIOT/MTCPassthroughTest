using MTConnect.Agents;
using MTConnect.Devices;
using MTConnect.Observations;

namespace MTCAgentTest;

public class Module : MTConnectInputAgentModule
{
    public const string ConfigurationTypeId = "datasource"; // This must match the module section in the 'agent.config.yaml' file
    public const string DefaultId = "DataSource Module"; // The ID is mainly just used for logging.
    private Sink _sink;

    public Module(IMTConnectAgentBroker agent, Sink sink) : base(agent)
    {
        _sink = sink;
        Id = DefaultId;
    }
    
    protected override void OnRead()
    {
        foreach (var message in _sink.Inbox)
        {
            var value1 = $"{((List<ObservationValue>)message.Value)[0].Value} TEST TEST";
            
            if (message.Key == "avail")
            {
                ProcessMessage("Device[Name=device1]/Availability[Category=Event]", value1);
            }
            
            if (message.Key == "execution")
            {
                ProcessMessage("Device[Name=device1]/Controller/Path/Execution[Category=Event]", value1);
            }
            
            if (message.Key == "Xload")
            {
                ProcessMessage("Device[Name=device1]/Axes/Linear[Name=X]/Load[Category=Sample]", value1);
            }
        }
        
        _sink.Inbox.Clear();
    }

    private void ProcessMessage(string path, object value)
    {
        var devices = Agent.GetDevices().ToDictionary(o => o.Name, o => (Device)o);

        var (wasModified, device, dataItem) =
        Builder.Build(
            devices,
            path,
            path);

        if (wasModified)
        {
            Agent.AddDevice(device);
        }

        Agent.AddObservation(device.Uuid, dataItem.Id, value);
    }
}