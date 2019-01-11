using GazRouter.DAL.Dashboards.DashboardFolder;
using GazRouter.DAL.Dashboards.Dashboards;
using GazRouter.DAL.Dashboards.Folders;
using GazRouter.DTO.Dashboards.Dashboard;
using GazRouter.DTO.Dashboards.DashboardFolder;
using GazRouter.DTO.Dashboards.Folders;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;

namespace DALTest.Dashboards
{
	[TestClass]
    public class DashboardFolderTest : TransactionTestsBase
	{
		[TestMethod ,TestCategory(Stable)]
        public void FullDashboardFolderTest()
		{
		    var folderId =
		        new AddFolderCommand(Context).Execute(new AddFolderParameterSet {Name = "ProbaName"});
		    var dashboardId =
                new AddDashboardCommand(Context).Execute(new AddDashboardParameterSet
		            {
		                DashboardName = "ProbaDash",
		                PeriodTypeId = PeriodType.Twohours
		            });

            new AddDashboardToFolderCommand(Context).Execute(new DashboardFolderParameterSet
		        {
		            DashboardId = dashboardId,
		            FolderId = folderId
		        });
            new DeleteDashboardFromFolderCommand(Context).Execute(new DashboardFolderParameterSet
		        {
		            DashboardId = dashboardId,
		            FolderId = folderId
		        });
		}
	}
}
