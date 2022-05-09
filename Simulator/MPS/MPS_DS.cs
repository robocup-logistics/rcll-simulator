using LlsfMsgs;
using System.Threading;
using Simulator.Utility;

namespace Simulator.MPS
{
    public class MPS_DS : Mps
    {
        // TODO Add some more space to the slots
        private Products? Slot1;
        private Products? Slot2;
        private Products? Slot3;
        public enum BaseSpecificActions
        {
            Reset = 400,
            DeliverToSlot = 401
        }
        public MPS_DS(string name, int port, int id, Team team, bool debug = false) : base(name, port, id, team, debug)
        {
            Type = MpsType.DeliveryStation;
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
        public bool ProductAtSlot(int slot)
        {
            switch (slot)
            {
                case 1:
                    if (Slot1 != null)
                        return true;
                    else
                        return false;
                case 2:
                    if (Slot2 != null)
                        return true;
                    else
                        return false;
                case 3:
                    if (Slot3 != null)
                        return true;
                    else
                        return false;
            }
            return false;
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
                    case (ushort)BaseSpecificActions.DeliverToSlot:
                        DeliverToSlotTask();
                        break;
                    default:
                        MyLogger.Log("In Action ID = " + InNodes.ActionId.Value);
                        break;

                }
                MyLogger.Log("enable = [" + InNodes.StatusNodes.enable.Value + "] ready = [" + InNodes.StatusNodes.ready.Value + "] busy = [" + InNodes.StatusNodes.busy.Value + "] error = [" + InNodes.StatusNodes.error.Value + "]");
                TaskDescription = "Idle";
            }
        }

        private void DeliverToSlotTask()
        {
            MyLogger.Log("DeliverToSlotTask!");
            InNodes.StatusNodes.busy.Value = true;
            Refbox.UpdateChanges(InNodes.StatusNodes.busy);
            InNodes.StatusNodes.enable.Value = false;
            Refbox.UpdateChanges(InNodes.StatusNodes.enable);

            if (ProductAtIn == null) return;
            MyLogger.Log("Deliver to slot " + InNodes.Data0.Value);
            Thread.Sleep(Configurations.GetInstance().DSTaskDuration);
            switch (InNodes.Data0.Value)
            {
                case 1:
                    Slot1 = ProductAtIn;
                    break;
                case 2:
                    Slot2 = ProductAtIn;
                    break;
                case 3:
                    Slot3 = ProductAtIn;
                    break;
            }
            ProductAtIn = null;
            InNodes.ActionId.Value = 0;
            Refbox.UpdateChanges(InNodes.ActionId);
            InNodes.Data0.Value = 0;
            Refbox.UpdateChanges(InNodes.Data0);
            InNodes.Data1.Value = 0;
            Refbox.UpdateChanges(InNodes.Data1);
            InNodes.StatusNodes.busy.Value = false;
            Refbox.UpdateChanges(InNodes.StatusNodes.busy);
        }
    }
}
