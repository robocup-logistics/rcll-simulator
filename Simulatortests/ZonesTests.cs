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
    public class ZonesTests
    {
        
        private MpsManager _mpsManager;
        private RobotManager _robotManager;
        private ZonesManager _zonesManager;
        private Configurations _configurations;
        private int _port;
        private int jersey = 1;
        private Team team = Team.Cyan;
        private string machineName = "M-BS";
        private TeamConfig TeamConfig = new TeamConfig("GRIPS", Team.Cyan, "127.0.0.1", 10000);
        
        [TestMethod]
        public void PathPlaningBorderTestsMagenta()
        {
            _port = 7000;
            _configurations = new Configurations();
            _configurations.AddConfig( new RobotConfig("TestBot", jersey, team ,"Test"));
            _configurations.AddConfig(new MpsConfig(machineName, Mps.MpsType.BaseStation, _port, team, true));
            _configurations.AddConfig(TeamConfig);
            _mpsManager = new MpsManager(_configurations, false);
            _robotManager = new RobotManager(_configurations, _mpsManager);
            _zonesManager = ZonesManager.GetInstance();
            MachineInfo machineinfo = new MachineInfo();
            machineinfo.Machines.Add(new Machine
            {
                Name = machineName,
                Type = "BS",
                TeamColor = team,
                Zone = Zone.MZ14,
                Rotation = 0
            });
            _mpsManager.PlaceMachines(machineinfo);
            var initialZone = _zonesManager.GetZone(Zone.CZ11);
            var robot = _robotManager.Robots[0];
            robot.SetZone(initialZone);
            Assert.AreEqual(robot.GetZone().ZoneId, initialZone.ZoneId);
            
            var task = new AgentTask();
            task.Move = new Move()
            {
                Waypoint = machineName,
                MachinePoint = "input"
            };
            robot.SetAgentTasks(task);
            Thread.Sleep(2000);
            Assert.AreNotEqual(robot.GetZone().ZoneId, initialZone.ZoneId);
            Thread.Sleep(3 * _configurations.RobotMoveZoneDuration + 300);
            Assert.AreEqual(robot.GetZone().ZoneId, Zone.CZ14);
            _mpsManager.StopAllMachines();
            robot.RobotStop();
        }
        
        [TestMethod]
        public void PathPlaningBorderTestsCyan()
        {
            _port = 7001;
            
            _configurations = new Configurations();
            _configurations.AddConfig( new RobotConfig("TestBot", jersey, team,"Test"));
            _configurations.AddConfig(new MpsConfig(machineName, Mps.MpsType.BaseStation, _port, team, true));
            _configurations.AddConfig(TeamConfig);
            _mpsManager = new MpsManager(_configurations, false);
            _robotManager = new RobotManager(_configurations, _mpsManager);
            _zonesManager = ZonesManager.GetInstance();
            MachineInfo machineinfo = new MachineInfo();
            machineinfo.Machines.Add(new Machine
            {
                Name = machineName,
                Type = "BS",
                TeamColor = team,
                Zone = Zone.CZ14,
                Rotation = 180
            });
            _mpsManager.PlaceMachines(machineinfo);
            var initialZone = _zonesManager.GetZone(Zone.CZ11);
            var robot = _robotManager.Robots[0];
            robot.SetZone(initialZone);
            Assert.AreEqual(robot.GetZone().ZoneId, initialZone.ZoneId);
            
            var task = new AgentTask();
            task.Move = new Move()
            {
                Waypoint = machineName,
                MachinePoint = "input"
            };
            robot.SetAgentTasks(task);
            Thread.Sleep(2000);
            Assert.AreNotEqual(robot.GetZone().ZoneId, initialZone.ZoneId);
            Thread.Sleep(3 * _configurations.RobotMoveZoneDuration + 300);
            Assert.AreEqual(robot.GetZone().ZoneId, Zone.MZ14);
            _mpsManager.StopAllMachines();
            robot.RobotStop();
        }
        
        [TestMethod]
        public void PathPlaningFieldMagenta()
        {
            _port = 7002;
            
            _configurations = new Configurations();
            _configurations.AddConfig( new RobotConfig("TestBot", jersey, team, "Test"));
            _configurations.AddConfig(new MpsConfig(machineName, Mps.MpsType.BaseStation, _port, team, true));
            _configurations.AddConfig(TeamConfig);
            _mpsManager = new MpsManager(_configurations, false);
            _robotManager = new RobotManager(_configurations, _mpsManager);
            _zonesManager = ZonesManager.GetInstance();
            MachineInfo machineinfo = new MachineInfo();
            machineinfo.Machines.Add(new Machine
            {
                Name = machineName,
                Type = "BS",
                TeamColor = team,
                Zone = Zone.MZ45,
                Rotation = 0
            });
            _mpsManager.PlaceMachines(machineinfo);
            var initialZone = _zonesManager.GetZone(Zone.MZ11);
            var robot = _robotManager.Robots[0];
            robot.SetZone(initialZone);
            Assert.AreEqual(robot.GetZone().ZoneId, initialZone.ZoneId);
            
            var task = new AgentTask();
            task.Move = new Move()
            {
                Waypoint = machineName,
                MachinePoint = "input"
            };
            robot.SetAgentTasks(task);
            Thread.Sleep(2000);
            Assert.AreNotEqual(robot.GetZone().ZoneId, initialZone.ZoneId);
            Thread.Sleep(5 * _configurations.RobotMoveZoneDuration + 300);
            Assert.AreEqual(robot.GetZone().ZoneId, Zone.MZ35);
            _mpsManager.StopAllMachines();
            robot.RobotStop();
        }
        
        [TestMethod]
        public void PathPlaningFieldCyan()
        {
            _port = 7003;
            
            _configurations = new Configurations();
            _configurations.AddConfig( new RobotConfig("TestBot", jersey, team, "Test"));
            _configurations.AddConfig(new MpsConfig(machineName, Mps.MpsType.BaseStation, _port, team, true));
            _configurations.AddConfig(TeamConfig);
            _mpsManager = new MpsManager(_configurations, false);
            _robotManager = new RobotManager(_configurations, _mpsManager);
            _zonesManager = ZonesManager.GetInstance();
            MachineInfo machineinfo = new MachineInfo();
            machineinfo.Machines.Add(new Machine
            {
                Name = machineName,
                Type = "BS",
                TeamColor = team,
                Zone = Zone.CZ45,
                Rotation = 0
            });
            _mpsManager.PlaceMachines(machineinfo);
            var initialZone = _zonesManager.GetZone(Zone.CZ11);
            var robot = _robotManager.Robots[0];
            robot.SetZone(initialZone);
            Assert.AreEqual(robot.GetZone().ZoneId, initialZone.ZoneId);
            
            var task = new AgentTask();
            task.Move = new Move()
            {
                Waypoint = machineName,
                MachinePoint = "input"
            };
            robot.SetAgentTasks(task);
            Thread.Sleep(2000);
            Assert.AreNotEqual(robot.GetZone().ZoneId, initialZone.ZoneId);
            Thread.Sleep(5 * _configurations.RobotMoveZoneDuration + 300);
            Assert.AreEqual(robot.GetZone().ZoneId, Zone.CZ55);
            _mpsManager.StopAllMachines();
            robot.RobotStop();
        }
    }
}
