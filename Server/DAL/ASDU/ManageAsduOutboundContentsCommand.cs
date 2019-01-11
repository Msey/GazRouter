using GazRouter.DAL.Core;
using GazRouter.DTO.ASDU;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.DispatcherTasks
{
    public class ManageAsduOutboundContentsCommand : CommandScalar<ManageRequestParams, int>
    {
        public ManageAsduOutboundContentsCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, ManageRequestParams parameters)
        {
            OutputParameter = command.AddReturnParameter<int>("dummy");
            command.AddInputParameter("p_id", parameters.Key);
            //command.AddInputParameter("p_delete", parameters.Delete ? 1 : 0);
        }

        protected override string GetCommandText(ManageRequestParams parameters)
        {
            return "integro.p_md_tree.manage_asdu_outbound_content";
        }
    }
}