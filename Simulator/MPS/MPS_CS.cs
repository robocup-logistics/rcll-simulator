using LlsfMsgs;
using Simulator.Utility;
using COMMAND = Simulator.MPS.MQTTCommand.COMMAND;
using MQTTStatus = Simulator.MPS.MQTThelper.MQTTStatus;
using ARG1 = Simulator.MPS.MQTTCommand.ARG1;

namespace Simulator.MPS;
public class MPS_CS : Mps {
    public CapElement? StoredCap { get; private set; }
    private Products? ShelfLeft;
    private Products? ShelfMiddle;
    private Products? ShelfRight;
    public MPS_CS(Configurations config, string name, bool debug = false) : base(config, name, debug) {
        Type = MpsType.CapStation;
        StoredCap = null;
        Replanish();
    }

    //TODO CONFIG
    public void Replanish() {
        ShelfLeft = Name.Contains("CS1") ? new Products(CapColor.CapBlack) : new Products(CapColor.CapGrey);
        ShelfMiddle = Name.Contains("CS1") ? new Products(CapColor.CapBlack) : new Products(CapColor.CapGrey);
        ShelfRight = Name.Contains("CS1") ? new Products(CapColor.CapBlack) : new Products(CapColor.CapGrey);

    }

    protected override void Work() {
        while (Working) {
            CommandEvent.WaitOne();
            CommandEvent.Reset();

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
                    MyLogger.Log("Unhandelt ActionType: " + command.command);
                    break;

            }
        }
    }

    public void CapTask(MQTTCommand command) {
        MyLogger.Log("Got a Cap Task!");
        StartTask();
        switch (command.arg1) {
            case ARG1.RETRIEVE: {
                    MyLogger.Log("Got a Retrieve CAP task!");
                    if (ProductOnBelt == null || StoredCap != null) {
                        MyLogger.Log("Can't retrieve the CAP as there is no product!");
                        MqttHelper.SetStatus(MQTTStatus.ERROR);
                    }
                    else {
                        Thread.Sleep(Config.CSTaskDuration);
                        StoredCap = ProductOnBelt.RetrieveCap();
                    }
                    break;
                }
            case ARG1.MOUNT: {
                    MyLogger.Log("Got a Mount Cap TASK!");
                    if (StoredCap != null && ProductOnBelt != null) {
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

    public override Products? RemoveProduct(string machinePoint, bool dryRun = false) {
        Products? returnProduct;
        MyLogger.Log("Someone trys to grabs a Item from!");

        switch (machinePoint.ToLower()) {
            case "output":
                MyLogger.Log("my Output!");
                returnProduct = ProductAtOut;
                if(!dryRun) {
                    ProductAtOut = null;
                }
                break;
            case "input":
                returnProduct = ProductAtIn;
                if(!dryRun) {
                    ProductAtIn = null;
                }
                break;
            case "shelf1":
            case "left":
                returnProduct = ShelfLeft;
                if(!dryRun) {
                    ShelfLeft = null;
                }
                break;
            case "shelf2":
            case "middle":
                returnProduct = ShelfRight;
                if(!dryRun) {
                    ShelfRight = null;
                }
                break;
            case "shelf3":
            case "right":
                returnProduct = ShelfLeft;
                if(!dryRun) {
                    ShelfLeft = null;
                }
                break;
            default:
                MyLogger.Log("Defaulting!?");
                returnProduct = null;
                break;
        }
        if(ShelfLeft == null && ShelfMiddle == null && ShelfRight == null) {
            Replanish();
        }
        return returnProduct;
    }
}
