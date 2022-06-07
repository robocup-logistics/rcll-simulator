using Simulator.MPS;
using Simulator.RobotEssentials;
using Simulator.Utility;
using System;
using System.Drawing.Printing;
using LlsfMsgs;
using Serilog;
using TerminalGui = Simulator.TerminalGui.TerminalGui;
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
        private static MyLogger Mainlogger;
        private static RobotManager RobotManager;
        private static MpsManager MachineManager;
        private static ZonesManager ZoneManager;
        private static bool ShowGui;
        private static void Main(string[] args)
        {
            var next = false;
            var path = "";
            ShowGui = false;
            foreach (var argument in args)
            {
                switch (argument.ToLower())
                {
                    case "-cfg":
                        next = true;
                        continue;
                    case "-gui":
                        ShowGui = true;
                        break;
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
            Configurations.GetInstance().LoadConfig(path);
            Mainlogger = new MyLogger("MainClass", true);
            if (!ShowGui)
            {
                Console.Write("Starting the Robots ... ");
                RobotManager = new RobotManager();
                Console.WriteLine("done!");
                Console.Write("Starting the Machines ... ");
                MachineManager = MpsManager.GetInstance();
                Console.WriteLine("done!");
                Console.Write("Creating the Zones ... ");
                ZoneManager = ZonesManager.GetInstance();
                Console.WriteLine("done!");
                if (Configurations.GetInstance().FixedMPSplacement)
                {
                    Console.WriteLine("Fixed Positions enabled! Placing machines .. ");
                    var mi = new MachineInfo();
                    foreach (var m in Configurations.GetInstance().MpsConfigs)
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
                Console.WriteLine("Everything is set up! Waiting for connections!");
            }
            else
            {
                RobotManager = new RobotManager();
                MachineManager = MpsManager.GetInstance();
                ZoneManager = ZonesManager.GetInstance();
                if (Configurations.GetInstance().FixedMPSplacement)
                {
                    var mi = new MachineInfo();
                    foreach (var m in Configurations.GetInstance().MpsConfigs)
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
                }
                //zonesManager.Astar(zonesManager.GetZone(Zone.CZ11), zonesManager.GetZone(Zone.MZ78));

                var gui = new TerminalGui.TerminalGui(RobotManager, MachineManager, ZoneManager);
            }
        }

        public static void CloseApplication()
        {
            if (!ShowGui)
            {
                Console.Write("Starting the cleanup ..");
            }
            foreach (var robot in RobotManager.Robots)
            {
                Mainlogger.Log("Starting the Cleanup....");
                robot.RobotStop();
                Mainlogger.Log("Finished the Cleanup....");

            }
            if (!ShowGui)
            {
                Console.Write(".. cleanup done");
            }
            Environment.Exit(0);
        }
    }
}
