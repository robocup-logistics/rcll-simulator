using LlsfMsgs;
using Simulator.Utility;
using COMMAND = Simulator.MPS.MQTTCommand.COMMAND;
using ARG1 = Simulator.MPS.MQTTCommand.ARG1;

namespace Simulator.MPS;
public class MPS_BS : Mps {
    public MPS_BS(Configurations config, string name, Team team, bool hasTag) : base(config, name, team, hasTag) {
        Type = MpsType.BaseStation;
    }
    public void DispenseBase(MQTTCommand command) {
        MyLogger.Info("Got a GetBase Task!");
        StartTask();
        Thread.Sleep(Config.BSTaskDuration);
        string name = Enum.GetName(typeof(ARG1), command.arg1) ?? "";
        MyLogger.Debug("Placed a Base from stock " + name + " on the belt");
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
                MyLogger.Error("Unknown Stock to get base from!");
                break;
        }

        FinishedTask();
    }

    protected override void Work() {
        while (Working) {
            CommandEvent.WaitOne();
            CommandEvent.Reset();

            if (ProductAtOut != null || ProductAtIn != null) {
                MqttHelper.SetWPSensor(MQTThelper.MQTTWPSensor.WP);
            }
            else {
                MqttHelper.SetWPSensor(MQTThelper.MQTTWPSensor.NoWP);
            }

            MQTTCommand? command;
            while (MqttHelper.command.TryDequeue(out command)) {
                switch (command.command) {
                    case COMMAND.RESET:
                        break;
                    case COMMAND.LIGHT:
                        HandleLights(command);
                        break;
                    case COMMAND.GET_BASE:
                        DispenseBase(command);
                        break;
                    case COMMAND.MOVE_CONVEYOR:
                        HandleBelt(command);
                        if (ProductAtOut != null || ProductAtIn != null) {
                            MqttHelper.SetWPSensor(MQTThelper.MQTTWPSensor.WP);
                        }
                        else {
                            MqttHelper.SetWPSensor(MQTThelper.MQTTWPSensor.NoWP);
                        }
                        break;
                    default:
                        MyLogger.Error("Unhandelt ActionType: " + command.command);
                        break;
                }
            }
        }
    }
}
