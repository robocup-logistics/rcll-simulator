using System;
using System.Threading;
using LlsfMsgs;
using Newtonsoft.Json;
using Org.BouncyCastle.Math.EC;
using Simulator.Utility;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Simulator.MPS
{
    public abstract class Mps
    {
        public readonly MyLogger MyLogger;
        public readonly MPSOPCUAServer Refbox;
        public string Name { get; private set; }
        public int Port { get; private set; }
        public MpsType Type;
        public NodeCollection InNodes;
        public NodeCollection BasicNodes;
        public MachineState MachineState;
        public ExplorationState ExplorationState;
        public MachineSide MachineSide;
        public Zone Zone { get; set; }
        public uint Rotation { get; set; }
        public bool Debug;
        public int InternalId { get; }
        public Team Team;
        public Light RedLight { get; }
        public Light GreenLight { get; }
        public Light YellowLight { get; }
        //public Belt Belt;
        public ManualResetEvent InEvent;
        public ManualResetEvent BasicEvent;
        public ManualResetEvent LightEvent;
        public ManualResetEvent BeltEvent;
        public bool GotConnection { get; protected set; }
        public bool GotPlaced;
        public Products? ProductOnBelt { get; set; }
        public Products? ProductAtIn { get; set; }
        public Products? ProductAtOut { get; set; }
        public string TaskDescription { get; set; }
        public uint SlideCount { get; set; }
        public string JsonInformation;
        protected Configurations Config;
        protected MQTThelper MqttHelper;
        protected bool MQTT;
        public bool Working { get; private set; }
        public enum MpsType
        {
            BaseStation = 100,
            RingStation = 200,
            CapStation = 300,
            DeliveryStation = 400,
            StorageStation = 500
        }
        public enum Actions : ushort
        {
            Reset = 0,
            NoJob = 0,
            MachineTyp = 10,
            ResetLights = 20,
            RedLight = 21,
            YellowLight = 22,
            GreenLight = 23,
            RYGLight = 25
        }
        protected Mps(Configurations config, string name, int port, int id, Team team, bool debug = false)
        {
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
            LightEvent = new ManualResetEvent(false); // block threads till a write event occurs
            BeltEvent = new ManualResetEvent(false); // block threads till a write event occurs
            ProductAtOut = null;
            ProductAtIn = null;
            ProductOnBelt = null;
            // Initializing simulated machine parts and logger
            MyLogger = new MyLogger(Name, Debug);
            MyLogger.Info("Starting Machine");
            RedLight = new Light(LightColor.Red, LightEvent);
            YellowLight = new Light(LightColor.Yellow, LightEvent);
            GreenLight = new Light(LightColor.Green, LightEvent);
            TaskDescription = "idle";
            Config = config;
            MQTT = true;
            // Belt = new Belt(this, BeltEvent);
            // Checking whether we have mockup mode or normal mode
            /*if (Configurations.GetInstance().MockUp)
                return;*/
            try
            {
                if (MQTT)
                {
                    MqttHelper= new MQTThelper(Name, "mosquitto", InEvent, BasicEvent, MyLogger);
                    MqttHelper.Connect();
                    MqttHelper.Setup();
                }
                else
                {
                    Refbox = new MPSOPCUAServer(Name, Port, BasicEvent, InEvent, MyLogger);
                    InNodes = Refbox.GetNodeCollection(true);
                    BasicNodes = Refbox.GetNodeCollection(false);
                    var th = new Thread(Refbox.Start);
                    th.Name = Name + "_OpcServerThread";
                    th.Start();
                }
                Rotation = 0;
                Zone = Zone.MZ41;
                Working = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public void Run()
        {
            MyLogger.Log("BaseClass has no run function!");
        }
        public void ResetMachine()
        {
            TaskDescription = "Reseting!";
            MqttHelper.InNodes.Status.SetBusy(true);
            //InNodes.StatusNodes.busy.Value = true;
            //Refbox.ApplyChanges(InNodes.StatusNodes.busy);
            MqttHelper.InNodes.Status.SetEnable(false);
            //InNodes.StatusNodes.enable.Value = false;
            //Refbox.ApplyChanges(InNodes.StatusNodes.enable);
            Thread.Sleep(1000);
            MqttHelper.InNodes.SetActionId(0);
            //InNodes.ActionId.Value = 0;
            ////Refbox.ApplyChanges(InNodes.ActionId);
            MqttHelper.InNodes.SetData0(0);
            //InNodes.Data0.Value = 0;
            ////Refbox.ApplyChanges(InNodes.Data0);
            MqttHelper.InNodes.SetData1(0);
            //InNodes.Data1.Value = 0;
            ////Refbox.ApplyChanges(InNodes.Data1);
            MqttHelper.InNodes.SetError(0);
            //InNodes.ByteError.Value = 0;
            ////Refbox.ApplyChanges(InNodes.ByteError);
            MqttHelper.InNodes.Status.SetReady(false);
            //InNodes.StatusNodes.ready.Value = false;
            ////Refbox.ApplyChanges(InNodes.StatusNodes.ready);
            MqttHelper.InNodes.Status.SetError(false);
            //InNodes.StatusNodes.error.Value = false;
            ////Refbox.ApplyChanges(InNodes.StatusNodes.error);
            ProductAtIn = null;
            ProductAtOut = null;
            ProductOnBelt = null;
            
            MqttHelper.InNodes.Status.SetBusy(false);
            //InNodes.StatusNodes.busy.Value = false;
            //Refbox.ApplyChanges(InNodes.StatusNodes.busy);
            TaskDescription = "Idle";
        }

        public void StartTask()
        {
            if (MQTT)
            {
                MqttHelper.InNodes.Status.SetBusy(true);
                MqttHelper.InNodes.Status.SetEnable(false);
            }
            else
            {
                InNodes.StatusNodes.busy.Value = true;
                Refbox.ApplyChanges(InNodes.StatusNodes.busy);
            }
            
        }

        public void FinishedTask()
        {
            if (MQTT)
            {
                MqttHelper.InNodes.Status.SetBusy(false);
            }
            else
            {
                InNodes.StatusNodes.busy.Value = false;
                Refbox.ApplyChanges(InNodes.StatusNodes.busy);
            }
        }
        public void HandleMachineType()
        {
            if (MQTT)
            {
                MqttHelper.BasicNodes.Status.SetBusy(true);
                Thread.Sleep(400);
                MqttHelper.BasicNodes.SetData0(0);
                MqttHelper.BasicNodes.SetData1(0);
                MqttHelper.BasicNodes.Status.SetEnable(false);
                MqttHelper.BasicNodes.Status.SetBusy(false);
            }
            else
            {
                MyLogger.Log("MachineTyp!");
                //TaskDescription = "MachineType!";
                BasicNodes.StatusNodes.busy.Value = true;
                //Refbox.ApplyChanges(InNodes.StatusNodes.busy);
                Thread.Sleep(400);
                BasicNodes.Data0.Value = 0;
                BasicNodes.Data1.Value = 0;
                //Refbox.ApplyChanges(InNodes.Data0);
                BasicNodes.StatusNodes.enable.Value = false;
                //Refbox.ApplyChanges(InNodes.StatusNodes.enable);
                //Thread.Sleep(20);
                BasicNodes.StatusNodes.busy.Value = false;
                Refbox.ApplyChanges(BasicNodes.StatusNodes.busy);
            }
        }
        public void HandleBasicTasks()
        {
            while(Working)
            {
                BasicEvent.WaitOne();
                //MyLogger.Info("We got a write and reset the wait!");
                BasicEvent.Reset();
                var actionId = 0;
                if (MQTT)
                {
                    actionId = MqttHelper.BasicNodes.ActionId;
                }
                else
                {
                    actionId = BasicNodes.ActionId.Value;
                }
                switch (actionId)
                {
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
                        MyLogger.Log("Basic Action ID = " + BasicNodes.ActionId.Value);
                        break;
                }
                
            }
            if(RedLight.LightOn && !YellowLight.LightOn && !GreenLight.LightOn)
            {
                TaskDescription = "Broken";
            }
        }
        public void HandleLights()
        {
            if (MQTT)
            {
                MqttHelper.BasicNodes.Status.SetBusy(true);   
            }
            else
            {
                BasicNodes.StatusNodes.busy.Value = true;
                Refbox.ApplyChanges(BasicNodes.StatusNodes.busy);
            }
            var state = MQTT ? MqttHelper.BasicNodes.Data[0].ToString() : BasicNodes.Data0.Value.ToString();
            var time = MQTT ? MqttHelper.BasicNodes.Data[1] : BasicNodes.Data1.Value; // not in use currently
            var LightState = (LightState)Enum.Parse(typeof(LightState), state);
            
            switch (MQTT ? MqttHelper.BasicNodes.ActionId : BasicNodes.ActionId.Value)
            {
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

            if (MQTT)
            {
                MqttHelper.BasicNodes.SetActionId(0);
                MqttHelper.BasicNodes.Status.SetEnable(false);
                MqttHelper.BasicNodes.Status.SetBusy(false);
            }
            else
            {
                BasicNodes.ActionId.Value = 0;
                Refbox.ApplyChanges(BasicNodes.ActionId);
                BasicNodes.StatusNodes.enable.Value = false;
                Refbox.ApplyChanges(BasicNodes.StatusNodes.enable);
                BasicNodes.StatusNodes.busy.Value = false;
                Refbox.ApplyChanges(BasicNodes.StatusNodes.busy);
            }
        }

        public void HandleBelt()
        {
            MyLogger.Log("Got a Band on Task!");
            TaskDescription = "Move via Belt";
            var target = Positions.NoTarget;
            var direction = Direction.FromInToOut;
            if (MQTT)
            {
                target = (Positions)MqttHelper.InNodes.Data[0];
                direction = (Direction)MqttHelper.InNodes.Data[1];
            }
            else
            {
                target = (Positions)InNodes.Data0.Value;
                direction = (Direction)InNodes.Data1.Value;
            }

            StartTask();
            MyLogger.Log("Product on belt?");
            for(var counter = 0; counter < 225 && (ProductAtIn == null && ProductAtOut == null && ProductOnBelt == null); counter++)
            {
                Thread.Sleep(200);
            }
            if (ProductAtIn == null && ProductAtOut == null && ProductOnBelt == null)
            {
                MyLogger.Log("Still no Product on the Belt!");
                return;
            }
            MyLogger.Log("Product on belt!");
            MyLogger.Log("Product is moving on the belt!");
            if (MQTT)
            {
                //TODO removed for a test
                //MqttHelper.InNodes.Status.SetReady(false);
            }
            else
            {
                InNodes.StatusNodes.ready.Value = false;
                Refbox.ApplyChanges(InNodes.StatusNodes.ready);
            }
            Thread.Sleep(Config.BeltActionDuration);
            MyLogger.Log("Product has reached its destination [" + target + "]!");
            switch (target)
            {
                case Positions.In:
                    ProductAtIn = ProductOnBelt;
                    ProductOnBelt = null;
                    MyLogger.Log("We place the Product onto the InputBeltPosition");
                    if (Config.BarcodeScanner)
                    {
                        if (MQTT)
                        {
                            MqttHelper.InNodes.SetBarCode(ProductAtIn.ID);
                        }
                        else
                        {
                            InNodes.BarCode.Value = (uint) ProductAtIn.ID;
                            Refbox.ApplyChanges(InNodes.BarCode);  
                        }

                    }
                    /*InNodes.StatusNodes.ready.Value = true;
                    Refbox.ApplyChanges(InNodes.StatusNodes.ready);*/
                    break;
                case Positions.Out:
                    ProductAtOut = ProductOnBelt;
                    ProductOnBelt = null;
                    MyLogger.Log("We place the Product onto the OutBeltPosition");
                    if (MQTT)
                    {
                        MyLogger.Log("Setting Ready to True!");
                        MqttHelper.InNodes.Status.SetReady(true);
                    }
                    else
                    {
                        InNodes.StatusNodes.ready.Value = true;
                        Refbox.ApplyChanges(InNodes.StatusNodes.ready);
                    }

                    break;
                case Positions.Mid:
                    if (direction == Direction.FromInToOut)
                    {
                        ProductOnBelt = ProductAtIn;
                        ProductAtIn = null;
                    }
                    else
                    {
                        ProductOnBelt = ProductAtOut;
                        ProductAtOut = null;
                    }
                    MyLogger.Log("We place the Product onto the Middle of the belt");
                    if (MQTT)
                    {   
                        //TODO check if this is correct?
                        //MqttHelper.InNodes.Status.SetReady(true);
                    }
                    else
                    {
                        InNodes.StatusNodes.ready.Value = true;
                        Refbox.ApplyChanges(InNodes.StatusNodes.ready);
                        /*InNodes.StatusNodes.ready.Value = false;
                        Refbox.ApplyChanges(InNodes.StatusNodes.ready);*/
                    }

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

        public void PlaceProduct(string machinePoint, Products? heldProduct)
        {
            MyLogger.Log("Got a PlaceProduct!");
            switch (machinePoint.ToLower())
            {
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
                if (ProductAtOut != null)
                {
                    if (MQTT)
                    {
                        MqttHelper.InNodes.Status.SetReady(true);
                    }
                    else
                    {
                        InNodes.StatusNodes.ready.Value = true;
                        Refbox.ApplyChanges(InNodes.StatusNodes.ready);
                    }
                }
            }
        }
        public Products RemoveProduct(string machinePoint)
        {
            Products? returnProduct;
            switch (machinePoint.ToLower())
            {
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
                if (MQTT)
                {
                    MqttHelper.InNodes.Status.SetReady(false);
                }
                else
                {
                    InNodes.StatusNodes.ready.Value = false;
                    Refbox.ApplyChanges(InNodes.StatusNodes.ready);
                }
            }
            return returnProduct;
        }
        public bool EmptyMachinePoint(string machinepoint)
        {
            MyLogger.Log("Checking the MachinePoint " + machinepoint);
            switch (machinepoint.ToLower())
            {
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
        public void SerializeMachineToJson()
        {
            JsonInformation = JsonSerializer.Serialize(this);
            //Console.WriteLine(JsonInformation);
        }

        public void StopMachine()
        {
            Refbox.Stop();
        }
    }
}