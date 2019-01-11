using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Repairs.Plan;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Repairs.Plan
{
    /// <summary>
    /// ДЛЯ КЦ
    /// </summary>
    public class GetRepairUpdateHistoryQuery : QueryReader<int, List<RepairUpdateDTO>>
    {
        public GetRepairUpdateHistoryQuery(ExecutionContext context)
            : base(context)
        {}

        protected override string GetCommandText(int parameters)
        {
            return @"   SELECT      t1.upd_user_name,
                                    t1.upd_entity_name, 
                                    t1.versions_operation,
                                    t1.upd_date
                        FROM        rd.V_REPAIRS_FLB t1
                        WHERE       t1.repair_id = :repairid
                        ORDER BY    t1.versions_time DESC";
        }

        protected override void BindParameters(OracleCommand command, int parameters)
		{
            command.AddInputParameter("repairid", parameters);
        }

        protected override List<RepairUpdateDTO> GetResult(OracleDataReader reader, int parameters)
        {
            var repairList = new List<RepairUpdateDTO>();
            while (reader.Read())
            {
            	repairList.Add(
                    new RepairUpdateDTO
                    {
                            UpdateDate = reader.GetValue<DateTime>("upd_date"),
                            UserName = reader.GetValue<string>("upd_user_name"),
                            SiteName = reader.GetValue<string>("upd_entity_name"),
                            Action = reader.GetValue<string>("versions_operation")

            			});
            }
            return repairList;
        }
    }
}
