using Simulator.MPS;
using Simulator.RobotEssentials;
using Simulator.Utility;
using System;
using System.Drawing.Printing;
using LlsfMsgs;
using Serilog;

// TODO Test Robot with teamserver
// Todo test Mps with refbox and teamserver
// todo GUI - split from console application 
// todo finish up robot sim and mps sim
// todo robot sim and mps sim communication needs to be finished
// TODO check if time can be updated with timer messages


namespace Simulator
{
    internal class MainClass
    {
        private static MyLogger? MainLogger;
        private static RobotManager? RobotManager;
        private static MpsManager? MachineManager;
        private static ZonesManager? ZoneManager;
        private static Configurations Config;
        

        private static void Main(string[] args)
        {
            var next = false;
            var path = "";
            foreach (var argument in args)
            {
                switch (argument.ToLower())
                {
                    case "-cfg":
                        next = true;
                        continue;
                    default:
                        {
                            if (next)
                            {
                                path = argument;
                                next = false;
                            }
                            break;
                        }
                }
            }

            var logfolder = "logs" + Path.DirectorySeparatorChar;
            if (!Directory.Exists(logfolder))
            {
                Directory.CreateDirectory(logfolder);
            }
            else
            {

                var di = new DirectoryInfo(logfolder);
                foreach (var file in di.GetFiles())
                {
                    file.Delete();
                }
            }

            if (path.Equals(""))
            {
                Console.WriteLine("No path to the Configuration file is given!");
                Console.WriteLine("Please use \"-cfg path\\to\\file\"!");
                Console.WriteLine("Something like \"-cfg cfg\\config.yaml should work if you just pulled the project!");
                return;
            }
            Config = new Configurations();
            Config.LoadConfig(path);
            MainLogger = new MyLogger("MainClass", true);
            Console.Write("Starting the Machines ... ");
            MachineManager = new MpsManager(Config);
            Console.WriteLine("done!");
            Console.Write("Starting the Robots ... ");
            RobotManager = new RobotManager(Config, MachineManager);
            Console.WriteLine("done!");
            Console.Write("Creating the Zones ... ");
            ZoneManager = ZonesManager.GetInstance();
            Console.WriteLine("done!");
            if (Config.FixedMPSplacement)
            {
                Console.WriteLine("Fixed Positions enabled! Placing machines .. ");
                var mi = new MachineInfo();
                foreach (var m in Config.MpsConfigs)
                {
                    var machine = new Machine()
                    {
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

        public static void CloseApplication()
        {
            Console.Write("Starting the cleanup ..");
            if(RobotManager != null)
            {
                foreach (var robot in RobotManager.Robots)
                {
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
