using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading;
using LlsfMsgs;
using Simulator.Utility;
namespace Simulator.MPS
{
    public class MPS_RS : Mps
    {
        private readonly Queue<RingElement> Stock1;
        private readonly Queue<RingElement> Stock2;
        private enum BaseSpecificActions
        {
            Reset = 200,
            WaitForXBases = 201,
            BandOnUntil = 202,
            MountRing = 203
        }
        
        public MPS_RS(string name, int port, int id, Team team, bool debug = false) : base(name, port, id, team, debug)
        {
            Type = MpsType.RingStation;
            Stock1 = new Queue<RingElement>();
            Stock2 = new Queue<RingElement>();
            for(int i = 0; i < 5; i++)
            {
                Stock1.Enqueue(new RingElement(RingColor.RingBlue));
                Stock2.Enqueue(new RingElement(RingColor.RingOrange));
            }
            //if (Configurations.GetInstance().MockUp) return;
        }
        public new void Run()
        {
            /*if (Configurations.GetInstance().MockUp)
            {
                return;
            }*/
            Work();
        }
        private void Work()
        {
            StartOpc(Type);
            
            while (true)
            {
                WriteEvent.WaitOne();
                WriteEvent.Reset();
                GotConnection = true;
                HandleBasicTasks();
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
                Refbox.UpdateChanges(InNodes.SlideCnt);
                MyLogger.Log("The Current SlideCnt after is = " + InNodes.SlideCnt.Value);
            }
            else{
                base.PlaceProduct(machinePoint, heldProduct);
            }
        }

        private void MountRingTask()
        {
            MyLogger.Log("Got a Mount Ring Task!");
            TaskDescription = "Mount Ring Task";
            InNodes.StatusNodes.busy.Value = true;
            Refbox.UpdateChanges(InNodes.StatusNodes.busy);
            InNodes.StatusNodes.enable.Value = false;
            Refbox.UpdateChanges(InNodes.StatusNodes.enable);
            if (ProductOnBelt == null) return;
            RingElement ringToMount;
            switch (InNodes.Data0.Value)
            {
                case 1:
                    ringToMount = Stock1.Dequeue();
                    break;
                case 2:
                    ringToMount = Stock2.Dequeue();
                    break;
                default:
                    return;
            }
            Thread.Sleep(Configurations.GetInstance().RSTaskDuration);
            ProductOnBelt.AddPart(ringToMount);
            InNodes.StatusNodes.busy.Value = false;
            Refbox.UpdateChanges(InNodes.StatusNodes.busy);
        }
    }
}
