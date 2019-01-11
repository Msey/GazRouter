using GazRouter.DAL.Dictionaries.SysEventTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;

namespace DALTest.Dictionaries
{
    [TestClass]
    public class SysEventTypesListTest : TransactionTestsBase
    {
        [TestMethod ,TestCategory(Stable)]
        public void SysEventTypesList()
        {
            var list = new GetSysEventTypesListQuery(Context).Execute();
            Assert.IsTrue(list.Count > 0);
        }
    }
}