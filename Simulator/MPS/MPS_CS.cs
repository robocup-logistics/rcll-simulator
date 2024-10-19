using LlsfMsgs;
using Simulator.Utility;
using COMMAND = Simulator.MPS.MQTTCommand.COMMAND;
using MQTTStatus = Simulator.MPS.MQTThelper.MQTTStatus;
using ARG1 = Simulator.MPS.MQTTCommand.ARG1;

namespace Simulator.MPS {
    public class MPS_CS : Mps {
        public CapElement? StoredCap { get; private set; }
        public MPS_CS(Configurations config, string name, bool debug = false) : base(config, name, debug, true) {
            Type = MpsType.CapStation;
            MqttHelper.ResetSlideCount();
            StoredCap = null;
        }

        protected override void Work() {
            SerializeMachineToJson();
            while (Working) {
                CommandEvent.WaitOne();
                CommandEvent.Reset();
                GotConnection = true;

                var command = MqttHelper.command;
                switch (command.command) {
                    case COMMAND.RESET:
                        StoredCap = null;
                        MqttHelper.ResetSlideCount();
                        ResetMachine();
                        break;
                    case COMMAND.LIGHT:
                        HandleLights(command);
                        break;
                    case COMMAND.CAP_ACTION:
                        CapTask(command);
                        break;
                    case COMMAND.MOVE_CONVEYOR:
                        HandleBelt(command);
                        break;
                    default:
                        MyLogger.Log("Unhandelt ActionType: " + command.command);
                        break;

                }
                TaskDescription = "Idle";
            }
        }

        public void CapTask(MQTTCommand command) {
            MyLogger.Log("Got a Cap Task!");
            StartTask();
            switch (command.arg1) {
                case ARG1.RETRIEVE: {
                        TaskDescription = "Cap Retrieve";
                        MyLogger.Log("Got a Retrieve CAP task!");
                        if (ProductOnBelt == null || StoredCap != null) {
                            MyLogger.Log("Can't retrieve the CAP as there is no product!");
                            MqttHelper.SetStatus(MQTTStatus.ERROR);
                        }
                        else {
                            TaskDescription = "Retrieving Cap";
                            Thread.Sleep(Config.CSTaskDuration);
                            StoredCap = ProductOnBelt.RetrieveCap();
                        }
                        break;
                    }
                case ARG1.MOUNT: {
                        TaskDescription = "Cap Mount";
                        MyLogger.Log("Got a Mount Cap TASK!");
                        if (StoredCap != null && ProductOnBelt != null) {
                            TaskDescription = "Mounting Cap";
                            Thread.Sleep(Config.CSTaskDuration);
                            ProductOnBelt.AddPart(StoredCap);
                        }
                        else {
                            MyLogger.Log("Can't retrieve the CAP as there is no product!");
                            MqttHelper.SetStatus(MQTTStatus.ERROR);
                        }

                        break;
                    }
            }
            FinishedTask();
        }

        //TODO SLIDE PAYMENT
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
            return returnProduct;
        }
    }
}
