using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.MeasStations;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.MeasStations
{
    public class AddMeasStationCommand : CommandScalar<AddMeasStationParameterSet, Guid>
    {
        public AddMeasStationCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddMeasStationParameterSet parameters)
        {
            if (parameters.Id.HasValue)
            {
                command.AddInputParameter("p_entity_id", parameters.Id.Value);
            }

            OutputParameter = command.AddReturnParameter<Guid>("entity_id");
            command.AddInputParameter("p_entity_name", parameters.Name);
            command.AddInputParameter("p_site_id", parameters.ParentId);
            command.AddInputParameter("p_neighbour_enterprise_id", parameters.NeighbourEnterpriseId);
            command.AddInputParameter("p_hidden", parameters.IsHidden);
            command.AddInputParameter("p_is_virtual", parameters.IsVirtual);
            command.AddInputParameter("p_balance_sign_id", parameters.BalanceSignId);
			command.AddInputParameter("p_sort_order", parameters.SortOrder);
			command.AddInputParameter("p_is_intermediate", parameters.IsIntermediate);
			command.AddInputParameter("p_bal_group_id", parameters.BalanceGroupId);
            command.AddInputParameter("p_bal_name", parameters.BalanceName);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);

        }

        protected override string GetCommandText(AddMeasStationParameterSet parameters)
        {
            return "rd.P_MEAS_STATION.AddF";
        }

    }

}