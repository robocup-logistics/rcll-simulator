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
            var machine = new MPS_RS("C-RS", 5300, 0, Team.Cyan, true);
            var thread = new Thread(machine.Run);
            thread.Start();
            Thread.Sleep(300);
            var testnode = machine.InNodes.SlideCnt;
            //Setting the shelf number to dispense a base
            var product = new Products(BaseColor.BaseBlack);
            Assert.AreEqual(testnode.Value, 0);
            machine.PlaceProduct("slide", product);
            Assert.AreEqual(testnode.Value, 1);
        }


        [TestMethod]
        public void AddRingToProduct()
        {
            var machine = new MPS_RS("C-RS", 5301, 0, Team.Cyan, true);
            var thread = new Thread(machine.Run);
            thread.Start();
            Thread.Sleep(300);

            var baseProduct = new Products(BaseColor.BaseBlack);
            var complexity = baseProduct.Complexity;
            machine.ProductOnBelt = baseProduct;
            machine.InNodes.Data0.Value = 1;
            machine.MountRingTask();
            Thread.Sleep(Configurations.GetInstance().RSTaskDuration + 100);
            Assert.AreNotEqual(baseProduct.Complexity, complexity);
        }
        
        [TestMethod]
        public void OPC_AddRingToProduct()
        {
            var port = 5302;
            var machine = new MPS_RS("C-RS", port, 0, Team.Cyan, true);
            var thread = new Thread(machine.Run);
            thread.Start();
            Thread.Sleep(300);

            var baseProduct = new Products(BaseColor.BaseBlack);
            var complexity = baseProduct.Complexity;
            machine.ProductAtIn = baseProduct;
            var client = new OpcClient("opc.tcp://localhost:" + port + "/");
            try
            {
                client.Connect();
                client.WriteNode(GeneralStationTests.NodePath + "ActionId", (ushort) MPS_RS.BaseSpecificActions.BandOnUntil);
                client.WriteNode(GeneralStationTests.NodePath + "Data/Data[0]", (ushort)Positions.Mid);
                client.WriteNode(GeneralStationTests.NodePath + "Data/Data[1]", (ushort)Direction.FromInToOut);
                client.WriteNode(GeneralStationTests.NodePath + "Status/Enable", true);
            }
            catch
            {
                Assert.Fail();
            }
            Thread.Sleep(Configurations.GetInstance().BeltActionDuration + 300);
            Assert.IsNull(machine.ProductAtIn);
            Assert.IsNotNull(machine.ProductOnBelt);
            try
            {
                client.Connect();
                client.WriteNode(GeneralStationTests.NodePath + "ActionId", (ushort) MPS_RS.BaseSpecificActions.MountRing);
                client.WriteNode(GeneralStationTests.NodePath + "Data/Data[0]", (ushort) 1);
                client.WriteNode(GeneralStationTests.NodePath + "Status/Enable", true);
            }
            catch
            {
                Assert.Fail();
            }
            Thread.Sleep(Configurations.GetInstance().RSTaskDuration + 200);
            Assert.AreEqual(1, baseProduct.RingCount);
        }
    }
}
