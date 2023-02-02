using System;
using System.Text.Json;
using System.Threading;
using LlsfMsgs;
using Org.BouncyCastle.Math.EC;
using Simulator.Utility;

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
        public bool GotConnection;
        public bool GotPlaced;
        public Products? ProductOnBelt { get; set; }
        public Products? ProductAtIn { get; set; }
        public Products? ProductAtOut { get; set; }
        public string TaskDescription { get; set; }
        public uint SlideCount { get; set; }
        public string JsonInformation;
        private Configurations Config;
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
        protected Mps(string name, int port, int id, Team team, bool debug = false)
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
            Config = Configurations.GetInstance();
            // Belt = new Belt(this, BeltEvent);
            // Checking whether we have mockup mode or normal mode
            /*if (Configurations.GetInstance().MockUp)
                return;*/
            try
            {
                Refbox = new MPSOPCUAServer(Name, Port, BasicEvent, InEvent, MyLogger);
                InNodes = Refbox.GetNodeCollection(true);
                BasicNodes = Refbox.GetNodeCollection(false);
                var th = new Thread(Refbox.Start);
                Rotation = 0;
                Zone = Zone.MZ41;
                th.Start();
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
            InNodes.StatusNodes.busy.Value = true;
            Refbox.ApplyChanges(InNodes.StatusNodes.busy);
            InNodes.StatusNodes.enable.Value = false;
            Refbox.ApplyChanges(InNodes.StatusNodes.enable);
            Thread.Sleep(1000);
            InNodes.ActionId.Value = 0;
            //Refbox.ApplyChanges(InNodes.ActionId);
            InNodes.Data0.Value = 0;
            //Refbox.ApplyChanges(InNodes.Data0);
            InNodes.Data1.Value = 0;
            //Refbox.ApplyChanges(InNodes.Data1);
            InNodes.ByteError.Value = 0;
            //Refbox.ApplyChanges(InNodes.ByteError);
            InNodes.StatusNodes.ready.Value = false;
            //Refbox.ApplyChanges(InNodes.StatusNodes.ready);
            InNodes.StatusNodes.error.Value = false;
            //Refbox.ApplyChanges(InNodes.StatusNodes.error);
            ProductAtIn = null;
            ProductAtOut = null;
            ProductOnBelt = null;

            InNodes.StatusNodes.busy.Value = false;
            Refbox.ApplyChanges(InNodes.StatusNodes.busy);
            TaskDescription = "Idle";
        }

        public void StartTask()
        {
            InNodes.StatusNodes.busy.Value = true;
            Refbox.ApplyChanges(InNodes.StatusNodes.busy);
        }

        public void FinishedTask()
        {
            int i = 0;
            InNodes.StatusNodes.busy.Value = false;
            Refbox.ApplyChanges(InNodes.StatusNodes.busy);
        }
        public void HandleBasicTasks()
        {
            while(true)
            {
                BasicEvent.WaitOne();
                //MyLogger.Info("We got a write and reset the wait!");
                BasicEvent.Reset();
                switch (BasicNodes.ActionId.Value)
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
            BasicNodes.StatusNodes.busy.Value = true;
            Refbox.ApplyChanges(BasicNodes.StatusNodes.busy);
            var state = BasicNodes.Data0.Value.ToString();
            var time = BasicNodes.Data1.Value; // not in use currently
            var LightState = (LightState)Enum.Parse(typeof(LightState), state);
            switch (BasicNodes.ActionId.Value)
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
            BasicNodes.ActionId.Value = 0;
            Refbox.ApplyChanges(BasicNodes.ActionId);
            BasicNodes.StatusNodes.enable.Value = false;
            Refbox.ApplyChanges(BasicNodes.StatusNodes.enable);
            BasicNodes.StatusNodes.busy.Value = false;
            Refbox.ApplyChanges(BasicNodes.StatusNodes.busy);
        }

        public void HandleBelt()
        {
            MyLogger.Log("Got a Band on Task!");
            TaskDescription = "Move via Belt";
            var target = (Positions)InNodes.Data0.Value;
            var direction = (Direction)InNodes.Data1.Value;
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
            InNodes.StatusNodes.ready.Value = false;
            Refbox.ApplyChanges(InNodes.StatusNodes.ready);
            Thread.Sleep(Configurations.GetInstance().BeltActionDuration);
            MyLogger.Log("Product has reached its destination [" + target + "]!");
            switch (target)
            {
                case Positions.In:
                    ProductAtIn = ProductOnBelt;
                    ProductOnBelt = null;
                    MyLogger.Log("We place the Product onto the InputBeltPosition");
                    if (Config.BarcodeScanner)
                    {
                        InNodes.BarCode.Value = (uint) ProductAtIn.ID;
                        Refbox.ApplyChanges(InNodes.BarCode);  
                    }
                    /*InNodes.StatusNodes.ready.Value = true;
                    Refbox.ApplyChanges(InNodes.StatusNodes.ready);*/
                    break;
                case Positions.Out:
                    ProductAtOut = ProductOnBelt;
                    ProductOnBelt = null;
                    MyLogger.Log("We place the Product onto the OutBeltPosition");
                    InNodes.StatusNodes.ready.Value = true;
                    Refbox.ApplyChanges(InNodes.StatusNodes.ready);
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
                    InNodes.StatusNodes.ready.Value = true;
                    Refbox.ApplyChanges(InNodes.StatusNodes.ready);
                    /*InNodes.StatusNodes.ready.Value = false;
                    Refbox.ApplyChanges(InNodes.StatusNodes.ready);*/
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
                    InNodes.StatusNodes.ready.Value = true;
                    Refbox.ApplyChanges(InNodes.StatusNodes.ready);
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
                InNodes.StatusNodes.ready.Value = false;
                Refbox.ApplyChanges(InNodes.StatusNodes.ready);
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
    }
}