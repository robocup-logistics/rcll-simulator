using LlsfMsgs;
using COMMAND = Simulator.MPS.MQTTCommand.COMMAND;

namespace Simulator.MPS {
    public class MPS_SS : Mps {
        public SSOp SSOp;

        public MPS_SS(Configurations config, string name, bool debug = false) : base(config, name, debug) {
            Type = MpsType.StorageStation;
            //if (Configurations.GetInstance().MockUp) return;
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
                    case COMMAND.MOVE_CONVEYOR:
                        HandleBelt(command);
                        break;
                    case COMMAND.STORE:
                        //TODO NOT IMPLEMENTED
                        MyLogger.Log("Got a Store Task! NOT IMPLEMENTED");
                        break;
                    case COMMAND.RETRIEVE:
                        //TODO NOT IMPLEMENTED
                        MyLogger.Log("Got a Store Task! NOT IMPLEMENTED");
                        break;
                    case COMMAND.RELOCATE:
                        //TODO NOT IMPLEMENTED
                        MyLogger.Log("Got a Store Task! NOT IMPLEMENTED");
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
