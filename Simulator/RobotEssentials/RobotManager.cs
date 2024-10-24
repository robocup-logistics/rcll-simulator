using LlsfMsgs;
using Simulator.MPS;
using Simulator.Utility;

namespace Simulator.RobotEssentials {
    public class RobotManager {
        public List<Robot> Robots { get; }
        private ZonesManager ZonesManager_;
        private MpsManager MpsManager;
        private readonly Configurations Config;

        public RobotManager(Configurations config, MpsManager mpsManager) {
            Robots = new List<Robot>();
            ZonesManager_ = ZonesManager.GetInstance();
            MpsManager = mpsManager;
            Config = config;
            Console.WriteLine("Creating Robots");
            CreateRobots();
        }
        private void CreateRobots() {
            var configs = Config.RobotConfigs;
            foreach (var rob in configs) {
                //Position is teamside x: 4 + jersey(i.e. 5,6,7), y: 1
                var zone = (Zone)((rob.TeamColor == Team.Magenta ? 1000 : 0) + (4 + rob.Jersey) * 10 + 1);

                var robot = new Robot(Config, rob, this, MpsManager, zone, true);
                robot.WorkingRobotThread = new Thread(() => robot.Run());
                robot.WorkingRobotThread.Name = "Robot" + robot.JerseyNumber + "_working_thread";
                robot.SetZone(ZonesManager.GetInstance().GetZone(zone));
                robot.WorkingRobotThread.Start();

                Robots.Add(robot);
            }
        }
        public void HandleRobotInfo(RobotInfo robotInfo) {
            foreach (var info in robotInfo.Robots) {
                foreach (var robot in Robots) {
                    if (robot.JerseyNumber == info.Number) {
                        robot.HandleRobotInfo(info);
                    }
                }
            }
        }

        public void StopAllRobots() {
            foreach (var robot in Robots) {
                robot.RobotStop();
            }
        }
    }
}
