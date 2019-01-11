using GazRouter.DAL.Core;
using GazRouter.DTO.Dashboards.DashboardContent;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Dashboards.DashboardContent
{
    public class UpdateDashboardContentCommand : CommandNonQuery<DashboardContentDTO>
    {
        public UpdateDashboardContentCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, DashboardContentDTO parameters)
        {
            command.AddInputParameter("P_DASHBOARD_ID", parameters.DashboardId);
            command.AddInputParameter("P_DASHBOARD_CONTENT", parameters.Content);
            command.AddInputParameter("P_USER_NAME", Context.UserIdentifier);
        }

        protected override string GetCommandText(DashboardContentDTO parameters)
        {
            return "rd.P_DASHBOARD.Edit";
        }

    }
}