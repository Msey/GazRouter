using System.Linq;
using GazRouter.DAL.Dictionaries.OperConsumerType;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;

namespace DALTest.Dictionaries
{
    [TestClass]
    public class OperConsumersListTest : TransactionTestsBase
    {
        [TestMethod ,TestCategory(Stable)]
        public void OperConsumersTest()
        {
            var list = new OperConsumerTypesQuery(Context).Execute();
            Assert.IsTrue(list.Any());
        }
    }
}