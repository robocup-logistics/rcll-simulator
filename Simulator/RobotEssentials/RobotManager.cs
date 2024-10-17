using System.Collections.Generic;
using System.Threading;
using LlsfMsgs;
using Simulator.MPS;
using Simulator.Utility;

namespace Simulator.RobotEssentials
{
    public class RobotManager
    {
        public List<Robot> Robots { get; }
        private ZonesManager ZonesManager;
        private MpsManager MpsManager;
        private Configurations Config;
        /// <returns>
        /// Returns the instance of the Configurations Singleton
        /// </returns>


        public RobotManager(Configurations config, MpsManager mpsManager)
        {
            Robots = new List<Robot>();
            ZonesManager = ZonesManager.GetInstance();
            MpsManager = mpsManager;
            Config = config;
            Console.WriteLine("Creating Robots");
            CreateRobots();
        }
        private void CreateRobots()
        {
            var configs = Config.RobotConfigs;
            foreach (var rob in configs)
            {
                var robot = new Robot(Config, rob.Name, this, rob.TeamColor, rob.Jersey, MpsManager, true);
                robot.WorkingRobotThread = new Thread(() => robot.Run());
                robot.WorkingRobotThread.Name = "Robot" + robot.JerseyNumber + "_working_thread";
                robot.WorkingRobotThread.Start();
                
                Robots.Add(robot);
                bool set = false;
                int x = 5;
                int y = 1;
                while (!set)
                {
                    var zone = Zone.CZ11;
                    if (robot.TeamColor == Team.Magenta)
                        zone = (Zone)(1000 + x * 10 + y);
                    else
                        zone = (Zone)(x * 10 + y);
                    if (ZonesManager.PlaceRobot(zone, 0, robot))
                    {
                        var z = ZonesManager.GetZone(zone);
                        if(z == null) {
                            throw new Exception("Zone is null");
                        }
                        robot.SetZone(z);
                        set = true;
                    }
                    x++;
                }

            }
        }

        public void StopAllRobots()
        {
            foreach (var robot in Robots)
            {
                robot.RobotStop();
            }
        }
    }
}
