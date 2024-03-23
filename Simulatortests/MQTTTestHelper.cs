using System;
using System.Threading;
using Google.Protobuf;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using Org.BouncyCastle.Asn1;
using Simulator.MPS;

namespace Simulatortests;

public class MQTTTestHelper
{
    private string Name;
    private string BasicTopic;
    private string InTopic;
    private string Url;
    private IMqttClient Client;
    private MqttFactory MqttFactory;
    public static readonly string In = "In";
    public static readonly string Basic = "Basic";

    public MQTTTestHelper(string ip, int port, string name)
    {
        Name = name;
        BasicTopic = $"MPS/{Name}/{Basic}/";
        InTopic = $"MPS/{Name}/{In}/";
        Url = ip;
        MqttFactory = new MqttFactory();
        Client = MqttFactory.CreateMqttClient();
    }

    public bool CreateConnection()
    {
        var mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer(Url)
            .WithClientId("MQTTTestHelper")
            .WithWillQualityOfServiceLevel(MqttQualityOfServiceLevel.ExactlyOnce)
            .Build();
        Client.ConnectAsync(mqttClientOptions, CancellationToken.None).GetAwaiter().GetResult();
        return true;
    }

    public void SendTask(ushort TaskId, ushort data0 = 0, ushort data1 = 0)
    {
        PublishChange(InTopic + "ActionId", TaskId);
        PublishChange(InTopic + "Data/Data[0]", data0);
        PublishChange(InTopic + "Data/Data[1]", data1);
        PublishChange(InTopic + "Status/Enable", true);
    }
    public void PublishChange(string topic, int value)
    {
        Console.Write("Topic: " + topic);
        var applicationMessage = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(value.ToString())
            .Build();
        Client.PublishAsync(applicationMessage, CancellationToken.None).GetAwaiter();
    }

    public void PublishChange(string topic, bool value)
    {
        Console.Write("Topic: " + topic);
        var applicationMessage = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(value.ToString())
            .Build();
        Client.PublishAsync(applicationMessage, CancellationToken.None).GetAwaiter();
    }

    public void CloseConnection()
    {
        Client.DisconnectAsync();
    }
}