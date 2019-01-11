using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.Segment;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.Segment.Pressure
{
    public class EditPressureSegmentCommand : CommandNonQuery<EditPressureSegmentParameterSet>
    {
        public EditPressureSegmentCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, EditPressureSegmentParameterSet parameters)
        {
            command.AddInputParameter("p_segments_by_pressure_id", parameters.SegmentId);
            command.AddInputParameter("p_pipeline_id", parameters.PipelineId);
            command.AddInputParameter("p_kilometer_start", parameters.KilometerOfStartPoint);
            command.AddInputParameter("p_kilometer_end", parameters.KilometerOfEndPoint);
            command.AddInputParameter("p_pressure", parameters.Pressure);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(EditPressureSegmentParameterSet parameters)
        {
            return "rd.P_SEGMENT_BY_PRESSURE.Edit";
        }
    }
}
