using System;
using System.Collections.Generic;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.CompUnitFailureCauses;
using GazRouter.DTO.Dictionaries.CompUnitFailureFeatures;
using GazRouter.DTO.Dictionaries.CompUnitRepairTypes;
using GazRouter.DTO.Dictionaries.CompUnitStopTypes;
using GazRouter.DTO.Dictionaries.StatesModel;
using GazRouter.DTO.ManualInput.CompUnitStates;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ManualInput.CompUnitStates
{
    public class GetCompUnitStateListQuery : QueryReader<GetCompUnitStateListParameterSet, List<CompUnitStateDTO>>
    {
        public GetCompUnitStateListQuery(ExecutionContext context)
            : base(context)
        {
        }

      
        protected override string GetCommandText(GetCompUnitStateListParameterSet parameters)
        {
            var q = @"  SELECT      s.comp_unit_state_change_id,
                                    s.comp_unit_id,
                                    s.state_change_date,
                                    s.state,
                                    s.change_date,
                                    s.change_user_name,
                                    s.change_user_site_name,
                                    s.comp_unit_stop_type_id,
                                    s.comp_unit_repair_type_id,
                                    s.completion_date_plan,
                                    s.is_repair_next,
                                    f.is_critical,
                                    f.failure_cause_id,
                                    f.failure_feature_id,
                                    f.external_view,
                                    f.cause_description,
                                    f.work_performed
                        FROM        v_comp_unit_state_changes s
                        INNER JOIN  v_comp_units u ON u.comp_unit_id = s.comp_unit_id
                        LEFT JOIN   v_comp_unit_failure_details f ON f.comp_unit_state_change_id = s.comp_unit_state_change_id
                        LEFT JOIN   v_entity_2_site site ON site.entity_id = s.comp_unit_id
                        WHERE       1=1
                            AND   (s.comp_unit_id, s.state_change_date) IN 
                                    (SELECT comp_unit_id, MAX(state_change_date) 
                                        FROM v_comp_unit_state_changes 
                                        {0}                                        
                                        GROUP BY comp_unit_id)";

            q = string.Format(q,
                parameters != null && parameters.Timestamp.HasValue
                    ? " WHERE state_change_date <= :timestamp "
                    : string.Empty);
            
            var sb = new StringBuilder(q);

            if (parameters != null)
            {
                if (parameters.SiteId.HasValue) sb.Append(" AND site.site_id = :siteId");
                if (parameters.ShopId.HasValue) sb.Append(" AND u.comp_shop_id = :shopId");
            }

            return sb.ToString();
        }

        protected override void BindParameters(OracleCommand command, GetCompUnitStateListParameterSet parameters)
        {
            if (parameters == null) return;

            if (parameters.Timestamp.HasValue)
                command.AddInputParameter("timestamp", parameters.Timestamp);

            if (parameters.SiteId.HasValue)
                command.AddInputParameter("siteId", parameters.SiteId);

            if (parameters.ShopId.HasValue)
                command.AddInputParameter("shopId", parameters.ShopId);
        }

        protected override List<CompUnitStateDTO> GetResult(OracleDataReader reader, GetCompUnitStateListParameterSet parameters)
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
                    UserName = reader.GetValue<string>("change_user_name"),
                    UserSite = reader.GetValue<string>("change_user_site_name"),
                    StopType = reader.GetValue<CompUnitStopType?>("comp_unit_stop_type_id"),
                    CompletionDatePlan = reader.GetValue<DateTime?>("completion_date_plan"),
                    RepairType = reader.GetValue<CompUnitRepairType?>("comp_unit_repair_type_id") ?? CompUnitRepairType.Scheduled,
                    IsRepairNext = reader.GetValue<bool>("is_repair_next")
                };

                var timestamp = parameters != null && parameters.Timestamp.HasValue
                    ? parameters.Timestamp.Value
                    : DateTime.Now;

                dto.InStateDuration = timestamp - dto.StateChangeDate;
                dto.IsDelayed = dto.CompletionDatePlan.HasValue && dto.CompletionDatePlan.Value < timestamp;
                    
                
                if (dto.StopType != CompUnitStopType.Planned)
                    dto.FailureDetails = new CompUnitFailureDetailsDTO
                    {
                        FailureId = dto.Id,
                        IsCritical = reader.GetValue<bool>("is_critical"),
                        FailureCause = reader.GetValue<CompUnitFailureCause?>("failure_cause_id") ?? CompUnitFailureCause.None,
                        FailureFeature = reader.GetValue<CompUnitFailureFeature?>("failure_feature_id") ?? CompUnitFailureFeature.None,
                        FailureExternalView = reader.GetValue<string>("external_view"),
                        FailureCauseDescription = reader.GetValue<string>("cause_description"),
                        FailureWorkPerformed = reader.GetValue<string>("work_performed")
                    };

                result.Add(dto);
            }
            return result;
        }
    }
}
