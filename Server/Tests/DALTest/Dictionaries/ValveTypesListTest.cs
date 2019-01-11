using GazRouter.DAL.Dictionaries.ValveTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;

namespace DALTest.Dictionaries
{
    [TestClass]
    public class ValveTypesListTest : TransactionTestsBase
    {
        [TestMethod ,TestCategory(Stable)]
        public void ValveTypesList()
        {
            var list = new GetValveTypesListQuery(Context).Execute();
            AssertHelper.IsNotEmpty(list);
        }
    }
}