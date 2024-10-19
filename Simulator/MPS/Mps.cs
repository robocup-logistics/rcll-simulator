using LlsfMsgs;
using Simulator.Utility;
using JsonSerializer = System.Text.Json.JsonSerializer;
using MQTTStatus = Simulator.MPS.MQTThelper.MQTTStatus;
using COMMAND = Simulator.MPS.MQTTCommand.COMMAND;
using ARG1 = Simulator.MPS.MQTTCommand.ARG1;
using ARG2 = Simulator.MPS.MQTTCommand.ARG2;

namespace Simulator.MPS {
    public abstract class Mps {
        public enum Positions {
            NoTarget = 0,
            In = 1,
            Mid = 2,
            Out = 3

        }
        public enum Direction {
            FromInToOut = 1,
            FromOutToIn = 2
        }
        public readonly MyLogger MyLogger;
        public string Name { get; private set; }
        public MpsType Type;
        //TODO EXPLORATION
        public ExplorationState ExplorationState;
        public Zone Zone { get; set; }
        public uint Rotation { get; set; }
        public bool Debug;
        public Light RedLight { get; }
        public Light GreenLight { get; }
        public Light YellowLight { get; }
        public bool GotConnection { get; protected set; }
        public bool GotPlaced;
        public Products? ProductOnBelt { get; set; }
        public Products? ProductAtIn { get; set; }
        public Products? ProductAtOut { get; set; }
        public string TaskDescription { get; set; }
        public uint SlideCount { get; set; }
        public string? JsonInformation;
        protected readonly Configurations Config;
        public MQTThelper MqttHelper;
        protected ManualResetEvent CommandEvent = new ManualResetEvent(false);
        public bool Working { get; private set; }
        public enum MpsType {
            BaseStation = 100,
            RingStation = 200,
            CapStation = 300,
            DeliveryStation = 400,
            StorageStation = 500
        }
        public enum Actions : ushort {
            Reset = 0,
            NoJob = 0,
            MachineTyp = 10,
            ResetLights = 20,
            RedLight = 21,
            YellowLight = 22,
            GreenLight = 23,
            RYGLight = 25
        }
        protected Mps(Configurations config, string name, bool debug = false, bool slideCount = false) {
            // Constructor for basic member initializations
            Config = config;
            Name = name;
            Debug = debug;

            TaskDescription = "Idle";
            GotConnection = false;
            GotPlaced = false;
            ProductAtOut = null;
            ProductAtIn = null;
            ProductOnBelt = null;
            Rotation = 0;
            Zone = Zone.MZ41;
            Working = true;

            MyLogger = new MyLogger(Name, Debug);
            MyLogger.Info("Starting Machine");

            RedLight = new Light(LightColor.Red);
            YellowLight = new Light(LightColor.Yellow);
            GreenLight = new Light(LightColor.Green);


            // TODO MOCKUP
            try {
                MqttHelper = new MQTThelper(Name, config.Refbox.BrokerIp, config.Refbox.BrokerPort, CommandEvent, MyLogger, slideCount);
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

        public void ResetMachine() {
            TaskDescription = "Reseting!";
            MqttHelper.SetStatus(MQTTStatus.BUSY);
            Thread.Sleep(1000);

            ProductAtIn = null;
            ProductAtOut = null;
            ProductOnBelt = null;
            MqttHelper.SetStatus(MQTTStatus.READY);
            TaskDescription = "Idle";
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

            switch (command.arg1) {
                case ARG1.RESET:
                    MyLogger.Log("Handle Lights got a ResetLights task!");
                    RedLight.SetLight(LightState.Off);
                    YellowLight.SetLight(LightState.Off);
                    GreenLight.SetLight(LightState.Off);
                    break;
                case ARG1.RED:
                    if(command.arg2 == ARG2.ON)
                        RedLight.SetLight(LightState.On);
                    else if(command.arg2 == ARG2.OFF)
                        RedLight.SetLight(LightState.Off);
                    else if(command.arg2 == ARG2.BLINK)
                        RedLight.SetLight(LightState.Blink);
                    MyLogger.Log("Handle Lights got a RedLight task with [" + nameof(command.arg2) + "]!");
                    break;
                case ARG1.YELLOW:
                    if(command.arg2 == ARG2.ON)
                        YellowLight.SetLight(LightState.On);
                    else if(command.arg2 == ARG2.OFF)
                        YellowLight.SetLight(LightState.Off);
                    else if(command.arg2 == ARG2.BLINK)
                        YellowLight.SetLight(LightState.Blink);
                    MyLogger.Log("Handle Lights got a YellowLight task with [" + nameof(command.arg2) + "]!");
                    break;
                case ARG1.GREEN:
                    if(command.arg2 == ARG2.ON)
                        GreenLight.SetLight(LightState.On);
                    else if(command.arg2 == ARG2.OFF)
                        GreenLight.SetLight(LightState.Off);
                    else if(command.arg2 == ARG2.BLINK)
                        GreenLight.SetLight(LightState.Blink);
                    MyLogger.Log("Handle Lights got a GreenLight task with [" + nameof(command.arg2) + "]!");
                    break;
                default:
                    break;
            }

            FinishedTask();
        }

        public void HandleBelt(MQTTCommand command) {
            MyLogger.Log("Got a Band on Task!");
            TaskDescription = "Move via Belt";
            StartTask();
            MyLogger.Log("Product on belt?");
            for (var counter = 0; counter < 225 && (ProductAtIn == null && ProductAtOut == null && ProductOnBelt == null); counter++) {
                Thread.Sleep(200);
            }
            if (ProductAtIn == null && ProductAtOut == null && ProductOnBelt == null) {
                MyLogger.Log("Still no Product on the Belt!");
                return;
            }
            MyLogger.Log("Product on belt!");
            MyLogger.Log("Product is moving on the belt!");
            Thread.Sleep(Config.BeltActionDuration);
            MyLogger.Log("Product has reached its destination [" + nameof(command.arg2) + "]!");
            switch (command.arg2) {
                case ARG2.IN:
                    ProductAtIn = ProductOnBelt;
                    ProductOnBelt = null;
                    MyLogger.Log("We place the Product onto the InputBeltPosition");
                    if (Config.BarcodeScanner && ProductAtIn != null) {
                        MqttHelper.SetBarcode(ProductAtIn.ID);
                    }
                    break;
                case ARG2.OUT:
                    ProductAtOut = ProductOnBelt;
                    ProductOnBelt = null;
                    MyLogger.Log("We place the Product onto the OutBeltPosition");
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
                    MyLogger.Log("We place the Product onto the Middle of the belt");
                    break;
            }
            //Belt.SetTarget(target, direction);
            FinishedTask();
        }

        public virtual void PlaceProduct(string machinePoint, Products? heldProduct) {
            //MyLogger.Log("Got a PlaceProduct!");
            switch (machinePoint.ToLower()) {
                case "input":
                    ProductAtIn = heldProduct;
                    break;
                case "output":
                    ProductAtOut = heldProduct;
                    break;
                default:
                    MyLogger.Log("Defaulting!?");
                    ProductAtIn = heldProduct;
                    break;
            }
        }
        public virtual Products? RemoveProduct(string machinePoint) {
            Products? returnProduct;
            switch (machinePoint.ToLower()) {
                case "input":
                    returnProduct = ProductAtIn;
                    ProductAtIn = null;
                    break;
                case "output":
                    returnProduct = ProductAtOut;
                    ProductAtOut = null;
                    break;
                default:
                    MyLogger.Log("Defaulting!?");
                    returnProduct = ProductAtIn;
                    ProductAtIn = null;
                    break;
            }
            return returnProduct;
        }
        public bool EmptyMachinePoint(string machinepoint) {
            //MyLogger.Log("Checking the MachinePoint " + machinepoint);
            switch (machinepoint.ToLower()) {
                case "input":
                    return ProductAtIn == null;
                case "output":
                    return ProductAtOut == null;
                case "slide":
                    return true;
                case "shelf1":
                case "shelf2":
                case "shelf3":
                    return false;
                default:
                    return false;
            }
        }
        public void SerializeMachineToJson() {
            JsonInformation = JsonSerializer.Serialize(this);
            //Console.WriteLine(JsonInformation);
        }
    }
}
