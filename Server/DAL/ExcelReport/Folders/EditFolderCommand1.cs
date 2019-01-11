using GazRouter.DAL.Core;
using GazRouter.DTO.Dashboards.Folders;

using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ExcelReport.Folders
{
    public class EditFolderCommand : CommandNonQuery<EditFolderParameterSet>
    {
        public EditFolderCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, EditFolderParameterSet parameters)
        {
            command.AddInputParameter("P_FOLDER_ID", parameters.FolderId);
            command.AddInputParameter("P_FOLDER_NAME", parameters.Name);
            command.AddInputParameter("P_PARENT_FOLDER_ID", parameters.ParentId);

            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(EditFolderParameterSet parameters)
        {
            return "P_FOLDER.Edit";
        }
    }
}