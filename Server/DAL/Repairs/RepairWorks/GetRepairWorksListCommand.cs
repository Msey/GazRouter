using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Repairs.RepairWorks;
using Oracle.ManagedDataAccess.Client;
using Utils.Extensions;

namespace GazRouter.DAL.Repairs.RepairWorks
{
    public class GetPlanRepairWorkListQuery : QueryReader<int, Dictionary<int, List<RepairWorkDTO>>>
    {
        public GetPlanRepairWorkListQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override string GetCommandText(int parameters)
        {
            return @"   SELECT      w.repair_work_id,
                                    w.work_type_id,
                                    w.repair_id,
	                                w.kilometer_start,
	                                w.kilometer_end,
	                                w.work_type_name,
									w.work_type_sys_name
                        FROM	    v_repair_works w
                        INNER JOIN  v_repairs r ON r.repair_id = w.repair_id
						WHERE       ((date_start_plan >= :startdate AND date_start_plan < :enddate) OR (date_end_plan >= :startdate AND date_end_plan < :enddate) OR (:startdate >=date_start_plan AND date_end_plan >= :enddate))
                        AND         r.plan_type_id = 1
                        ORDER BY	sort_order";
        }

        protected override Dictionary<int, List<RepairWorkDTO>> GetResult(OracleDataReader reader, int parameters)
        {
            var result = new Dictionary<int, List<RepairWorkDTO>>();
            while (reader.Read())
            {
                var dto = new RepairWorkDTO
                {
                    Id = reader.GetValue<int>("repair_work_id"),
                    WorkTypeId = reader.GetValue<int>("work_type_id"),
                    RepairId = reader.GetValue<int>("repair_id"),
                    KilometerStart = reader.GetValue<double?>("kilometer_start"),
                    KilometerEnd = reader.GetValue<double?>("kilometer_end"),
                    WorkTypeName = reader.GetValue<string>("work_type_name")
                };
                result.GetOrInsertNew(dto.RepairId).Add(dto);
            }
            return result;
        }

        protected override void BindParameters(OracleCommand command, int parameters)
        {
            //parameters - год плана
            var startDate = new DateTime(parameters, 1, 1, 0, 0, 0, DateTimeKind.Local);

            command.AddInputParameter("startdate", startDate);
            command.AddInputParameter("enddate", startDate.AddYears(1));
        }
    }
}