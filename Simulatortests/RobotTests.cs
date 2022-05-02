using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simulator.Utility;

using Simulator;
using LlsfMsgs;
using System.Threading;

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
            var task = new GripsMidlevelTasks()
            {
                TaskId = 0,
                TeamColor = Team.Cyan,
                RobotId = 1,
                MoveToWaypoint = new MoveToWaypoint
                {
                    Waypoint = "C_Z21"
                }
            };
            robot.SetGripsTasks(task);
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
            var task = new GripsMidlevelTasks()
            {
                TaskId = 0,
                TeamColor = Team.Cyan,
                RobotId = 1,
                MoveToWaypoint = new MoveToWaypoint
                {
                    Waypoint = "M_Z78"
                }
            };
            robot.SetGripsTasks(task);
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
            var product = new Products(BaseColor.BaseBlack);
            product.AddPart(new RingElement(RingColor.RingBlue));
            product.AddPart(new CapElement(CapColor.CapBlack));
            Assert.AreEqual(product.Complexity, Order.Types.Complexity.C1);
        }
    }
}