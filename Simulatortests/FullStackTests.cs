using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simulator.MPS;
using LlsfMsgs;
using System.Threading;
using Simulator;
using Simulator.Utility;
using Simulator.RobotEssentials;
using Simulator.MPS;

namespace Simulatortests
{
    [TestClass]
    public class FullStackTests
    {
        [TestMethod]
        public void CreateOneC0()
        {

            /***
             * Setup of this Test
             * One Robot, a BaseStation, a CapStation and a Delivery Station
             * Goal is a Produced C0
             * **/
            var config = Configurations.GetInstance();
            var startzone = Zone.CZ52;
            var robotconf = new RobotConfig("TestBot", 0, Team.Cyan);
            var bsconfig = new MpsConfig("C-BS", Mps.MpsType.BaseStation, 10000, Team.Cyan, true);
            var csconfig = new MpsConfig("C-CS", Mps.MpsType.CapStation, 10001, Team.Cyan, true);
            var dsconfig = new MpsConfig("C-DS", Mps.MpsType.DeliveryStation, 10002, Team.Cyan, true);
            var rsconfig = new MpsConfig("C-RS", Mps.MpsType.RingStation, 10003, Team.Cyan, true);
            var teamconf = new TeamConfig("GRIPS", Team.Cyan, "127.0.0.1", 10000);
            config.AddConfig(robotconf);
            config.AddConfig(bsconfig);
            config.AddConfig(csconfig);
            config.AddConfig(dsconfig);
            config.AddConfig(rsconfig);
            config.AddConfig(teamconf);
            var robotmanager = new RobotManager();
            var machinemanager = MpsManager.GetInstance();
            var zonesManager = ZonesManager.GetInstance();
            var rob = robotmanager.Robots[0];
            rob.SetZone(zonesManager.GetZone(startzone));

            var bs = new TestHelper(bsconfig.Port);
            var cs = new TestHelper(csconfig.Port);
            var ds = new TestHelper(dsconfig.Port);
            if (!bs.CreateConnection())
                Assert.Fail();
            if (!cs.CreateConnection())
                Assert.Fail();
            if (!ds.CreateConnection())
                Assert.Fail();
            MachineInfo machineinfo = new MachineInfo();
            var movetask = new GripsMidlevelTasks
            {
                TaskId = 1,
                TeamColor = Team.Cyan,
                RobotId = 1,
                MoveToWaypoint = new MoveToWaypoint
                {
                    Waypoint = "C-CS_input"
                }
            };
            var gettask = new GripsMidlevelTasks
            {
                TaskId = 1,
                TeamColor = Team.Cyan,
                RobotId = 1,
                GetFromStation = new GetFromStation
                {
                    MachineId = "C-CS",
                    MachinePoint = "output"
                }
            };
            GripsMidlevelTasks puttask = new GripsMidlevelTasks()
            {
                TaskId = 1,
                TeamColor = Team.Cyan,
                RobotId = 1,
                DeliverToStation = new DeliverToStation
                {
                    MachineId = "C-RS",
                    MachinePoint = "slide"
                }
            };
            var buffertask = new GripsMidlevelTasks
            {

                TaskId = 2,
                TeamColor = Team.Cyan,
                RobotId = 1,
                BufferCapStation = new BufferCapStation
                {
                    MachineId = "C-CS",
                    ShelfNumber = 1
                }


            };
            machineinfo.Machines.Add(new Machine
            {
                Name = "C-BS",
                Type = "BS",
                TeamColor = Team.Cyan,
                Zone = Zone.CZ14,
                Rotation = 180
            });
            machineinfo.Machines.Add(new Machine
            {
                Name = "C-CS",
                Type = "CS",
                TeamColor = Team.Cyan,
                Zone = Zone.CZ63,
                Rotation = 180
            });
            machineinfo.Machines.Add(new Machine
            {
                Name = "C-DS",
                Type = "DS",
                TeamColor = Team.Cyan,
                Zone = Zone.CZ38,
                Rotation = 180
            });
            machineinfo.Machines.Add(new Machine
            {
                Name = "C-RS",
                Type = "RS",
                TeamColor = Team.Cyan,
                Zone = Zone.CZ11,
                Rotation = 0
            });
            machinemanager.PlaceMachines(machineinfo);
            Thread.Sleep(300);

            // --------------- Start of the test ------------------

            Assert.AreEqual(startzone, rob.GetZone().ZoneId);
            movetask.MoveToWaypoint.Waypoint = "C-CS_input";
            rob.SetGripsTasks(movetask);
            Thread.Sleep((3 * config.RobotMoveZoneDuration) + 300);
            Assert.AreNotEqual(startzone, rob.GetZone().ZoneId);
            Assert.AreEqual(zonesManager.GetWaypoint("C-CS_input"), rob.GetZone().ZoneId);
            buffertask.BufferCapStation = new BufferCapStation
            {
                MachineId = "C-CS",
                ShelfNumber = 1
            };
            rob.SetGripsTasks(buffertask);
            Thread.Sleep(config.CSTaskDuration * 2);
            //Assert.AreEqual(true, rob.IsHoldingSomething());
            movetask.MoveToWaypoint.Waypoint = "C-CS_output";
            rob.SetGripsTasks(movetask);
            Assert.IsNotNull(machinemanager.Machines[1].ProductAtIn);
            cs.SendTask((ushort)MPS_CS.BaseSpecificActions.BandOnUntil, (ushort)Positions.Mid, (ushort)Direction.FromInToOut);
            Thread.Sleep(config.BeltActionDuration + 100);
            Assert.IsNotNull(machinemanager.Machines[1].ProductOnBelt);
            cs.SendTask((ushort)MPS_CS.BaseSpecificActions.Cap, (ushort)CSOp.RetrieveCap);
            Thread.Sleep(config.CSTaskDuration + 300);
            Assert.IsNotNull(((MPS_CS)machinemanager.Machines[1]).StoredCap);
            cs.SendTask((ushort)MPS_CS.BaseSpecificActions.BandOnUntil, (ushort)Positions.Out, (ushort)Direction.FromInToOut);
            Thread.Sleep(config.BeltActionDuration + 100);
            Assert.IsNotNull(machinemanager.Machines[1].ProductAtOut);
            gettask.GetFromStation = new GetFromStation
            {
                MachineId = "C-CS",
                MachinePoint = "output"
            };

            rob.SetGripsTasks(gettask);
            Thread.Sleep(3000);
            Assert.IsTrue(rob.IsHoldingSomething());
            movetask.MoveToWaypoint.Waypoint = "C-RS_slide";
            rob.SetGripsTasks(movetask);
            Thread.Sleep((config.RobotMoveZoneDuration * 6) + 100);
            Assert.AreEqual(zonesManager.GetWaypoint("C-RS_slide"), rob.GetZone().ZoneId);
            puttask.DeliverToStation = new DeliverToStation
            {
                MachineId = "C-RS",
                MachinePoint = "slide"
            };
            rob.SetGripsTasks(puttask);
            Thread.Sleep(6000);
            Assert.AreEqual(1, machinemanager.Machines[3].InNodes.SlideCnt.Value);
            movetask.MoveToWaypoint.Waypoint = "C-BS_output";
            rob.SetGripsTasks(movetask);
            Thread.Sleep(config.RobotMoveZoneDuration * 5 + 100);
            
            bs.SendTask((ushort)MPS_BS.BaseSpecificActions.GetBase, 1);
            Thread.Sleep(config.BSTaskDuration + 100);
            bs.SendTask((ushort)MPS_BS.BaseSpecificActions.BandOnUntil, (ushort)Positions.Out, (ushort)Direction.FromInToOut);
            Thread.Sleep(config.BeltActionDuration + 100);
            gettask.GetFromStation = new GetFromStation
            {
                MachineId = "C-BS",
                MachinePoint = "output"
            };
            rob.SetGripsTasks(gettask);
            Thread.Sleep(8000);
            Assert.IsTrue(rob.IsHoldingSomething());
            movetask.MoveToWaypoint.Waypoint = "C-CS_input";
            rob.SetGripsTasks(movetask);
            Thread.Sleep((config.RobotMoveZoneDuration * 5) + 300);
            puttask.DeliverToStation = new DeliverToStation
            {
                MachineId = "C-CS",
                MachinePoint = "input"
            };
            rob.SetGripsTasks(puttask);
            Thread.Sleep(4000);
            movetask.MoveToWaypoint.Waypoint = "C-CS_output";
            rob.SetGripsTasks(movetask);
            cs.SendTask((ushort)MPS_CS.BaseSpecificActions.BandOnUntil, (ushort)Positions.Mid, (ushort)Direction.FromInToOut);
            Thread.Sleep(config.BeltActionDuration+300);
            cs.SendTask((ushort)MPS_CS.BaseSpecificActions.Cap, (ushort)CSOp.MountCap);
            Thread.Sleep(config.CSTaskDuration + 300);
            cs.SendTask((ushort)MPS_CS.BaseSpecificActions.BandOnUntil, (ushort)Positions.Out, (ushort)Direction.FromInToOut);
            Thread.Sleep(config.BeltActionDuration + 300);
            gettask.GetFromStation = new GetFromStation
            {
                MachineId = "C-CS",
                MachinePoint = "output"
            };
            rob.SetGripsTasks(gettask);
            Thread.Sleep(13000);
            Assert.IsTrue(rob.IsHoldingSomething());
            movetask.MoveToWaypoint.Waypoint = "C-DS";
            rob.SetGripsTasks(movetask);
            Thread.Sleep(config.RobotMoveZoneDuration * 6 + 300);
            puttask.DeliverToStation = new DeliverToStation
            {
                MachineId = "C-DS",
                MachinePoint = "input"
            };
            rob.SetGripsTasks(puttask);
            Thread.Sleep(3000);
            ds.SendTask((ushort)MPS_DS.BaseSpecificActions.DeliverToSlot, (ushort)1);
            Thread.Sleep(config.DSTaskDuration + 300);
            Assert.IsNotNull(((MPS_DS)machinemanager.Machines[2]).ProductAtSlot(1));
        }

    }
}
