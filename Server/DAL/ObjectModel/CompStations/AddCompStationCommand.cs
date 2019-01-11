using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.CompStations;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.CompStations
{
    public class AddCompStationCommand : CommandScalar<AddCompStationParameterSet, Guid>
    {
        public AddCompStationCommand(ExecutionContext context) : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddCompStationParameterSet parameters)
        {
            if (parameters.Id.HasValue)
            {
                command.AddInputParameter("p_entity_id", parameters.Id.Value);
            }
            OutputParameter = command.AddReturnParameter<Guid>("entity_id");
            command.AddInputParameter("p_site_id", parameters.ParentId);
            command.AddInputParameter("p_region_id", parameters.RegionId);
            command.AddInputParameter("p_entity_name", parameters.Name);
			command.AddInputParameter("p_sort_order", parameters.SortOrder);
            command.AddInputParameter("p_use_in_balance", parameters.UseInBalance);

            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(AddCompStationParameterSet parameters)
        {
            return "P_COMP_STATION.AddF";
        }
    }
}