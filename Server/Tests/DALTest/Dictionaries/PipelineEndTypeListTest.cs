using GazRouter.DAL.Dictionaries.PipelineEndType;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;

namespace DALTest.Dictionaries
{
    [TestClass]
    public class PipelineEndTypeListTest : TransactionTestsBase
    {
        [TestMethod ,TestCategory(Stable)]
        public void PipelineEndTypeList()
        {
            var list = new GetPipelineEndTypeListQuery(Context).Execute();
            AssertHelper.CheckDictionary(list, c => c.PipelineEndType);
        }
    }
}