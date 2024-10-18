using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Threading;
using LlsfMsgs;
using Simulator.Utility;
namespace Simulator.MPS {
    public class MPS_RS : Mps {

        public enum BaseSpecificActions {
            Reset = 200,
            WaitForXBases = 201,
            BandOnUntil = 202,
            MountRing = 203
        }

        public MPS_RS(Configurations config, string name, int id, Team team, bool debug = false) : base(config, name, id, team, debug) {
            Type = MpsType.RingStation;
            SlideCount = 0;
        }

        protected override void Work() {
            SerializeMachineToJson();
            while (Working) {
                InEvent.WaitOne();
                InEvent.Reset();
                GotConnection = true;
                //HandleBasicTasks();
                switch (MqttHelper.InNodes.ActionId) {
                    case (ushort)BaseSpecificActions.Reset:
                        ResetMachine();
                        break;
                    case (ushort)BaseSpecificActions.BandOnUntil:
                        HandleBelt();
                        break;
                    case (ushort)BaseSpecificActions.WaitForXBases:
                        MyLogger.Log("Not Implemented!");
                        break;
                    case (ushort)BaseSpecificActions.MountRing:
                        MountRingTask();
                        break;
                    default:
                        MyLogger.Log("In Action ID = " + (MqttHelper.InNodes.ActionId));
                        break;

                }
                TaskDescription = "Idle";
                //MyLogger.Log("enable = [" + InNodes.StatusNodes.enable.Value + "] ready = [" + InNodes.StatusNodes.ready.Value + "] busy = [" + InNodes.StatusNodes.busy.Value + "] error = [" + InNodes.StatusNodes.error.Value + "]");
            }
        }
        public new void PlaceProduct(string machinePoint, Products? heldProduct) {
            MyLogger.Log("Got a PlaceProduct for RingStation!");
            if (machinePoint.Equals("slide")) {
                MyLogger.Log("The Current SlideCnt is = " + (MqttHelper.InNodes.SlideCnt));
                MyLogger.Log("Added a Base to the slide!");
                MqttHelper.InNodes.SetSlideCount(MqttHelper.InNodes.SlideCnt + 1);
                SlideCount = (uint)MqttHelper.InNodes.SlideCnt;

                MyLogger.Log("The Current SlideCnt after is = " + (MqttHelper.InNodes.SlideCnt));
            }
            else {
                base.PlaceProduct(machinePoint, heldProduct);
            }
        }

        public void MountRingTask() {
            MyLogger.Log("Got a Mount Ring Task!");
            TaskDescription = "Mount Ring Task";
            var ringNumber = MqttHelper.InNodes.Data[0];
            StartTask();
            for (var count = 0; count < 45 && ProductOnBelt == null; count++) {
                Thread.Sleep(1000);
            }
            if (ProductOnBelt == null) return;
            RingElement ringToMount;
            switch (ringNumber) {
                case 1:
                    ringToMount = Name.Contains("RS1") ? new RingElement(RingColor.RingYellow) : new RingElement(RingColor.RingBlue);
                    break;
                case 2:
                    ringToMount = Name.Contains("RS1") ? new RingElement(RingColor.RingGreen) : new RingElement(RingColor.RingOrange);
                    break;
                default:
                    return;
            }
            Thread.Sleep(Config.RSTaskDuration);
            ProductOnBelt.AddPart(ringToMount);
            FinishedTask();
        }
    }
}
