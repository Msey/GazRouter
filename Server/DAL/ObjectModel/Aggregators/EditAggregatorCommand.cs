using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.Aggregators;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.Aggregators
{
    public class EditAggregatorCommand : CommandNonQuery<EditAggregatorParameterSet>
    {
        public EditAggregatorCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, EditAggregatorParameterSet parameters)
        {
            command.AddInputParameter("p_entity_id", parameters.Id);
            command.AddInputParameter("p_entity_name", parameters.Name);
            command.AddInputParameter("p_aggr_type_id", parameters.AggregatorType);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(EditAggregatorParameterSet parameters)
        {
            return "P_AGGREGATOR.Edit";
        }
    }
}