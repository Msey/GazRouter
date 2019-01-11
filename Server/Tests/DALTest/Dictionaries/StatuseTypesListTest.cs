using GazRouter.DAL.Dictionaries.StatusTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;

namespace DALTest.Dictionaries
{
    [TestClass]
	public class StatuseTypesListTest : TransactionTestsBase
    {
        [TestMethod ,TestCategory(Stable)]
		public void StatuseTypesList()
        {
            var list = new GetStatusTypeListQuery(Context).Execute();
            AssertHelper.CheckDictionary(list, c => c.StatusType);
        }
    }
}