using System.Text;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using Simulator.Utility;

namespace Simulator.MPS;
public class MQTThelper {
    private IMqttClient Client;
    private string CommandToppic;
    private MqttFactory MqttFactory;
    private string Name;
    private string Url;
    private string TopicPrefix;

    private MyLogger MyLogger;
    public enum MQTTStatus {
        READY = 0,
        BUSY = 1,
        ERROR = 2,
        DISABLED = 3
    }

    public int BarCode { get; private set; }
    public MQTTStatus Status { get; private set; }
    public uint SlideCnt { get; private set; }
    public MQTTCommand command { get; private set; }
    private ManualResetEvent CommandEvent;
    private Configurations Config;

    public MQTThelper(string name, string url, int port, Configurations config,
                      ManualResetEvent command_event, MyLogger logger,
                      bool slideCount = false) {
        Name = name;
        MyLogger = logger;
        Url = url;
        CommandToppic = $"MPS/{Name}/Command";
        TopicPrefix = $"MPS/{Name}/";
        CommandEvent = command_event;
        command = new MQTTCommand();
        Config = config;

        MqttFactory = new MqttFactory();
        Client = MqttFactory.CreateMqttClient();
        MyLogger.Log("Starting connection!");
        var mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer(Url)
            .WithClientId(Name)
            .WithWillQualityOfServiceLevel(MqttQualityOfServiceLevel.ExactlyOnce)
            .Build();
        Client.ConnectAsync(mqttClientOptions, CancellationToken.None).GetAwaiter().GetResult();
        MyLogger.Log("Connected!");
        Client.ApplicationMessageReceivedAsync += HandleUpdate;
        Subscribe();

        SetBarcode(0);
        SetStatus(MQTTStatus.READY);

        if (slideCount)
            ResetSlideCount();
    }

    public Task HandleUpdate(MqttApplicationMessageReceivedEventArgs args) {
        var topic = args.ApplicationMessage.Topic;
        //_myLogger.Log($"Handle Message for topic {topic}");
        string topic_name = topic.Split("/")[^1];
        string payload = Encoding.UTF8.GetString(args.ApplicationMessage.PayloadSegment);
        if (topic_name == "Command") {
            MyLogger.Log($"Received Command {payload}");
            var m_command = new MQTTCommand(payload);
            if (m_command.validate()) {
                //FIXME POTENTIALY RACY
                command = m_command;
                CommandEvent.Set();
            }
        }
        else {
            MyLogger.Log($"Received unknown topic {topic_name}");
        }

        return Task.CompletedTask;
    }

    public void Subscribe() {
        var mqttSubscribeOptions = MqttFactory.CreateSubscribeOptionsBuilder()
            .WithTopicFilter(f => { f.WithTopic(CommandToppic); })
            .Build();

        var response = Client.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None).GetAwaiter().GetResult();
        MyLogger.Log("Created Subscriptions");
    }

    public void SetStatus(MQTTStatus value) {
        Status = value;
        string name = Enum.GetName(typeof(MQTTStatus), value) ?? "";
        PublishChange("Status", name);
    }

    public void SetBarcode(int value) {
        BarCode = value;
        if (Config.BarcodeScanner)
            PublishChange("Barcode", BarCode.ToString());
    }

    public void ResetSlideCount() {
        SlideCnt = 0;
        PublishChange("SlideCount", SlideCnt.ToString());
    }

    public void IncreaseSlideCount() {
        SlideCnt += 1;
        PublishChange("SlideCount", SlideCnt.ToString());
    }

    private void PublishChange(string topic_name, string value) {
        var applicationMessage = new MqttApplicationMessageBuilder()
            .WithTopic(TopicPrefix + topic_name)
            .WithPayload(value.ToString())
            .Build();
        MyLogger.Log($"Publishing {TopicPrefix}{topic_name} to value {value}");
        Client.PublishAsync(applicationMessage, CancellationToken.None).GetAwaiter();
    }

    public async Task Disconnect() {
        MyLogger.Log("Closing the MQTT client.");

        await Client.DisconnectAsync();
    }
}
