using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.CompShops;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.CompShops
{
    public class EditCompShopCommand : CommandNonQuery<EditCompShopParameterSet>
    {
        public EditCompShopCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, EditCompShopParameterSet parameters)
        {

            command.AddInputParameter("p_entity_id", parameters.Id);
            command.AddInputParameter("p_comp_station_id", parameters.ParentId);
            command.AddInputParameter("p_pipeline_id", parameters.PipelineId);
            command.AddInputParameter("p_entity_name", parameters.Name);
            command.AddInputParameter("p_engine_class_id", parameters.EngineClassId);
            command.AddInputParameter("p_hidden", parameters.IsHidden);
            command.AddInputParameter("p_is_virtual", parameters.IsVirtual);
            command.AddInputParameter("P_KILOMETER_CONN", parameters.KmOfConn);
            command.AddInputParameter("p_piping_volume", parameters.PipingVolume);
            command.AddInputParameter("p_piping_volume_in", parameters.PipingVolumeIn);
            command.AddInputParameter("p_piping_volume_out", parameters.PipingVolumeOut);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(EditCompShopParameterSet parameters)
        {
            return "P_COMP_SHOP.Edit";
        }

    }
}