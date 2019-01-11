using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.ReducingStations;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.ReducingStations
{
    public class EditReducingStationCommand : CommandNonQuery<EditReducingStationParameterSet>
    {
        public EditReducingStationCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, EditReducingStationParameterSet parameters)
        {
            command.AddInputParameter("p_entity_id", parameters.ReducingStationId);
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

        protected override string GetCommandText(EditReducingStationParameterSet parameters)
        {
            return "rd.P_REDUCING_STATION.Edit";
        }
    }
}
