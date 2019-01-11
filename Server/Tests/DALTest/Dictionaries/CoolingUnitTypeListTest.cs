using GazRouter.DAL.Dictionaries.CoolingUnitTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;

namespace DALTest.Dictionaries
{
    [TestClass]
	public class CoolingUnitTypeListTest : TransactionTestsBase
    {
		[TestMethod, TestCategory(Stable)]
		public void BoilerTypesList()
        {
			var list = new GetCoolingUnitTypeListQuery(Context).Execute();
			Assert.IsTrue(list.Count > 0);
        }
    }
}