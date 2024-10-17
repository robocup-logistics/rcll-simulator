using System.Reflection.Metadata;
using System.Text;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using Org.BouncyCastle.Crypto.Prng;
using Simulator.Utility;
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
    private MyLogger _myLogger;
    
    public MQTThelper(string name, string url, int port, ManualResetEvent inevent, ManualResetEvent basicevent, MyLogger logger)
    {
        Name = name;
        _myLogger = logger;
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
        _myLogger.Log("Starting connection!");
        var mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer(Url)
            .WithClientId(Name)
            .WithWillQualityOfServiceLevel(MqttQualityOfServiceLevel.ExactlyOnce)
            .Build();
        Client.ConnectAsync(mqttClientOptions, CancellationToken.None).GetAwaiter().GetResult();
        _myLogger.Log("Connected!");
    }

    public void Setup()
    {
        Client.ApplicationMessageReceivedAsync += HandleUpdate;
        InNodes = new MqttNodeVariables(InTopic, Client, MqttFactory, InEvent, _myLogger);
        BasicNodes = new MqttNodeVariables(BasicTopic, Client, MqttFactory, BasicEvent, _myLogger);
        Subscribe();
    }

    public Task HandleUpdate(MqttApplicationMessageReceivedEventArgs args)
    {
        var topic = args.ApplicationMessage.Topic;
        //_myLogger.Log($"Handle Message for topic {topic}");
        var parts = topic.Split("/");
        var payload = Encoding.UTF8.GetString(args.ApplicationMessage.PayloadSegment);
        
        if (topic.Contains("/Data/"))
        {
            parts[3] = parts[3] + "/" + parts[4];
            //_myLogger.Log("for data we rebuild the complete topic");
        }
        var isInNodes = (parts[2] == In);
        _myLogger.Log($"HandleUpdateTopic with {parts[2]}/{parts[3]} -> {payload}");
        if (HandleUpdateTopic(isInNodes ? InNodes : BasicNodes, parts[3], payload))
            return Task.CompletedTask;
        if (parts.Length > 3)
        {
            _myLogger.Log($"Handling StatusUpdateTopic for {parts[2]}/{parts[4]} -> {payload}");
            HandleStatusUpdateTopic(isInNodes ? InNodes.Status : BasicNodes.Status, parts[4], payload);
        }
        return Task.CompletedTask;
    }

    private bool HandleUpdateTopic(MqttNodeVariables nodes, string topic, string payload)
    {
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
                _myLogger.Log($"Topic of message is {proto[6]} further handling required");
                return false;
            default:
                _myLogger.Log($"Unknown Topic to handle [{topic}]");
                return false;
        }

        return true;
    }

    private bool HandleStatusUpdateTopic(StatusBits nodes, string topic, string payload)
    {
        var numericValue = 0;
        bool isNumeric = int.TryParse(payload, out numericValue);
        var newValue = false;
        if (isNumeric)
        {
            //_myLogger.Log($"Converting Integer to boolean!");
            newValue = Convert.ToBoolean(numericValue);
        }
        else
        {
            newValue = bool.Parse(payload);
        }
        _myLogger.Log($"Setting {topic} to {newValue}");
        switch (topic)
        {
            case nameof(bits.Enable):
                nodes.SetEnable(newValue, false);
                break;
            case nameof(bits.Error):
                nodes.SetError(newValue, false);
                break;
            case nameof(bits.unused0):
                nodes.SetUnused0(newValue, false);
                break;
            case nameof(bits.unused1):
                nodes.SetUnused1(newValue, false);
                break;
            case nameof(bits.inSensor):
                nodes.SetInSensor(newValue, false);
                break;
            case nameof(bits.outSensor):
                nodes.SetOutSensor(newValue, false);
                break;
            case nameof(bits.Busy):
                nodes.SetBusy(newValue, false);
                break;
            case nameof(bits.Ready):
                nodes.SetReady(newValue, false);
                break;
            default:
                _myLogger.Log("Unknown Topic to handle!");
                return false;
        }
        return true;
    }
    public void Subscribe()
    {
        var mqttSubscribeOptions = MqttFactory.CreateSubscribeOptionsBuilder()

            .WithTopicFilter(f => { f.WithTopic($"MPS/{Name}/Basic/ActionId"); })
            .WithTopicFilter(f => { f.WithTopic($"MPS/{Name}/Basic/Data/Data[0]"); })
            .WithTopicFilter(f => { f.WithTopic($"MPS/{Name}/Basic/Data/Data[1]"); })
            .WithTopicFilter(f => { f.WithTopic($"MPS/{Name}/Basic/Status/Error"); })
            .WithTopicFilter(f => { f.WithTopic($"MPS/{Name}/Basic/Status/Enable"); })
            //.WithTopicFilter(f => { f.WithTopic($"MPS/{Name}/In/BarCode"); })
            //.WithTopicFilter(f => { f.WithTopic($"MPS/{Name}/Basic/#"); })
            //.WithTopicFilter(f => { f.WithTopic($"MPS/{Name}/In/SlideCnt"); })
            .WithTopicFilter(f => { f.WithTopic($"MPS/{Name}/In/ActionId"); })
            .WithTopicFilter(f => { f.WithTopic($"MPS/{Name}/In/Data/Data[0]"); })
            .WithTopicFilter(f => { f.WithTopic($"MPS/{Name}/In/Data/Data[1]"); })
            .WithTopicFilter(f => { f.WithTopic($"MPS/{Name}/In/Status/Error"); })
            .WithTopicFilter(f => { f.WithTopic($"MPS/{Name}/In/Status/Enable"); })
            //.WithTopicFilter(f => { f.WithTopic($"MPS/{Name}/In/BarCode"); })
            //.WithTopicFilter(f => { f.WithTopic($"MPS/{Name}/In/SlideCnt"); })
            //.WithTopicFilter(f => { f.WithTopic($"MPS/{Name}/In/Status/Error"); })
            .Build();
        
        var response = Client.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None).GetAwaiter().GetResult();
        _myLogger.Log("Created Subscriptions");
    }

    public async Task Disconnect()
    {
        _myLogger.Log("Closing the MQTT client.");

        await Client.DisconnectAsync();
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
    private MyLogger _myLogger;
    public StatusBits(string topic_prefix, IMqttClient client, MqttFactory mqtt_factory, ManualResetEvent resetEvent, MyLogger logger)
    {
        Client = client;
        TopicPrefix = topic_prefix;
        MqttFactory = mqtt_factory;
        ResetEvent = resetEvent;
        _myLogger = logger;
        SetBusy(false);
        SetEnable(false);
        SetError(false);
        SetInSensor(false);
        SetOutSensor(false);
        SetReady(false);
        SetUnused0(false);
        SetUnused1(false);
        //Subscribe();
    }

    public bool GetBusy()
    {
        return Busy;
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
        Thread.Sleep(40);
        _myLogger.Log($"Publishing {TopicPrefix}{topicid.ToString()} to value {value}");
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
        _myLogger.Log(response.Items.First().ResultCode.ToString());
    }
}

public class MqttNodeVariables
{
    //{ "ActionId", "BarCode", "Data/Data[0]", "Data/Data[1]", "Error", "SlideCnt", "Status" };
    public int ActionId { get; private set; }
    public int BarCode{ get; private set; }
    public int[] Data{ get; }
    public int Error{ get; private set; }
    public int SlideCnt{ get; private set; }
    public StatusBits Status;
    private IMqttClient Client;
    private string TopicPrefix;
    private MqttFactory MqttFactory;
    private ManualResetEvent ResetEvent;
    private MyLogger _myLogger;
    
    public MqttNodeVariables(string topic_prefix, IMqttClient client, MqttFactory mqtt_factory, ManualResetEvent resetEvent, MyLogger logger)
    {
        TopicPrefix = topic_prefix;
        Client = client;
        MqttFactory = mqtt_factory;
        ResetEvent = resetEvent;
        _myLogger = logger;
        Data = new int[2];
        SetActionId(0);
        SetBarCode(0);
        SetData0(0);
        SetData1(0);
        SetError(0);
        SetSlideCount(0);
        Status = new StatusBits(TopicPrefix + MQTThelper.proto[6] + "/", Client, MqttFactory, ResetEvent, logger);
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
        _myLogger.Log($"Publishing {TopicPrefix}{MQTThelper.proto[topicid]} to value {value}");
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
