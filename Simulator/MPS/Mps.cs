using LlsfMsgs;
using Simulator.Utility;
using MQTTStatus = Simulator.MPS.MQTThelper.MQTTStatus;
using ARG1 = Simulator.MPS.MQTTCommand.ARG1;
using ARG2 = Simulator.MPS.MQTTCommand.ARG2;

//TODO Compare gameinfo with machine states and error out all missmatches
//TODO TEAM ONLY INTERACTION
//TODO RESET ON SETUP PHASE
namespace Simulator.MPS;

public enum MpsType {
    BaseStation = 100,
    RingStation = 200,
    CapStation = 300,
    DeliveryStation = 400,
    StorageStation = 500
}

public abstract class Mps {
    public readonly MyLogger MyLogger;
    public string Name { get; private set; }
    public MpsType Type;

    public bool HasTag { get; private set; }
    public bool FoundCyan;
    public bool FoundMagenta;
    public Zone Zone { get; set; }
    public uint Rotation { get; set; }
    public Light RedLight { get; }
    public Light GreenLight { get; }
    public Light YellowLight { get; }
    public bool GotPlaced;
    public Products? ProductOnBelt { get; set; }
    public Products? ProductAtIn { get; set; }
    public Products? ProductAtOut { get; set; }
    protected readonly Configurations Config;
    public MQTThelper MqttHelper;
    protected ManualResetEvent CommandEvent;
    public Mutex CommandMutex;
    public Mutex robotAtInput;
    public Mutex robotAtOutput;
    public bool Working { get; private set; }

    protected Mps(Configurations config, string name, bool hasTag, bool slideCount = false) {
        // Constructor for basic member initializations
        Config = config;
        Name = name;
        HasTag = hasTag;

        GotPlaced = false;
        ProductAtOut = null;
        ProductAtIn = null;
        ProductOnBelt = null;
        Rotation = 0;
        Zone = Zone.MZ41;
        Working = true;
        robotAtInput = new Mutex();
        robotAtOutput = new Mutex();

        MyLogger = new MyLogger(Name);
        MyLogger.Info("Starting Machine");

        RedLight = new Light(LightColor.Red);
        YellowLight = new Light(LightColor.Yellow);
        GreenLight = new Light(LightColor.Green);

        CommandEvent = new ManualResetEvent(false);
        CommandMutex = new Mutex();

        try {
            MqttHelper = new MQTThelper(Name, config.Refbox.BrokerIp, config.Refbox.BrokerPort,
                                        config, CommandEvent, CommandMutex, MyLogger, slideCount);
        }
        catch (Exception e) {
            Console.WriteLine(e);
            throw new Exception("Could not connect to MQTT Broker!");
            //TODO add recovery
        }
    }

    protected abstract void Work();
    public void Run() {
        Work();
    }

    public virtual void ResetMachine() {
        MqttHelper.SetStatus(MQTTStatus.BUSY);
        Thread.Sleep(1000);

        // ProductAtIn = null;
        // ProductAtOut = null;
        // ProductOnBelt = null;
        MqttHelper.SetStatus(MQTTStatus.READY);
    }

    public void StartTask() {
        MqttHelper.SetStatus(MQTTStatus.BUSY);
    }

    public void FinishedTask() {
        Thread.Sleep(250);
        MqttHelper.SetStatus(MQTTStatus.READY);
        Thread.Sleep(250);
    }

    public void HandleLights(MQTTCommand command) {
        StartTask();

        string name = Enum.GetName(typeof(ARG2), command.arg2) ?? "";
        switch (command.arg1) {
            case ARG1.RESET:
                MyLogger.Debug("Handle Lights got a ResetLights task!");
                RedLight.SetLight(LightState.Off);
                YellowLight.SetLight(LightState.Off);
                GreenLight.SetLight(LightState.Off);
                break;
            case ARG1.RED:
                if (command.arg2 == ARG2.ON)
                    RedLight.SetLight(LightState.On);
                else if (command.arg2 == ARG2.OFF)
                    RedLight.SetLight(LightState.Off);
                else if (command.arg2 == ARG2.BLINK)
                    RedLight.SetLight(LightState.Blink);
                MyLogger.Debug("Handle Lights got a RedLight task with [" + name + "]!");
                break;
            case ARG1.YELLOW:
                if (command.arg2 == ARG2.ON)
                    YellowLight.SetLight(LightState.On);
                else if (command.arg2 == ARG2.OFF)
                    YellowLight.SetLight(LightState.Off);
                else if (command.arg2 == ARG2.BLINK)
                    YellowLight.SetLight(LightState.Blink);
                MyLogger.Debug("Handle Lights got a YellowLight task with [" + name + "]!");
                break;
            case ARG1.GREEN:
                if (command.arg2 == ARG2.ON)
                    GreenLight.SetLight(LightState.On);
                else if (command.arg2 == ARG2.OFF)
                    GreenLight.SetLight(LightState.Off);
                else if (command.arg2 == ARG2.BLINK)
                    GreenLight.SetLight(LightState.Blink);
                MyLogger.Debug("Handle Lights got a GreenLight task with [" + name + "]!");
                break;
            default:
                break;
        }

        FinishedTask();
    }

    public void HandleBelt(MQTTCommand command) {
        MyLogger.Info("Got a Band on Task!");
        StartTask();
        MyLogger.Debug("Product on belt?");
        for (var counter = 0; counter < 225 && (ProductAtIn == null && ProductAtOut == null && ProductOnBelt == null); counter++) {
            Thread.Sleep(200);
        }
        if (ProductAtIn == null && ProductAtOut == null && ProductOnBelt == null) {
            MyLogger.Warn("Still no Product on the Belt!");
            MqttHelper.SetStatus(MQTTStatus.ERROR);
            return;
        }
        MyLogger.Info("Product on belt!");
        MyLogger.Info("Product is moving on the belt!");
        Thread.Sleep(Config.BeltActionDuration);
        string name = Enum.GetName(typeof(ARG2), command.arg2) ?? "";
        switch (command.arg2) {
            case ARG2.IN:
                ProductAtIn = ProductOnBelt;
                ProductOnBelt = null;
                MyLogger.Info("We place the Product onto the InputBeltPosition");
                if (ProductAtIn != null) {
                    MqttHelper.SetBarcode(ProductAtIn.ID);
                }
                break;
            case ARG2.OUT:
                ProductAtOut = ProductOnBelt;
                ProductOnBelt = null;
                MyLogger.Info("We place the Product onto the OutBeltPosition");
                break;
            case ARG2.MID:
                if (command.arg1 == ARG1.TO_OUTPUT) {
                    ProductOnBelt = ProductAtIn;
                    ProductAtIn = null;
                }
                else {
                    ProductOnBelt = ProductAtOut;
                    ProductAtOut = null;
                }
                MyLogger.Info("We place the Product onto the Middle of the belt");
                break;
        }

        FinishedTask();
    }

    public virtual bool PlaceProduct(string machinePoint, Products heldProduct) {
        switch (machinePoint.ToLower()) {
            case "input":
                if (ProductAtIn != null)
                    return false;
                ProductAtIn = heldProduct;
                return true;
            case "output":
                if (ProductAtOut != null)
                    return false;
                ProductAtOut = heldProduct;
                return true;
            default:
                MyLogger.Warn("Defaulting!?");
                if (ProductAtIn != null)
                    return false;
                ProductAtIn = heldProduct;
                return true;
        }
    }
    public virtual Products? RemoveProduct(string machinePoint, bool dryRun = false) {
        Products? returnProduct;
        switch (machinePoint.ToLower()) {
            case "input":
                returnProduct = ProductAtIn;
                if (!dryRun) {
                    ProductAtIn = null;
                }
                break;
            case "output":
                returnProduct = ProductAtOut;
                if (!dryRun) {
                    ProductAtOut = null;
                }
                break;
            default:
                MyLogger.Warn("Defaulting!?");
                returnProduct = ProductAtIn;
                if (!dryRun) {
                    ProductAtIn = null;
                }
                break;
        }
        return returnProduct;
    }

    public virtual bool EmptyMachinePoint(string machinepoint) {
        MyLogger.Debug("Checking the MachinePoint " + machinepoint);
        switch (machinepoint.ToLower()) {
            case "input":
                return ProductAtIn == null;
            case "output":
                return ProductAtOut == null;
            case "slide":
                return true;
            default:
                return false;
        }
    }
}
