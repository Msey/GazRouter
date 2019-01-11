using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazRouter.DAL.ObjectModel.Segment.Regions
{
    public class DeleteRegionSegmentCommand : CommandNonQuery<int>
    {
        public DeleteRegionSegmentCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, int parameters)
        {
            command.AddInputParameter("p_segments_by_region_id", parameters);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }


        protected override string GetCommandText(int parameters)
        {
            return "rd.P_SEGMENT_BY_REGION.Remove";
        }
    }
}
