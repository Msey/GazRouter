using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.Aggregators;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.Aggregators
{
    public class AddAggregatorCommand : CommandScalar<AddAggregatorParameterSet, Guid>
    {
        public AddAggregatorCommand(ExecutionContext context) : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddAggregatorParameterSet parameters)
        {
            OutputParameter = command.AddReturnParameter<Guid>("entity_id");
            command.AddInputParameter("p_entity_name", parameters.Name);
            command.AddInputParameter("p_aggr_type_id", parameters.AggregatorType);
			command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(AddAggregatorParameterSet parameters)
        {
            return "P_AGGREGATOR.AddF";
        }
    }
}