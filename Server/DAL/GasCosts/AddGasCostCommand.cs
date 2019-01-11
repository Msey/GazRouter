using GazRouter.DAL.Core;
using GazRouter.DTO.GasCosts;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.GasCosts
{
    public class AddGasCostCommand : CommandScalar<AddGasCostParameterSet, int>
    {
        public AddGasCostCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override string GetCommandText(AddGasCostParameterSet parameters)
        {
            return "P_AUX_COST.AddF";
        }

        protected override void BindParameters(OracleCommand command, AddGasCostParameterSet parameters)
        {
            OutputParameter = command.AddReturnParameter<int>("aux_cost_id");
            command.AddInputParameter("p_aux_item_id", parameters.CostType);
            command.AddInputParameter("p_entity_id", parameters.EntityId);
            //command.AddInputParameter("p_aux_date", parameters.Date);
            command.AddInputParameter("p_series_id", parameters.SeriesId);
            command.AddInputParameter("p_calculated_volume", parameters.CalculatedVolume);
            command.AddInputParameter("p_measured_volume", parameters.MeasuredVolume);
            command.AddInputParameter("p_raw_data", parameters.InputData);
            command.AddInputParameter("p_target_id", parameters.Target);
            command.AddInputParameter("p_site_id", parameters.SiteId);
            command.AddInputParameter("p_aux_cost_import_id", parameters.ImportId);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }
    }
}