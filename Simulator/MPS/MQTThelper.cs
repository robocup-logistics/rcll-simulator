using System.Reflection.Metadata;
using System.Text;
using MQTTnet;
using MQTTnet.Client;
using Org.BouncyCastle.Crypto.Prng;
using static System.String;

namespace Simulator.MPS;

public class MQTThelper
{
    public enum bits
    {
        Busy,
        Ready,
        Error,
        Enable,
        unused0,
        unused1,
        inSensor,
        outSensor
    }

    public static readonly string[] proto =
        { "ActionId", "BarCode", "Data/Data[0]", "Data/Data[1]", "Error", "SlideCnt", "Status" };

    public static readonly string In = "In";
    public static readonly string Basic = "Basic";
    private string BasicTopic;
    private IMqttClient Client;
    private string InTopic;
    private MqttFactory MqttFactory;
    private string Name;
    private string Url;
    public MqttNodeVariables InNodes;
    public MqttNodeVariables BasicNodes;
    private ManualResetEvent InEvent;
    private ManualResetEvent BasicEvent;
    public MQTThelper(string name, string url, ManualResetEvent inevent, ManualResetEvent basicevent)
    {
        Name = name;
        BasicTopic = $"MPS/{Name}/{Basic}/";
        InTopic = $"MPS/{Name}/{In}/";
        Url = url;
        MqttFactory = new MqttFactory();
        InEvent = inevent;
        BasicEvent = basicevent;
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
        Console.WriteLine("Connected!");
    }

    public void Setup()
    {
        Client.ApplicationMessageReceivedAsync += HandleUpdate;
        InNodes = new MqttNodeVariables(InTopic, Client, MqttFactory, InEvent);
        BasicNodes = new MqttNodeVariables(BasicTopic, Client, MqttFactory, BasicEvent);
        Subscribe();
    }

    public Task HandleUpdate(MqttApplicationMessageReceivedEventArgs args)
    {
        var topic = args.ApplicationMessage.Topic;
        Console.WriteLine($"Handle Message for topic {topic}");
        var parts = topic.Split("/");
        var payload = Encoding.UTF8.GetString(args.ApplicationMessage.PayloadSegment);
        if (parts[2] == In)
        {
            if (topic.Contains("/Data/"))
            {
                parts[3] = parts[3] + "/" + parts[4];
            }
            HandleUpdateTopic(InNodes, parts[3], payload);
            if (parts.Length > 3)
            {
                HandleStatusUpdateTopic(InNodes.Status, parts[4], payload);
            }
        }
        else
        {
            if (topic.Contains("/Data/"))
            {
                parts[3] = parts[3] + "/" + parts[4];
            }
            HandleUpdateTopic(BasicNodes, parts[3], payload);
            if (parts.Length > 3)
            {
                HandleStatusUpdateTopic(BasicNodes.Status, parts[4], payload);
            }
        }
        Console.WriteLine("Finished handling the message");
        return Task.CompletedTask;
    }

    private bool HandleUpdateTopic(MqttNodeVariables nodes, string topic, string payload)
    {
        Console.WriteLine($"HandleUpdateTopic with {topic} {payload}");
        switch (topic)
        {
            case var value when value == proto[0]:
                nodes.SetActionId(int.Parse(payload), false);
                break;
            case var value when value == proto[1]:
                nodes.SetBarCode(int.Parse(payload), false);
                break;
            case var value when value == proto[2]:
                nodes.SetData0(int.Parse(payload), false);
                break;
            case var value when value == proto[3]:
                nodes.SetData1(int.Parse(payload), false);
                break;
            case var value when value == proto[4]:
                nodes.SetError(int.Parse(payload), false);
                break;
            case var value when value == proto[5]:
                nodes.SetSlideCount(int.Parse(payload), false);
                break;
            case var value when value == proto[6]:
                break;
            default:
                Console.WriteLine("Unknown Topic to handle!");
                return false;
        }

        return true;
    }

    private bool HandleStatusUpdateTopic(StatusBits nodes, string topic, string payload)
    {
        switch (topic)
        {
            case var value when value == Enum.GetName(bits.Enable):
                nodes.SetEnable(bool.Parse(payload), false);
                break;
            case var value when value == Enum.GetName(bits.Error):
                nodes.SetError(bool.Parse(payload), false);
                break;
            case var value when value == Enum.GetName(bits.unused0):
                nodes.SetUnused0(bool.Parse(payload), false);
                break;
            case var value when value == Enum.GetName(bits.unused1):
                nodes.SetUnused1(bool.Parse(payload), false);
                break;
            case var value when value == Enum.GetName(bits.inSensor):
                nodes.SetInSensor(bool.Parse(payload), false);
                break;
            case var value when value == Enum.GetName(bits.outSensor):
                nodes.SetOutSensor(bool.Parse(payload), false);
                break;
            case var value when value == Enum.GetName(bits.Busy):
                nodes.SetBusy(bool.Parse(payload), false);
                break;
            case var value when value == Enum.GetName(bits.Ready):
                nodes.SetReady(bool.Parse(payload), false);
                break;
            default:
                Console.WriteLine("Unknown Topic to handle!");
                return false;
        }

        return true;
    }
    public void Subscribe()
    {
        Console.Write("Creating Subscriptions...");
        var mqttSubscribeOptions = MqttFactory.CreateSubscribeOptionsBuilder()
            .WithTopicFilter(
                f => { f.WithTopic($"MPS/{Name}/#"); })
            .Build();
        var response = Client.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None).GetAwaiter().GetResult();
        Console.WriteLine(" Done");
    }

    public async Task Disconnect()
    {
        Console.WriteLine("Closing the MQTT client.");

        Client.DisconnectAsync().GetAwaiter().GetResult();
    }
}

public class StatusBits
{
    //{ "Busy", "Ready", "Error", "Enable", "unused0", "unused1", "inSensor", "outSensor" };
    private bool Busy;
    private bool Enable;
    private bool Error;
    private bool inSensor;
    private bool outSensor;
    private bool Ready;
    private bool unused0;
    private bool unused1;
    private IMqttClient Client;
    private string TopicPrefix;
    private MqttFactory MqttFactory;
    private ManualResetEvent ResetEvent;
    public StatusBits(string topic_prefix, IMqttClient client, MqttFactory mqtt_factory, ManualResetEvent resetEvent)
    {
        Client = client;
        TopicPrefix = topic_prefix;
        MqttFactory = mqtt_factory;
        ResetEvent = resetEvent;
        SetBusy(false);
        SetEnable(false);
        SetError(false);
        SetInSensor(false);
        SetOutSensor(false);
        SetReady(false);
        SetUnused0(false);
        SetUnused1(false);
        Subscribe();
    }

    public void SetBusy(bool value, bool publish = true)
    {
        Busy = value;
        if (publish)
            PublishChange(MQTThelper.bits.Busy, Busy);
    }

    public void SetEnable(bool value, bool publish = true)
    {
        Enable = value;
        if (publish)
            PublishChange(MQTThelper.bits.Enable, Enable);
        if (Enable)
        {
            ResetEvent.Set();
        }
    }

    public void SetError(bool value, bool publish = true)
    {
        Error = value;
        if (publish)
            PublishChange(MQTThelper.bits.Error, Error);
    }

    public void SetInSensor(bool value, bool publish = true)
    {
        inSensor = value;
        if (publish)
            PublishChange(MQTThelper.bits.inSensor, inSensor);
    }

    public void SetOutSensor(bool value, bool publish = true)
    {
        outSensor = value;
        if (publish)
            PublishChange(MQTThelper.bits.outSensor, outSensor);
    }

    public void SetReady(bool value, bool publish = true)
    {
        Ready = value;
        if (publish)
            PublishChange(MQTThelper.bits.Ready, Ready);
    }

    public void SetUnused0(bool value, bool publish = true)
    {
        unused0 = value;
        if (publish)
            PublishChange(MQTThelper.bits.unused0, unused0);
    }

    public void SetUnused1(bool value, bool publish = true)
    {
        unused1 = value;
        if (publish)
            PublishChange(MQTThelper.bits.unused1, unused1);
    }

    public void PublishChange(MQTThelper.bits topicid, bool value)
    {
        var applicationMessage = new MqttApplicationMessageBuilder()
            .WithTopic(TopicPrefix + Enum.GetName(topicid))
            .WithPayload(value.ToString())
            .Build();
        Client.PublishAsync(applicationMessage, CancellationToken.None).GetAwaiter();
    }

    public void Subscribe()
    {
        var mqttSubscribeOptions = MqttFactory.CreateSubscribeOptionsBuilder()
            .WithTopicFilter(
                f => { f.WithTopic(TopicPrefix + Enum.GetName(MQTThelper.bits.Enable)); })
            .Build();
        var response = Client.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None).GetAwaiter().GetResult();
        Console.WriteLine(response);
    }
}

public class MqttNodeVariables
{
    //{ "ActionId", "BarCode", "Data/Data[0]", "Data/Data[1]", "Error", "SlideCnt", "Status" };
    public int ActionId { get; private set; }
    public int BarCode{ get; private set; }
    public int[] Data{ get; private set; }
    public int Error{ get; private set; }
    public int SlideCnt{ get; private set; }
    public StatusBits Status;
    private IMqttClient Client;
    private string TopicPrefix;
    private MqttFactory MqttFactory;
    private ManualResetEvent ResetEvent;
    
    public MqttNodeVariables(string topic_prefix, IMqttClient client, MqttFactory mqtt_factory, ManualResetEvent resetEvent)
    {
        TopicPrefix = topic_prefix;
        Client = client;
        MqttFactory = mqtt_factory;
        ResetEvent = resetEvent;
        Data = new int[2];
        SetActionId(0);
        SetBarCode(0);
        SetData0(0);
        SetData1(0);
        SetError(0);
        SetSlideCount(0);
        Status = new StatusBits(TopicPrefix + MQTThelper.proto[6] + "/", Client, MqttFactory, ResetEvent);
    }

    public void SetActionId(int value, bool publish = true)
    {
        ActionId = value;
        if (publish)
            PublishChange(0, ActionId);
    }

    public void SetBarCode(int value, bool publish = true)
    {
        BarCode = value;
        if (publish)
            PublishChange(1, BarCode);
    }

    public void SetData0(int value, bool publish = true)
    {
        Data[0] = value;
        if (publish)
            PublishChange(2, Data[0]);
    }

    public void SetData1(int value, bool publish = true)
    {
        Data[1] = value;
        if (publish)
            PublishChange(3, Data[1]);
    }

    public void SetError(int value, bool publish = true)
    {
        Error = value;
        if (publish)
            PublishChange(4, Error);
    }

    public void SetSlideCount(int value, bool publish = true)
    {
        SlideCnt = value;
        if (publish)
            PublishChange(5, SlideCnt);
    }

    public void PublishChange(int topicid, int value)
    {
        var applicationMessage = new MqttApplicationMessageBuilder()
            .WithTopic(TopicPrefix + MQTThelper.proto[topicid])
            .WithPayload(value.ToString())
            .Build();
        Client.PublishAsync(applicationMessage, CancellationToken.None).GetAwaiter();
    }
}

public class MqttHelperEntry<T>
{
    public MqttHelperEntry(string name, T value)
    {
        Name = name;
        Value = value;
    }

    public string Name { get; private set; }
    public T Value { get; private set; }
}