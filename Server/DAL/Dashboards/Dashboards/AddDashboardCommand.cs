using GazRouter.DAL.Core;
using GazRouter.DTO.Dashboards.Dashboard;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Dashboards.Dashboards
{
    public class AddDashboardCommand : CommandScalar<AddDashboardParameterSet, int>
    {
        public AddDashboardCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddDashboardParameterSet parameters)
        {
            OutputParameter = command.AddReturnParameter<int>("dashboard_id");
            command.AddInputParameter("P_DASHBOARD_NAME", parameters.DashboardName);
            command.AddInputParameter("P_PERIOD_TYPE_ID", parameters.PeriodTypeId);
            command.AddInputParameter("P_FOLDER_ID", parameters.FolderId);
            command.AddInputParameter("P_SORT_ORDER", parameters.SortOrder);
            command.AddInputParameter("P_USER_NAME", Context.UserIdentifier);
        }

        protected override string GetCommandText(AddDashboardParameterSet parameters)
        {
            return "P_dashBoard.AddF";
        }
    }
}