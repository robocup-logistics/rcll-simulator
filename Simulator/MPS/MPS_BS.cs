using System.Collections.Generic;
using LlsfMsgs;
using Simulator.Utility;
using System.Threading;
using System.Text.Json;


namespace Simulator.MPS
{
    public class MPS_BS : Mps
    {
        private readonly Queue<Products> Stock1;
        private readonly Queue<Products> Stock2;
        private readonly Queue<Products> Stock3;

        public enum BaseSpecificActions
        {
            Reset = 100,
            GetBase = 101,
            BandOnUntil = 102
        }

        public MPS_BS(string name, int port, int id, Team team, bool debug = false) : base(name, port, id, team, debug)
        {
            Type = MpsType.BaseStation;
            Stock1 = new Queue<Products>();
            Stock2 = new Queue<Products>();
            Stock3 = new Queue<Products>();

            /*if (Configurations.GetInstance().MockUp)
                return;*/

            for(var i = 0; i < 5; i++)
            {
                Stock1.Enqueue(new Products(BaseColor.BaseRed));
                Stock2.Enqueue(new Products(BaseColor.BaseSilver));
                Stock3.Enqueue(new Products(BaseColor.BaseBlack));
            }
        }
        public new void Run()
        {
            /*if (Configurations.GetInstance().MockUp)
            {
                return;
            }*/
            var BasicThread = new Thread(base.HandleBasicTasks);
            BasicThread.Start();
            Work();

        }
        public void DispenseBase()
        {
            MyLogger.Log("Got a GetBase Task!");
            TaskDescription = "Dispensing a Base";
            var stock = InNodes.Data0.Value;
            StartTask();
            Thread.Sleep(Configurations.GetInstance().BSTaskDuration);
            MyLogger.Log("Placed a Base from stock " + InNodes.Data0.Value + " on the belt");
            switch (stock)
            {
                case 1:
                    ProductOnBelt = Stock1.Dequeue();
                    break;
                case 2:
                    ProductOnBelt = Stock2.Dequeue();
                    break;
                case 3:
                    ProductOnBelt = Stock3.Dequeue();
                    break;
                default:
                    MyLogger.Log("Unknown Stock to get base from!");
                    break;
            }
            InNodes.StatusNodes.ready.Value = true;
            Refbox.ApplyChanges(InNodes.StatusNodes.ready);
            FinishedTask();
        }
        private void Work()
        {
            StartOpc(Type);
            SerializeMachineToJson();
            while (true)
            {
                //MyLogger.Info("We will wait for a Signal!");
                InEvent.WaitOne();
                //MyLogger.Info("We got a write and reset the wait!");
                InEvent.Reset();
                GotConnection = true;
                //HandleBasicTasks();
                switch (InNodes.ActionId.Value)
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
                        MyLogger.Log("In Action ID = " + InNodes.ActionId.Value);
                        break;
                }

                TaskDescription = "Idle";
                // Thread.Sleep(1000);
            }
        }
        public void SerializeMachineToJson()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            JsonInformation = JsonSerializer.Serialize<MPS_BS>(this, options);
            Console.WriteLine(JsonInformation);
        }
    }

}
