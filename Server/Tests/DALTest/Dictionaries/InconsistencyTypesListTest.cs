using GazRouter.DAL.Dictionaries.InconsistencyTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;

namespace DALTest.Dictionaries
{
    [TestClass]
	public class InconsistencyTypesListTest : TransactionTestsBase
    {
		[TestMethod, TestCategory(Stable)]
		public void InconsistencyTypesList()
        {
			var list = new GetInconsistencyTypesQuery(Context).Execute();
			Assert.IsTrue(list.Count > 0);
        }
    }
}