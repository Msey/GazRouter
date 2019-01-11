using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.Segment;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.Segment.Site
{
    public class AddSiteSegmentCommand : CommandScalar<AddSiteSegmentParameterSet, int>
    {
        public AddSiteSegmentCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
            IntegrityConstraints.Add("ORA-20104: У смежных сегментов НЕ должно быть одинаковых ЛПУ", "У смежных сегментов НЕ должно быть одинаковых ЛПУ");
        }

        protected override void BindParameters(OracleCommand command, AddSiteSegmentParameterSet parameters)
        {
            OutputParameter = command.AddReturnParameter<int>("p_entity_id");
            command.AddInputParameter("p_site_id", parameters.SiteId);
            command.AddInputParameter("p_pipeline_id", parameters.PipelineId);
            command.AddInputParameter("p_kilometer_start", parameters.KilometerOfStartPoint);
            command.AddInputParameter("p_kilometer_end", parameters.KilometerOfEndPoint);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(AddSiteSegmentParameterSet parameters)
        {
            return "rd.P_SEGMENT_BY_SITE.AddF";
        }
    }
}
