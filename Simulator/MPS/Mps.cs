using System;
using System.Threading;
using LlsfMsgs;
using Simulator.Utility;

namespace Simulator.MPS
{
    public abstract class Mps
    {
        public readonly MyLogger MyLogger;
        public readonly MPSOPCUAServer Refbox;
        public readonly string Name;
        public readonly int Port;
        public MpsType Type;
        public NodeCollection InNodes;
        public NodeCollection BasicNodes;
        public MachineState MachineState;
        public ExplorationState ExplorationState;
        public MachineSide MachineSide;
        public Zone Zone;
        public uint Rotation;
        public bool Debug;
        public int InternalId { get; }
        public Team Team;
        public Light RedLight;
        public Light GreenLight;
        public Light YellowLight;
        //public Belt Belt;
        public ManualResetEvent WriteEvent;
        public ManualResetEvent LightEvent;
        public ManualResetEvent BeltEvent;
        public bool GotConnection;
        public bool GotPlaced;
        public Products? ProductOnBelt { get; set; }
        public Products? ProductAtIn { get; set; }
        public Products? ProductAtOut { get; set; }
        public string TaskDescription { get; set; }
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
            WriteEvent = new ManualResetEvent(false); // block threads till a write event occurs
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
            // Belt = new Belt(this, BeltEvent);
            // Checking whether we have mockup mode or normal mode
            /*if (Configurations.GetInstance().MockUp)
                return;*/
            try
            {
                Refbox = new MPSOPCUAServer(Name, Port, WriteEvent, MyLogger);
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
            InNodes.StatusNodes.enable.Value = false;
            Refbox.UpdateChanges(InNodes.StatusNodes.enable);
            InNodes.StatusNodes.busy.Value = true;
            Refbox.UpdateChanges(InNodes.StatusNodes.busy);
            InNodes.ActionId.Value = 0;
            Refbox.UpdateChanges(InNodes.ActionId);
            InNodes.Data0.Value = 0;
            Refbox.UpdateChanges(InNodes.Data0);
            InNodes.Data1.Value = 0;
            Refbox.UpdateChanges(InNodes.Data1);
            InNodes.ByteError.Value = 0;
            Refbox.UpdateChanges(InNodes.ByteError);
            InNodes.StatusNodes.ready.Value = false;
            Refbox.UpdateChanges(InNodes.StatusNodes.ready);
            InNodes.StatusNodes.error.Value = false;
            Refbox.UpdateChanges(InNodes.StatusNodes.error);
            ProductAtIn = null;
            ProductAtOut = null;
            ProductOnBelt = null;
            Thread.Sleep(1000);
            InNodes.StatusNodes.busy.Value = false;
            Refbox.UpdateChanges(InNodes.StatusNodes.busy);
        }

        public void StartOpc(MpsType Station)
        {
            /*InNodes.ActionId.Value = (ushort)Actions.MachineTyp;
            refbox.UpdateChanges(InNodes.ActionId);
            InNodes.Data0.Value = (ushort) ((int)Station / 100);
            refbox.UpdateChanges(InNodes.Data0); */
            /*BasicNodes.ActionId.Value = (ushort)Actions.MachineTyp;
            refbox.UpdateChanges(BasicNodes.ActionId);
            BasicNodes.Data0.Value = (ushort)((int)Station / 100);
            refbox.UpdateChanges(BasicNodes.Data0); */
        }

        public void StartTask()
        {
            InNodes.StatusNodes.busy.Value = true;
            Refbox.UpdateChanges(InNodes.StatusNodes.busy);
            InNodes.StatusNodes.enable.Value = false;
            Refbox.UpdateChanges(InNodes.StatusNodes.enable);
        }

        public void FinishedTask()
        {
            InNodes.ActionId.Value = 0;
            Refbox.UpdateChanges(InNodes.ActionId);
            InNodes.Data0.Value = 0;
            Refbox.UpdateChanges(InNodes.Data0);
            InNodes.Data1.Value = 0;
            Refbox.UpdateChanges(InNodes.Data1);
            InNodes.StatusNodes.busy.Value = false;
            Refbox.UpdateChanges(InNodes.StatusNodes.busy);
        }
        public void HandleBasicTasks()
        {
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
                    TaskDescription = "MachineType!";
                    if (BasicNodes.StatusNodes.enable.Value)
                    {
                        BasicNodes.StatusNodes.busy.Value = true;
                        Refbox.UpdateChanges(InNodes.StatusNodes.busy);
                        BasicNodes.Data0.Value = 0;
                        Refbox.UpdateChanges(InNodes.Data0);
                        BasicNodes.StatusNodes.enable.Value = false;
                        Refbox.UpdateChanges(InNodes.StatusNodes.enable);
                        //Thread.Sleep(20);
                        BasicNodes.StatusNodes.busy.Value = false;
                        Refbox.UpdateChanges(BasicNodes.StatusNodes.busy);
                    }
                    else
                    {
                        MyLogger.Log("Called Handle Machinetype but enable is false");
                    }
                    break;
                default:
                    MyLogger.Log("Basic Action ID = " + BasicNodes.ActionId.Value);
                    break;
            }

        }
        public void HandleLights()
        {
            BasicNodes.StatusNodes.busy.Value = true;
            Refbox.UpdateChanges(BasicNodes.StatusNodes.busy);
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
            Refbox.UpdateChanges(BasicNodes.ActionId);
            BasicNodes.StatusNodes.enable.Value = false;
            Refbox.UpdateChanges(BasicNodes.StatusNodes.enable);
            BasicNodes.StatusNodes.busy.Value = false;
            Refbox.UpdateChanges(BasicNodes.StatusNodes.busy);
        }

        public void HandleBelt()
        {
            MyLogger.Log("Got a Band on Task!");
            TaskDescription = "BandOnUntilTask";
            StartTask();
            var target = (Positions)InNodes.Data0.Value;
            var direction = (Direction)InNodes.Data1.Value;
            MyLogger.Log("Product on belt?");
            for(var counter = 0; counter < 225 && (ProductAtIn == null && ProductAtOut == null && ProductOnBelt == null); counter++)
            {
                Thread.Sleep(200);
            }
            MyLogger.Log("Product on belt!");
            MyLogger.Log("Product is moving on the belt!");
            Thread.Sleep(Configurations.GetInstance().BeltActionDuration);
            MyLogger.Log("Product has reached its destination [" + target + "]!");
            switch (target)
            {
                case Positions.In:
                    ProductAtIn = ProductOnBelt;
                    ProductOnBelt = null;
                    MyLogger.Log("We place the Product onto the InputBeltPosition");
                    /*InNodes.StatusNodes.ready.Value = true;
                    Refbox.UpdateChanges(InNodes.StatusNodes.ready);*/
                    break;
                case Positions.Out:
                    ProductAtOut = ProductOnBelt;
                    ProductOnBelt = null;
                    MyLogger.Log("We place the Product onto the OutBeltPosition");
                    InNodes.StatusNodes.ready.Value = true;
                    Refbox.UpdateChanges(InNodes.StatusNodes.ready);
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
                    /*InNodes.StatusNodes.ready.Value = false;
                    Refbox.UpdateChanges(InNodes.StatusNodes.ready);*/
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
            switch (machinePoint)
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
                    Refbox.UpdateChanges(InNodes.StatusNodes.ready);
                }
            }
        }
        public Products RemoveProduct(string machinePoint)
        {
            Products? returnProduct;
            switch (machinePoint)
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
                Refbox.UpdateChanges(InNodes.StatusNodes.ready);
            }
            return returnProduct;
        }
        public bool EmptyMachinePoint(string machinepoint)
        {
            switch (machinepoint)
            {
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
}