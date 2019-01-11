using System.Linq;
using GazRouter.DAL.Authorization.User;
using GazRouter.DAL.Dashboards.DashboardGrants;
using GazRouter.DAL.Dashboards.Dashboards;
using GazRouter.DAL.Dashboards.Folders;
using GazRouter.DTO.Authorization.User;
using GazRouter.DTO.Dashboards.Dashboard;
using GazRouter.DTO.Dashboards.DashboardGrants;
using GazRouter.DTO.Dashboards.Folders;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DALTest.Dashboards
{
	[TestClass]
    public class DashboardGrantTest : DalTestBase
	{
		[TestMethod ,TestCategory(Stable)]
		public void FullDashboardTest()
		{
            var user = GetCurrentUser();
		    var folderId =
                new AddFolderCommand(Context).Execute(new AddFolderParameterSet { Name = "testName" });
		    var dashboardId =
                new AddDashboardCommand(Context).Execute(new AddDashboardParameterSet
		            {
		                DashboardName = "TestDash",
		                PeriodTypeId = PeriodType.Month,
		                FolderId = folderId
		            });
            var dashboards = new GetDashboardListQuery(Context).Execute(user.Id);
		    var dash = dashboards.FirstOrDefault(p => p.Id == dashboardId);
		    Assert.IsNotNull(dash);
		    Assert.AreEqual(dash.DashboardName, "TestDash");
		    //Assert.AreEqual(dash.PeriodTypeID, PeriodType.Month);
		    Assert.AreEqual(dash.FolderId, folderId);

            new EditDashboardCommand(Context).Execute(new EditDashboardParameterSet
		        {
		            DashboardName = "Test2",
		            DashboardId = dashboardId,
		            PeriodTypeId = PeriodType.Year,
		            FolderId = null
		        });
            dashboards = new GetDashboardListQuery(Context).Execute(user.Id);
		    dash = dashboards.FirstOrDefault(p => p.Id == dashboardId);
		    Assert.IsNotNull(dash);
		    Assert.AreEqual(dash.DashboardName, "Test2");
		    //Assert.AreEqual(dash.PeriodTypeID, PeriodType.Year);
		    Assert.IsFalse(dash.FolderId.HasValue);

			var list = new GetDashboardGrantListQuery(Context).Execute(dashboardId);
		    Assert.AreNotEqual(list.Count(p => p.UserId == user.Id), 0);

		    var siteId =
		        CreateSite();

            var userid = new AddUserCommand(Context).Execute(
		        new AddUserParameterSet
		            {
		                Login = "TestLogin",
		                Description = "TestDescription",
		                FullName = "TestUser",
		                SiteId = siteId,
		                SettingsUser = new UserSettings()
		            });

            new AddDashboardGrantCommand(Context).Execute(new ShareDashboardParameterSet
		        {
		            DashboardId = dashboardId,
		            IsEditable = true,
		            IsGrantable = true,
		            //SharedUserId = userid
		        });


			list = new GetDashboardGrantListQuery(Context).Execute(dashboardId);
		    var grant = list.FirstOrDefault(p => p.UserId == userid);
		    Assert.IsNotNull(grant);
		    Assert.IsTrue(grant.IsGrantable);
		    //Assert.IsTrue(grant.IsEdit);

            new EditDashboardGrantCommand(Context).Execute(new DashboardGrantParameterSet
		        {
		            DashboardId = dashboardId,
		            IsEditable = false,
		            IsGrantable = true,
		            UserId = userid
		        });
			list = new GetDashboardGrantListQuery(Context).Execute(dashboardId);
		    grant = list.FirstOrDefault(p => p.UserId == userid);
		    Assert.IsNotNull(grant);
		    Assert.IsTrue(grant.IsGrantable);
		    //Assert.IsFalse(grant.IsEdit);

            new DeleteDashboardGrantCommand(Context).Execute(new DeleteDashboardGrantParameterSet
		        {
		            DashboardId = dashboardId,
		            UserId = userid
		        });
			list = new GetDashboardGrantListQuery(Context).Execute(dashboardId);
		    grant = list.FirstOrDefault(p => p.UserId == userid);
			Assert.IsNotNull(grant);
			Assert.IsFalse(grant.IsGrantable);
			//Assert.IsFalse(grant.IsEdit);
            new DeleteDashboardCommand(Context).Execute(dashboardId);
            new DeleteFolderCommand(Context).Execute(folderId);
            new DeleteUserCommand(Context).Execute(userid);
		}
	}
}
