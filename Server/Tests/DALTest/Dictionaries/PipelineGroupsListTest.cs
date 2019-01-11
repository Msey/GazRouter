using GazRouter.DAL.Dictionaries.PipelineGroups;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;

namespace DALTest.Dictionaries
{
	[TestClass]
	public class PipelineGroupsListTest : TransactionTestsBase
	{
		[TestMethod ,TestCategory(Stable)]
		public void TestPipelineGroupsListGet()
		{
			{
				var list = new GetPipelineGroupListQuery(Context).Execute();
				Assert.IsTrue(list.Count > 0);
			}
		}
	}
}
