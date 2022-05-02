using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simulator.MPS;
using LlsfMsgs;
using System.Threading;
using Simulator;
using Simulator.Utility;
using Opc.UaFx.Client;

namespace Simulatortests
{
    [TestClass]
    public class GeneralStationTests
    {
        public static string NodePath = "ns=4;s=DeviceSet/CPX-E-CEC-C1-PN/Resources/Application/GlobalVars/G/In/p/";


        [TestMethod]
        public void ResetMachine()
        {
            var machine = new MPS_BS("C-BS", 5001, 0, Team.Cyan, true);
            var thread = new Thread(machine.Run);
            thread.Start();
            Thread.Sleep(300);
            var testnode = machine.InNodes.ActionId;
            testnode.Value = 10;
            Assert.AreNotEqual(testnode.Value, 0);
            machine.ResetMachine();
            Thread.Sleep(1400);
            Assert.AreEqual(testnode.Value, 0);
        }
        [TestMethod]
        public void BeltTest1()
        {
            var machine = new MPS_BS("C-BS", 5002, 0, Team.Cyan, true);
            var thread = new Thread(machine.Run);
            var product = new Products(CapColor.CapBlack);
            thread.Start();
            Thread.Sleep(300);
            machine.InNodes.Data0.Value = 1;
            machine.DispenseBase();
            Thread.Sleep(Configurations.GetInstance().BSTaskDuration + 100);
            Assert.IsNotNull(machine.ProductOnBelt);

            machine.InNodes.Data0.Value = (ushort)Positions.Out;
            machine.InNodes.Data1.Value = (ushort)Direction.FromInToOut;
            machine.HandleBelt();
            Thread.Sleep(Configurations.GetInstance().BeltActionDuration + 100);
            Assert.IsNotNull(machine.ProductAtOut);
            Assert.IsNull(machine.ProductOnBelt);
        }
        [TestMethod]
        public void OPC_Connection()
        {
            // Testing the opc connection generally
            var port = 5003;
            var machine = new MPS_BS("C-BS", 5003, 0, Team.Cyan, true);
            var thread = new Thread(machine.Run);
            var product = new Products(CapColor.CapBlack);
            thread.Start();
            Thread.Sleep(300);
            var value = true;
            try
            {
                using (var client = new OpcClient("opc.tcp://localhost:" + port + "/"))
                {
                    client.Connect();
                    client.WriteNode(GeneralStationTests.NodePath + "Status/Enable", value);
                }

            }
            catch
            {
                Assert.Fail();
            }
            Assert.AreEqual(machine.InNodes.StatusNodes.enable.Value, value);
        }

        [TestMethod]
        public void OPC_ResetMachine()
        {
            var port = 5004;
            var machine = new MPS_BS("C-BS", port, 0, Team.Cyan, true);
            var thread = new Thread(machine.Run);
            thread.Start();
            Thread.Sleep(300);
            var testnode = machine.InNodes.ActionId;
            testnode.Value = 100;
            Assert.AreNotEqual(testnode.Value,0);
            try
            {
                using (var client = new OpcClient("opc.tcp://localhost:" + port + "/"))
                {
                    client.Connect();
                    client.WriteNode(GeneralStationTests.NodePath + "Status/Enable", true);
                }

            }
            catch
            {
                Assert.Fail();
            }
            Thread.Sleep(1400);
            Assert.AreEqual(testnode.Value, 0);
        }
    }
}
