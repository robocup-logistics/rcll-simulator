﻿using LlsfMsgs;
using Simulator.Utility;


namespace Simulator.MPS {
    public class MPS_BS : Mps {
        public enum BaseSpecificActions {
            Reset = 100,
            GetBase = 101,
            BandOnUntil = 102
        }

        public MPS_BS(Configurations config, string name, int id, Team team, bool debug = false) : base(config, name, id, team, debug) {
            Type = MpsType.BaseStation;
        }
        public void DispenseBase() {
            MyLogger.Log("Got a GetBase Task!");
            TaskDescription = "Dispensing a Base";
            var stock = MqttHelper.InNodes.Data[0];
            StartTask();
            Thread.Sleep(Config.BSTaskDuration);
            MyLogger.Log("Placed a Base from stock " + (MqttHelper.InNodes.Data[0]) + " on the belt");
            switch (stock) {
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

            MqttHelper.InNodes.Status.SetReady(true);

            FinishedTask();
        }
        protected override void Work() {
            SerializeMachineToJson();
            while (true) {
                //MyLogger.Info("We will wait for a Signal!");
                InEvent.WaitOne();

                //MyLogger.Info("We got a write and reset the wait!");
                InEvent.Reset();
                GotConnection = true;
                //HandleBasicTasks();
                switch (MqttHelper.InNodes.ActionId) {
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
                        MyLogger.Log("In Action ID = " + (MqttHelper.InNodes.ActionId));
                        break;
                }
                TaskDescription = "Idle";
            }
        }
    }
}
