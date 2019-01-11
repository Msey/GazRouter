using GazRouter.DAL.Dictionaries.EmergencyValveTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;

namespace DALTest.Dictionaries
{
    [TestClass]
    public class EmergencyValveTypesListTest : TransactionTestsBase
    {
        [TestMethod ,TestCategory(Stable)]
        public void TestRegulatorTypeListGet()
        {
            {
                var list = new GetEmergencyValveTypeListQuery(Context).Execute();
                AssertHelper.CheckDictionary(list, c => c.EmergencyValveType);
            }
        }
    }
}