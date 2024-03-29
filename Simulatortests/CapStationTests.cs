﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            var config = new Configurations();
            var machine = new MPS_CS(config, "C-BS", port, 0, Team.Cyan, true);
            var product = new Products(CapColor.CapBlack);
            machine.PlaceProduct("inupt", product);
            Assert.IsNotNull(machine.ProductAtIn);
            machine.InNodes.Data0.Value = (ushort)Positions.Mid;
            machine.InNodes.Data1.Value = (ushort)Direction.FromInToOut;
            machine.HandleBelt();
            Assert.IsNotNull(machine.ProductOnBelt);
            Assert.IsNull(machine.ProductAtIn);
            machine.InNodes.Data0.Value = (ushort)CSOp.RetrieveCap;
            machine.CapTask();
            Assert.IsNotNull(machine.StoredCap);
        }
        
        [TestMethod]
        public void OPC_BufferCapStationWithProductOnInput()
        {
            var port = 5201;
            var config = new Configurations();
            var machine = new MPS_CS(config, "C-CS", port, 0, Team.Cyan, true);
            var thread = new Thread(machine.Run);
            thread.Start();
            Thread.Sleep(500);
            var cs = new OPCTestHelper(port);
            if (!cs.CreateConnection())
                Assert.Fail();
            var product = new Products(CapColor.CapBlack);
            machine.PlaceProduct("input", product);
            Assert.IsNotNull(machine.ProductAtIn);
            cs.SendTask((ushort)MPS_CS.BaseSpecificActions.BandOnUntil, (ushort)Positions.Mid, (ushort)Direction.FromInToOut);
            Thread.Sleep(config.BeltActionDuration + 300);
            cs.SendTask((ushort)MPS_CS.BaseSpecificActions.Cap, (ushort)CSOp.RetrieveCap);
            Thread.Sleep(config.CSTaskDuration + 200);
            Assert.IsNotNull(machine.StoredCap);
            Assert.IsNull(machine.ProductOnBelt?.RetrieveCap());
            cs.CloseConnection();
            machine.StopMachine();
        }

        [TestMethod]
        public void OPC_MountCapWithBufferingAction()
        {
            var port = 5202;
            var config = new Configurations();
            var machine = new MPS_CS(config, "C-BS", port, 0, Team.Cyan, true);
            var thread = new Thread(machine.Run);
            thread.Start();
            Thread.Sleep(500);
            var testhelper = new OPCTestHelper(port);
            if (!testhelper.CreateConnection())
                Assert.Fail();
            var product = new Products(CapColor.CapBlack);
            machine.PlaceProduct("input", product);

            Assert.IsNotNull(machine.ProductAtIn);
            testhelper.SendTask((ushort)MPS_CS.BaseSpecificActions.BandOnUntil, (ushort)Positions.Mid, (ushort)Direction.FromInToOut);
            Thread.Sleep(config.BeltActionDuration + 300);
            testhelper.SendTask((ushort)MPS_CS.BaseSpecificActions.Cap, (ushort)CSOp.RetrieveCap);
            Thread.Sleep(config.CSTaskDuration + 200);
            Assert.IsNotNull(machine.StoredCap);
            testhelper.SendTask((ushort)MPS_CS.BaseSpecificActions.BandOnUntil, (ushort)Positions.Out, (ushort)Direction.FromInToOut);
            Thread.Sleep(config.BeltActionDuration + 300);
            var secondProduct = new Products(BaseColor.BaseBlack);
            machine.PlaceProduct("input", secondProduct);
            testhelper.SendTask((ushort)MPS_CS.BaseSpecificActions.BandOnUntil, (ushort)Positions.Mid, (ushort)Direction.FromInToOut);
            Thread.Sleep(config.BeltActionDuration + 300);
            testhelper.SendTask((ushort)MPS_CS.BaseSpecificActions.Cap, (ushort)CSOp.MountCap);
            Thread.Sleep(config.CSTaskDuration + 300);
            Assert.IsNotNull(machine.ProductOnBelt.RetrieveCap());
            testhelper.CloseConnection();
            machine.StopMachine();
        }
    }
}
