﻿using LlsfMsgs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simulator;
using Simulator.MPS;
using Simulator.Utility;
using System.Threading;

namespace Simulatortests
{
    [TestClass]
    public class BaseStationTests
    {

        [TestMethod]
        public void DispenseBase()
        {
            var port = 5100;
            var config = new Configurations();
            var machine = new MPS_BS(config, "C-BS", port, 0, Team.Cyan, true);
            //Setting the shelf number to dispense a base
            machine.InNodes.Data0.Value = 1;
            machine.DispenseBase();
            Assert.IsNotNull(machine.ProductOnBelt);
        }

        [TestMethod]
        public void TestResetMps()
        {
            var port = 5104;
            var config = new Configurations();
            var machine = new MPS_BS(config, "C-BS", port, 0, Team.Cyan, true);
            //Setting the shelf number to dispense a base
            machine.InNodes.Data0.Value = 1;
            machine.DispenseBase();
            machine.ProductAtOut = new Products(BaseColor.BaseBlack);
            machine.ProductAtIn = new Products(BaseColor.BaseRed);
            Assert.IsNotNull(machine.ProductAtIn);
            Assert.IsNotNull(machine.ProductOnBelt);
            Assert.IsNotNull(machine.ProductAtOut);
            machine.ResetMachine();
            Assert.IsNull(machine.ProductAtIn);
            Assert.IsNull(machine.ProductOnBelt);
            Assert.IsNull(machine.ProductAtOut);
        }
        
        [TestMethod]
        public void OPC_DispenseBase()
        {
            var port = 5101;
            var config = new Configurations();
            var machine = new MPS_BS(config,"C-BS", port, 0, Team.Cyan, true);
            var thread = new Thread(machine.Run);
            thread.Start();
            Thread.Sleep(500);
            Assert.AreEqual(machine.InNodes.ActionId.Value, 0);
            var testHelper = new OPCTestHelper(port);
            if (!testHelper.CreateConnection())
                Assert.Fail();
            testHelper.SendTask((ushort)MPS_BS.BaseSpecificActions.GetBase, (ushort)1, (ushort)0);
            Thread.Sleep(config.BSTaskDuration + 300);
            Assert.IsNotNull(machine.ProductOnBelt);
            testHelper.CloseConnection();
            machine.StopMachine();
        }

        [TestMethod]
        public void MQTT_DispenseBase()
        {
            var ip = "mosquitto";
            var port = 1883;
            var name = "C-BS";
            var config = new Configurations();
            config.Refbox = new RefboxConfig(ip:"127.0.0.1",0,0,0,0,0,0,0, ip, port, true);
            var machine = new MPS_BS(config, name, port, 0, Team.Cyan, true);
            var thread = new Thread(machine.Run);
            thread.Start();
            Thread.Sleep(500);
            Assert.AreEqual(machine.MqttHelper.InNodes.ActionId, 0);
            var testHelper = new MQTTTestHelper(ip, port, name);
            if (!testHelper.CreateConnection())
                Assert.Fail();
            testHelper.SendTask((ushort)MPS_BS.BaseSpecificActions.GetBase, (ushort)1, (ushort)0);
            Thread.Sleep(config.BSTaskDuration + 300);
            Assert.IsNotNull(machine.ProductOnBelt);
            testHelper.CloseConnection();
            machine.StopMachine();
        }


        [TestMethod]
        public void OPC_WrongDispenseBase()
        {
            var port = 5102;
            var config = new Configurations();
            var machine = new MPS_BS(config, "C-BS", port, 0, Team.Cyan, true);
            var thread = new Thread(machine.Run);
            thread.Start();
            Thread.Sleep(500);
            Assert.AreEqual(machine.InNodes.ActionId.Value, 0);
            var testHelper = new OPCTestHelper(port);
            if (!testHelper.CreateConnection())
                Assert.Fail();
            testHelper.SendTask((ushort)MPS_BS.BaseSpecificActions.GetBase, (ushort)5, (ushort)1);
            Assert.IsNull(machine.ProductOnBelt);
            testHelper.CloseConnection();
            machine.StopMachine();
        }

        [TestMethod]
        public void OPC_DispenseBaseAndStartBeltWithReady()
        {
            var port = 5103;
            var config = new Configurations();
            var machine = new MPS_BS(config, "C-BS", port, 0, Team.Cyan, true);
            var thread = new Thread(machine.Run);
            thread.Start();
            Thread.Sleep(500);
            Assert.AreEqual(machine.InNodes.ActionId.Value, 0);
            var testHelper = new OPCTestHelper(port);
            if (!testHelper.CreateConnection())
                Assert.Fail();
            testHelper.SendTask((ushort)MPS_BS.BaseSpecificActions.GetBase, (ushort)1, (ushort)1);
            Thread.Sleep(config.BSTaskDuration + 300);
            Assert.IsNotNull(machine.ProductOnBelt);
            testHelper.SendTask((ushort)MPS_BS.BaseSpecificActions.BandOnUntil, (ushort)Positions.Out, (ushort)Direction.FromInToOut);
           
            Thread.Sleep(config.BSTaskDuration + 100);
            Assert.IsNotNull(machine.ProductAtOut);
            Assert.IsNull(machine.ProductOnBelt);
            Assert.AreEqual(machine.InNodes.StatusNodes.ready.Value, true);
            testHelper.CloseConnection();
            machine.StopMachine();
        }

       

    }
}
