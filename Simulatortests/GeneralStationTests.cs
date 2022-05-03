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
            var machine = new MPS_BS("C-BS", port, 0, Team.Cyan, true);
            var thread = new Thread(machine.Run);
            var product = new Products(CapColor.CapBlack);
            thread.Start();
            Thread.Sleep(300);
            var value = true;
            var bs = new TestHelper(port);
            if (!bs.CreateConnection())
                Assert.Fail();
            bs.EnableTask();
            Assert.AreEqual(machine.InNodes.StatusNodes.enable.Value, value);
            bs.CloseConnection();
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
            var testhelper = new TestHelper(port);
            if (!testhelper.CreateConnection())
                Assert.Fail();
            testhelper.SendTask((ushort)200);
            Assert.AreEqual(200,testnode.Value);
            testhelper.SendTask((ushort)MPS_BS.BaseSpecificActions.Reset);
            Assert.AreEqual(100 ,testnode.Value);
            Thread.Sleep(1400);
            Assert.AreEqual(testnode.Value, 0);
            testhelper.CloseConnection();
        }
    }
}
