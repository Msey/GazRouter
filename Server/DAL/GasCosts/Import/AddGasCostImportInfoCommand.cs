using GazRouter.DAL.Core;
using GazRouter.DTO.GasCosts.Import;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.GasCosts.Import
{
    public class AddGasCostImportInfoCommand : CommandScalar<AddGasCostImportInfoParameterSet, int>
    {
        public AddGasCostImportInfoCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override string GetCommandText(AddGasCostImportInfoParameterSet parameters)
        {
            return "P_AUX_COST_IMPORT.AddF";
        }

        protected override void BindParameters(OracleCommand command, AddGasCostImportInfoParameterSet parameters)
        {
            OutputParameter = command.AddReturnParameter<int>("p_aux_cost_import_id");
            command.AddInputParameter("p_import_date", parameters.ImportDate);
            command.AddInputParameter("p_external_id", parameters.ExternalId);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }
    }
}