using GazRouter.DAL.Core;
using GazRouter.DTO.Dashboards.Folders;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ExcelReport.Folders
{
    public class AddExcelReportFolderCommand : CommandScalar<AddFolderParameterSet, int>
    {
        public AddExcelReportFolderCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddFolderParameterSet parameters)
        {
            OutputParameter = command.AddReturnParameter<int>("folder_id");
            command.AddInputParameter("P_FOLDER_NAME", parameters.Name);
            command.AddInputParameter("P_PARENT_FOLDER_ID", parameters.ParentId);
            command.AddInputParameter("p_sort_order", parameters.SortOrder);
            command.AddInputParameter("P_ROW_TYPE", 2);
            command.AddInputParameter("P_USER_NAME", Context.UserIdentifier);
        }

        protected override string GetCommandText(AddFolderParameterSet parameters)
        {
            return "P_Folder.AddF";
        }
    }
}