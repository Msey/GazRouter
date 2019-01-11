using GazRouter.Service.Exchange.Lib.Import.Astra;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;

namespace ExchangeTest
{
    [TestClass]
    public class AstraImportUnitTest : TransactionTestsBase
    {

        [TestMethod]
        public void TestFileAction()
        {
            new AstraParsing(Context).Parse();
        }
    }
}
