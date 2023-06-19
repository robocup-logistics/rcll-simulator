using MQTTnet;
using MQTTnet.Client;
using static System.String;

namespace Simulator.MPS;

public class MQTThelper
{
    private string Name;
    private string BasicTopic;
    private string InTopic;
    private string Url;
    private IMqttClient Client;
    private MqttFactory MqttFactory;
    private readonly string[] bits = { "Busy", "Ready", "Error", "Enable", "unused0", "unused1", "inSensor", "outSensor" };
    private readonly string[] proto = { "ActionId", "BarCode", "Data/Data[0]", "Data/Data[1]", "Error", "SlideCnt", "Status" };

    public MQTThelper(string name, string url)
    {
        Name = name;
        BasicTopic = $"MPS/{Name}/Basic/";
        InTopic = $"MPS/{Name}/In/";
        Url = url;
        MqttFactory = new MqttFactory();
        Client = MqttFactory.CreateMqttClient();
    }

    public void Connect()
    {
        Console.WriteLine("Starting connection!");
        var mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer(Url
            )
            .Build();
        Client.ConnectAsync(mqttClientOptions, CancellationToken.None).GetAwaiter().GetResult();
        Console.WriteLine("Connected?");
    }

    public void Setup()
    {
        Console.WriteLine("Starting Setup!?");
        foreach (var registers in proto)
        {
            Publish($"{InTopic}{registers}", "Test");
            Publish($"{BasicTopic}{registers}", "Test");
        }

        foreach (var bit in bits)
        {
            Publish($"{InTopic}Status/{bit}", "Test");
            Publish($"{BasicTopic}Status/{bit}", "Test");
        }
        

    }

    public void Subscribe()
    {
        var mqttSubscribeOptions = MqttFactory.CreateSubscribeOptionsBuilder()
            .WithTopicFilter(
                f =>
                {
                    f.WithTopic(BasicTopic + "/enabled");
                })
            .Build();
        var response = Client.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None).GetAwaiter().GetResult();
        
    }

    public void Publish(string topic, string value)
    {
        Console.WriteLine(string.Format("Publishing {0} on topic {1}", value, topic));
        var applicationMessage = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(value)
            .Build();
        
        Client.PublishAsync(applicationMessage, CancellationToken.None).GetAwaiter();
    }

    public async Task Disconnect()
    {
        Console.WriteLine("Closing the MQTT client.");

        Client.DisconnectAsync().GetAwaiter().GetResult();

    }

}