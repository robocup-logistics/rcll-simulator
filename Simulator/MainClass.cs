using Simulator.MPS;
using Simulator.RobotEssentials;
using Simulator.Utility;
using LlsfMsgs;

// TODO Test Robot with teamserver
// TODO test Mps with refbox and teamserver
// TODO GUI - split from console application
// TODO finish up robot sim and mps sim
// TODO robot sim and mps sim communication needs to be finished
// TODO check if time can be updated with timer messages


namespace Simulator {
    internal class MainClass {
        private static MyLogger? MainLogger;
        private static RobotManager? RobotManager;
        private static MpsManager? MachineManager;
        private static ZonesManager? ZoneManager;
        private static Configurations? Config;
        private static Thread? RefboxThread;


        private static void Main(string[] args) {
            string path = args
                .SkipWhile(arg => arg.ToLower() != "-cfg")
                .Skip(1)
                .FirstOrDefault() ?? "";

            if (path.Equals("")) {
                Console.WriteLine("No path to the Configuration file is given!");
                Console.WriteLine("Please use \"-cfg path\\to\\file\"!");
                Console.WriteLine("Something like \"-cfg cfg\\config.yaml should work if you just pulled the project!");
                return;
            }

            string logfolder = "logs" + Path.DirectorySeparatorChar;
            if (!Directory.Exists(logfolder)) {
                Directory.CreateDirectory(logfolder);
            }
            else {
                DirectoryInfo di = new DirectoryInfo(logfolder);
                foreach (var file in di.GetFiles()) {
                    file.Delete();
                }
            }

            Config = new Configurations(path);
            MainLogger = new MyLogger("MainClass", true);
            Console.Write("Starting the Machines ... ");
            MachineManager = new MpsManager(Config);
            Console.WriteLine("done!");
            Console.Write("Starting the Robots ... ");
            RobotManager = new RobotManager(Config, MachineManager);
            Console.WriteLine("done!");
            RefboxThread = new Thread(() => new TcpConnector(Config, Config.Refbox.IP, Config.Refbox.TcpPort, MachineManager, RobotManager, MainLogger));
            RefboxThread.Start();
            Console.Write("Creating the Zones ... ");
            ZoneManager = ZonesManager.GetInstance();
            Console.WriteLine("done!");

            if (Config.FixedMPSplacement) {
                Console.WriteLine("Fixed Positions enabled! Placing machines .. ");
                var mi = new MachineInfo();
                foreach (var m in Config.MpsConfigs) {
                    var machine = new Machine() {
                        Name = m.Name,
                        Zone = m.Zone,
                        Rotation = (uint)m.Orientation

                    };
                    mi.Machines.Add(machine);
                }

                MachineManager.PlaceMachines(mi);
                Console.WriteLine("done!");
            }

            var web = new WebGui.WebGui(Config, MachineManager, RobotManager);
            Console.WriteLine("Everything is set up! Waiting for connections!");

        }

        public static void CloseApplication() {
            Console.Write("Starting the cleanup ..");
            if (RobotManager != null) {
                foreach (var robot in RobotManager.Robots) {
                    MainLogger?.Log("Starting the Cleanup....");
                    robot.RobotStop();
                    MainLogger?.Log("Finished the Cleanup....");

                }
            }
            Console.Write(".. cleanup done");
            Environment.Exit(0);
        }
    }
}
