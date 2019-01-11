using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.Consumers;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.Consumers
{
    public class AddConsumerCommand : CommandScalar<AddConsumerParameterSet, Guid>
    {
        public AddConsumerCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddConsumerParameterSet parameters)
        {
            if (parameters.Id.HasValue)
            {
                command.AddInputParameter("p_entity_id", parameters.Id.Value);
            }

            OutputParameter = command.AddReturnParameter<Guid>("entity_id");
            command.AddInputParameter("p_entity_name", parameters.Name);
            command.AddInputParameter("p_hidden", parameters.IsHidden);
            command.AddInputParameter("p_is_virtual", parameters.IsVirtual);
            command.AddInputParameter("p_consumer_type_id", parameters.ConsumerType);
            command.AddInputParameter("p_distr_station_id", parameters.ParentId);
            command.AddInputParameter("p_region_id", parameters.RegionId);
			command.AddInputParameter("p_sort_order", parameters.SortOrder);
            command.AddInputParameter("p_use_in_balance", parameters.UseInBalance);
            command.AddInputParameter("p_distr_network_id", parameters.DistrNetworkId);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(AddConsumerParameterSet parameters)
        {
            return "rd.P_GAS_CONSUMER.AddF";
        }

    }
    
}