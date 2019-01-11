using GazRouter.DAL.Dictionaries.BoilerTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;

namespace DALTest.Dictionaries
{
    [TestClass]
	public class BoilerTypesListTest : TransactionTestsBase
    {
		[TestMethod, TestCategory(Stable)]
		public void BoilerTypesList()
        {
			var list = new GetBoilerTypeListQuery(Context).Execute();
			Assert.IsTrue(list.Count > 0);
        }
    }
}