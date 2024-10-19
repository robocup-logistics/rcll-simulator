using Simulator.Utility;
using COMMAND = Simulator.MPS.MQTTCommand.COMMAND;
using ARG1 = Simulator.MPS.MQTTCommand.ARG1;

namespace Simulator.MPS {
    public class MPS_DS : Mps {
        // TODO Add some more space to the slots
        private List<Products> Slot0;
        private List<Products> Slot1;
        private List<Products> Slot2;
        public MPS_DS(Configurations config, string name, bool debug = false) : base(config, name, debug) {
            Type = MpsType.DeliveryStation;
            Slot0 = new List<Products>();
            Slot1 = new List<Products>();
            Slot2 = new List<Products>();
        }

        protected override void Work() {
            while (Working) {
                CommandEvent.WaitOne();
                CommandEvent.Reset();
                GotConnection = true;

                var command = MqttHelper.command;
                switch (command.command) {
                    case COMMAND.RESET:
                        Slot0 = new List<Products>();
                        Slot1 = new List<Products>();
                        Slot2 = new List<Products>();
                        ResetMachine();
                        break;
                    case COMMAND.LIGHT:
                        HandleLights(command);
                        break;
                    case COMMAND.DELIVER:
                        DeliverToSlotTask(command);
                        break;
                    default:
                        MyLogger.Log("Unhandelt ActionType: " + command.command);
                        break;

                }
                TaskDescription = "Idle";
            }
        }

        private void DeliverToSlotTask(MQTTCommand command) {
            MyLogger.Log("DeliverToSlotTask!");
            TaskDescription = "Delivering Product";
            StartTask();
            for (var count = 0; count < 45 && ProductAtIn == null; count++) {
                Thread.Sleep(1000);
            }
            if (ProductAtIn == null) return;
            string name = Enum.GetName(typeof(ARG1), command.arg1) ?? "";
            MyLogger.Log("Deliver to slot " + name);
            Thread.Sleep(Config.DSTaskDuration);
            switch (command.arg1) {
                case ARG1.SLOT0:
                    Slot0.Add(ProductAtIn);
                    break;
                case ARG1.SLOT1:
                    Slot1.Add(ProductAtIn);
                    break;
                case ARG1.SLOT2:
                    Slot2.Add(ProductAtIn);
                    break;
            }
            ProductAtIn = null;
            FinishedTask();
        }
    }
}
