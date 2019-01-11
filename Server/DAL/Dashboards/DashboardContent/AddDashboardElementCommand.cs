using DAL.Core;
using DTO.Dashboards.DashboardElements;
using DTO.Dashboards.DashboardFolder;
using Oracle.ManagedDataAccess.Client;

namespace DAL.Dashboards.DashboardElements
{
    public class AddDashboardElementCommand : CommandScalar<AddDashboardElementParameterSet, int>
    {
        public AddDashboardElementCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddDashboardElementParameterSet parameters)
        {
            OutputParameter = command.AddReturnParameter<int>("p_dashboard_element_id");
            command.AddInputParameter("P_DASHBOARD_ID", parameters.DashboardId);
            command.AddInputParameter("P_ENTITY_ID", parameters.EntityId);
            command.AddInputParameter("P_X", parameters.X);
            command.AddInputParameter("P_Y", parameters.Y);
            command.AddInputParameter("P_Z", parameters.Z);
            command.AddInputParameter("P_VIEW_TYPE", (int)parameters.DashboardElementViewType);
            command.AddInputParameter("P_USER_NAME", Context.UserIdentifier);
        }

        protected override string GetCommandText(AddDashboardElementParameterSet parameters)
        {
            return "P_DASHBOARD_ELEMENT.AddF";
        }

    }
}