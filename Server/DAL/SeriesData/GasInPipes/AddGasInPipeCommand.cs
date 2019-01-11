using GazRouter.DAL.Core;
using GazRouter.DTO.SeriesData.GasInPipes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.SeriesData.GasInPipes
{
    public class AddGasInPipeCommand : CommandNonQuery<AddGasInPipeParameterSet>
    {
        public AddGasInPipeCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
            
        }

        protected override void BindParameters(OracleCommand command, AddGasInPipeParameterSet parameters)
        {
            command.AddInputParameter("p_series_id", parameters.SeriesId);
            command.AddInputParameter("p_pipeline_id", parameters.PipelineId);
            command.AddInputParameter("p_kilometer_start", parameters.StartKm);
            command.AddInputParameter("p_kilometer_end", parameters.EndKm);
            command.AddInputParameter("p_gaz_volume", parameters.Value);
            command.AddInputParameter("p_description", parameters.Description);
            
        }

        protected override string GetCommandText(AddGasInPipeParameterSet parameters)
        {
            return "rd.P_GAS_SUPPLY.Add";
        }

    }
   
}
