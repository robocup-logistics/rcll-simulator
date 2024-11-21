using LlsfMsgs;
using Simulator.Utility;
using COMMAND = Simulator.MPS.MQTTCommand.COMMAND;
using MQTTStatus = Simulator.MPS.MQTThelper.MQTTStatus;
using ARG1 = Simulator.MPS.MQTTCommand.ARG1;

namespace Simulator.MPS;
public class MPS_CS : Mps {
    public CapElement? StoredCap { get; private set; }
    public CapColor capColor { get; private set; }
    private Products? ShelfLeft;
    private Products? ShelfMiddle;
    private Products? ShelfRight;
    public MPS_CS(Configurations config, string name, Team team, bool hasTag, CapColor? cap = null) : base(config, name, team, hasTag) {
        Type = MpsType.CapStation;
        StoredCap = null;
        if(cap == null) {
            capColor = Name.Contains("CS1") ? CapColor.CapBlack : CapColor.CapGrey;
        } else {
            capColor = (CapColor)cap;
        }
        Replanish();
    }

    public override void HardResetMachine() {
        StoredCap = null;
        Replanish();
        base.HardResetMachine();
    }

    public void Replanish() {
        ShelfLeft = new Products(capColor);
        ShelfMiddle = new Products(capColor);
        ShelfRight = new Products(capColor);
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
                        StoredCap = null;
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
                        MyLogger.Error("Unhandelt ActionType: " + command.command);
                        break;
                }
            }
            finally {
                CommandMutex.ReleaseMutex();
            }
        }
    }

    public void CapTask(MQTTCommand command) {
        MyLogger.Info("Got a Cap Task!");
        StartTask();
        switch (command.arg1) {
            case ARG1.RETRIEVE: {
                    MyLogger.Info("Got a Retrieve CAP task!");
                    if (ProductOnBelt == null || StoredCap != null) {
                        MyLogger.Error("Can't retrieve the CAP as there is no product!");
                        MqttHelper.SetStatus(MQTTStatus.ERROR);
                        return;
                    }
                    else {
                        Thread.Sleep(Config.CSTaskDuration);
                        StoredCap = ProductOnBelt.RetrieveCap();
                    }
                    break;
                }
            case ARG1.MOUNT: {
                    MyLogger.Info("Got a Mount Cap TASK!");
                    if (StoredCap != null && ProductOnBelt != null) {
                        Thread.Sleep(Config.CSTaskDuration);
                        ProductOnBelt.AddPart(StoredCap);
                    }
                    else {
                        MyLogger.Error("Can't retrieve the CAP as there is no product!");
                        MqttHelper.SetStatus(MQTTStatus.ERROR);
                        return;
                    }

                    break;
                }
        }
        FinishedTask();
    }

    public override Products? RemoveProduct(string machinePoint, bool dryRun = false) {
        Products? returnProduct;
        MyLogger.Debug("Someone trys to grabs a Item from: " + machinePoint + dryRun.ToString());

        switch (machinePoint.ToLower()) {
            case "output":
                MyLogger.Debug("my Output: " + ProductAtOut?.ToString());
                returnProduct = ProductAtOut;
                if (!dryRun) {
                    ProductAtOut = null;
                }
                break;
            case "input":
                MyLogger.Debug("my Input: " + ProductAtIn?.ToString());
                returnProduct = ProductAtIn;
                if (!dryRun) {
                    ProductAtIn = null;
                }
                break;
            case "shelf1":
            case "left":
                MyLogger.Debug("my shelf left: : " + ShelfLeft?.ToString());
                returnProduct = ShelfLeft;
                if (!dryRun) {
                    ShelfLeft = null;
                }
                break;
            case "shelf2":
            case "middle":
                MyLogger.Debug("my shelf mid: : " + ShelfMiddle?.ToString());
                returnProduct = ShelfRight;
                if (!dryRun) {
                    ShelfRight = null;
                }
                break;
            case "shelf3":
            case "right":
                MyLogger.Debug("my shelf Right: : " + ShelfRight?.ToString());
                returnProduct = ShelfLeft;
                if (!dryRun) {
                    ShelfLeft = null;
                }
                break;
            default:
                MyLogger.Warn("Defaulting!?");
                returnProduct = null;
                break;
        }
        if (ShelfLeft == null && ShelfMiddle == null && ShelfRight == null) {
            Replanish();
        }
        return returnProduct;
    }

    public override bool EmptyMachinePoint(string machinepoint) {
        MyLogger.Debug("Checking the MachinePoint " + machinepoint);
        switch (machinepoint.ToLower()) {
            case "input":
                return ProductAtIn == null;
            case "output":
                return ProductAtOut == null;
            case "slide":
                return true;
            case "shelf1":
                return ShelfLeft == null;
            case "shelf2":
                return ShelfMiddle == null;
            case "shelf3":
                return ShelfRight == null;
            default:
                return false;
        }
    }
}
