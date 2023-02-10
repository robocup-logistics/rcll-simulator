using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simulator.MPS;
using LlsfMsgs;
using System.Threading;
using Simulator;
using Simulator.Utility;
using Simulator.RobotEssentials;

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
            var config = new Configurations();
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
            var machinemanager = new MpsManager(config);
            var robotmanager = new RobotManager(config, machinemanager);
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
            var movetask = new AgentTask
            {
                TaskId = 1,
                TeamColor = Team.Cyan,
                RobotId = 1,
                Move = new Move
                {
                    Waypoint = "C-CS",
                    MachinePoint = "input"

                }
            };
            var gettask = new AgentTask
            {
                TaskId = 1,
                TeamColor = Team.Cyan,
                RobotId = 1,
                Retrieve = new Retrieve
                {
                    MachineId = "C-CS",
                    MachinePoint = "output"
                }
            };
            AgentTask puttask = new AgentTask()
            {
                TaskId = 1,
                TeamColor = Team.Cyan,
                RobotId = 1,
                Deliver = new Deliver
                {
                    MachineId = "C-RS",
                    MachinePoint = "slide"
                }
            };
            var buffertask = new AgentTask
            {

                TaskId = 2,
                TeamColor = Team.Cyan,
                RobotId = 1,
                Buffer = new BufferStation
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
            Thread.Sleep(500);

            // --------------- Start of the test ------------------

            Assert.AreEqual(startzone, rob.GetZone().ZoneId);
            movetask.Move.Waypoint = "C-CS";
            movetask.Move.MachinePoint = "input";
            rob.SetAgentTasks(movetask);
            Thread.Sleep((10 * config.RobotMoveZoneDuration) + 300);
            Assert.AreNotEqual(startzone, rob.GetZone().ZoneId);
            Assert.AreEqual(zonesManager.GetWaypoint("C-CS_input"), rob.GetZone().ZoneId);
            buffertask.Buffer = new BufferStation
            {
                MachineId = "C-CS",
                ShelfNumber = 1
            };
            rob.SetAgentTasks(buffertask); 
            Thread.Sleep(config.CSTaskDuration * 3 + config.RobotPlaceDuration * 3 + 100);
            //Assert.AreEqual(true, rob.IsHoldingSomething());
            movetask.Move.Waypoint = "C-CS";
            movetask.Move.MachinePoint = "output";
            rob.SetAgentTasks(movetask);
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
            gettask.Retrieve = new Retrieve
            {
                MachineId = "C-CS",
                MachinePoint = "output"
            };

            rob.SetAgentTasks(gettask);
            Thread.Sleep(config.RobotGrabProductDuration * 20);
            Assert.IsTrue(rob.IsHoldingSomething());
            movetask.Move.Waypoint = "C-RS";
            movetask.Move.MachinePoint = "slide";
            rob.SetAgentTasks(movetask);
            Thread.Sleep((config.RobotMoveZoneDuration * 30));
            Assert.AreEqual(zonesManager.GetWaypoint("C-RS_slide"), rob.GetZone().ZoneId);
            puttask.Deliver = new Deliver
            {
                MachineId = "C-RS",
                MachinePoint = "slide"
            };
            rob.SetAgentTasks(puttask);
            Thread.Sleep(config.RobotPlaceDuration * 10);
            Assert.AreEqual(1, machinemanager.Machines[3].InNodes.SlideCnt.Value);
            movetask.Move.Waypoint = "C-BS";
            movetask.Move.MachinePoint = "output";
            rob.SetAgentTasks(movetask);
            Thread.Sleep(config.RobotMoveZoneDuration * 5 + 100);

            bs.SendTask((ushort)MPS_BS.BaseSpecificActions.GetBase, 1);
            Thread.Sleep(config.BSTaskDuration + 100);
            bs.SendTask((ushort)MPS_BS.BaseSpecificActions.BandOnUntil, (ushort)Positions.Out, (ushort)Direction.FromInToOut);
            Thread.Sleep(config.BeltActionDuration + 100);
            gettask.Retrieve = new Retrieve
            {
                MachineId = "C-BS",
                MachinePoint = "output"
            };
            rob.SetAgentTasks(gettask);
            Thread.Sleep(config.RobotGrabProductDuration * 20);
            Assert.IsTrue(rob.IsHoldingSomething());
            movetask.Move.Waypoint = "C-CS";
            movetask.Move.MachinePoint = "input";
            rob.SetAgentTasks(movetask);
            Thread.Sleep((config.RobotMoveZoneDuration * 5) + 300);
            puttask.Retrieve = new Retrieve
            {
                MachineId = "C-CS",
                MachinePoint = "input"
            };
            rob.SetAgentTasks(puttask);
            Thread.Sleep(config.RobotPlaceDuration * 10);
            movetask.Move.Waypoint = "C-CS";
            movetask.Move.MachinePoint = "output";
            rob.SetAgentTasks(movetask);
            cs.SendTask((ushort)MPS_CS.BaseSpecificActions.BandOnUntil, (ushort)Positions.Mid, (ushort)Direction.FromInToOut);
            Thread.Sleep(config.BeltActionDuration + 300);
            cs.SendTask((ushort)MPS_CS.BaseSpecificActions.Cap, (ushort)CSOp.MountCap);
            Thread.Sleep(config.CSTaskDuration + 300);
            cs.SendTask((ushort)MPS_CS.BaseSpecificActions.BandOnUntil, (ushort)Positions.Out, (ushort)Direction.FromInToOut);
            Thread.Sleep(config.BeltActionDuration + 300);
            Thread.Sleep(config.RobotMoveZoneDuration * 3 + 200);
            gettask.Retrieve = new Retrieve
            {
                MachineId = "C-CS",
                MachinePoint = "output"
            };
            //return;
            //Thread.Sleep(6000); // waiting for finished move

            rob.SetAgentTasks(gettask);
            Thread.Sleep(config.RobotGrabProductDuration + 300); // grabing an item
            Assert.IsTrue(rob.IsHoldingSomething());
            movetask.Move.Waypoint = "C-DS";
            movetask.Move.MachinePoint = "";
            rob.SetAgentTasks(movetask);
            Thread.Sleep(config.RobotMoveZoneDuration * 6 + 300);
            puttask.Deliver = new Deliver
            {
                MachineId = "C-DS",
                MachinePoint = "input"
            };
            rob.SetAgentTasks(puttask);
            Thread.Sleep(config.RobotPlaceDuration + 300);
            ds.SendTask((ushort)MPS_DS.BaseSpecificActions.DeliverToSlot, (ushort)1);
            Thread.Sleep(config.DSTaskDuration + 300);
            Assert.IsNotNull(((MPS_DS)machinemanager.Machines[2]).ProductAtSlot(1));
            bs.CloseConnection();
            cs.CloseConnection();
            ds.CloseConnection();
        }

        [TestMethod]
        public void CreateOneC1()
        {
            Assert.AreEqual(3,3);
        }
    }
}
