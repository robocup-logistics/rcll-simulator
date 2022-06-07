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
            var port = 6000;
            var jersey = 1;
            var team = Team.Cyan;
            var robconf = new RobotConfig("TestBot", jersey, team);
            var mpsconf = new MpsConfig("C-BS", Mps.MpsType.BaseStation, port, team, true);
            var teamconf = new TeamConfig("GRIPS", Team.Cyan, "127.0.0.1", 10000);

            var config = Configurations.GetInstance();
            config.AddConfig(robconf);
            config.AddConfig(mpsconf);
            config.AddConfig(teamconf);
            var robotmanger = new RobotManager();
            var mpsmanager = MpsManager.GetInstance();
            var zonesmanager = ZonesManager.GetInstance();
            MachineInfo machineinfo = new MachineInfo();
            machineinfo.Machines.Add(new Machine
            {
                Name = "C-BS",
                Type = "BS",
                TeamColor = team,
                Zone = Zone.CZ14,
                Rotation = 180
            });
            mpsmanager.PlaceMachines(machineinfo);
            robotmanger.Robots[0].SetZone(zonesmanager.GetZone(Zone.CZ15));
            Thread.Sleep(400);
            var bs = new TestHelper(port);
            if (!bs.CreateConnection())
                Assert.Fail();
            Thread.Sleep(400);
            bs.SendTask((ushort)MPS_BS.BaseSpecificActions.GetBase, (ushort)1);
            Thread.Sleep(config.BSTaskDuration+300);
            bs.SendTask((ushort)MPS_BS.BaseSpecificActions.BandOnUntil, (ushort)Positions.Out, (ushort)Direction.FromInToOut);
            Thread.Sleep(config.BeltActionDuration+300);
            var task = new AgentTask
            {
                RobotId = (uint)jersey,
                TeamColor = team,
                TaskId = 1,
                Retrieve = new Retrieve
                {
                    MachineId = "C-BS",
                    MachinePoint = "output"
                }
            };
            robotmanger.Robots[0].SetAgentTasks(task);
            Thread.Sleep(config.RobotPlaceDuration + 1000);
            Assert.IsTrue(robotmanger.Robots[0].IsHoldingSomething());
        }

        [TestMethod]
        public void GrabWithInvalidMachine()
        {
            var port = 6001;
            var jersey = 1;
            var team = Team.Cyan;
            var robconf = new RobotConfig("TestBot", jersey, team);
            var mpsconf = new MpsConfig("C-BS", Mps.MpsType.BaseStation, port, team, true);
            var teamconf = new TeamConfig("GRIPS", Team.Cyan, "127.0.0.1", 10000);

            var config = Configurations.GetInstance();
            config.AddConfig(robconf);
            config.AddConfig(mpsconf);
            config.AddConfig(teamconf);
            var robotmanger = new RobotManager();
            var mpsmanager = MpsManager.GetInstance();
            var zonesmanager = ZonesManager.GetInstance();
            MachineInfo machineinfo = new MachineInfo();
            machineinfo.Machines.Add(new Machine
            {
                Name = "C-BS",
                Type = "BS",
                TeamColor = team,
                Zone = Zone.CZ14,
                Rotation = 180
            });
            mpsmanager.PlaceMachines(machineinfo);
            robotmanger.Robots[0].SetZone(zonesmanager.GetZone(Zone.CZ15));
            Thread.Sleep(400);
            var bs = new TestHelper(port);
            if (!bs.CreateConnection())
                Assert.Fail();
            Thread.Sleep(400);
            bs.SendTask((ushort)MPS_BS.BaseSpecificActions.GetBase, (ushort)1);
            Thread.Sleep(config.BSTaskDuration + 300);
            bs.SendTask((ushort)MPS_BS.BaseSpecificActions.BandOnUntil, (ushort)Positions.Out, (ushort)Direction.FromInToOut);
            Thread.Sleep(config.BeltActionDuration + 300);
            var task = new AgentTask
            {
                RobotId = (uint)jersey,
                TeamColor = team,
                TaskId = 1,
                Retrieve = new Retrieve
                {
                    MachineId = "C-CS",
                    MachinePoint = "output"
                }
            };
            robotmanger.Robots[0].SetAgentTasks(task);
            Thread.Sleep(config.RobotPlaceDuration);
            Assert.IsFalse(robotmanger.Robots[0].IsHoldingSomething());
        }
    }
}