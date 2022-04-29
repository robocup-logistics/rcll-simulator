using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simulator.MPS;
using LlsfMsgs;
using System.Threading;
using Simulator;
using Simulator.Utility;

namespace Simulatortests
{
    [TestClass]
    public class RingStationTests
    {
        [TestMethod]
        public void IncreaseSlideconut()
        {
            var machine = new MPS_RS("C-BS", 5300, 0, Team.Cyan, true);
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
        
    }
}
