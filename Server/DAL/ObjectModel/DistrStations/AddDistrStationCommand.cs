using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.DistrStations;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.DistrStations
{
    public class AddDistrStationCommand : CommandScalar<AddDistrStationParameterSet, Guid>
    {
        public AddDistrStationCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddDistrStationParameterSet parameters)
        {
            if (parameters.Id.HasValue)
            {
                command.AddInputParameter("p_entity_id", parameters.Id.Value);
            }

            OutputParameter = command.AddReturnParameter<Guid>("entity_id");
            command.AddInputParameter("p_entity_name", parameters.Name);
            command.AddInputParameter("p_region_id", parameters.RegionId);
            command.AddInputParameter("p_site_id", parameters.ParentId);
            command.AddInputParameter("p_hidden", parameters.IsHidden);
            command.AddInputParameter("p_is_virtual", parameters.IsVirtual);
            command.AddInputParameter("p_is_foreign", parameters.IsForeign);
            command.AddInputParameter("p_use_in_balance", parameters.UseInBalance);
            command.AddInputParameter("p_pressure_rated", parameters.PressureRated);
            command.AddInputParameter("p_capacity_rated", parameters.CapacityRated);
            command.AddInputParameter("p_bal_group_id", parameters.BalanceGroupId);
			command.AddInputParameter("p_sort_order", parameters.SortOrder);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(AddDistrStationParameterSet parameters)
        {
            return "rd.P_DISTR_STATION.AddF";
        }

    }
}