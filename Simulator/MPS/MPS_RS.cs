using LlsfMsgs;
using Simulator.Utility;
using COMMAND = Simulator.MPS.MQTTCommand.COMMAND;
using ARG1 = Simulator.MPS.MQTTCommand.ARG1;

namespace Simulator.MPS;
public class MPS_RS : Mps {
    public RingColor Ring1;
    public RingColor Ring2;

    public MPS_RS(Configurations config, string name, Team team, bool hasTag, RingColor? ring1 = null, RingColor? ring2 = null) : base(config, name, team, hasTag, true) {
        Type = MpsType.RingStation;
        MqttHelper.ResetSlideCount();

        if (ring1 == null) {
            Ring1 = Name.Contains("RS1") ? RingColor.RingYellow : RingColor.RingBlue;
        }
        else {
            Ring1 = (RingColor)ring1;
        }

        if (ring2 == null) {
            Ring2 = Name.Contains("RS1") ? RingColor.RingGreen : RingColor.RingOrange;
        }
        else {
            Ring2 = (RingColor)ring2;
        }
    }

    public override void HardResetMachine() {
        base.HardResetMachine();
        MqttHelper.ResetSlideCount();
    }

    protected override void Work() {
        while (Working) {
            CommandEvent.WaitOne();
            CommandEvent.Reset();

            if (ProductAtOut != null) {
                MqttHelper.SetWPSensor(MQTThelper.MQTTWPSensor.WP);
            }
            else {
                MqttHelper.SetWPSensor(MQTThelper.MQTTWPSensor.NoWP);
            }

            MQTTCommand? command;
            while (MqttHelper.command.TryDequeue(out command)) {
                if (ProductAtOut != null) {
                    MqttHelper.SetWPSensor(MQTThelper.MQTTWPSensor.WP);
                }
                else {
                    MqttHelper.SetWPSensor(MQTThelper.MQTTWPSensor.NoWP);
                }
                switch (command.command) {
                    case COMMAND.RESET:
                        break;
                    case COMMAND.LIGHT:
                        HandleLights(command);
                        break;
                    case COMMAND.MOUNT_RING:
                        MountRingTask(command);
                        break;
                    case COMMAND.MOVE_CONVEYOR:
                        HandleBelt(command);
                        if (ProductAtOut != null) {
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

    public override bool PlaceProduct(string machinePoint, Products heldProduct) {
        MyLogger.Info("Got a PlaceProduct for RingStation!");
        if (machinePoint.ToLower().Equals("slide")) {
            MyLogger.Info("Added a Base to the slide!");
            MqttHelper.IncreaseSlideCount();
            MyLogger.Debug("The Current SlideCnt is = " + (MqttHelper.SlideCnt));
            return true;
        }
        return base.PlaceProduct(machinePoint, heldProduct);
    }

    public void MountRingTask(MQTTCommand command) {
        MyLogger.Info("Got a Mount Ring Task!");
        StartTask();
        for (var count = 0; count < 45 && ProductOnBelt == null; count++) {
            Thread.Sleep(1000);
        }
        if (ProductOnBelt == null) return;
        RingElement ringToMount;
        switch (command.arg1) {
            case ARG1.RING1:
                ringToMount = new RingElement(Ring1);
                break;
            case ARG1.RING2:
                ringToMount = new RingElement(Ring2);
                break;
            default:
                throw new Exception("Unknown Ring to mount!");
        }
        Thread.Sleep(Config.RSTaskDuration);
        ProductOnBelt.AddPart(ringToMount);
        MyLogger.Info("Ring Mounted! Color: " + ringToMount.RingColor.ToString());
        FinishedTask();
    }

    public static string ToText(RingColor color) {
        return color switch {
            RingColor.RingBlue => "RING_BLUE",
            RingColor.RingGreen => "RING_GREEN",
            RingColor.RingOrange => "RING_ORANGE",
            RingColor.RingYellow => "RING_YELLOW",
            _ => "Unknown"
        };
    }


    public override bool DeepEquals(Machine machine) {
        if (!base.DeepEquals(machine)) {
            return false;
        }
        if (machine.RingColors.Count < 2) {
            return true;
        }

        if (Ring1 != machine.RingColors[0]) {
            return false;

        }
        if (Ring2 != machine.RingColors[1]) {
            return false;
        }
        return true;
    }

}
