using GazRouter.Modes.GasCosts.Dialogs.ChemicalAnalysisCosts;
using GazRouter.Modes.GasCosts.Dialogs.CompUnitsHeatingCosts;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace ClientTests.Modules.Modes
{
    [TestClass]
    [Tag("Modes.GasCosts")]
    public class GasCostsTests
    {
        [TestMethod]public void ChemicalAnalysisCostsModelTest()
        {
            var model = new ChemicalAnalysisCostsModel();
            Assert.AreEqual(0, model.Calculate());
            model.MeasCount = 100;
            model.Time = 5;
            model.Q = 10000;
            Assert.AreEqual(5, model.Calculate());
        }
        [TestMethod]public void GCostsModelTest()
        {
            var model = new CompUnitsHeatingCostsModel
            {
                Kt = 2,
                Qba = 1500,
                Time = 25
            };
            var calculate = model.Calculate();
            Assert.AreEqual(75, calculate);
        }
    }
}