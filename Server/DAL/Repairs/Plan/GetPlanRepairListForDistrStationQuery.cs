using System;
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

namespace GazRouter.DAL.Repairs.Plan
{
    /// <summary>
    /// Для ГАЗОПРОВОДОВ
    /// </summary>
    public class GetPlanRepairListForDistrStationQuery : QueryReader<GetRepairPlanParameterSet, List<RepairPlanBaseDTO>>
    {
        public GetPlanRepairListForDistrStationQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override string GetCommandText(GetRepairPlanParameterSet parameters)
        {
            return @"   SELECT	    r.repair_id,
                                    r.entity_id,
                                    r.entity_name,
                                    r.entity_type_id,
                                    r.bleed_amount,
                                    r.gas_saving,
                                    
                                    r.repair_type_id,
                                    r.repair_type_name,

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

r.duration,
r.gazprom_plan_id,
    r.consumers_state,
    r.plan_reg_date,

                                    site.site_name

                                    
                        FROM	    V_REPAIRS r
                        LEFT JOIN   V_PPW_COMPLEXES cmpl ON r.complex_id = cmpl.complex_id
                        LEFT JOIN   rd.V_SITES site on rd.P_ENTITY.GetSiteId(r.Entity_Id) = site.Site_Id
                        
                        WHERE       site.system_id = :systemid
                        AND         ((date_start_plan >= :startdate AND date_start_plan < :enddate) OR (date_end_plan >= :startdate AND date_end_plan < :enddate) OR (:startdate >=date_start_plan AND date_end_plan >= :enddate))
                        AND         r.entity_type_id IN (8, 27)
                        AND         plan_type_id = 1" +
                        (parameters.SiteId.HasValue ? " AND site.Site_Id = :siteid " : " ")
                        + " ORDER BY    entity_name";
        }

        protected override void BindParameters(OracleCommand command, GetRepairPlanParameterSet parameters)
        {
            command.AddInputParameter("systemid", parameters.SystemId);
            if (parameters.SiteId.HasValue)
                command.AddInputParameter("siteid", parameters.SiteId.Value);
            var startDate = new DateTime(parameters.Year, 1, 1);
            command.AddInputParameter("startdate", startDate.ToLocal());
            command.AddInputParameter("enddate", startDate.AddYears(1).AddSeconds(-1).ToLocal());
        }

        protected override List<RepairPlanBaseDTO> GetResult(OracleDataReader reader,
            GetRepairPlanParameterSet parameters)
        {
            var repairList = new List<RepairPlanBaseDTO>();
            while (reader.Read())
            {
                var complexId = reader.GetValue<int?>("complex_id");
                repairList.Add(
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
                    });
            }
            return repairList;
        }
    }
}
