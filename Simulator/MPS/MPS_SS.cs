using LlsfMsgs; using Simulator.Utility; using COMMAND = Simulator.MPS.MQTTCommand.COMMAND;
using MQTTStatus = Simulator.MPS.MQTThelper.MQTTStatus;

namespace Simulator.MPS;
public class MPS_SS : Mps {public List<List<Products?>> Storage;
    public static readonly int ShelfCount = 6;
    public static readonly int SlotCount = 8;

    public MPS_SS(Configurations config, string name, bool debug = false) : base(config, name, debug) {
        Type = MpsType.StorageStation;
        List<Products?> baseList = Enumerable.Repeat<Products?>(null, SlotCount).ToList();
        Storage = Enumerable.Repeat(baseList, ShelfCount).ToList();
        Storage[0][1] = new Products(BaseColor.BaseRed, CapColor.CapGrey);
        Storage[1][1] = new Products(BaseColor.BaseRed, CapColor.CapBlack);
        Storage[2][1] = new Products(BaseColor.BaseSilver, CapColor.CapGrey);
        Storage[3][1] = new Products(BaseColor.BaseSilver, CapColor.CapBlack);
        Storage[4][1] = new Products(BaseColor.BaseBlack, CapColor.CapGrey);
        Storage[5][1] = new Products(BaseColor.BaseBlack, CapColor.CapBlack);
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
                        ResetMachine();
                        break;
                    case COMMAND.LIGHT:
                        HandleLights(command);
                        break;
                    case COMMAND.MOVE_CONVEYOR:
                        HandleBelt(command);
                        break;
                    case COMMAND.STORE:
                        HandleStore(command);
                        break;
                    case COMMAND.RETRIEVE:
                        HandleRetrieve(command);
                        break;
                    case COMMAND.RELOCATE:
                        HandleRelocate(command);
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

    public void HandleStore(MQTTCommand command) {
        if(command.arg1_shelf == null || command.arg1_slot == null) {
            throw new Exception("Command shelf or slot is null");
        }
        StartTask();
        int shelf = (int)command.arg1_shelf;
        int slot = (int)command.arg1_slot;

        if(Storage[shelf][slot] != null) {
            MyLogger.Log("Not going to store since this slot is already used");
            MqttHelper.SetStatus(MQTTStatus.ERROR);
            return;
        }

        if(ProductOnBelt == null) {
            MyLogger.Log("No Product on Belt");
            MqttHelper.SetStatus(MQTTStatus.ERROR);
            return;
        }

        Thread.Sleep(Config.SSTaskDuration);
        Storage[shelf][slot] = ProductOnBelt;
        ProductOnBelt = null;

        FinishedTask();
    }

    public void HandleRetrieve(MQTTCommand command) {
        if(command.arg1_shelf == null || command.arg1_slot == null) {
            throw new Exception("Command shelf or slot is null");
        }
        StartTask();
        int shelf = (int)command.arg1_shelf;
        int slot = (int)command.arg1_slot;

        if(Storage[shelf][slot] == null) {
            MyLogger.Log("Not going to retrieve since this slot is empty");
            MqttHelper.SetStatus(MQTTStatus.ERROR);
            return;
        }

        if(ProductOnBelt != null) {
            MyLogger.Log("Product already on Belt");
            MqttHelper.SetStatus(MQTTStatus.ERROR);
            return;
        }

        Thread.Sleep(Config.SSTaskDuration);
        ProductOnBelt = Storage[shelf][slot];
        Storage[shelf][slot] = null;

        FinishedTask();
    }

    public void HandleRelocate(MQTTCommand command) {
        if(command.arg1_shelf == null || command.arg1_slot == null ||
           command.arg2_slot == null || command.arg2_shelf == null) {
            throw new Exception("Command shelf or slot is null");
        }
        StartTask();
        int fromShelf = (int)command.arg1_shelf;
        int fromSlot = (int)command.arg1_slot;
        int toShelf = (int)command.arg2_shelf;
        int toSlot = (int)command.arg2_slot;

        if(Storage[fromShelf][fromSlot] == null) {
            MyLogger.Log("Not going to relocate since this slot is empty");
            MqttHelper.SetStatus(MQTTStatus.ERROR);
            return;
        }
        if(Storage[toShelf][toSlot] == null) {
            MyLogger.Log("Not going to relocate since this slot is already used");
            MqttHelper.SetStatus(MQTTStatus.ERROR);
            return;
        }


        Thread.Sleep(Config.SSTaskDuration);
        Storage[toShelf][toSlot] = Storage[fromShelf][fromSlot];
        Storage[fromShelf][fromSlot] = null;

        FinishedTask();
    }
}
