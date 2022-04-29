using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simulator.MPS;
using LlsfMsgs;
using System.Threading;
using Simulator;
using Simulator.Utility;

namespace Simulatortests
{
    [TestClass]
    public class CapStationTests
    {
        [TestMethod]
        public void BufferCapStation()
        {
            var machine = new MPS_CS("C-BS", 5200, 0, Team.Cyan, true);
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
        
    }
}
