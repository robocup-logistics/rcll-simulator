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
            Thread.Sleep(500);
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
            Thread.Sleep(500);
            Assert.AreEqual(machine.InNodes.ActionId.Value, 0);
            var testhelper = new TestHelper(port);
            if (!testhelper.CreateConnection())
                Assert.Fail();
            testhelper.SendTask((ushort)MPS_BS.BaseSpecificActions.GetBase, (ushort)1, (ushort)0);
            Thread.Sleep(Configurations.GetInstance().BSTaskDuration + 300);
            Assert.IsNotNull(machine.ProductOnBelt);
            testhelper.CloseConnection();
        }
        [TestMethod]
        public void OPC_WrongDispenseBase()
        {
            var port = 5102;
            var machine = new MPS_BS("C-BS", port, 0, Team.Cyan, true);
            var thread = new Thread(machine.Run);
            thread.Start();
            Thread.Sleep(500);
            Assert.AreEqual(machine.InNodes.ActionId.Value, 0);
            var testhelper = new TestHelper(port);
            if (!testhelper.CreateConnection())
                Assert.Fail();
            testhelper.SendTask((ushort)MPS_BS.BaseSpecificActions.GetBase, (ushort)5, (ushort)1);
            Assert.IsNull(machine.ProductOnBelt);
        }

        [TestMethod]
        public void OPC_DispenseBaseAndStartBeltWithReady()
        {
            var port = 5103;
            var machine = new MPS_BS("C-BS", port, 0, Team.Cyan, true);
            var thread = new Thread(machine.Run);
            thread.Start();
            Thread.Sleep(500);
            Assert.AreEqual(machine.InNodes.ActionId.Value, 0);
            var testhelper = new TestHelper(port);
            if (!testhelper.CreateConnection())
                Assert.Fail();
            testhelper.SendTask((ushort)MPS_BS.BaseSpecificActions.GetBase, (ushort)1, (ushort)1);
            Thread.Sleep(Configurations.GetInstance().BSTaskDuration + 300);
            Assert.IsNotNull(machine.ProductOnBelt);
            testhelper.SendTask((ushort)MPS_BS.BaseSpecificActions.BandOnUntil, (ushort)Positions.Out, (ushort)Direction.FromInToOut);
           
            Thread.Sleep(Configurations.GetInstance().BSTaskDuration + 100);
            Assert.IsNotNull(machine.ProductAtOut);
            Assert.IsNull(machine.ProductOnBelt);
            Assert.AreEqual(machine.InNodes.StatusNodes.ready.Value, true);
            testhelper.CloseConnection();
        }

       

    }
}
