using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.Segment;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazRouter.DAL.ObjectModel.Segment.Regions
{
    public class AddRegionSegmentCommand : CommandScalar<AddRegionSegmentParameterSet, int>
    {
        public AddRegionSegmentCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
            IntegrityConstraints.Add("ORA-20104: У смежных сегментов НЕ должно быть одинаковых ЛПУ", "У смежных сегментов НЕ должно быть одинаковых ЛПУ");
        }

        protected override void BindParameters(OracleCommand command, AddRegionSegmentParameterSet parameters)
        {
            OutputParameter = command.AddReturnParameter<int>("p_entity_id");
            command.AddInputParameter("p_pipeline_id", parameters.PipelineId);
            command.AddInputParameter("p_region_id", parameters.RegionId);
            command.AddInputParameter("p_kilometer_start", parameters.KilometerOfStartPoint);
            command.AddInputParameter("p_kilometer_end", parameters.KilometerOfEndPoint);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(AddRegionSegmentParameterSet parameters)
        {
            return "rd.P_SEGMENT_BY_REGION.AddF";
        }
    }
}
