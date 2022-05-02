using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simulator.MPS;
using LlsfMsgs;
using System.Threading;
using Simulator;
using Opc.UaFx.Client;

namespace Simulatortests
{
    [TestClass]
    public class BaseStationTests
    {

        [TestMethod]
        public void DispenseBase()
        {
            var port = 5100;
            var machine = new MPS_BS("C-BS", port, 0, Team.Cyan, true);
            var thread = new Thread(machine.Run);
            var testnode = machine.InNodes.ActionId;
            thread.Start();
            Thread.Sleep(300);
            //Setting the shelf number to dispense a base
            machine.InNodes.Data0.Value = 1;
            machine.DispenseBase();
            Thread.Sleep(Configurations.GetInstance().BSTaskDuration + 100);
            Assert.IsNotNull(machine.ProductOnBelt);
        }
        [TestMethod]
        public void OPC_DispenseBase()
        {
            var port = 5101;
            var machine = new MPS_BS("C-BS", port, 0, Team.Cyan, true);
            var thread = new Thread(machine.Run);
            thread.Start();
            Thread.Sleep(300);
            Assert.AreEqual(machine.InNodes.ActionId.Value, 0);
            try
            {
                using (var client = new OpcClient("opc.tcp://localhost:" + port + "/"))
                {
                    client.Connect();
                    client.WriteNode(GeneralStationTests.NodePath + "ActionId", (ushort) MPS_BS.BaseSpecificActions.GetBase);
                    client.WriteNode(GeneralStationTests.NodePath + "Data/Data[0]", (ushort)1);
                    client.WriteNode(GeneralStationTests.NodePath + "Status/Enable", true);
                }

            }
            catch
            {
                Assert.Fail();
            }

            Thread.Sleep(Configurations.GetInstance().BSTaskDuration + 300);
            Assert.IsNotNull(machine.ProductOnBelt);
        }
        [TestMethod]
        public void OPC_WrongDispenseBase()
        {
            var port = 5102;
            var machine = new MPS_BS("C-BS", port, 0, Team.Cyan, true);
            var thread = new Thread(machine.Run);
            thread.Start();
            Thread.Sleep(300);
            Assert.AreEqual(machine.InNodes.ActionId.Value, 0);
            try
            {
                using (var client = new OpcClient("opc.tcp://localhost:" + port + "/"))
                {
                    client.Connect();
                    client.WriteNode(GeneralStationTests.NodePath + "ActionId", (ushort) MPS_BS.BaseSpecificActions.GetBase);
                    client.WriteNode(GeneralStationTests.NodePath + "Data/Data[0]", (ushort)5);
                    client.WriteNode(GeneralStationTests.NodePath + "Status/Enable", true);
                }
            }
            catch
            {
                Assert.Fail();
            }
            Assert.IsNull(machine.ProductOnBelt);
        }

        [TestMethod]
        public void OPC_DispenseBaseAndStartBeltWithReady()
        {
            var port = 5103;
            var machine = new MPS_BS("C-BS", port, 0, Team.Cyan, true);
            var thread = new Thread(machine.Run);
            thread.Start();
            Thread.Sleep(300);
            Assert.AreEqual(machine.InNodes.ActionId.Value, 0);
            var client = new OpcClient("opc.tcp://localhost:" + port + "/");
            try
            {
                client.Connect();
                client.WriteNode(GeneralStationTests.NodePath + "ActionId", (ushort) MPS_BS.BaseSpecificActions.GetBase);
                client.WriteNode(GeneralStationTests.NodePath + "Data/Data[0]", (ushort)1);
                client.WriteNode(GeneralStationTests.NodePath + "Status/Enable", true);
            }
            catch
            {
                Assert.Fail();
            }

            Thread.Sleep(Configurations.GetInstance().BSTaskDuration + 300);
            Assert.IsNotNull(machine.ProductOnBelt);
            try
            {
                client.WriteNode(GeneralStationTests.NodePath + "ActionId", (ushort) MPS_BS.BaseSpecificActions.BandOnUntil);
                client.WriteNode(GeneralStationTests.NodePath + "Data/Data[0]", (ushort)Positions.Out);
                client.WriteNode(GeneralStationTests.NodePath + "Data/Data[1]", (ushort)Direction.FromInToOut);
                client.WriteNode(GeneralStationTests.NodePath + "Status/Enable", true);
            }
            catch
            {
                Assert.Fail();
            }
            Thread.Sleep(Configurations.GetInstance().BSTaskDuration + 100);
            Assert.IsNotNull(machine.ProductAtOut);
            Assert.IsNull(machine.ProductOnBelt);
            Assert.AreEqual(machine.InNodes.StatusNodes.ready.Value, true);
            client.Disconnect();
        }
    }
}
