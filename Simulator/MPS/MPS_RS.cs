using LlsfMsgs;
using Simulator.Utility;
using COMMAND = Simulator.MPS.MQTTCommand.COMMAND;
using ARG1 = Simulator.MPS.MQTTCommand.ARG1;

namespace Simulator.MPS {
    public class MPS_RS : Mps {

        public MPS_RS(Configurations config, string name, bool debug = false) : base(config, name, debug, true) {
            Type = MpsType.RingStation;
            MqttHelper.ResetSlideCount();
        }

        protected override void Work() {
            while (Working) {
                CommandEvent.WaitOne();
                CommandEvent.Reset();
                GotConnection = true;

                var command = MqttHelper.command;
                switch (command.command) {
                    case COMMAND.RESET:
                        MqttHelper.ResetSlideCount();
                        ResetMachine();
                        break;
                    case COMMAND.LIGHT:
                        HandleLights(command);
                        break;
                    case COMMAND.MOUNT_RING:
                        MountRingTask(command);
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

        //TODO BOOL REPLACE IT
        public override bool PlaceProduct(string machinePoint, Products heldProduct) {
            MyLogger.Log("Got a PlaceProduct for RingStation!");
            if (machinePoint.ToLower().Equals("slide")) {
                MyLogger.Log("Added a Base to the slide!");
                MqttHelper.IncreaseSlideCount();
                MyLogger.Log("The Current SlideCnt is = " + (MqttHelper.SlideCnt));
                return true;
            }
            return base.PlaceProduct(machinePoint, heldProduct);
        }

        public void MountRingTask(MQTTCommand command) {
            MyLogger.Log("Got a Mount Ring Task!");
            TaskDescription = "Mount Ring Task";
            StartTask();
            for (var count = 0; count < 45 && ProductOnBelt == null; count++) {
                Thread.Sleep(1000);
            }
            if (ProductOnBelt == null) return;
            RingElement ringToMount;
            switch (command.arg1) {
                case ARG1.RING0:
                    //TODO GET COLOR FROM REFBOX
                    ringToMount = Name.Contains("RS1") ? new RingElement(RingColor.RingYellow) : new RingElement(RingColor.RingBlue);
                    break;
                case ARG1.RING1:
                    ringToMount = Name.Contains("RS1") ? new RingElement(RingColor.RingGreen) : new RingElement(RingColor.RingOrange);
                    break;
                default:
                    FinishedTask();
                    return;
            }
            Thread.Sleep(Config.RSTaskDuration);
            ProductOnBelt.AddPart(ringToMount);
            MyLogger.Log("Ring Mounted!");
            FinishedTask();
        }
    }
}
