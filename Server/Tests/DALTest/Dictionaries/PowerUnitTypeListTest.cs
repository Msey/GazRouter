using GazRouter.DAL.Dictionaries.PowerUnitTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;

namespace DALTest.Dictionaries
{
    [TestClass]
	public class PowerUnitTypeListTest : TransactionTestsBase
    {
        [TestMethod ,TestCategory(Stable)]
        public void TestRegulatorTypeListGet()
        {
				var list = new GetPowerUnitTypeListQuery(Context).Execute();
				Assert.IsTrue(list.Count > 0);
        }
    }
}