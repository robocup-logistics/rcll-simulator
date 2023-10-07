using System.Collections.Generic;
using LlsfMsgs;
using Simulator.Utility;
using System.Threading;
using System.Text.Json;


namespace Simulator.MPS
{
    public class MPS_BS : Mps
    {
        public enum BaseSpecificActions
        {
            Reset = 100,
            GetBase = 101,
            BandOnUntil = 102
        }

        public MPS_BS(Configurations config, string name, int port, int id, Team team, bool debug = false) : base(config, name, port, id, team, debug)
        {
            Type = MpsType.BaseStation;
        }
        public new void Run()
        {
            var BasicThread = new Thread(base.HandleBasicTasks);
            BasicThread.Start();
            BasicThread.Name = Name + "_HandleBasicThread";
            Work();
        }
        public void DispenseBase()
        {
            MyLogger.Log("Got a GetBase Task!");
            TaskDescription = "Dispensing a Base";
            var stock = MQTT ? MqttHelper.InNodes.Data[0] : InNodes.Data0.Value;
            StartTask();
            Thread.Sleep(Config.BSTaskDuration);
            MyLogger.Log("Placed a Base from stock " + (MQTT ? MqttHelper.InNodes.Data[0] : InNodes.Data0.Value) + " on the belt");
            switch (stock)
            {
                case 1:
                    ProductOnBelt = new Products(BaseColor.BaseRed);
                    break;
                case 2:
                    ProductOnBelt = new Products(BaseColor.BaseSilver);
                    break;
                case 3:
                    ProductOnBelt = new Products(BaseColor.BaseBlack);
                    break;
                default:
                    MyLogger.Log("Unknown Stock to get base from!");
                    break;
            }

            if (MQTT)
            {   
                //MqttHelper.InNodes.Status.SetReady(true);
            }
            else
            {
                InNodes.StatusNodes.ready.Value = true;
                Refbox.ApplyChanges(InNodes.StatusNodes.ready);
            }

            FinishedTask();
        }
        private void Work()
        {
            SerializeMachineToJson();
            while (true)
            {
                //MyLogger.Info("We will wait for a Signal!");
                InEvent.WaitOne();
                
                //MyLogger.Info("We got a write and reset the wait!");
                InEvent.Reset();
                GotConnection= true;
                //HandleBasicTasks();
                switch (MQTT ? MqttHelper.InNodes.ActionId : InNodes.ActionId.Value)
                {
                    case (ushort)Actions.NoJob:
                        MyLogger.Log("No In Job!");
                        break;
                    case (ushort)BaseSpecificActions.Reset:
                        ResetMachine();
                        break;
                    case (ushort)BaseSpecificActions.BandOnUntil:
                        HandleBelt();
                        break;
                    case (ushort)BaseSpecificActions.GetBase:
                        DispenseBase();
                        break;
                    default:
                        MyLogger.Log("In Action ID = " + (MQTT? MqttHelper.InNodes.ActionId : InNodes.ActionId.Value));
                        break;
                }
                TaskDescription = "Idle"; 
            }
        }
        public void SerializeMachineToJson()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            JsonInformation = JsonSerializer.Serialize<MPS_BS>(this, options);
            //Console.WriteLine(JsonInformation);
        }
    }

}
