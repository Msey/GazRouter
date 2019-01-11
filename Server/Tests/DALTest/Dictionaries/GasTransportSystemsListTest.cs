using GazRouter.DAL.Dictionaries.GasTransportSystem;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;

namespace DALTest.Dictionaries
{
	[TestClass]
	public class GasTransportSystemsListTest : TransactionTestsBase
	{
		[TestMethod ,TestCategory(Stable)]
		public void TestRepairTypeListGet()
		{
			{
				var list = new GetGasTransportSystemListQuery(Context).Execute();
				Assert.IsTrue(list.Count > 0);
			}
		}
	}
}
