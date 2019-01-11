using GazRouter.DAL.Dictionaries.PipelineTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;

namespace DALTest.Dictionaries
{
    [TestClass]
    public class PipelineTypesListTest : TransactionTestsBase
    {
        [TestMethod ,TestCategory(Stable)]
        public void PipelineTypesList()
        {
            var list = new GetPipelineTypesListQuery(Context).Execute();
            AssertHelper.CheckDictionary(list, c=>c.PipelineType);

    
        }
    }
}