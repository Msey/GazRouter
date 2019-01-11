using GazRouter.DAL.Dictionaries.Diameters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;

namespace DALTest.Dictionaries
{
    [TestClass]
	public class DiameterListTest : TransactionTestsBase
    {
		[TestMethod, TestCategory(Stable)]
		public void BoilerTypesList()
        {
			var list = new GetDiameterListQuery(Context).Execute();
			Assert.IsTrue(list.Count > 0);
        }
    }
}