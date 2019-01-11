using GazRouter.DAL.Core;
using GazRouter.DTO.Balances.Docs;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Balances.Docs
{
    public class EditDocCommand : CommandNonQuery<EditDocParameterSet>
    {
        public EditDocCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, EditDocParameterSet parameters)
        {
            command.AddInputParameter("p_doc_id", parameters.DocId);
            command.AddInputParameter("p_contract_id", parameters.ExternalId);
            command.AddInputParameter("p_doc_date", parameters.DocDate);
            command.AddInputParameter("p_data", parameters.Data);
            command.AddInputParameter("p_file_name", parameters.FileName);
            command.AddInputParameter("p_description", parameters.Description);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(EditDocParameterSet parameters)
        {
            return "P_BL_DOC.Edit";
        }
    }
}
