using GazRouter.DAL.Dictionaries.CompUnitSealingTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;

namespace DALTest.Dictionaries
{
    [TestClass]
	public class CompUnitSealingTypeListTest : TransactionTestsBase
    {
		[TestMethod, TestCategory(Stable)]
		public void BoilerTypesList()
		{
		    var list = new GetCompUnitSealingTypeListQuery(Context).Execute();
			Assert.IsTrue(list.Count > 0);
        }
    }
}