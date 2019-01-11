using GazRouter.DAL.Core;
using GazRouter.DTO.Dashboards.Dashboard;
using Oracle.ManagedDataAccess.Client;
namespace GazRouter.DAL.Dashboards.Dashboards
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
            command.AddInputParameter("P_SORT_ORDER", parameters.SortOrder);
            command.AddInputParameter("P_IS_DELETED", parameters.IsDeleted);
            command.AddInputParameter("P_USER_NAME", Context.UserIdentifier);
        }

        protected override string GetCommandText(EditDashboardParameterSet parameters)
        {
            return "P_dashBoard.Edit";
        }
    }
}