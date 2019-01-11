using System.Collections.Generic;
using System.Linq;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.RepairTypes;
using GazRouter.DTO.Dictionaries.RepairWorksType;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Dictionaries.RepairTypes
{
	public class GetRepairTypeListQuery : QueryReader<List<RepairTypeDTO>>
	{
        public GetRepairTypeListQuery(ExecutionContext context)
			: base(context)
		{
		}

        protected override string GetCommandText()
		{
            return @"   SELECT      rt.repair_type_id,
                                    rt.entity_type_id,
                                    rt.repair_type_name,
                                    rt.sys_name AS rt_sys_name,
                                    rt.description AS rt_desc,
                                    rt.sort_order AS rt_sort_order,
                                    wt.work_type_id,
                                    wt.work_type_name                                                                                                        
                        FROM	    v_repair_types rt
                        LEFT JOIN   v_repair_works_types wt ON wt.repair_type_id = rt.repair_type_id
                        ORDER BY    rt.sort_order, wt.sort_order";
		}

        protected override List<RepairTypeDTO> GetResult(OracleDataReader reader)
		{
            var result = new List<RepairTypeDTO>();
			while (reader.Read())
			{
			    var rtid = reader.GetValue<int>("repair_type_id");
			    var repairType = result.SingleOrDefault(rt => rt.Id == rtid);
			    if (repairType == null)
			    {
			        repairType = new RepairTypeDTO
                    {
			            Id = rtid,
                        EntityType = reader.GetValue<EntityType>("entity_type_id"),
			            Name = reader.GetValue<string>("repair_type_name"),
			            SystemName = reader.GetValue<string>("rt_sys_name"),
			            Description = reader.GetValue<string>("rt_desc"),
			            SortOrder = reader.GetValue<int>("rt_sort_order")
			        };
			        result.Add(repairType);
			    }

                repairType.RepairWorkTypes.Add(
			        new RepairWorkTypeDTO
			        {
			            Id = reader.GetValue<int>("work_type_id"),
			            Name = reader.GetValue<string>("work_type_name")
			        });
			}
			return result;
		}
	}
}