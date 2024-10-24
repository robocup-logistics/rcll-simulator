using LlsfMsgs;
using Google.Protobuf;
using Simulator.Utility;
// using Timer = System.Threading.Timer;
using Timer = Simulator.Utility.Timer;

namespace Simulator.RobotEssentials {
    internal class PBMessageFactoryRobot {
        private Robot Robot;
        public ulong SequenzNr;
        public Timer Timer;
        public readonly MyLogger MyLogger;
        public readonly Configurations Config;

        public PBMessageFactoryRobot(Configurations config, Robot robot, MyLogger logger) {
            logger.Info("Created a PBMessageFactoryRobot!");
            SequenzNr = 0;
            MyLogger = logger;
            Config = config;
            Timer = Timer.GetInstance(Config);
            Robot = robot;
        }
        public Time GetTimeMessage() {
            return Timer.GetTime();
        }

        public Message CreateMessage<T>(T signal, ushort cmp, ushort msg) where T : Google.Protobuf.IMessage<T>
        {
            var payloadsize = (uint)signal.CalculateSize() + 4;
            var bytes = signal.ToByteArray();
            var fh = new FrameHeader(payloadsize);
            var mh = new MessageHeader(cmp, msg);
            var mb = new MessageBody(bytes);
            return new Message(fh, mh, mb);
        }

        public Message CreateBeaconSignal() {
            var bs = new BeaconSignal {
                Time = GetTimeMessage(),
                TeamColor = Robot.TeamColor,
                Number = Robot.JerseyNumber
            };
            var pose = GetPose2DMessage();
            bs.TeamName = Robot.TeamName;
            bs.PeerName = Robot.RobotName;
            bs.Seq = ++SequenzNr;
            bs.Number = Robot.JerseyNumber;
            bs.Pose = pose;
            bs.FinishedTasks.Clear();
            bs.Task = Robot.CurrentTask;
            var desc = Robot.HeldProduct?.GetProtoDescription();
            if(desc != null && bs.Task != null) {
                bs.Task.WorkpieceDescription = desc;
            }
            if (Robot != null && Robot.FinishedTasks.Count != 0) {
                foreach (var t in Robot.FinishedTasks) {
                    var task = new FinishedTask {
                        TaskId = t.TaskId,
                        Successful = t.Successful
                    };
                    bs.FinishedTasks.Add(task);
                }
            }
            //MyLogger.Log(bs.ToString());
            var cmp = (ushort)BeaconSignal.Types.CompType.CompId;
            var msg = (ushort)BeaconSignal.Types.CompType.MsgType;
            return CreateMessage<BeaconSignal>(bs, cmp, msg);
        }

        public Message? GetAgentTask() {
            var task = Robot.CurrentTask;
            if(task == null){
                return null;
            }
            var cmp = (ushort)AgentTask.Types.CompType.CompId;
            var msg = (ushort)AgentTask.Types.CompType.MsgType;
            return CreateMessage<AgentTask>(task, cmp, msg);
        }

        public Message? GetLastTask() {
            var task = Robot.LastTask;
            if(task == null){
                return null;
            }
            var cmp = (ushort)AgentTask.Types.CompType.CompId;
            var msg = (ushort)AgentTask.Types.CompType.MsgType;
            return CreateMessage<AgentTask>(task, cmp, msg);
        }

        private Pose2D GetPose2DMessage() {
            return new Pose2D {
                Timestamp = GetTimeMessage(),
                Ori = Robot.Position.Orientation,
                X = Robot.Position.X,
                Y = Robot.Position.Y,
            };

        }
    }
}
