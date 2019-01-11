using GazRouter.DAL.Core;
using GazRouter.DTO.ASDU;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.DispatcherTasks
{
    public class ApplyChangeToIusCommand : CommandScalar<MatchingTreeNode, int>
    {
        public ApplyChangeToIusCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, MatchingTreeNode parameters)
        {
            OutputParameter = command.AddReturnParameter<int>("dummy");
            command.AddInputParameter("p_nodetype", parameters.Nodetype);
            command.AddInputParameter("p_type", parameters.Type);
            command.AddInputParameter("p_id", parameters.Id);
        }

        protected override string GetCommandText(MatchingTreeNode parameters)
        {
            return "integro.p_md_tree.applyAsduToIus";
        }
    }
}