using GazRouter.DAL.Dictionaries.EventTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;

namespace DALTest.Event
{
    [TestClass]
    public class EventTypesTests : TransactionTestsBase
    {
        [TestMethod ,TestCategory(Stable)]
        public void TestModifyPhisicalTypes()
        {
            var result = new GetEventTypesListQuery(Context).Execute();

            AssertHelper.IsNotEmpty(result);
        }

    }
}
