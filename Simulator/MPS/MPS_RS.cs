using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Threading;
using LlsfMsgs;
using Simulator.Utility;
namespace Simulator.MPS
{
    public class MPS_RS : Mps
    {

        public enum BaseSpecificActions
        {
            Reset = 200,
            WaitForXBases = 201,
            BandOnUntil = 202,
            MountRing = 203
        }
        
        public MPS_RS(Configurations config,  string name, int port, int id, Team team, bool debug = false) : base(config, name, port, id, team, debug)
        {
            Type = MpsType.RingStation;
            SlideCount = 0;
        }
        public new void Run()
        {
            var BasicThread = new Thread(base.HandleBasicTasks);
            BasicThread.Start();
            BasicThread.Name = Name + "_HandleBasicThread";
            Work();
        }
        private void Work()
        {
            SerializeMachineToJson();
            while (true)
            {
                InEvent.WaitOne();
                InEvent.Reset();
                GotConnection = true;
                //HandleBasicTasks();
                switch (InNodes.ActionId.Value)
                {
                    case (ushort)BaseSpecificActions.Reset:
                        ResetMachine();
                        break;
                    case (ushort)BaseSpecificActions.BandOnUntil:
                        HandleBelt();
                        break;
                    case (ushort) BaseSpecificActions.WaitForXBases:
                        MyLogger.Log("Not Implemented!");
                        break;
                    case (ushort) BaseSpecificActions.MountRing:
                        MountRingTask();
                        break;
                    default:
                        MyLogger.Log("In Action ID = " + InNodes.ActionId.Value);
                        break;

                }
                TaskDescription = "Idle";                
                MyLogger.Log("enable = [" + InNodes.StatusNodes.enable.Value + "] ready = [" + InNodes.StatusNodes.ready.Value + "] busy = [" + InNodes.StatusNodes.busy.Value + "] error = [" + InNodes.StatusNodes.error.Value + "]");
            }
        }
        public new void PlaceProduct(string machinePoint, Products? heldProduct)
        {
            MyLogger.Log("Got a PlaceProduct for RingStation!");
            if(machinePoint.Equals("slide"))
            {
                MyLogger.Log("The Current SlideCnt is = " + InNodes.SlideCnt.Value);
                MyLogger.Log("Added a Base to the slide!");
                InNodes.SlideCnt.Value += 1;
                SlideCount = InNodes.SlideCnt.Value;
                Thread.Sleep(500);
                Refbox.ApplyChanges(InNodes.SlideCnt);
                MyLogger.Log("The Current SlideCnt after is = " + InNodes.SlideCnt.Value);
            }
            else{
                base.PlaceProduct(machinePoint, heldProduct);
            }
        }

        public void MountRingTask()
        {
            MyLogger.Log("Got a Mount Ring Task!");
            TaskDescription = "Mount Ring Task";
            var ringNumber = InNodes.Data0.Value;
            StartTask();
            for(var count = 0; count  < 45 && ProductOnBelt == null; count++)
            {
                Thread.Sleep(1000);
            }
            if (ProductOnBelt == null) return;
            RingElement ringToMount;
            switch (ringNumber)
            {
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
        public void SerializeMachineToJson()
        {
            JsonInformation = JsonSerializer.Serialize(this);
            //Console.WriteLine(JsonInformation);
        }
    }
}
