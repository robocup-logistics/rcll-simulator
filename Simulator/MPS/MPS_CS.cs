using System.Collections.Generic;
using LlsfMsgs;
using System.Threading;
using Simulator.Utility;

namespace Simulator.MPS
{
    public class MPS_CS : Mps
    {
        private readonly Queue<Products> Shelf1;
        private readonly Queue<Products> Shelf2;
        private readonly Queue<Products> Shelf3;
        public CapElement? StoredCap { get; private set; }
        public enum BaseSpecificActions
        {
            Reset = 300,
            Cap = 301,
            BandOnUntil = 302
        }
        public MPS_CS(string name, int port, int id, Team team, bool debug = false) : base(name, port, id, team, debug)
        {
            Type = MpsType.CapStation;
            Shelf1 = new Queue<Products>();
            Shelf2 = new Queue<Products>();
            Shelf3 = new Queue<Products>();
            for(int i = 0; i < 5; i++)
            {
                Shelf1.Enqueue(new Products(CapColor.CapBlack));
                Shelf2.Enqueue(new Products(CapColor.CapGrey));
                Shelf3.Enqueue(new Products(CapColor.CapGrey));
            }
            StoredCap = null;
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
            Work();
        }
        private void Work()
        {
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
                    case (ushort)BaseSpecificActions.Cap:
                        CapTask();
                        break;
                    case (ushort)BaseSpecificActions.BandOnUntil:
                        HandleBelt();
                        break;
                    default:
                        MyLogger.Log("In Action ID = " + InNodes.ActionId.Value);
                        break;

                }
                MyLogger.Log("enable = [" + InNodes.StatusNodes.enable.Value + "] ready = [" + InNodes.StatusNodes.ready.Value + "] busy = [" + InNodes.StatusNodes.busy.Value + "] error = [" + InNodes.StatusNodes.error.Value + "]");
                TaskDescription = "Idle";
            }
        }

        public void CapTask()
        {
            MyLogger.Log("Got a Cap Task!");
            StartTask();
            switch (InNodes.Data0.Value)
            {
                case (ushort)CSOp.RetrieveCap:
                {
                    TaskDescription = "Cap Retrieve";
                    MyLogger.Log("Got a Retrieve CAP task!");
                    /*for(var count = 0; count  < 45 && ProductOnBelt == null; count++)
                    {
                        Thread.Sleep(1000);
                    }*/
                    if (ProductOnBelt == null) 
                    {
                        MyLogger.Log("Can't retrieve the CAP as there is no product!");
                        InNodes.StatusNodes.error.Value = true;
                        Refbox.UpdateChanges(InNodes.StatusNodes.error);
                    }
                    else
                    {
                        TaskDescription = "Retrieving Cap";
                        Thread.Sleep(Configurations.GetInstance().CSTaskDuration);
                        StoredCap = ProductOnBelt.RetrieveCap();
                    }
                    break;
                }
                case (ushort)CSOp.MountCap:
                {
                    TaskDescription = "Cap Mount";
                    MyLogger.Log("Got a Mount Cap TASK!");
                    /*for(var count = 0; count  < 45 && ProductOnBelt == null; count++)
                    {
                        Thread.Sleep(1000);
                    }*/
                    if (StoredCap != null && ProductOnBelt != null)
                    {
                        TaskDescription = "Mounting Cap";
                        Thread.Sleep(Configurations.GetInstance().CSTaskDuration);
                        ProductOnBelt.AddPart(StoredCap);
                    }
                    else
                    {
                        MyLogger.Log("Can't retrieve the CAP as there is no product!");
                        InNodes.StatusNodes.error.Value = true;
                        Refbox.UpdateChanges(InNodes.StatusNodes.error);
                    }

                    break;
                }
            }
            FinishedTask();
        }
        public new void PlaceProduct(string machinePoint, Products? heldProduct)
        {
            MyLogger.Log("Got a PlaceProduct for CapStation!");
            if (machinePoint.Equals("slide"))
            {
                MyLogger.Log("The Current SlideCnt is = " + InNodes.SlideCnt.Value);
                MyLogger.Log("Added a Base to the slide!");
                InNodes.SlideCnt.Value += 1;
                Refbox.UpdateChanges(InNodes.SlideCnt);
                MyLogger.Log("The Current SlideCnt after is = " + InNodes.SlideCnt.Value);
            }
            else
            {
                base.PlaceProduct(machinePoint, heldProduct);
            }
        }

        public new Products RemoveProduct(string machinePoint)
        {
            Products returnProduct;
            MyLogger.Log("Someone trys to grabs a Item from!");

            switch (machinePoint)
            {
                case "shelf1":
                    returnProduct = Shelf1.Dequeue();
                    break;
                case "shelf2":
                    returnProduct = Shelf2.Dequeue();
                    break;
                case "shelf3":
                    returnProduct = Shelf3.Dequeue();
                    break;
                case "output":
                    MyLogger.Log("my Output!");
                    returnProduct = ProductAtOut;
                    ProductAtOut = null;
                    break;
                case "input":
                    returnProduct = ProductAtIn;
                    ProductAtIn = null;
                    break;
                default:
                    MyLogger.Log("Defaulting!?");
                    returnProduct = Shelf1.Dequeue();
                    break;
            }
            //if (!Configurations.GetInstance().MockUp)
            {
                InNodes.StatusNodes.ready.Value = false;
                Refbox.UpdateChanges(InNodes.StatusNodes.ready);
            }
            return returnProduct;
        }
    }
}
