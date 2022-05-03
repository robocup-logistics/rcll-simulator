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
    public class CapStationTests
    {

        [TestMethod]
        public void BufferCapStation()
        {
            var port = 5200;
            var machine = new MPS_CS("C-BS", port, 0, Team.Cyan, true);
            var thread = new Thread(machine.Run);
            thread.Start();
            Thread.Sleep(300);
            var testnode = machine.InNodes.ActionId;
            //Setting the shelf number to dispense a base
            var product = new Products(CapColor.CapBlack);
            machine.PlaceProduct("inupt", product);
            machine.InNodes.Data0.Value = 1;
            Thread.Sleep(Configurations.GetInstance().BSTaskDuration + 100);
            Assert.IsNotNull(machine.ProductAtIn);
            machine.InNodes.Data0.Value = (ushort)Positions.Mid;
            machine.InNodes.Data1.Value = (ushort)Direction.FromInToOut;
            machine.HandleBelt();
            Thread.Sleep(Configurations.GetInstance().BeltActionDuration + 100);
            Assert.IsNotNull(machine.ProductOnBelt);
            Assert.IsNull(machine.ProductAtIn);
            machine.InNodes.Data0.Value = (ushort)CSOp.RetrieveCap;
            machine.CapTask();
            Thread.Sleep(Configurations.GetInstance().CSTaskDuration + 100);
            Assert.IsNotNull(machine.StoredCap);
        }
        
        [TestMethod]
        public void OPC_BufferCapStationWithProductOnInput()
        {
            var port = 5201;
            var machine = new MPS_CS("C-CS", port, 0, Team.Cyan, true);
            var thread = new Thread(machine.Run);
            thread.Start();
            Thread.Sleep(300);
            var cs = new TestHelper(port);
            if (!cs.CreateConnection())
                Assert.Fail();
            var product = new Products(CapColor.CapBlack);
            machine.PlaceProduct("input", product);
            Assert.IsNotNull(machine.ProductAtIn);
            cs.SendTask((ushort)MPS_CS.BaseSpecificActions.BandOnUntil, (ushort)Positions.Mid, (ushort)Direction.FromInToOut);
            Thread.Sleep(Configurations.GetInstance().BeltActionDuration + 300);
            cs.SendTask((ushort)MPS_CS.BaseSpecificActions.Cap, (ushort)CSOp.RetrieveCap);
            Thread.Sleep(Configurations.GetInstance().CSTaskDuration + 200);
            Assert.IsNotNull(machine.StoredCap);
            Assert.IsNull(machine.ProductOnBelt?.RetrieveCap());
        }

        [TestMethod]
        public void OPC_MountCapWithBufferingAction()
        {
            var port = 5202;
            var machine = new MPS_CS("C-BS", port, 0, Team.Cyan, true);
            var thread = new Thread(machine.Run);
            thread.Start();
            Thread.Sleep(300);
            var testhelper = new TestHelper(port);
            if (!testhelper.CreateConnection())
                Assert.Fail();
            var product = new Products(CapColor.CapBlack);
            machine.PlaceProduct("input", product);

            Assert.IsNotNull(machine.ProductAtIn);
            testhelper.SendTask((ushort)MPS_CS.BaseSpecificActions.BandOnUntil, (ushort)Positions.Mid, (ushort)Direction.FromInToOut);
            Thread.Sleep(Configurations.GetInstance().BeltActionDuration + 300);
            testhelper.SendTask((ushort)MPS_CS.BaseSpecificActions.Cap, (ushort)CSOp.RetrieveCap);
            Thread.Sleep(Configurations.GetInstance().CSTaskDuration + 200);
            Assert.IsNotNull(machine.StoredCap);
            testhelper.SendTask((ushort)MPS_CS.BaseSpecificActions.BandOnUntil, (ushort)Positions.Out, (ushort)Direction.FromInToOut);
            Thread.Sleep(Configurations.GetInstance().BeltActionDuration + 300);
            var secondProduct = new Products(BaseColor.BaseBlack);
            machine.PlaceProduct("input", secondProduct);
            testhelper.SendTask((ushort)MPS_CS.BaseSpecificActions.BandOnUntil, (ushort)Positions.Mid, (ushort)Direction.FromInToOut);
            Thread.Sleep(Configurations.GetInstance().BeltActionDuration + 300);
            testhelper.SendTask((ushort)MPS_CS.BaseSpecificActions.Cap, (ushort)CSOp.MountCap);
            Thread.Sleep(Configurations.GetInstance().CSTaskDuration + 300);
            Assert.IsNotNull(machine.ProductOnBelt.RetrieveCap());
            testhelper.CloseConnection();
        }
    }
}
