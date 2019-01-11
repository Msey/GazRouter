using GazRouter.DAL.Dictionaries.EngineClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;

namespace DALTest.Dictionaries
{
    [TestClass]
    public class EngineClassesListTest : TransactionTestsBase
    {
        [TestMethod ,TestCategory(Stable)]
        public void EngineClassesList()
        {
            var list = new GetEngineClassesListQuery(Context).Execute();
            AssertHelper.CheckDictionary(list, c=>c.EngineClass);
        }
    }
}