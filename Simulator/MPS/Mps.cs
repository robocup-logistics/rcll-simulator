using LlsfMsgs;
using Simulator.Utility;
using JsonSerializer = System.Text.Json.JsonSerializer;

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
        public int Port { get; private set; }
        public MpsType Type;
        public MachineState MachineState;
        //TODO EXPLORATION
        public ExplorationState ExplorationState;
        public Zone Zone { get; set; }
        public uint Rotation { get; set; }
        public bool Debug;
        public int InternalId { get; }
        public Team Team;
        public Light RedLight { get; }
        public Light GreenLight { get; }
        public Light YellowLight { get; }
        public ManualResetEvent InEvent;
        public ManualResetEvent BasicEvent;
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
        protected Mps(Configurations config, string name, int port, int id, Team team, bool debug = false) {
            // Constructor for basic member initializations
            Debug = debug;
            MachineState = MachineState.Idle;
            Name = name;
            Port = port;
            InternalId = id;
            Team = team;
            GotConnection = false;
            GotPlaced = false;
            InEvent = new ManualResetEvent(false); // block threads till a write event occurs
            BasicEvent = new ManualResetEvent(false); // block threads till a write event occurs
            ProductAtOut = null;
            ProductAtIn = null;
            ProductOnBelt = null;
            Rotation = 0;
            Zone = Zone.MZ41;
            Working = true;

            // Initializing simulated machine parts and logger
            MyLogger = new MyLogger(Name, Debug);
            MyLogger.Info("Starting Machine");

            RedLight = new Light(LightColor.Red);
            YellowLight = new Light(LightColor.Yellow);
            GreenLight = new Light(LightColor.Green);

            TaskDescription = "idle";
            Config = config;

            // Checking whether we have mockup mode or normal mode
            // TODO MOCKUP
            /*if (Configurations.GetInstance().MockUp)
                return;*/
            try {
                MqttHelper = new MQTThelper(Name, config.Refbox.BrokerIp, config.Refbox.BrokerPort, InEvent, BasicEvent, MyLogger);
                MqttHelper.Connect();
                MqttHelper.Setup();
            }
            catch (Exception e) {
                Console.WriteLine(e);
                throw new Exception("Could not connect to MQTT Broker!");
                //TODO add recovery
            }
        }

        protected abstract void Work();
        public void Run() {
            var BasicThread = new Thread(HandleBasicTasks);
            BasicThread.Start();
            BasicThread.Name = Name + "_HandleBasicThread";
            Work();
        }

        public void ResetMachine() {
            TaskDescription = "Reseting!";
            MqttHelper.InNodes.Status.SetBusy(true);
            MqttHelper.InNodes.Status.SetEnable(false);
            Thread.Sleep(1000);
            MqttHelper.InNodes.SetActionId(0);
            MqttHelper.InNodes.SetData0(0);
            MqttHelper.InNodes.SetData1(0);
            MqttHelper.InNodes.SetError(0);
            MqttHelper.InNodes.Status.SetReady(false);
            MqttHelper.InNodes.Status.SetError(false);


            ProductAtIn = null;
            ProductAtOut = null;
            ProductOnBelt = null;
            MqttHelper.InNodes.Status.SetBusy(false);

            TaskDescription = "Idle";
        }

        public void StartTask() {
            MqttHelper.InNodes.Status.SetBusy(true);
            MqttHelper.InNodes.Status.SetEnable(false);
        }

        public void FinishedTask() {
            Thread.Sleep(250);
            MqttHelper.InNodes.Status.SetBusy(false);
            Thread.Sleep(250);
        }
        public void HandleMachineType() {
            MqttHelper.BasicNodes.Status.SetBusy(true);
            Thread.Sleep(400);
            MqttHelper.BasicNodes.SetData0(0);
            MqttHelper.BasicNodes.SetData1(0);
            MqttHelper.BasicNodes.Status.SetEnable(false);
            MqttHelper.BasicNodes.Status.SetBusy(false);
        }
        public void HandleBasicTasks() {
            while (Working) {
                BasicEvent.WaitOne();
                //MyLogger.Info("We got a write and reset the wait!");
                BasicEvent.Reset();
                GotConnection = true;
                var actionId = 0;
                actionId = MqttHelper.BasicNodes.ActionId;
                switch (actionId) {
                    case (ushort)Actions.RedLight:
                    case (ushort)Actions.YellowLight:
                    case (ushort)Actions.GreenLight:
                    case (ushort)Actions.RYGLight:
                    case (ushort)Actions.ResetLights:
                        HandleLights();
                        break;
                    case (ushort)Actions.NoJob:
                        MyLogger.Log("No Basic Job!");
                        break;
                    case (ushort)Actions.MachineTyp:
                        HandleMachineType();
                        break;
                    default:
                        MyLogger.Log("Basic Action ID = " + (MqttHelper.InNodes.ActionId));
                        break;
                }

            }
            if (RedLight.LightOn && !YellowLight.LightOn && !GreenLight.LightOn) {
                TaskDescription = "Broken";
            }
        }
        public void HandleLights() {
            MqttHelper.BasicNodes.Status.SetBusy(true);
            var state = MqttHelper.BasicNodes.Data[0].ToString();
            var time = MqttHelper.BasicNodes.Data[1]; // not in use currently
            var LightState = (LightState)Enum.Parse(typeof(LightState), state);

            switch (MqttHelper.BasicNodes.ActionId) {
                case (ushort)Actions.ResetLights:
                    MyLogger.Log("Handle Lights got a ResetLights task!");
                    RedLight.SetLight(LightState.Off);
                    YellowLight.SetLight(LightState.Off);
                    GreenLight.SetLight(LightState.Off);
                    break;
                case (ushort)Actions.RedLight:
                    MyLogger.Log("Handle Lights got a RedLight task with [" + LightState.ToString() + "]!");
                    RedLight.SetLight(LightState);
                    break;
                case (ushort)Actions.YellowLight:
                    MyLogger.Log("Handle Lights got a YellowLight task [" + LightState.ToString() + "]!");
                    YellowLight.SetLight(LightState);
                    break;
                case (ushort)Actions.GreenLight:
                    MyLogger.Log("Handle Lights got a GreenLight task [" + LightState.ToString() + "]!");
                    GreenLight.SetLight(LightState);
                    break;
                case (ushort)Actions.RYGLight:
                    MyLogger.Log("Handle Lights got a RYGLight task [" + LightState.ToString() + "]!");
                    RedLight.SetLight(LightState.Off);
                    YellowLight.SetLight(LightState.Off);
                    GreenLight.SetLight(LightState.Off);
                    break;
                default:
                    break;
            }

            MqttHelper.BasicNodes.SetActionId(0);
            MqttHelper.BasicNodes.Status.SetEnable(false);
            MqttHelper.BasicNodes.Status.SetBusy(false);
        }

        public void HandleBelt() {
            MyLogger.Log("Got a Band on Task!");
            TaskDescription = "Move via Belt";
            var target = Positions.NoTarget;
            var direction = Direction.FromInToOut;
            target = (Positions)MqttHelper.InNodes.Data[0];
            direction = (Direction)MqttHelper.InNodes.Data[1];

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
            MqttHelper.InNodes.Status.SetReady(false);
            Thread.Sleep(Config.BeltActionDuration);
            MyLogger.Log("Product has reached its destination [" + target + "]!");
            switch (target) {
                case Positions.In:
                    ProductAtIn = ProductOnBelt;
                    ProductOnBelt = null;
                    MyLogger.Log("We place the Product onto the InputBeltPosition");
                    if (Config.BarcodeScanner && ProductAtIn != null) {
                        MqttHelper.InNodes.SetBarCode(ProductAtIn.ID);
                    }
                    MyLogger.Log("Setting Ready to True!");
                    MqttHelper.InNodes.Status.SetReady(true);
                    break;
                case Positions.Out:
                    ProductAtOut = ProductOnBelt;
                    ProductOnBelt = null;
                    MyLogger.Log("We place the Product onto the OutBeltPosition");
                    MyLogger.Log("Setting Ready to True!");
                    MqttHelper.InNodes.Status.SetReady(true);
                    break;
                case Positions.Mid:
                    if (direction == Direction.FromInToOut) {
                        ProductOnBelt = ProductAtIn;
                        ProductAtIn = null;
                    }
                    else {
                        ProductOnBelt = ProductAtOut;
                        ProductAtOut = null;
                    }
                    MyLogger.Log("We place the Product onto the Middle of the belt");
                    //TODO check if this is correct?
                    MqttHelper.InNodes.Status.SetReady(true);
                    break;
                case Positions.NoTarget:
                    MyLogger.Log("Placing Product on NoTarget?");
                    break;
                default:
                    MyLogger.Log("Default!?");
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
            //if (!Configurations.GetInstance().MockUp)
            {
                if (ProductAtOut != null) {
                    MqttHelper.InNodes.Status.SetReady(true);
                }
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
            //if (!Configurations.GetInstance().MockUp)
            {
                MqttHelper.InNodes.Status.SetReady(false);
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
