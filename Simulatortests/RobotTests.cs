using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simulator.Utility;
using Simulator;
using LlsfMsgs;
using System.Threading;
using Simulator.RobotEssentials;
using Simulator.MPS;

namespace Simulatortests
{
    [TestClass]
    public class RobotTests
    {
        private MpsManager _mpsManager;
        private RobotManager _robotManager;
        private ZonesManager _zonesManager;
        private Configurations _configurations;
        private int _port;
        
        [TestInitialize]
        public void SetUp()
        {
            _port = 5303;
            var jersey = 1;
            var team = Team.Cyan;
            var robconf = new RobotConfig("TestBot", jersey, team);
            var mpsconf = new MpsConfig("C-BS", Mps.MpsType.BaseStation, _port, team, true);
            var teamconf = new TeamConfig("GRIPS", Team.Cyan, "127.0.0.1", 10000);

            _configurations = Configurations.GetInstance();
            _configurations.AddConfig(robconf);
            _configurations.AddConfig(mpsconf);
            _configurations.AddConfig(teamconf);
            _robotManager = new RobotManager();
            _mpsManager = MpsManager.GetInstance();
            _zonesManager = ZonesManager.GetInstance();
            MachineInfo machineinfo = new MachineInfo();
            machineinfo.Machines.Add(new Machine
            {
                Name = "C-BS",
                Type = "BS",
                TeamColor = team,
                Zone = Zone.CZ14,
                Rotation = 180
            });
            _mpsManager.PlaceMachines(machineinfo);
        }
        [TestMethod]
        public void ShortMove()
        {
            var configurations = Configurations.GetInstance();
            configurations.AddTestData();
            var robot = new Simulator.RobotEssentials.Robot("TestBot", null, Team.Cyan, 1, false);
            robot.WorkingRobotThread = new Thread(() => robot.Run());
            robot.WorkingRobotThread.Start();
            var zonesManager = ZonesManager.GetInstance();
            var zone = zonesManager.GetZone(Zone.CZ22);
            robot.SetZone(zone);
            Assert.AreEqual(robot.GetZone().ZoneId, zone.ZoneId);
            var task = new AgentTask()
            {
                TaskId = 0,
                TeamColor = Team.Cyan,
                RobotId = 1,
                Move = new Move
                {
                    Waypoint = "C_Z21",
                    MachinePoint = ""
                }
            };
            robot.SetAgentTasks(task);
            Thread.Sleep(Configurations.GetInstance().RobotMoveZoneDuration + 3000);
            Assert.AreNotEqual(robot.GetZone().ZoneId, zone.ZoneId);
        }


        [TestMethod]
        public void LongMove()
        {
            var configurations = Configurations.GetInstance();
            configurations.AddTestData();
            var robot = new Simulator.RobotEssentials.Robot("TestBot", null, Team.Cyan, 1, false);
            robot.WorkingRobotThread = new Thread(() => robot.Run());
            robot.WorkingRobotThread.Start();
            var zonesManager = ZonesManager.GetInstance();
            var zone = zonesManager.GetZone(Zone.CZ72);
            robot.SetZone(zone);
            Assert.AreEqual(robot.GetZone().ZoneId, zone.ZoneId);
            var task = new AgentTask()
            {
                TaskId = 0,
                TeamColor = Team.Cyan,
                RobotId = 1,
                Move = new Move
                {
                    Waypoint = "M_Z78",
                    MachinePoint = ""
                }
            };
            robot.SetAgentTasks(task);
            Thread.Sleep((15 * Configurations.GetInstance().RobotMoveZoneDuration) + 3000);
            Assert.AreNotEqual(robot.GetZone().ZoneId, zone.ZoneId);
        }


        [TestMethod]
        public void Place()
        {
            var product = new Products(CapColor.CapBlack);
            Assert.IsNotNull(product.RetrieveCap());
            Assert.IsNull(product.RetrieveCap());
        }


        [TestMethod]
        public void Grab()
        {
            return;
            _robotManager.Robots[0].SetZone(_zonesManager.GetZone(Zone.CZ15));
            Thread.Sleep(400);
            var bs = new TestHelper(_port);
            Thread.Sleep(2000);
            if (!bs.CreateConnection())
                Assert.Fail();
            Thread.Sleep(400);
            bs.SendTask((ushort)MPS_BS.BaseSpecificActions.GetBase, (ushort)1);
            Thread.Sleep(_configurations.BSTaskDuration+300);
            bs.SendTask((ushort)MPS_BS.BaseSpecificActions.BandOnUntil, (ushort)Positions.Out, (ushort)Direction.FromInToOut);
            Thread.Sleep(_configurations.BeltActionDuration+300);
            var task = new AgentTask
            {
                RobotId = _robotManager.Robots[0].JerseyNumber,
                TeamColor = _robotManager.Robots[0].TeamColor,
                TaskId = 1,
                Retrieve = new Retrieve
                {
                    MachineId = "C-BS",
                    MachinePoint = "output"
                }
            };
            _robotManager.Robots[0].SetAgentTasks(task);
            Thread.Sleep(_configurations.RobotPlaceDuration + 1000);
            Assert.IsTrue(_robotManager.Robots[0].IsHoldingSomething());
            bs.CloseConnection();
        }

        [TestMethod]
        public void GrabWithInvalidMachine()
        {
            return;
            _robotManager.Robots[0].SetZone(_zonesManager.GetZone(Zone.CZ15));
            Thread.Sleep(400);
            var bs = new TestHelper(_port);
            Thread.Sleep(2000);
            if (!bs.CreateConnection())
                Assert.Fail();
            Thread.Sleep(400);
            bs.SendTask((ushort)MPS_BS.BaseSpecificActions.GetBase, (ushort)1);
            Thread.Sleep(_configurations.BSTaskDuration + 300);
            bs.SendTask((ushort)MPS_BS.BaseSpecificActions.BandOnUntil, (ushort)Positions.Out, (ushort)Direction.FromInToOut);
            Thread.Sleep(_configurations.BeltActionDuration + 300);
            var task = new AgentTask
            {
                RobotId = _robotManager.Robots[0].JerseyNumber,
                TeamColor =  _robotManager.Robots[0].TeamColor,
                TaskId = 1,
                Retrieve = new Retrieve
                {
                    MachineId = "C-CS",
                    MachinePoint = "output"
                }
            };
            _robotManager.Robots[0].SetAgentTasks(task);
            Thread.Sleep(_configurations.RobotPlaceDuration);
            Assert.IsFalse(_robotManager.Robots[0].IsHoldingSomething());
            bs.CloseConnection();
        }
    }
}
