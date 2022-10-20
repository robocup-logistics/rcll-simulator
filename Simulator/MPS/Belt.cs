using System.Threading;
using Simulator.Utility;

namespace Simulator.MPS
{
    public enum Positions
    {
        NoTarget = 0,
        In = 1,
        Mid = 2,
        Out = 3

    }
    public enum Direction
    {
        FromInToOut = 1,
        FromOutToIn = 2
    }
    public class Belt
    {
        private Positions Position;
        private Positions TargetPosition;
        public Direction Direction { get; private set; }
        private bool On;
        private readonly ManualResetEvent Event;
        private readonly Mps Machine;
        private Products? ProductOnBelt;
        private bool Work;
        public Belt(Mps machine, ManualResetEvent mre)
        {
            Position = Positions.In;
            Direction = Direction.FromInToOut;
            TargetPosition = Positions.NoTarget;
            Event = mre;
            On = false;
            Work = true;
            Machine = machine;
            var beltThread = new Thread(StateMachine);
            beltThread.Start();
        }

        private void UpdatePosition()
        {
            //TODO add handling of several products on the belt!
            Position = Direction switch
            {
                Direction.FromInToOut => Position switch
                {
                    Positions.In => Positions.Mid,
                    Positions.Mid => Positions.Out,
                    Positions.Out => Positions.Out,
                    _ => Position
                },
                Direction.FromOutToIn => Position switch
                {
                    Positions.In => Positions.In,
                    Positions.Mid => Positions.In,
                    Positions.Out => Positions.Mid,
                    _ => Position
                },
                _ => Position
            };
        }

        private void StateMachine()
        {
            while (Work)
            {
                if (On)
                {

                    if (TargetPosition == Position)
                    {
                        On = false;
                        TargetPosition = Positions.NoTarget;
                        Machine.InNodes.StatusNodes.ready.Value = true;
                        Machine.Refbox.ApplyChanges(Machine.InNodes.StatusNodes.ready);
                        Machine.InNodes.StatusNodes.busy.Value = false;
                        Machine.Refbox.ApplyChanges(Machine.InNodes.StatusNodes.busy);
                        continue;
                    }

                    if (!Machine.InNodes.StatusNodes.busy.Value)
                    {
                        Machine.InNodes.StatusNodes.busy.Value = true;
                        Machine.Refbox.ApplyChanges(Machine.InNodes.StatusNodes.busy);
                    }


                    Thread.Sleep(Configurations.GetInstance().BeltActionDuration);
                    switch (Direction)
                    {
                        case Direction.FromInToOut:
                            UpdatePosition();
                            break;
                        case Direction.FromOutToIn:
                            UpdatePosition();
                            break;
                        default:
                            break;
                    }

                }
                else
                {
                    //Nothing to do when belt is switched off

                }
                //Console.WriteLine("Waiting on BeltEvent");
                Event.WaitOne();
                //Console.WriteLine("Got a BeltEvent");
            }

            return;
        }

        public void SetTarget(Positions target, Direction direction)
        {
            TargetPosition = target;
            Direction = direction;
        }

        public void PlaceProduct(Products prod)
        {
            ProductOnBelt = prod;
        }
        public Products? GetProduct(Positions pos)
        {
            if(Position ==  pos)
            {
                var product = ProductOnBelt;
                ProductOnBelt = null;

                return product;
            }
            else
            {
                return null;
            }
        }
    }
}
