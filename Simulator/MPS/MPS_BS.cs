using LlsfMsgs;
using Simulator.Utility;
using COMMAND = Simulator.MPS.MQTTCommand.COMMAND;
using MQTTStatus = Simulator.MPS.MQTThelper.MQTTStatus;
using ARG1 = Simulator.MPS.MQTTCommand.ARG1;

namespace Simulator.MPS {
    public class MPS_BS : Mps {
        public MPS_BS(Configurations config, string name, bool debug = false) : base(config, name, debug) {
            Type = MpsType.BaseStation;
        }
        public void DispenseBase(MQTTCommand command) {
            MyLogger.Log("Got a GetBase Task!");
            TaskDescription = "Dispensing a Base";
            StartTask();
            Thread.Sleep(Config.BSTaskDuration);
            string name = Enum.GetName(typeof(ARG1), command.arg1) ?? "";
            MyLogger.Log("Placed a Base from stock " + name + " on the belt");
            switch (command.arg1) {
                case ARG1.RED:
                    ProductOnBelt = new Products(BaseColor.BaseRed);
                    break;
                case ARG1.SILVER:
                    ProductOnBelt = new Products(BaseColor.BaseSilver);
                    break;
                case ARG1.BLACK:
                    ProductOnBelt = new Products(BaseColor.BaseBlack);
                    break;
                default:
                    MyLogger.Log("Unknown Stock to get base from!");
                    break;
            }

            FinishedTask();
        }

        protected override void Work() {
            while (Working) {
                CommandEvent.WaitOne();
                CommandEvent.Reset();
                GotConnection = true;

                var command = MqttHelper.command;
                switch (command.command) {
                    case COMMAND.RESET:
                        ResetMachine();
                        break;
                    case COMMAND.LIGHT:
                        HandleLights(command);
                        break;
                    case COMMAND.GET_BASE:
                        DispenseBase(command);
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
    }
}
