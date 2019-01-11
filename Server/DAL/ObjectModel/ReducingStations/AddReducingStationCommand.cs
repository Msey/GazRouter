using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.ReducingStations;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.ReducingStations
{
    public class AddReducingStationCommand : CommandScalar<AddReducingStationParameterSet, Guid>
    {
        public AddReducingStationCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddReducingStationParameterSet parameters)
        {
            if (parameters.Id.HasValue)
            {
                command.AddInputParameter("p_entity_id", parameters.Id.Value);
            }
            OutputParameter = command.AddReturnParameter<Guid>("entity_id");
            command.AddInputParameter("p_entity_name", parameters.Name);
            command.AddInputParameter("p_status", parameters.Status);
            command.AddInputParameter("p_sort_order", parameters.SortOrder);
            command.AddInputParameter("p_hidden", parameters.Hidden);
            command.AddInputParameter("p_is_virtual", parameters.IsVirtual);
            command.AddInputParameter("p_site_id", parameters.SiteId);
            command.AddInputParameter("p_pipeline_id", parameters.MainPipelineId);
            command.AddInputParameter("p_kilometer", parameters.Kilometer);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(AddReducingStationParameterSet parameters)
        {
            return "rd.P_REDUCING_STATION.AddF";
        }
    }
}
