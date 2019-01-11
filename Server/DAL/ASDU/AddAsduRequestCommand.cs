using GazRouter.DAL.Core;
using GazRouter.DTO.ASDU;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.DispatcherTasks
{
    public class AddAsduRequestCommand : CommandScalar<AsduRequestParams, int>
    {
        public AddAsduRequestCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AsduRequestParams parameters)
        {
            OutputParameter = command.AddReturnParameter<int>("dummy");
            command.AddInputParameter("p_entity_raw_id", parameters.EntityId);
            command.AddInputParameter("p_rawid", parameters.Id);
            command.AddInputParameter("p_rawtype", parameters.Type);
            command.AddInputParameter("p_request_key", parameters.RequestKey);
            command.AddInputParameter("p_request_kind", parameters.RequestKind);
        }

        protected override string GetCommandText(AsduRequestParams parameters)
        {
            return "integro.P_MD_BINDINGDATA.add_to_asdu_request";
        }
    }
}