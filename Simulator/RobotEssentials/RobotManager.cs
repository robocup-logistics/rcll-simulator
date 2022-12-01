using System.Collections.Generic;
using System.Threading;
using LlsfMsgs;
using Simulator.Utility;

namespace Simulator.RobotEssentials
{
    public class RobotManager
    {
        public List<Robot> Robots { get; }
        private ZonesManager ZonesManager;
        private static RobotManager Instance;
        /// <returns>
        /// Returns the instance of the Configurations Singleton
        /// </returns>
        public static RobotManager GetInstance()
        {
            return Instance ??= new RobotManager();
        }

        public RobotManager()
        {
            Robots = new List<Robot>();
            ZonesManager = ZonesManager.GetInstance();
            Instance = this;
            CreateRobots();
        }
        private void CreateRobots()
        {
            var configs = Configurations.GetInstance().RobotConfigs;
            foreach (var rob in configs)
            {
                var robot = new Robot(rob.Name, this,rob.TeamColor, rob.Jersey, true);
                robot.WorkingRobotThread = new Thread(() => robot.Run());
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
                        robot.SetZone(ZonesManager.GetZone(zone));
                        set = true;
                    }
                    x++;
                }

            }
        }
    }
}
