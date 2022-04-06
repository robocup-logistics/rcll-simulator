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
        public RobotManager()
        {
            Robots = new List<Robot>();
            ZonesManager = ZonesManager.GetInstance();
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
                    var zone = (Zone)(x * 10 + y);
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
