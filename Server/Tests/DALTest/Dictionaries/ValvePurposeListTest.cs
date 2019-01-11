using GazRouter.DAL.Dictionaries.ValvePurposes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;

namespace DALTest.Dictionaries
{
    [TestClass]
    public class ValvePurposeListTest : TransactionTestsBase
    {
        [TestMethod ,TestCategory(Stable)]
        public void ValvePurposeList()
        {
            var list = new GetValvePurposeListQuery(Context).Execute();
            AssertHelper.CheckDictionary(list, c => c.ValvePurpose);
        }
    }
}