using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simulator.MPS;
using LlsfMsgs;
using System.Threading;
using Simulator;


namespace Simulatortests
{
    [TestClass]
    public class BaseStationTests
    {
        [TestMethod]
        public void DispenseBase()
        {
            var machine = new MPS_BS("C-BS", 5100, 0, Team.Cyan, true);
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
    }
}
