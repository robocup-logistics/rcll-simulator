using LlsfMsgs;
using Simulator.Utility;

namespace Simulator.MPS {
    public class MPS_CS : Mps {
        public CapElement? StoredCap { get; private set; }
        public enum BaseSpecificActions {
            Reset = 300,
            Cap = 301,
            BandOnUntil = 302
        }
        public MPS_CS(Configurations config, string name, Team team, bool debug = false) : base(config, name, team, debug) {
            Type = MpsType.CapStation;
            StoredCap = null;
        }

        protected override void Work() {
            SerializeMachineToJson();
            while (Working) {
                InEvent.WaitOne();
                InEvent.Reset();
                GotConnection = true;
                //HandleBasicTasks();
                switch (MqttHelper.InNodes.ActionId) {
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
                        MyLogger.Log("In Action ID = " + (MqttHelper.InNodes.ActionId));
                        break;

                }
                TaskDescription = "Idle";
            }
        }

        public void CapTask() {
            MyLogger.Log("Got a Cap Task!");
            StartTask();
            switch (MqttHelper.InNodes.Data[0]) {
                case (ushort)CSOp.RetrieveCap: {
                        TaskDescription = "Cap Retrieve";
                        MyLogger.Log("Got a Retrieve CAP task!");
                        /*for(var count = 0; count  < 45 && ProductOnBelt == null; count++)
                        {
                            Thread.Sleep(1000);
                        }*/
                        if (ProductOnBelt == null) {
                            MyLogger.Log("Can't retrieve the CAP as there is no product!");
                            MqttHelper.InNodes.Status.SetError(true);
                        }
                        else {
                            TaskDescription = "Retrieving Cap";
                            Thread.Sleep(Config.CSTaskDuration);
                            StoredCap = ProductOnBelt.RetrieveCap();
                        }
                        break;
                    }
                case (ushort)CSOp.MountCap: {
                        TaskDescription = "Cap Mount";
                        MyLogger.Log("Got a Mount Cap TASK!");
                        /*for(var count = 0; count  < 45 && ProductOnBelt == null; count++)
                        {
                            Thread.Sleep(1000);
                        }*/
                        if (StoredCap != null && ProductOnBelt != null) {
                            TaskDescription = "Mounting Cap";
                            Thread.Sleep(Config.CSTaskDuration);
                            ProductOnBelt.AddPart(StoredCap);
                        }
                        else {
                            MyLogger.Log("Can't retrieve the CAP as there is no product!");
                            MqttHelper.InNodes.Status.SetError(true);
                        }

                        break;
                    }
            }
            FinishedTask();
        }
        public override void PlaceProduct(string machinePoint, Products? heldProduct) {
            MyLogger.Log("Got a PlaceProduct for CapStation!");
            base.PlaceProduct(machinePoint, heldProduct);
        }

        public override Products? RemoveProduct(string machinePoint) {
            Products? returnProduct;
            MyLogger.Log("Someone trys to grabs a Item from!");

            switch (machinePoint.ToLower()) {
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
                MqttHelper.InNodes.Status.SetReady(false);
            }
            return returnProduct;
        }
    }
}
