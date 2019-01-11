using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Balances.BalanceGroups;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Balances.BalanceGroups
{
    public class GetBalanceGroupListQuery : QueryReader<int, List<BalanceGroupDTO>>
	{
		public GetBalanceGroupListQuery(ExecutionContext context) : base(context)
		{
		}

        protected override string GetCommandText(int parameters)
		{
            var sql = @"    SELECT      g.group_id,
                                        g.name,
                                        g.system_id,
                                        g.sort_order,
                                        s.system_name                                        
                            FROM        v_bl_groups g
                            INNER JOIN  v_systems s ON s.system_id = g.system_id  
                            WHERE       1=1
                                AND     g.system_id = :sysId
                            ORDER BY    g.sort_order";

            return sql;
		}

        protected override void BindParameters(OracleCommand command, int parameters)
        {
            command.AddInputParameter("sysId", parameters);
        }

        protected override List<BalanceGroupDTO> GetResult(OracleDataReader reader, int parameters)
		{
			var result = new List<BalanceGroupDTO>();
			while (reader.Read())
			{
				result.Add(
                    new BalanceGroupDTO
                    {
                        Id = reader.GetValue<int>("group_id"),
                        Name = reader.GetValue<string>("name"),
                        SortOrder = reader.GetValue<int>("sort_order"),
                        SystemId = reader.GetValue<int>("system_id"),
                        SystemName = reader.GetValue<string>("system_name")
                    });

			}
			return result;
		}
	}
}