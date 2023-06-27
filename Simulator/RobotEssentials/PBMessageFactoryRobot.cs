using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using LlsfMsgs;
using Simulator.Utility;
using Timer = System.Threading.Timer;

namespace Simulator.RobotEssentials
{
    internal class PBMessageFactoryRobot : PBMessageFactoryBase
    {
        private Robot Peer;
        
        public PBMessageFactoryRobot(Configurations config, Robot peer, MyLogger log) :base(config, log)
        {
            log.Info("Created a PBMessageFactoryRobot!");
            Peer = peer;
        }
        public override byte[] CreateMessage(MessageTypes mtype)
        {
            Timer ??= Utility.Timer.GetInstance(Config);
            ushort cmp = 0;
            ushort msg = 0;
            uint payloadsize = 0;
            byte[] bytes;

            MyLogger.Log("[RobotMessageFactory] Creating a : " + mtype + " message!");
            switch (mtype)
            {
                case MessageTypes.BeaconSignal:
                    {
                        var Signal = new BeaconSignal();
                        Signal = CreateBeaconSignal();
                        cmp = (ushort)BeaconSignal.Types.CompType.CompId;
                        msg = (ushort)BeaconSignal.Types.CompType.MsgType;
                        payloadsize = (uint)Signal.CalculateSize() + 4;
                        bytes = Signal.ToByteArray();
                        break;
                    }
                case MessageTypes.GripsBeaconSignal:
                    {
                        var Signal = new GripsBeaconSignal();
                        var bs = CreateBeaconSignal();
                        Signal.BeaconSignal = bs;
                        cmp = (ushort)GripsBeaconSignal.Types.CompType.CompId;
                        msg = (ushort)GripsBeaconSignal.Types.CompType.MsgType;
                        payloadsize = (uint)Signal.CalculateSize() + 4;
                        bytes = Signal.ToByteArray();
                        break;
                    }
                case MessageTypes.PrepareMachine:
                case MessageTypes.GripsPrepareMachine:
                    var machineId = "";
                    var machinePoint = "";
                    if (Peer is { CurrentTask: { } })
                    {
                        var robotId = Peer.JerseyNumber;
                        if (Peer.CurrentTask.Deliver != null)
                        {
                            machineId = Peer.CurrentTask.Deliver.MachineId;
                            machinePoint = Peer.CurrentTask.Deliver.MachinePoint;
                        }
                        else if (Peer.CurrentTask.Buffer != null)
                        {
                            machineId = Peer.CurrentTask.Buffer.MachineId;
                            machinePoint = "input"; // Peer.CurrentTask.BufferCapStation.ShelfNumber.ToString();
                        }
                        else if (Peer.CurrentTask.Retrieve != null)
                        {
                            machineId = Peer.CurrentTask.Retrieve.MachineId;
                            machinePoint = Peer.CurrentTask.Retrieve.MachinePoint;
                        }
                        var machine = new GripsPrepareMachine()
                        {
                            RobotId = robotId,
                            MachineId = machineId,
                            MachinePoint = machinePoint,
                            MachinePrepared = true
                        };
                        cmp = (ushort)GripsPrepareMachine.Types.CompType.CompId;
                        msg = (ushort)GripsPrepareMachine.Types.CompType.MsgType;
                        payloadsize = (uint)machine.CalculateSize() + 4;
                        bytes = machine.ToByteArray();
                        MyLogger.Log("Created a prepare machine message!");
                        MyLogger.Log(machine.ToString());
                        break;
                    }
                    MyLogger.Log("Cant't create the GripPrepareMachineTask as the Peer is not set or there is no task");
                    return Array.Empty<byte>();
                case MessageTypes.GameState:
                    var gamestate = new GameState()
                    {
                        GameTime = GetTimeMessage(),
                        Phase = GameState.Types.Phase.Exploration,
                        PointsCyan = 0,
                        PointsMagenta = 0,
                        State = GameState.Types.State.Init,
                        TeamCyan = Config.Teams[0].Name,
                        TeamMagenta = Config.Teams[1].Name,
                    };
                    cmp = (ushort)GameState.Types.CompType.CompId;
                    msg = (ushort)GameState.Types.CompType.MsgType;
                    payloadsize = (uint)gamestate.CalculateSize() + 4;
                    bytes = gamestate.ToByteArray();
                    break;
                case MessageTypes.AgentTask:
                    var answer = new AgentTask()
                    {
                        TeamColor = Peer.TeamColor,
                        TaskId = Peer.CurrentTask.TaskId,
                        RobotId = Peer.JerseyNumber,
                        Move = Peer.CurrentTask.Move,
                        Retrieve = Peer.CurrentTask.Retrieve,
                        Deliver = Peer.CurrentTask.Deliver,
                        Buffer = Peer.CurrentTask.Buffer,
                        ExploreMachine = Peer.CurrentTask.ExploreMachine,
                        CancelTask = Peer.CurrentTask.CancelTask,
                        PauseTask = Peer.CurrentTask.PauseTask,
                        Successful = Peer.CurrentTask.Successful,
                        Canceled = Peer.CurrentTask.Canceled
                    };
                    cmp = (ushort)AgentTask.Types.CompType.CompId;
                    msg = (ushort)AgentTask.Types.CompType.MsgType;
                    payloadsize = (uint)answer.CalculateSize() + 4;
                    bytes = answer.ToByteArray();
                    MyLogger.Log("The Created AgentTask = " + answer.ToString());
                    break;
                default:
                    return Array.Empty<byte>();
            }
            var fh = new FrameHeader(payloadsize);
            var mh = new MessageHeader(cmp, msg);
            var mb = new MessageBody(bytes);
            var message = new Message(fh, mh, mb);
            return message.GetBytes();
        }
        private BeaconSignal CreateBeaconSignal()
        {
            var bs = new BeaconSignal
            {
                Time = GetTimeMessage(),
                TeamColor = Peer?.TeamColor ?? Config.Teams[0].Color,
                Number = Peer?.JerseyNumber ?? 0
            };
            var pose = GetPose2DMessage();
            bs.TeamName = Peer?.TeamName ?? Config.Teams[0].Name;
            bs.PeerName = Peer?.RobotName ?? "Client";
            bs.Seq = ++SequenzNr;
            bs.Number = Peer?.JerseyNumber ?? 9999;
            bs.Pose = pose;
            bs.FinishedTasks.Clear();
            bs.Task = Peer?.CurrentTask;
            if (Peer != null && Peer.FinishedTasksList.Count != 0)
            {
                foreach (var t in Peer.FinishedTasksList)
                {
                    var task = new FinishedTask
                    {
                        TaskId = t.TaskId,
                        Successful = t.Successful
                    };
                    bs.FinishedTasks.Add(task);
                }
            }
            //MyLogger.Log(bs.ToString());
            return bs;
        }

        private Pose2D GetPose2DMessage()
        {
            return new Pose2D
            {
                Timestamp = GetTimeMessage(),
                Ori = Peer.Position.Orientation,
                X = Peer.Position.X,
                Y = Peer.Position.Y,
            };

        }
    }
}
