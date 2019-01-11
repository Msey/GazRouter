using GazRouter.DAL.Dashboards.DashboardContent;
using GazRouter.DAL.Dashboards.Dashboards;
using GazRouter.DTO.Dashboards.Dashboard;
using GazRouter.DTO.Dashboards.DashboardContent;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;

namespace DALTest.Dashboards
{
    [TestClass]
    public class DashboardElementTest : TransactionTestsBase
    {
        [TestMethod ,TestCategory(Stable)]
        public void FullDashboardElementTest()
        {
            var dashboardId = new AddDashboardCommand(Context).Execute(new AddDashboardParameterSet { DashboardName = "ProbaDash", PeriodTypeId = PeriodType.Twohours });
            
            new UpdateDashboardContentCommand(Context).Execute(new DashboardContentDTO {DashboardId = dashboardId, Content = "xxx"});
            var content = new GetDashboardContentQuery(Context).Execute(dashboardId);
            Assert.IsTrue(content.Content == "xxx");
            
        }
    }
}