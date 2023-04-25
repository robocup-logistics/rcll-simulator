using System;
using System.Diagnostics;
using System.Net.Sockets;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simulator.MPS;
using System.Threading;
using LlsfMsgs;
using Simulator;
using Opc.UaFx.Client;
using Simulator.RobotEssentials;
using Robot = Simulator.RobotEssentials.Robot;

namespace Simulatortests
{
    [TestClass]
    public class NetworkTests
    {
        [TestMethod]
        public void TcpConnectorTest()
        {
            return;
            var port = 5500;
            var teamname = "TestTeam";
            var ip = "localhost";
            var teamcolor = Team.Cyan;
            var tcp = new TCPTestHelper(5500);
            tcp.CreateConnection();
            var config = new Configurations();
            Thread.Sleep(1000);
            config.AddConfig(new RobotConfig("TestBot", 1, teamcolor));
            config.AddConfig(new TeamConfig(teamname, teamcolor, ip, port));
            config.AddConfig(new RefboxConfig(ip, port,port,port,port,port,port,port));
            config.ToggleMockUp();
            var mpsManager = new MpsManager(config);
            var robotManager = new RobotManager(config,mpsManager);

            Thread.Sleep(19000);
            Assert.IsTrue(true);
        }
    }
}
