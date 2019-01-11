using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utils.Calculations;
namespace ClientTests.Common
{
    [TestClass]
    [Tag("Common.K")]
    public class KTest : SilverlightTest
    {
        [TestMethod]
        public void Test()
        {
            var k = K.CalcK(10, 0.15, 220);
            // 
            double calcK;
            calcK = K.CalcK(10, 0.11, 220);
            calcK = K.CalcK(75, 0.1, 10100);
            var y1 = Interpolation.Interpolate(0.12, 0.15, 0.1, 901, 822);
            var y2 = Interpolation.Interpolate(6378, 6000, 8000, 15.5, 19.2);
            var y3 = Interpolation.Interpolate(0.12, 0.15, 0.1, 822, 901);
            var y4 = Interpolation.Interpolate(0.12, 0.10, 0.1, 822, 901);
            var y5 = Interpolation.Interpolate2(.12, .15, .1, 220, 200, 300, 901, 867, 822, 766);
            // 
            k = K.CalcK(10, 0.11, 220);
            var arr = new[] { -3.0, -2, -1, 0, 1, 2, 3 };
            var i = 0;
            k = K.GetFirstValue(arr, 1, ref i);
            Assert.AreEqual(k, 1); Assert.AreEqual(i, 4);
            //
            k = K.GetFirstValue(arr, -1.3, ref i);
            Assert.AreEqual(k, -1); Assert.AreEqual(i, 2);
            //
            k = K.GetFirstValue(arr, 1.3, ref i);
            Assert.AreEqual(k, 2); Assert.AreEqual(i, 5);
            //
            k = K.GetFirstValue(arr, 4, ref i);
            Assert.AreEqual(k, 3); Assert.AreEqual(i, 6);
            //
            k = K.GetFirstValue(arr, -4, ref i);
            Assert.AreEqual(k, -3); Assert.AreEqual(i, 0);
        }
    }
}
