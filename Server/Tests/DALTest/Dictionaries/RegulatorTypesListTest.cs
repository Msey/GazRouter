using GazRouter.DAL.Dictionaries.RegulatorTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;

namespace DALTest.Dictionaries
{
    [TestClass]
    public class RegulatorTypesListTest : TransactionTestsBase
    {
        [TestMethod ,TestCategory(Stable)]
        public void TestRegulatorTypeListGet()
        {
            {
                var list = new GetRegulatorTypeListQuery(Context).Execute();
                AssertHelper.CheckDictionary(list, c => c.RegulatorType);
            }
        }
    }
}