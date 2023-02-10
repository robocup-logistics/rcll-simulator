using System.Collections.Generic;
using System.Text.Json;
using LlsfMsgs;
using System.Threading;
using Simulator.Utility;

namespace Simulator.MPS
{
    public class MPS_CS : Mps
    {
        public CapElement? StoredCap { get; private set; }
        public enum BaseSpecificActions
        {
            Reset = 300,
            Cap = 301,
            BandOnUntil = 302
        }
        public MPS_CS(Configurations config, string name, int port, int id, Team team, bool debug = false) : base(config, name, port, id, team, debug)
        {
            Type = MpsType.CapStation;
            StoredCap = null;
        }
        public new void Run()
        {
            var BasicThread = new Thread(base.HandleBasicTasks);
            BasicThread.Start();
            Work();
        }
        private void Work()
        {
            SerializeMachineToJson();
            while (true)
            {
                InEvent.WaitOne();
                InEvent.Reset();
                GotConnection = true;
                //HandleBasicTasks();
                switch (InNodes.ActionId.Value)
                {
                    case (ushort)BaseSpecificActions.Reset:
                        ResetMachine();
                        break;
                    case (ushort)BaseSpecificActions.Cap:
                        CapTask();
                        break;
                    case (ushort)BaseSpecificActions.BandOnUntil:
                        HandleBelt();
                        break;
                    default:
                        MyLogger.Log("In Action ID = " + InNodes.ActionId.Value);
                        break;

                }
                MyLogger.Log("enable = [" + InNodes.StatusNodes.enable.Value + "] ready = [" + InNodes.StatusNodes.ready.Value + "] busy = [" + InNodes.StatusNodes.busy.Value + "] error = [" + InNodes.StatusNodes.error.Value + "]");
                TaskDescription = "Idle";
            }
        }

        public void CapTask()
        {
            MyLogger.Log("Got a Cap Task!");
            StartTask();
            switch (InNodes.Data0.Value)
            {
                case (ushort)CSOp.RetrieveCap:
                    {
                        TaskDescription = "Cap Retrieve";
                        MyLogger.Log("Got a Retrieve CAP task!");
                        /*for(var count = 0; count  < 45 && ProductOnBelt == null; count++)
                        {
                            Thread.Sleep(1000);
                        }*/
                        if (ProductOnBelt == null)
                        {
                            MyLogger.Log("Can't retrieve the CAP as there is no product!");
                            InNodes.StatusNodes.error.Value = true;
                            Refbox.ApplyChanges(InNodes.StatusNodes.error);
                        }
                        else
                        {
                            TaskDescription = "Retrieving Cap";
                            Thread.Sleep(Config.CSTaskDuration);
                            StoredCap = ProductOnBelt.RetrieveCap();
                        }
                        break;
                    }
                case (ushort)CSOp.MountCap:
                    {
                        TaskDescription = "Cap Mount";
                        MyLogger.Log("Got a Mount Cap TASK!");
                        /*for(var count = 0; count  < 45 && ProductOnBelt == null; count++)
                        {
                            Thread.Sleep(1000);
                        }*/
                        if (StoredCap != null && ProductOnBelt != null)
                        {
                            TaskDescription = "Mounting Cap";
                            Thread.Sleep(Config.CSTaskDuration);
                            ProductOnBelt.AddPart(StoredCap);
                        }
                        else
                        {
                            MyLogger.Log("Can't retrieve the CAP as there is no product!");
                            InNodes.StatusNodes.error.Value = true;
                            Refbox.ApplyChanges(InNodes.StatusNodes.error);
                        }

                        break;
                    }
            }
            FinishedTask();
        }
        public new void PlaceProduct(string machinePoint, Products? heldProduct)
        {
            MyLogger.Log("Got a PlaceProduct for CapStation!");
            base.PlaceProduct(machinePoint, heldProduct);
        }

        public new Products RemoveProduct(string machinePoint)
        {
            Products returnProduct;
            MyLogger.Log("Someone trys to grabs a Item from!");

            switch (machinePoint.ToLower())
            {
                case "output":
                    MyLogger.Log("my Output!");
                    returnProduct = ProductAtOut;
                    ProductAtOut = null;
                    break;
                case "input":
                    returnProduct = ProductAtIn;
                    ProductAtIn = null;
                    break;
                case "shelf3":
                case "shelf2":
                case "shelf1":
                case "left":
                case "middle":
                case "right":
                    returnProduct = Name.Contains("CS1") ? new Products(CapColor.CapBlack) : new Products(CapColor.CapGrey);
                    break;
                default:
                    MyLogger.Log("Defaulting!?");
                    returnProduct = Name.Contains("CS1") ? new Products(CapColor.CapBlack) : new Products(CapColor.CapGrey);
                    break;
            }
            //if (!Configurations.GetInstance().MockUp)
            {
                InNodes.StatusNodes.ready.Value = false;
                Refbox.ApplyChanges(InNodes.StatusNodes.ready);
            }
            return returnProduct;
        }
        public void SerializeMachineToJson()
        {
            JsonInformation = JsonSerializer.Serialize(this);
            //Console.WriteLine(JsonInformation);
        }
    }
}
