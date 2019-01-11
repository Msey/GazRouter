using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.Segment;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.Segment.Site
{
    public class EditSiteSegmentCommand : CommandNonQuery<EditSiteSegmentParameterSet>
    {
        public EditSiteSegmentCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
            IntegrityConstraints.Add("ORA-20104: У смежных сегментов НЕ должно быть одинаковых ЛПУ", "У смежных сегментов НЕ должно быть одинаковых ЛПУ");
        }

        protected override void BindParameters(OracleCommand command, EditSiteSegmentParameterSet parameters)
        {
            command.AddInputParameter("p_segments_by_sites_id", parameters.SegmentId);
            command.AddInputParameter("p_site_id", parameters.SiteId);
            command.AddInputParameter("p_pipeline_id", parameters.PipelineId);
            command.AddInputParameter("p_kilometer_start", parameters.KilometerOfStartPoint);
            command.AddInputParameter("p_kilometer_end", parameters.KilometerOfEndPoint);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(EditSiteSegmentParameterSet parameters)
        {
            return "rd.P_SEGMENT_BY_SITE.Edit";
        }
    }
}
