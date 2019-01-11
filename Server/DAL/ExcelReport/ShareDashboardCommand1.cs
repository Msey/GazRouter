using GazRouter.DAL.Core;
using GazRouter.DTO.Dashboards.Dashboard;
using GazRouter.DTO.ExcelReports;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ExcelReport
{
    public class ShareDashboardCommand : CommandNonQuery<ShareDashboardParameterSet>
    {
        public ShareDashboardCommand(ExecutionContext context)
            : base(context)

        {
            IsStoredProcedure = true;
        }


        protected override void BindParameters(OracleCommand command, ShareDashboardParameterSet parameters)
        {
            command.AddInputParameter("P_DASHBOARD_ID", parameters.DashboardId);
            command.AddInputParameter("P_SHARE_USER_ID", parameters.UserId);
            command.AddInputParameter("P_IS_EDITABLE", parameters.IsEditable);
            command.AddInputParameter("P_IS_GRANTABLE", parameters.IsGrantable);
            command.AddInputParameter("P_USER_NAME", Context.UserIdentifier);
        }

        protected override string GetCommandText(ShareDashboardParameterSet parameters)
        {
            return "P_dashBoard.SHARE_DASHBOARD";
        }
    }
}