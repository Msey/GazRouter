using GazRouter.DAL.Dictionaries.ParameterTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;

namespace DALTest.Dictionaries
{
    [TestClass]
	public class ParameterTypesListTest : TransactionTestsBase
    {
        [TestMethod ,TestCategory(Stable)]
		public void ParameterTypesList()
        {
            var list = new GetParameterTypesListQuery(Context).Execute();
            AssertHelper.CheckDictionary(list, c => c.ParameterType);
        }
    }
}