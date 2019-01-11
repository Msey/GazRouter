using GazRouter.DAL.Core;
using GazRouter.DTO.Dashboards.Folders;
using Oracle.ManagedDataAccess.Client;
namespace GazRouter.DAL.ExcelReport
{
    public class SetDashboardForderSortOrderCommand : CommandNonQuery<SetSortOrderParameterSet>
    {
        public SetDashboardForderSortOrderCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, SetSortOrderParameterSet parameters)
        {
            command.AddInputParameter("P_DASHBOARD_ID", parameters.Id);
            command.AddInputParameter("P_FOLDER_ID", parameters.FolderId);
            command.AddInputParameter("p_sort_order", parameters.SortOrder);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(SetSortOrderParameterSet parameters)
        {
            return "rd.P_DASHBOARD_FOLDER.EDIT";
        }
    }
}