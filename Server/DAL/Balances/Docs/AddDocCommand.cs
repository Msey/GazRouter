using GazRouter.DAL.Core;
using GazRouter.DTO.Balances.Docs;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Balances.Docs
{
    public class AddDocCommand : CommandScalar<AddDocParameterSet, int>
    {
        public AddDocCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddDocParameterSet parameters)
        {
            OutputParameter = command.AddReturnParameter<int>("doc_id");
            command.AddInputParameter("p_contract_id", parameters.ExternalId);
            command.AddInputParameter("p_doc_date", parameters.DocDate);
            command.AddInputParameter("p_data", parameters.Data);
            command.AddInputParameter("p_file_name", parameters.FileName);
            command.AddInputParameter("p_description", parameters.Description);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(AddDocParameterSet parameters)
        {
            return "P_BL_DOC.AddF";
        }
    }
}
