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
     
        private static void Main(string[] args)
        {

            var next = false;
            var path = "";
            var showGui = false;
            foreach (var argument in args)
            {
                switch (argument.ToLower())
                {
                    case "-cfg":
                        next = true;
                        continue;
                    case "-gui":
                        showGui = true;
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

            if (path.Equals(""))
            {
                Console.WriteLine("No path to the Configuration file is given!");
                Console.WriteLine("Please use \"-cfg path\\to\\file\"!");
                Console.WriteLine("Something like \"-cfg cfg\\config.yaml should work if you just pulled the project!");
                return;
            }
            Configurations.GetInstance().LoadConfig(path);
            var mainlogger = new MyLogger("MainClass", true);
            if (!showGui)
            {
                Console.Write("Starting the Robots ... ");
                var managerRobot = new RobotManager();
                Console.WriteLine("done!");
                Console.Write("Starting the Machines ... ");
                var managerMachines = MpsManager.GetInstance();
                Console.WriteLine("done!");
                Console.Write("Creating the Zones ... ");
                var zonesManager = ZonesManager.GetInstance();
                Console.WriteLine("done!");
                Console.WriteLine("Everything is set up! Waiting for connections!");
            }
            else
            {
                var managerRobot = new RobotManager();
                var managerMachines = MpsManager.GetInstance();
                var zonesManager = ZonesManager.GetInstance();
                //zonesManager.Astar(zonesManager.GetZone(Zone.CZ11), zonesManager.GetZone(Zone.MZ78));

                var gui = new TerminalGui.TerminalGui(managerRobot, managerMachines, zonesManager);
            }

        }

    }
}
