using System.Text;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using Simulator.Utility;

namespace Simulator.MPS;

public class MQTTCommand {
    public enum COMMAND {
        NONE,
        GET_BASE,
        LIGHT,
        CAP_ACTION,
        MOUNT_RING,
        DELIVER,
        STORE,
        RETRIEVE,
        RELOCATE,
        MOVE_CONVEYOR,
        RESET,
    }
    public enum ARG1 {
        NONE,
        RED,
        BLACK,
        SILVER,
        GREEN,
        YELLOW,
        RESET,
        RETRIEVE,
        MOUNT,
        RING0,
        RING1,
        SLOT0,
        SLOT1,
        SLOT2,
        TARGET,
        TO_INPUT,
        TO_OUTPUT
    }
    public enum ARG2{
        NONE,
        ON,
        OFF,
        BLINK,
        IN,
        MID,
        OUT,
        TARGET
    }

    public COMMAND command;
    public ARG1 arg1;
    public ARG2 arg2;
    public uint? arg1_shelf, arg1_slot, arg2_shelf, arg2_slot;

    public MQTTCommand(string command_string) {
        string[] command_parts = command_string.Split(" ");
        command = (COMMAND) Enum.Parse(typeof(COMMAND), command_parts[0]);
        if(command == COMMAND.STORE || command == COMMAND.RETRIEVE || command == COMMAND.RELOCATE){
            arg1 = ARG1.TARGET;
            arg2 = ARG2.NONE;
            var parts = command_parts[1].Split(",");

            if (parts.Length > 0)
                uint.TryParse(parts[0], out uint arg1_shelf);
            if (parts.Length > 1)
                uint.TryParse(parts[1], out uint arg1_slot);

            if(command == COMMAND.RELOCATE && command_parts.Length > 2){
                arg2 = ARG2.TARGET;
                parts = command_parts[2].Split(",");

                if (parts.Length > 0)
                    uint.TryParse(parts[0], out uint arg2_shelf);
                if (parts.Length > 1)
                    uint.TryParse(parts[1], out uint arg2_slot);
            }
            return;
        }

        arg1 = (command_parts.Length > 1) ? (ARG1) Enum.Parse(typeof(ARG1), command_parts[1]) : ARG1.NONE;
        arg2 = (command_parts.Length > 2) ? (ARG2) Enum.Parse(typeof(ARG2), command_parts[2]) : ARG2.NONE;
    }
    public MQTTCommand() {
        command = COMMAND.NONE;
        arg1 = ARG1.NONE;
        arg2 = ARG2.NONE;
    }

    public bool validate() {
        switch(command){
            case(COMMAND.GET_BASE):
                if(arg1 == ARG1.SILVER || arg1 == ARG1.BLACK || arg1 == ARG1.RED)
                    return true;
                else
                    return false;
            case(COMMAND.LIGHT):
                switch(arg1){
                    case ARG1.GREEN:
                    case ARG1.YELLOW:
                    case ARG1.RED:
                        if(arg2 == ARG2.ON || arg2 == ARG2.OFF || arg2 == ARG2.BLINK)
                            return true;
                        else
                            return false;
                    case ARG1.RESET:
                        return true;
                    default:
                        return false;
                }
            case(COMMAND.CAP_ACTION):
                if(arg1 == ARG1.RETRIEVE || arg1 == ARG1.RESET)
                    return true;
                else
                    return false;
            case(COMMAND.MOUNT_RING):
                if(arg1 == ARG1.RING0 || arg1 == ARG1.RING1)
                    return true;
                else
                    return false;
            case(COMMAND.DELIVER):
                if(arg1 == ARG1.SLOT0 || arg1 == ARG1.SLOT1 || arg1 == ARG1.SLOT2)
                    return true;
                else
                    return false;

            case(COMMAND.RETRIEVE):
            case(COMMAND.STORE):
                if(arg1 == ARG1.TARGET && arg1_shelf >= 0 && arg1_shelf <= 5 && arg1_slot >= 0 && arg1_slot <= 7)
                    return true;
                else
                    return false;

            case(COMMAND.RELOCATE):
                if(arg1 == ARG1.TARGET && arg2 == ARG2.TARGET &&
                    arg1_shelf >= 0 && arg1_shelf <= 5 && arg1_slot >= 0 && arg1_slot <= 7 &&
                    arg2_shelf >= 0 && arg2_shelf <= 5 && arg2_slot >= 0 && arg2_slot <= 7)
                    return true;
                else
                    return false;
            case(COMMAND.MOVE_CONVEYOR):
                if((arg1 == ARG1.TO_INPUT || arg1 == ARG1.TO_OUTPUT) &&
                   (arg2 == ARG2.IN || arg2 == ARG2.MID || arg2 == ARG2.OUT))
                    return true;
                else
                    return false;
            case(COMMAND.RESET):
                return true;
        }
        return false;
    }
}

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

    public MQTThelper(string name, string url, int port, ManualResetEvent command_event, MyLogger logger, bool slideCount = false) {
        Name = name;
        MyLogger = logger;
        Url = url;
        CommandToppic = $"MPS/{Name}/Command";
        TopicPrefix = $"MPS/{Name}/";
        CommandEvent = command_event;
        command = new MQTTCommand();

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

        if(slideCount)
            ResetSlideCount();
    }

    public Task HandleUpdate(MqttApplicationMessageReceivedEventArgs args) {
        var topic = args.ApplicationMessage.Topic;
        //_myLogger.Log($"Handle Message for topic {topic}");
        string topic_name = topic.Split("/")[^1];;
        string payload = Encoding.UTF8.GetString(args.ApplicationMessage.PayloadSegment);
        if(topic_name == "Command") {
            MyLogger.Log($"Received Command {payload}");
            var m_command = new MQTTCommand(payload);
            if(m_command.validate()){
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

    public void SetStatus(MQTTStatus value, bool publish = true) {
        Status = value;
        if (publish)
            PublishChange("Status", nameof(Status));
    }

    public void SetBarcode(int value, bool publish = true) {
        BarCode = value;
        if (publish)
            PublishChange("Barcode", BarCode.ToString());
    }

    public void ResetSlideCount(bool publish = true) {
        SlideCnt = 0;
        if (publish)
            PublishChange("SlideCount", SlideCnt.ToString());
    }

    public void IncreaseSlideCount(bool publish = true) {
        SlideCnt += 1;
        if(publish)
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
