using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.OperConsumers;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.OperConsumers
{
    public class EditOperConsumerCommand : CommandNonQuery<AddEditOperConsumerParameterSet>
    {
        public EditOperConsumerCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddEditOperConsumerParameterSet parameters)
        {
            command.AddInputParameter("p_entity_id", parameters.Id);
            command.AddInputParameter("p_entity_name", parameters.ConsumerName);
            command.AddInputParameter("p_oper_consumer_type_id", parameters.ConsumerType);
            command.AddInputParameter("p_is_direct_connection", parameters.IsDirectConnection);
            command.AddInputParameter("p_region_id", parameters.RegionId);

            command.AddInputParameter("p_site_id", parameters.SiteId);
            command.AddInputParameter("p_distr_station_id", parameters.DistrStationId);

            command.AddInputParameter("p_bal_group_id", parameters.BalanceGroupId);

            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(AddEditOperConsumerParameterSet parameters)
        {
            return "rd.P_OPER_CONSUMER.Edit";
        }

    }
}