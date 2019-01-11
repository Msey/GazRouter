using GazRouter.DAL.Core;
using GazRouter.DTO.Dashboards.Dashboard;
using Oracle.ManagedDataAccess.Client;
namespace GazRouter.DAL.ExcelReport
{
    public class EditDashboardCommand : CommandNonQuery<EditDashboardParameterSet>
    {
        public EditDashboardCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, EditDashboardParameterSet parameters)
        {
            command.AddInputParameter("P_DASHBOARD_ID", parameters.DashboardId);
            command.AddInputParameter("P_DASHBOARD_NAME", parameters.DashboardName);
            command.AddInputParameter("P_PERIOD_TYPE_ID", parameters.PeriodTypeId);
            command.AddInputParameter("P_FOLDER_ID", parameters.FolderId);
            command.AddInputParameter("P_USER_NAME", Context.UserIdentifier);
        }

        protected override string GetCommandText(EditDashboardParameterSet parameters)
        {
            return "P_dashBoard.Edit";
        }
    }
}