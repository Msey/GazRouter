using DAL.Core;
using DTO.Dashboards.DashboardFolder;
using Oracle.ManagedDataAccess.Client;

namespace DAL.Dashboards.DashboardElements
{
    public class DeleteDashboardElementCommand : CommandNonQuery<int>
    {
        public DeleteDashboardElementCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, int parameters)
        {
            command.AddInputParameter("p_dashboard_element_id", parameters);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(int parameters)
        {
            return "P_DASHBOARD_ELEMENT.Remove";

        }

    }
}