using System.Text;
using System.Collections.Concurrent;
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
        IDLE = 0,
        BUSY = 1,
    }
    public enum MQTTWPSensor {
        NoWP = 0,
        WP = 1,
    }

    public int BarCode { get; private set; }
    public MQTTStatus Status { get; private set; }
    public MQTTWPSensor WPSensor { get; private set; }
    public uint SlideCnt { get; private set; }
    public ConcurrentQueue<MQTTCommand> command { get; private set; }
    private ManualResetEvent CommandEvent;
    private Configurations Config;
    private bool _isConnected = false;
    public bool SlideCount;
    private readonly ConcurrentQueue<(string Topic, string Value)> MessageQueue = new ConcurrentQueue<(string, string)>();

    public MQTThelper(string name, string url, int port, Configurations config,
                      ManualResetEvent commandEvent, MyLogger logger,
                      bool slideCount = false) {
        Name = name;
        MyLogger = logger;
        Url = url;
        CommandToppic = $"MPS/{Name}/Command";
        TopicPrefix = $"MPS/{Name}/";
        CommandEvent = commandEvent;
        SlideCount = slideCount;
        command = new ConcurrentQueue<MQTTCommand>();
        Config = config;

        MqttFactory = new MqttFactory();
        Client = MqttFactory.CreateMqttClient();
        MyLogger.Info("Starting connection!");

        // Set up event handlers
        Client.ConnectedAsync += async e => {
            _isConnected = true;
            MyLogger.Info("Connected!");
            await Subscribe();
            await PublishQueuedMessages();
        };

        Client.DisconnectedAsync += async e => {
            _isConnected = false;
            MyLogger.Warn("Disconnected! Attempting to reconnect...");

            // Retry connection loop
            while (!_isConnected) {
                try {
                    await Task.Delay(5000); // Wait 5 seconds before trying to reconnect
                    await ConnectAsync();
                }
                catch (Exception ex) {
                    MyLogger.Error($"Reconnection attempt failed: {ex.Message}");
                }
            }
        };
    }

    private async Task PublishQueuedMessages() {
        while (MessageQueue.TryDequeue(out var message)) {
            var (topic, value) = message;
            try {
                var applicationMessage = new MqttApplicationMessageBuilder()
                    .WithTopic(topic)
                    .WithPayload(value)
                    .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce)
                    .Build();
                MyLogger.Debug($"Publishing queued message for {topic} with value {value}");
                await Client.PublishAsync(applicationMessage, CancellationToken.None);
            }
            catch (Exception ex) {
                MyLogger.Error($"Failed to publish queued message: {ex.Message}");
                EnqueueMessage(topic, value); // Re-enqueue message if publish fails
                break; // Exit the loop if a message fails to publish
            }
        }
    }

    private void EnqueueMessage(string topic, string value) {
        MessageQueue.Enqueue((topic, value));
    }

    private (string Topic, string Value)? DequeueMessage() {
        if (MessageQueue.TryDequeue(out var message)) {
            return message;
        }
        return null;
    }

    public void Initialize() {
        // Try connecting initially
        Task.Run(() => ConnectAsync());

        // Set up message handler and other initial setup
        Client.ApplicationMessageReceivedAsync += HandleUpdate;

        SetBarcode(0);
        SetStatus(MQTTStatus.IDLE);
        PublishChange("WP-Sensor", "NoWP");

        if (SlideCount) {
            ResetSlideCount();
        }
    }

    public async Task ConnectAsync() {
        var mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer(Url)
            .WithClientId(Name)
            .WithWillQualityOfServiceLevel(MqttQualityOfServiceLevel.ExactlyOnce)
            .Build();

        while (!_isConnected) {
            try {
                await Client.ConnectAsync(mqttClientOptions, CancellationToken.None);
                MyLogger.Info("Connected!");
                _isConnected = true;
            }
            catch (Exception ex) {
                MyLogger.Error($"Connection attempt failed: {ex.Message}");
                await Task.Delay(5000); // Wait 5 seconds before retrying
            }
        }
    }

    public Task HandleUpdate(MqttApplicationMessageReceivedEventArgs args) {
        var topic = args.ApplicationMessage.Topic;
        //_myLogger.Log($"Handle Message for topic {topic}");
        string topic_name = topic.Split("/")[^1];
        string payload = Encoding.UTF8.GetString(args.ApplicationMessage.PayloadSegment);
        if (topic_name == "Command") {
            MyLogger.Debug($"Received Command {payload}");
            var m_command = new MQTTCommand(payload);
            if (m_command.validate()) {
                command.Enqueue(m_command);
                CommandEvent.Set();
            }
            else {
                MyLogger.Error($"Received invalid command {payload}");
            }
        }
        else {
            MyLogger.Debug($"Received unknown topic {topic_name}");
        }

        return Task.CompletedTask;
    }

    public Task Subscribe() {
        var mqttSubscribeOptions = MqttFactory.CreateSubscribeOptionsBuilder()
            .WithTopicFilter(f => {
                f.WithTopic(CommandToppic)
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.ExactlyOnce);
            })
            .Build();

        var response = Client.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None).GetAwaiter().GetResult();
        MyLogger.Info("Created Subscriptions");
        return Task.CompletedTask;
    }

    public void SetStatus(MQTTStatus value) {
        Status = value;
        string name = Enum.GetName(typeof(MQTTStatus), value) ?? "";
        PublishChange("Status", name);
    }

    public void SetWPSensor(MQTTWPSensor value) {
        if (value != WPSensor) {
            WPSensor = value;
            string name = Enum.GetName(typeof(MQTTWPSensor), value) ?? "";
            PublishChange("WP-Sensor", name);
        }
    }

    public void SetBarcode(int? value) {
        if (value == null) {
            return;
        }
        BarCode = (int)value;
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
        try {
            var applicationMessage = new MqttApplicationMessageBuilder()
                .WithTopic(TopicPrefix + topic_name)
                .WithPayload(value)
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.ExactlyOnce)
                .Build();
            MyLogger.Debug($"Publishing {TopicPrefix}{topic_name} to value {value}");
            Client.PublishAsync(applicationMessage, CancellationToken.None).GetAwaiter();
        }
        catch {
            MyLogger.Debug($"Pubslihing failed, enqueuing {topic_name} with value {value}");
            EnqueueMessage(TopicPrefix + topic_name, value);
        }
    }

    public async Task Disconnect() {
        MyLogger.Info("Closing the MQTT client.");

        await Client.DisconnectAsync();
    }
}
