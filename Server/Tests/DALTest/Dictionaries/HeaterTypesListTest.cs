using GazRouter.DAL.Dictionaries.HeaterTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;

namespace DALTest.Dictionaries
{
    [TestClass]
    public class HeaterTypesListTest : TransactionTestsBase
    {
        [TestMethod ,TestCategory(Stable)]
        public void GetHeaterTypes()
        {
            var list = new GetHeaterTypesListQuery(Context).Execute();
            Assert.IsTrue(list.Count > 0);
        }
    }
}