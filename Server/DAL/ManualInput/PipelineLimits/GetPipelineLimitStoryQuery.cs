using GazRouter.DAL.Core;
using GazRouter.DTO.ManualInput.PipelineLimits;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazRouter.DAL.ManualInput.PipelineLimits
{
    public class GetPipelineLimitStoryQuery : QueryReader<int?, List<PipelineLimitsStoryDTO>>
    {
    public GetPipelineLimitStoryQuery(ExecutionContext context)
            : base(context)
        { }

    protected override string GetCommandText(int? parameters)
    {
            var q =
                @"select    t1.limit_id,
                            t1.change_date,
                            t1.status,
                            t1.change_user,
                            u.name,
                            u.description,
                            s.entity_name
                from rd.v_pipeline_limits_story t1 join ( rd.v_users u left join rd.v_entities s on u.site_id = s.entity_id) on u.login = t1.change_user 
                WHERE 1=1";


            var sb = new StringBuilder(q);

            if (parameters != null)
            {
                sb.Append(" AND t1.limit_id = : limit");
            }

            return sb.ToString();
        }


    protected override void BindParameters(OracleCommand command,  int? parameters)
    {
        if (parameters == null) return;
        command.AddInputParameter("limit", parameters);
    }

    protected override List<PipelineLimitsStoryDTO> GetResult(OracleDataReader reader, int? parameters)
    {
        var tests = new List<PipelineLimitsStoryDTO>();
        while (reader.Read())
        {
            var test =
                new PipelineLimitsStoryDTO
                {
                    EntityId = reader.GetValue<int>("limit_id"),
                    ChangeDate = reader.GetValue<DateTime>("change_date"),
                    Status = reader.GetValue<LimitStatus>("status"),
                    User = reader.GetValue<string>("change_user"),
                    UserName = reader.GetValue<string>("name"),
                    UserDescription = reader.GetValue<string>("description"),
                    UserSite = reader.GetValue<string>("entity_name"),
                };
            tests.Add(test);
        }
        return tests;
    }

}
}
