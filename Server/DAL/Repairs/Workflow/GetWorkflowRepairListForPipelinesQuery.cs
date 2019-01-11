﻿using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.RepairExecutionMeans;
using GazRouter.DTO.Repairs.Complexes;
using GazRouter.DTO.Repairs.Plan;
using Oracle.ManagedDataAccess.Client;
using Utils.Extensions;
using GazRouter.DTO.Repairs.RepairWorks;
using GazRouter.DTO.Repairs.Workflow;

namespace GazRouter.DAL.Repairs.Workflow
{
    /// <summary>
    /// Для ГАЗОПРОВОДОВ
    /// </summary>
    public class GetWorkflowRepairListForPipelinesQuery : QueryReader<GetRepairWorkflowsParameterSet, List<RepairPlanBaseDTO>>
    {
        public GetWorkflowRepairListForPipelinesQuery(ExecutionContext context)
            : base(context)
        {
        }

//                    return @"
//SELECT
//    r.repair_id,
//    r.entity_id,
//    r.entity_name,
//    r.entity_type_id,
//    r.bleed_amount,
//    r.gas_saving,
                                    
//    r.repair_type_id,
//    r.repair_type_name,

//    r.plan_type_id,

//    r.date_start_plan,
//    r.date_end_plan,
//    r.description,
//    r.description_gtp,
//    r.description_cpdd,
                                    
//    r.date_start_sched,
//    r.date_end_sched,
                                    
//    r.transfer_fact_w,
//    r.transfer_fact_s,
//    r.transfer_fact_x,
//    r.capacity_plan_w,
//    r.capacity_plan_s,
//    r.capacity_plan_x,
//    r.transfer_work,
//    r.is_transfer_relation,
//    r.execution_means_id,
//    r.is_ext_condition,
//    r.parts_delivery_date,
//    r.complex_id,
//    cmpl.complex_type,
//    cmpl.start_date as complex_start_date,
//    cmpl.end_date as complex_end_date,
//    cmpl.is_local,
                                    
//    r.upd_entity_name,
//    r.upd_user_name,
//    r.upd_date,

//    r.workflow_state,
//    r.repair_state,
//    r.fireworks_type,
//    r.date_start_fact,
//    r.date_end_fact,

//    r.resolution_date,
//    r.resolution_num,
//    r.resolution_date_cpdd,
//    r.resolution_num_cpdd,

//    r.Duration,
//    r.gazprom_plan_id, 
//    r.consumers_state,
//    r.plan_reg_date,

//    site.site_name,
//    site.site_id,
//    g.pipeline_group_name"
//    +
//    (parameters.UserId.HasValue? ", a.agreed_repair_record_id, a.creation_date, a.agreed_user_id, u.fio as agreed_fio" : "")
//    +

//@"
//FROM
//    V_REPAIRS r
//    LEFT JOIN   V_PPW_COMPLEXES cmpl ON r.complex_id = cmpl.complex_id
//    LEFT JOIN   V_PIPELINES p ON p.pipeline_id = r.entity_id
//    LEFT JOIN   (select w.repair_id,min(kilometer_start)min_kilometer_start FROM v_repair_works w group by w.repair_id) rw on rw.repair_id = r.repair_id
//    LEFT JOIN   rd.V_PIPELINE_GROUPS g ON g.pipeline_group_id = rd.P_SEGMENT_BY_GROUP.Get_GROUP_ID(r.Entity_Id, rw.min_kilometer_start)                        
//    LEFT JOIN   rd.V_SITES site on rd.P_ENTITY.GetSiteId(r.Entity_Id, rw.min_kilometer_start) = site.Site_Id"
//    +
//    (parameters.UserId.HasValue?
//    @"
//    left join v_agreed_repair_records a on a.repair_id = r.repair_id
//    left join v_agreed_users u on u.agreed_user_id = a.agreed_user_id "
//    : "")
//    +

        protected override string GetCommandText(GetRepairWorkflowsParameterSet parameters)
        {
            return
@"

with
W_REPAIR_WORKS as
(
  select t1.repair_id, min(t1.kilometer_start) kilometer_start, max(t1.kilometer_end) kilometer_end,count(1)cnt
        ,t2.entity_id
    from rd.V_REPAIR_WORKS t1
    join rd.V_REPAIRS t2 on t2.repair_id = t1.repair_id
    join rd.V_ENTITIES t3 on t3.entity_id = t2.entity_id and t3.entity_type_id = 34
   group by t1.repair_id,t2.entity_id
)
,W_REPAIRS as
(
  select t1.repair_id,count(1)cnt        ,max(t2.site_id)site_id
    from W_REPAIR_WORKS t1
    join rd.V_SEGMENTS_BY_SITES t2 on t2.pipeline_id = t1.entity_id and t2.kilometer_start < t1.kilometer_end and t1.kilometer_start <= t2.kilometer_end
  group by t1.repair_id
)
 
select 
r.repair_id,
    r.entity_id,
    r.entity_name,
    r.entity_type_id,
    r.bleed_amount,
    r.gas_saving,
                                    
    r.repair_type_id,
    r.repair_type_name,

    r.plan_type_id,

    r.date_start_plan,
    r.date_end_plan,
    r.description,
    r.description_gtp,
    r.description_cpdd,
                                    
    r.date_start_sched,
    r.date_end_sched,
                                    
    r.transfer_fact_w,
    r.transfer_fact_s,
    r.transfer_fact_x,
    r.capacity_plan_w,
    r.capacity_plan_s,
    r.capacity_plan_x,
    r.transfer_work,
    r.is_transfer_relation,
    r.execution_means_id,
    r.is_ext_condition,
    r.parts_delivery_date,
    r.complex_id,
    cmpl.complex_type,
    cmpl.start_date as complex_start_date,
    cmpl.end_date as complex_end_date,
    cmpl.is_local,
                                    
    r.upd_entity_name,
    r.upd_user_name,
    r.upd_date,

    r.workflow_state,
    r.repair_state,
    r.fireworks_type,
    r.date_start_fact,
    r.date_end_fact,

    r.resolution_date,
    r.resolution_num,
    r.resolution_date_cpdd,
    r.resolution_num_cpdd,

    r.Duration,
    r.gazprom_plan_id, 
    r.consumers_state,
    r.plan_reg_date,

    site.site_name,
    site.site_id,
    g.pipeline_group_name"
    +
    (parameters.UserId.HasValue ? ", a.agreed_repair_record_id, a.creation_date, a.agreed_user_id, u.fio as agreed_fio" : "")
    +
   @"
  from V_REPAIRS r
  left join V_PPW_COMPLEXES cmpl on r.complex_id = cmpl.complex_id
  left join V_PIPELINES p on p.pipeline_id = r.entity_id
  left join W_REPAIR_WORKS rw on rw.repair_id = r.repair_id
  left join W_REPAIRS rr on rr.repair_id = r.repair_id
  left join rd.V_PIPELINE_GROUPS g on g.pipeline_group_id = rd.P_SEGMENT_BY_GROUP.Get_GROUP_ID(r.Entity_Id, rw.kilometer_start)
  left join rd.V_SITES site on site.Site_Id = rr.site_id
"

    


    +
    (parameters.UserId.HasValue ?
    @"
    left join v_agreed_repair_records a on a.repair_id = r.repair_id
    left join v_agreed_users u on u.agreed_user_id = a.agreed_user_id "
    : "")
    +
@"
WHERE
    r.entity_type_id = 34
    AND "
    +
    (parameters.UserId.HasValue ?
    @"
    a.agreed_repair_record_id in 
    (
        SELECT
            a.agreed_repair_record_id
        FROM
            V_AGREED_REPAIR_RECORDS a
            LEFT JOIN V_AGREED_USERS u ON u.agreed_user_id = a.agreed_user_id
        WHERE
            (u.user_id = :p_user_id
            or
            u.user_id in 
            (
                select v2.user_id from
                    v_agreed_users v1
                    left join v_agreed_users v2 on v1.agreed_user_id_ref = v2.agreed_user_id
                where v1.user_id = :p_user_id and v2.user_id is not null)
            ) 
            and a.agreed_date is null
    )
    "
    :
    (@"
    p.system_id = :systemid
    AND ((date_start_plan >= :startdate AND date_start_plan < :enddate) OR (date_end_plan >= :startdate AND date_end_plan < :enddate) OR (:startdate >=date_start_plan AND date_end_plan >= :enddate))
    "
    +
    (parameters.SiteId.HasValue ? " AND site.Site_Id = :siteid " : " ")
    + 
    WorkStateDTO.GetQuery(parameters.Stage) + " ORDER BY    repair_id"
    )
    );
        }

        protected override void BindParameters(OracleCommand command, GetRepairWorkflowsParameterSet parameters)
        {
            if (parameters.UserId.HasValue)
            {
                command.AddInputParameter("p_user_id", parameters.UserId.Value);
            }
            else
            {
                command.AddInputParameter("systemid", parameters.SystemId);
                if (parameters.SiteId.HasValue)
                    command.AddInputParameter("siteid", parameters.SiteId.Value);
                var startDate = new DateTime(parameters.Year, 1, 1);
                command.AddInputParameter("startdate", startDate.ToLocal());
                command.AddInputParameter("enddate", startDate.AddYears(1).AddSeconds(-1).ToLocal());
            }
        }

        protected override List<RepairPlanBaseDTO> GetResult(OracleDataReader reader,
            GetRepairWorkflowsParameterSet parameters)
        {
            var repairList = new List<RepairPlanBaseDTO>();
            while (reader.Read())
            {
                var complexId = reader.GetValue<int?>("complex_id");

                var rep =
                    new RepairPlanBaseDTO
                    {
                        Id = reader.GetValue<int>("repair_id"),
                        EntityId = reader.GetValue<Guid>("entity_id"),
                        EntityType = reader.GetValue<EntityType>("entity_type_id"),
                        EntityName = reader.GetValue<string>("entity_name"),
                        SiteName = reader.GetValue<string>("site_name"),
                        BleedAmount = reader.GetValue<double>("bleed_amount"),
                        SavingAmount = reader.GetValue<double>("gas_saving"),
                        StartDate = reader.GetValue<DateTime>("date_start_plan"),
                        EndDate = reader.GetValue<DateTime>("date_end_plan"),

                        DateStartSched = reader.GetValue<DateTime?>("date_start_sched"),
                        DateEndSched = reader.GetValue<DateTime?>("date_end_sched"),

                        Description = reader.GetValue<string>("description"),
                        DescriptionGtp = reader.GetValue<string>("description_gtp"),
                        DescriptionCpdd = reader.GetValue<string>("description_cpdd"),
                        MaxTransferWinter = reader.GetValue<double>("transfer_fact_w"),
                        MaxTransferSummer = reader.GetValue<double>("transfer_fact_s"),
                        MaxTransferTransition = reader.GetValue<double>("transfer_fact_x"),
                        CapacityWinter = reader.GetValue<double>("capacity_plan_w"),
                        CapacitySummer = reader.GetValue<double>("capacity_plan_s"),
                        CapacityTransition = reader.GetValue<double>("capacity_plan_x"),
                        CalculatedTransfer = reader.GetValue<double>("transfer_work"),
                        IsCritical = reader.GetValue<bool>("is_transfer_relation"),
                        RepairTypeId = reader.GetValue<int>("repair_type_id"),
                        ExecutionMeans = reader.GetValue<ExecutionMeans>("execution_means_id"),
                        IsExternalCondition = reader.GetValue<bool>("is_ext_condition"),
                        PartsDeliveryDate = reader.GetValue<DateTime>("parts_delivery_date"),

                        PlanType = reader.GetValue<int>("plan_type_id"),

                        WFWState = new WorkStateDTO()
                        {
                            WFState = reader.GetValue<int?>("workflow_state").HasValue ?
                            (WorkStateDTO.WorkflowStates)reader.GetValue<int>("workflow_state") : WorkStateDTO.WorkflowStates.Undefined,
                            WState = reader.GetValue<int?>("repair_state").HasValue ?
                            (WorkStateDTO.WorkStates)reader.GetValue<int>("repair_state") : WorkStateDTO.WorkStates.Undefined
                        },

                        Firework = reader.GetValue<int?>("fireworks_type").HasValue ?
                         (FireworksDTO.FireTypes)reader.GetValue<int>("fireworks_type") : FireworksDTO.FireTypes.OtherWork,

                        DateStartFact = reader.GetValue<DateTime?>("date_start_fact"),
                        DateEndFact = reader.GetValue<DateTime?>("date_end_fact"),


                        Complex = complexId.HasValue
                            ? new ComplexDTO
                            {
                                Id = reader.GetValue<int>("complex_id"),
                                ComplexName = reader.GetValue<string>("complex_type"),
                                StartDate = reader.GetValue<DateTime>("complex_start_date"),
                                EndDate = reader.GetValue<DateTime>("complex_end_date"),
                                IsLocal = reader.GetValue<bool>("is_local"),
                            }
                            : null,
                        PipelineGroupName = reader.GetValue<string>("pipeline_group_name"),
                        UserName = reader.GetValue<string>("upd_user_name"),
                        UserSiteName = reader.GetValue<string>("upd_entity_name"),
                        LastUpdateDate = reader.GetValue<DateTime>("upd_date"),

                        ResolutionDate = reader.GetValue<DateTime?>("resolution_date"),
                        ResolutionDateCpdd = reader.GetValue<DateTime?>("resolution_date_cpdd"),
                        ResolutionNum = reader.GetValue<string>("resolution_num"),
                        ResolutionNumCpdd = reader.GetValue<string>("resolution_num_cpdd"),

                        GazpromPlanID = reader.GetValue<string>("gazprom_plan_id"),
                        Duration = reader.GetValue<int?>("duration"),
                        ConsumersState = reader.GetValue<string>("consumers_state"),
                        GazpromPlanDate = reader.GetValue<DateTime?>("plan_reg_date"),

                        SiteId = reader.GetValue<Guid>("site_id"),

                    };
                if (parameters.UserId.HasValue)
                {
                    rep.AgreedRecordID = reader.GetValue<int>("agreed_repair_record_id");
                    rep.AgreedUserName = reader.GetValue<string>("agreed_fio");
                    rep.AgreedCreationDate = reader.GetValue<DateTime>("creation_date");
                    rep.AgreedUserID = reader.GetValue<int>("agreed_user_id");
                }
                repairList.Add(rep);
            }
            return repairList;
        }
    }
}