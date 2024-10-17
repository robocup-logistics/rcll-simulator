using System.Text.Json;
using LlsfMsgs;

namespace Simulator.MPS
{
    public class MPS_SS : Mps
    {
        public enum BaseSpecificActions
        {
            Reset = 500,
            BandOnUntil = 502
        }
        public SSOp SSOp;

        public MPS_SS(Configurations config,  string name, int port, int id, Team team, bool debug = false) : base(config, name, port, id, team, debug)
        {
            Type = MpsType.StorageStation;
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
                    case (ushort)BaseSpecificActions.BandOnUntil:
                        HandleBelt();
                        break;
                    default:
                        MyLogger.Log("In Action ID = " + (MqttHelper.InNodes.ActionId));
                        break;

                }
                TaskDescription = "Idle";                
            }
        }
    }
}
