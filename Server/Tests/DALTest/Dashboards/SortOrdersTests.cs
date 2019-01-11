using System.Linq;
using GazRouter.DAL.Dashboards.Folders;
using GazRouter.DTO.Dashboards.Folders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DALTest.ObjectModel
{
    [TestClass]
    public class FolderSortOrdersTests : DalTestBase
    {
        [TestMethod ,TestCategory(Stable)]
		public void SortOrdersTest()
        {
			var user = GetCurrentUser();
			var sourceId =
				new AddFolderCommand(Context).Execute(new AddFolderParameterSet { Name = "testName"});
			new SetForderSortOrderCommand(Context).Execute(new SetSortOrderParameterSet { Id = sourceId, SortOrder = 10 });
			var list = new GetFolderListQuery(Context).Execute(user.Id);
			Assert.AreNotEqual(list.Count(p => p.Id == sourceId), 0);
	        var folder = list.FirstOrDefault(t => t.Id == sourceId);

			Assert.AreEqual(folder.SortOrder,10);
			new SetForderSortOrderCommand(Context).Execute(new SetSortOrderParameterSet { Id = sourceId ,SortOrder = 15});
			list = new GetFolderListQuery(Context).Execute(user.Id);
			Assert.AreNotEqual(list.Count(p => p.Id == sourceId), 0);
			folder = list.FirstOrDefault(t => t.Id == sourceId);

			Assert.AreEqual(folder.SortOrder, 15);
			new DeleteFolderCommand(Context).Execute(sourceId);
        }
    }

}
