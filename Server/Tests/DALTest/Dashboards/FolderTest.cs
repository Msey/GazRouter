using System.Linq;
using GazRouter.DAL.Dashboards.Folders;
using GazRouter.DTO.Dashboards.Folders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;

namespace DALTest.Dashboards
{
	[TestClass]
    public class FolderTest : TransactionTestsBase
	{
		[TestMethod ,TestCategory(Stable)]
		public void FullFolderTest()
		{
			{
				var user = GetCurrentUser();
				var sourceId =
					new AddFolderCommand(Context).Execute(new AddFolderParameterSet {Name = "testName"});
                var list = new GetFolderListQuery(Context).Execute(user.Id);
				Assert.AreNotEqual(list.Count(p => p.Id == sourceId), 0);
                new EditFolderCommand(Context).Execute(new EditFolderParameterSet { FolderId = sourceId, Name = "Name1" });
                var source = new GetFolderListQuery(Context).Execute(user.Id).SingleOrDefault(p => p.Id == sourceId);
				Assert.IsNotNull(source);
				Assert.AreEqual(source.Id,sourceId);
				Assert.AreEqual(source.Name, "Name1");

				var sourceId2 =
                    new AddFolderCommand(Context).Execute(new AddFolderParameterSet { Name = "testName", ParentId = sourceId });
                list = new GetFolderListQuery(Context).Execute(user.Id);
				Assert.AreNotEqual(list.Count(p => p.Id == sourceId2), 0);
                new EditFolderCommand(Context).Execute(new EditFolderParameterSet { FolderId = sourceId2, Name = "Name3" });
                source = new GetFolderListQuery(Context).Execute(user.Id).SingleOrDefault(p => p.Id == sourceId2);
				Assert.IsNotNull(source);
				Assert.AreEqual(source.Id, sourceId2);
				Assert.AreEqual(source.Name, "Name3");

                new DeleteFolderCommand(Context).Execute(sourceId);
			}
		}
	}
}
