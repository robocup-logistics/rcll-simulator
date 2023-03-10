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
    public class RingStationTests
    {
        [TestMethod]
        public void IncreaseSlideconut()
        {
            var config = new Configurations();
            var machine = new MPS_RS(config, "C-RS", 5300, 0, Team.Cyan, true);
            var testnode = machine.InNodes.SlideCnt;
            var product = new Products(BaseColor.BaseBlack);
            Assert.AreEqual(testnode.Value, 0);
            machine.PlaceProduct("slide", product);
            Assert.AreEqual(testnode.Value, 1);
        }


        [TestMethod]
        public void AddRingToProduct()
        {
            var config = new Configurations();
            var machine = new MPS_RS(config, "C-RS", 5301, 0, Team.Cyan, true);
            var baseProduct = new Products(BaseColor.BaseBlack);
            var complexity = baseProduct.Complexity;
            machine.ProductOnBelt = baseProduct;
            machine.InNodes.Data0.Value = 1;
            machine.MountRingTask();
            Assert.AreNotEqual(baseProduct.Complexity, complexity);
        }
        
        [TestMethod]
        public void OPC_AddRingToProduct()
        {
            var port = 5302;
            var config = new Configurations();
            var machine = new MPS_RS(config,"C-RS", port, 0, Team.Cyan, true);
            var thread = new Thread(machine.Run);
            thread.Start();
            Thread.Sleep(500);

            var baseProduct = new Products(BaseColor.BaseBlack);
            var complexity = baseProduct.Complexity;
            machine.ProductAtIn = baseProduct;
            var testhelper = new OPCTestHelper(port);
            if (!testhelper.CreateConnection())
                Assert.Fail();
            testhelper.SendTask((ushort)MPS_RS.BaseSpecificActions.BandOnUntil, (ushort)Positions.Mid, (ushort)Direction.FromInToOut);
            var client = new OpcClient("opc.tcp://localhost:" + port + "/");
            Thread.Sleep(config.BeltActionDuration + 300);
            Assert.IsNull(machine.ProductAtIn);
            Assert.IsNotNull(machine.ProductOnBelt);
            testhelper.SendTask((ushort)MPS_RS.BaseSpecificActions.MountRing, (ushort)1, (ushort)0);
            Thread.Sleep(config.RSTaskDuration + 200);
            Assert.AreEqual(1, baseProduct.RingCount);
            testhelper.CloseConnection();
        }
    }
}
