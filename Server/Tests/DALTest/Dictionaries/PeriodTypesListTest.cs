using GazRouter.DAL.Dictionaries.PeriodTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;

namespace DALTest.Dictionaries
{
    [TestClass]
    public class PeriodTypesListTest : TransactionTestsBase
    {
        [TestMethod ,TestCategory(Stable)]
        public void PeriodTypesList()
        {
            var list = new GetPeriodTypesListQuery(Context).Execute();
            AssertHelper.CheckDictionary(list, c => c.PeriodType);
        }
    }
}