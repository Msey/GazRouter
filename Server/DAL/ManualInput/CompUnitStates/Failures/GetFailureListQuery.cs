using System;
using System.Collections.Generic;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.CompUnitFailureCauses;
using GazRouter.DTO.Dictionaries.CompUnitFailureFeatures;
using GazRouter.DTO.Dictionaries.CompUnitStopTypes;
using GazRouter.DTO.Dictionaries.StatesModel;
using GazRouter.DTO.ManualInput.CompUnitStates;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ManualInput.CompUnitStates.Failures
{
    public class GetFailureListQuery : QueryReader<GetFailureListParameterSet, List<CompUnitStateDTO>>
    {
        public GetFailureListQuery(ExecutionContext context)
            : base(context)
        {
        }


        protected override string GetCommandText(GetFailureListParameterSet parameters)
        {
            var q = @"  SELECT      f.comp_unit_state_change_id,
                                    f.comp_unit_id,
                                    f.state_change_date,
                                    f.state,
                                    f.change_date,
                                    f.change_user,
                                    f.comp_unit_stop_type_id,
                                    f.is_critical,
                                    f.failure_cause_id,
                                    f.failure_feature_id,
                                    f.cause_description,
                                    f.external_view,
                                    f.work_performed,
                                    (SELECT MIN(s.state_change_date)
                                        FROM v_comp_unit_state_changes s
                                        WHERE s.comp_unit_id = f.comp_unit_id
                                            AND s.state = 1
                                            AND s.state_change_date > f.state_change_date) AS to_work_date,
                                    (SELECT MIN(s.state_change_date)
                                        FROM v_comp_unit_state_changes s
                                        WHERE s.comp_unit_id = f.comp_unit_id
                                            AND s.state = 2
                                            AND s.state_change_date > f.state_change_date) AS to_reserve_date
                        FROM        v_comp_unit_failure_details f
                        LEFT JOIN   v_entity_2_site site ON site.entity_id = f.comp_unit_id
                        WHERE       1=1";

            
            var sb = new StringBuilder(q);

            if (parameters != null)
            {
                sb.Append(" AND f.state_change_date BETWEEN :begin AND :end");
                if (parameters.SiteId.HasValue) sb.Append(" AND site.site_id = :siteId");
            }

            sb.Append(" ORDER BY f.state_change_date");

            return sb.ToString();
        }

        protected override void BindParameters(OracleCommand command, GetFailureListParameterSet parameters)
        {
            if (parameters == null) return;

            command.AddInputParameter("begin", parameters.Begin);
            command.AddInputParameter("end", parameters.End);

            if (parameters.SiteId.HasValue)
                command.AddInputParameter("siteId", parameters.SiteId);

            
        }

        protected override List<CompUnitStateDTO> GetResult(OracleDataReader reader, GetFailureListParameterSet parameters)
        {
            var result = new List<CompUnitStateDTO>();
            while (reader.Read())
            {
                var dto = new CompUnitStateDTO
                {
                    Id = reader.GetValue<int>("comp_unit_state_change_id"),
                    CompUnitId = reader.GetValue<Guid>("comp_unit_id"),
                    StateChangeDate = reader.GetValue<DateTime>("state_change_date"),
                    State = reader.GetValue<CompUnitState>("state"),
                    InputDate = reader.GetValue<DateTime>("change_date"),
                    UserName = reader.GetValue<string>("change_user"),
                    StopType = reader.GetValue<CompUnitStopType?>("comp_unit_stop_type_id"),
                    
                    FailureDetails = new CompUnitFailureDetailsDTO
                    {
                        FailureId = reader.GetValue<int>("comp_unit_state_change_id"),
                        IsCritical = reader.GetValue<bool>("is_critical"),
                        FailureCause = reader.GetValue<CompUnitFailureCause?>("failure_cause_id") ?? CompUnitFailureCause.None,
                        FailureFeature = reader.GetValue<CompUnitFailureFeature?>("failure_feature_id") ?? CompUnitFailureFeature.None,
                        FailureExternalView = reader.GetValue<string>("external_view"),
                        FailureCauseDescription = reader.GetValue<string>("cause_description"),
                        FailureWorkPerformed = reader.GetValue<string>("work_performed"),
                        ToWorkDate = reader.GetValue<DateTime?>("to_work_date"),
                        ToReserveDate = reader.GetValue<DateTime?>("to_reserve_date")
                    }
                };
                
                result.Add(dto);
            }
            return result;
        }
    }
}
