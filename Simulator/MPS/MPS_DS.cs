using System.Text.Json;
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
        public MPS_DS(Configurations config,  string name, int port, int id, Team team, bool debug = false) : base(config, name, port, id, team, debug)
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
            var BasicThread = new Thread(base.HandleBasicTasks);
            BasicThread.Start();
            BasicThread.Name = Name + "_HandleBasicThread";
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
            SerializeMachineToJson();
            while (Working)
            {
                InEvent.WaitOne();
                InEvent.Reset();
                GotConnection = true;
                //HandleBasicTasks();
                switch (MqttHelper.InNodes.ActionId)
                {
                    case (ushort)BaseSpecificActions.Reset:
                        ResetMachine();
                        break;
                    case (ushort)BaseSpecificActions.DeliverToSlot:
                        DeliverToSlotTask();
                        break;
                    default:
                        MyLogger.Log("In Action ID = " + (MqttHelper.InNodes.ActionId));
                        break;

                }
                //MyLogger.Log("enable = [" + InNodes.StatusNodes.enable.Value + "] ready = [" + InNodes.StatusNodes.ready.Value + "] busy = [" + InNodes.StatusNodes.busy.Value + "] error = [" + InNodes.StatusNodes.error.Value + "]");
                TaskDescription = "Idle";
            }
        }

        private void DeliverToSlotTask()
        {
            MyLogger.Log("DeliverToSlotTask!");
            TaskDescription = "Delivering Product";
            var slot = MqttHelper.InNodes.Data[0];
            StartTask();
            for(var count = 0; count  < 45 && ProductAtIn == null; count++)
            {
                Thread.Sleep(1000);
            }
            if (ProductAtIn == null) return;
            MyLogger.Log("Deliver to slot " + slot);
            Thread.Sleep(Config.DSTaskDuration);
            switch (slot)
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
            FinishedTask();
        }
    }
}
