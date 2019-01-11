using GazRouter.DAL.Dictionaries.PlanTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;

namespace DALTest.Dictionaries
{
    [TestClass]
    public class PlanTypesListTest : TransactionTestsBase
    {
        [TestMethod ,TestCategory(Stable)]
        public void PlanTypesList()
        {
            var list = new GetPlanTypesListQuery(Context).Execute();
            AssertHelper.CheckDictionary(list, c => c.PlanType);
        }
    }
}