using System;
using System.Collections.Generic;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.GasCosts;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.GasCosts
{


    public class GetGasCostAccessListQuery : QueryReader<GetGasCostAccessListParameterSet, List<GasCostAccessDTO>>
    {
        public GetGasCostAccessListQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override string GetCommandText(GetGasCostAccessListParameterSet parameters)
        {
            var q = @"  SELECT      s.site_id,
                                    s.site_name, 
                                    NVL(f.is_restrict, 0) AS is_fact_restricted,
                                    NVL(p.is_restrict, 0) AS is_plan_restricted,
                                    NVL(n.is_restrict, 0) AS is_norm_restricted,
                                    f.user_name

                        FROM        v_sites s
                        LEFT JOIN   v_aux_access f ON (s.site_id = f.site_id AND f.target_id = 1 AND f.key_date = :keydate AND f.period_type_id=:period_type_id)
                        LEFT JOIN   v_aux_access p ON (s.site_id = p.site_id AND p.target_id = 2 AND p.key_date = :keydate AND p.period_type_id=:period_type_id)
                        LEFT JOIN   v_aux_access n ON (s.site_id = n.site_id AND n.target_id = 6 AND n.key_date = :keydate AND n.period_type_id=:period_type_id)
                        WHERE       1=1";
                        

            var sb = new StringBuilder(q);
            if (parameters.EnterpriseId.HasValue) sb.Append(" AND s.enterprise_id = :enterpriseid");
            if (parameters.SiteId.HasValue) sb.Append(" AND s.site_id = :siteid");

            sb.Append(" ORDER BY s.sort_order ASC");

            return sb.ToString();
        }

        protected override void BindParameters(OracleCommand command, GetGasCostAccessListParameterSet parameters)
        {
            command.AddInputParameter("keydate", parameters.Date);
            command.AddInputParameter("period_type_id", parameters.PeriodType);
            if (parameters.EnterpriseId.HasValue) command.AddInputParameter("enterpriseid", parameters.EnterpriseId);
            if (parameters.SiteId.HasValue) command.AddInputParameter("siteid", parameters.SiteId);
        }

        protected override List<GasCostAccessDTO> GetResult(OracleDataReader reader, GetGasCostAccessListParameterSet parameters)
        {
            var result = new List<GasCostAccessDTO>();
            while (reader.Read())
            {
                result.Add(new GasCostAccessDTO
                {
                    Date = parameters.Date,
                    SiteId = reader.GetValue<Guid>("site_id"),
                    SiteName = reader.GetValue<string>("site_name"),
                    Fact = !reader.GetValue<bool>("is_fact_restricted"),
                    Plan = !reader.GetValue<bool>("is_plan_restricted"),
                    Norm = !reader.GetValue<bool>("is_norm_restricted"),
                    ChangeUser = reader.GetValue<string>("user_name"),
                });
                
            }
            return result;
        }
    }
}