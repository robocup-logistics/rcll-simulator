using LlsfMsgs;
using Simulator.Utility;
// using Timer = System.Threading.Timer;
using Timer = Simulator.Utility.Timer;

namespace Simulator.RobotEssentials {
    internal class PBMessageFactoryRobot {
        private Robot Peer;
        public ulong SequenzNr;
        public Timer Timer;
        public readonly MyLogger MyLogger;
        public readonly Configurations Config;

        public PBMessageFactoryRobot(Configurations config, Robot peer, MyLogger log) {
            log.Info("Created a PBMessageFactoryRobot!");
            SequenzNr = 0;
            MyLogger = log;
            Config = config;
            Timer = Timer.GetInstance(Config);
            Peer = peer;
        }
        public Time GetTimeMessage() {
            return Timer.GetTime();
        }

        public byte[] CreateBeaconMessage() {
            var signal = CreateBeaconSignal();

            // Use a MemoryStream for serialization
            using (var memoryStream = new MemoryStream()) {
                // Create a CodedOutputStream that wraps the MemoryStream
                using (var codedOutputStream = new Google.Protobuf.CodedOutputStream(memoryStream)) {
                    // Write the signal to the coded output stream
                    signal.WriteTo(codedOutputStream);

                    // Flush the stream to ensure all data is written to the MemoryStream
                    codedOutputStream.Flush();

                    // Convert the MemoryStream to a byte array
                    return memoryStream.ToArray();
                }
            }
        }

        public BeaconSignal CreateBeaconSignal() {
            var bs = new BeaconSignal {
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
            if (Peer != null && Peer.FinishedTasksList.Count != 0) {
                foreach (var t in Peer.FinishedTasksList) {
                    var task = new FinishedTask {
                        TaskId = t.TaskId,
                        Successful = t.Successful
                    };
                    bs.FinishedTasks.Add((IEnumerable<LlsfMsgs.FinishedTask>)task);
                }
            }
            //MyLogger.Log(bs.ToString());
            return bs;
        }

        private Pose2D GetPose2DMessage() {
            return new Pose2D {
                Timestamp = GetTimeMessage(),
                Ori = Peer.Position.Orientation,
                X = Peer.Position.X,
                Y = Peer.Position.Y,
            };

        }
    }
}
