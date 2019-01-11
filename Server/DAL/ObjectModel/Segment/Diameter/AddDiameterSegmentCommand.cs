using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.Segment;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.Segment.Diameter
{
    public class AddDiameterSegmentCommand : CommandScalar<AddDiameterSegmentParameterSet, int>
    {
		public AddDiameterSegmentCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

		protected override void BindParameters(OracleCommand command, AddDiameterSegmentParameterSet parameters)
        {
            OutputParameter = command.AddReturnParameter<int>("p_entity_id");
            command.AddInputParameter("p_pipeline_id", parameters.PipelineId);
            command.AddInputParameter("p_kilometer_start", parameters.KilometerOfStartPoint);
            command.AddInputParameter("p_kilometer_end", parameters.KilometerOfEndPoint);
            command.AddInputParameter("P_DIAMETRS_EXTERNAL_ID", parameters.ExternalDiameterId);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

		protected override string GetCommandText(AddDiameterSegmentParameterSet parameters)
        {
            return "rd.P_SEGMENT_BY_DIAMETR.AddF";
        }
    }
}
