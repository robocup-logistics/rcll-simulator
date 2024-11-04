using LlsfMsgs;
using Simulator.Utility;
using COMMAND = Simulator.MPS.MQTTCommand.COMMAND;
using ARG1 = Simulator.MPS.MQTTCommand.ARG1;

namespace Simulator.MPS;
public class MPS_RS : Mps {
    public RingColor Ring1;
    public RingColor Ring2;

    public MPS_RS(Configurations config, string name, Team team, bool hasTag) : base(config, name, team, hasTag, true) {
        Type = MpsType.RingStation;
        MqttHelper.ResetSlideCount();
        Ring1 = Name.Contains("RS1") ? RingColor.RingYellow : RingColor.RingBlue;
        Ring2 = Name.Contains("RS1") ? RingColor.RingGreen : RingColor.RingOrange;
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
                        MyLogger.Error("Unhandelt ActionType: " + command.command);
                        break;
                }
            }
            finally {
                CommandMutex.ReleaseMutex();
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
                //TODO GET COLOR FROM REFBOX
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
        MyLogger.Info("Ring Mounted!");
        FinishedTask();
    }
}
