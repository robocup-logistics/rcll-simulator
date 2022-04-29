using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simulator.Utility;
using LlsfMsgs;

namespace Simulatortests
{
    [TestClass]
    public class ProductTests
    {
        [TestMethod]
        public void AddCap()
        {
            var product = new Products(BaseColor.BaseRed);
            Assert.IsNull(product.RetrieveCap());
            product.AddPart(new CapElement(CapColor.CapBlack));
            Assert.AreEqual(product.Complexity, Order.Types.Complexity.C0);
            Assert.IsNotNull(product.RetrieveCap());
            Assert.IsNull(product.RetrieveCap());
        }
        [TestMethod]
        public void RemoveCap()
        {
            var product = new Products(CapColor.CapBlack);
            Assert.IsNotNull(product.RetrieveCap());
            Assert.IsNull(product.RetrieveCap());
        }
        [TestMethod]
        public void AddRing()
        {
            var product = new Products(BaseColor.BaseBlack);
            product.AddPart(new RingElement(RingColor.RingBlue));
            product.AddPart(new CapElement(CapColor.CapBlack));
            Assert.AreEqual(product.Complexity, Order.Types.Complexity.C1);
        }

    }
}