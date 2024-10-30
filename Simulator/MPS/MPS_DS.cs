using Simulator.Utility;
using COMMAND = Simulator.MPS.MQTTCommand.COMMAND;
using ARG1 = Simulator.MPS.MQTTCommand.ARG1;
using MQTTStatus = Simulator.MPS.MQTThelper.MQTTStatus;

namespace Simulator.MPS;
public class MPS_DS : Mps {
    private List<Products> Slot1;
    private List<Products> Slot2;
    private List<Products> Slot3;
    public MPS_DS(Configurations config, string name, bool debug = false) : base(config, name, debug) {
        Type = MpsType.DeliveryStation;
        Slot1 = new List<Products>();
        Slot2 = new List<Products>();
        Slot3 = new List<Products>();
    }

    protected override void Work() {
        while (Working) {
            CommandEvent.WaitOne();
            CommandEvent.Reset();

            CommandMutex.WaitOne();

            try {
                var command = MqttHelper.command;
                switch (command.command) {
                    case COMMAND.RESET:
                        Slot1 = new List<Products>();
                        Slot2 = new List<Products>();
                        Slot3 = new List<Products>();
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
            }
            finally {
                CommandMutex.ReleaseMutex();
            }
        }
    }

    public override bool PlaceProduct(string machinePoint, Products heldProduct) {
        //MyLogger.Log("Got a PlaceProduct!");
        switch (machinePoint.ToLower()) {
            case "input":
                if (ProductAtIn != null)
                    return false;
                ProductAtIn = heldProduct;
                return true;
            case "output":
                return false;
            default:
                MyLogger.Log("Defaulting!?");
                if (ProductAtIn != null)
                    return false;
                ProductAtIn = heldProduct;
                return false;
        }
    }

    private void DeliverToSlotTask(MQTTCommand command) {
        MyLogger.Log("DeliverToSlotTask!");
        StartTask();
        for (var count = 0; count < 45 && ProductAtIn == null; count++) {
            Thread.Sleep(1000);
        }

        if (ProductAtIn == null) {
            MqttHelper.SetStatus(MQTTStatus.ERROR);
            return;
        }
        string name = Enum.GetName(typeof(ARG1), command.arg1) ?? "";
        MyLogger.Log("Deliver to slot " + name);
        Thread.Sleep(Config.DSTaskDuration);
        switch (command.arg1) {
            case ARG1.SLOT1:
                Slot1.Add(ProductAtIn);
                break;
            case ARG1.SLOT2:
                Slot2.Add(ProductAtIn);
                break;
            case ARG1.SLOT3:
                Slot3.Add(ProductAtIn);
                break;
        }
        ProductAtIn = null;
        FinishedTask();
    }
}
