using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.Targets;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Dictionaries.Targets
{
	public class GetTargetListQuery : QueryReader<List<TargetDTO>>
	{
        public GetTargetListQuery(ExecutionContext context)
			: base(context)
		{
		}

		protected override string GetCommandText()
		{
		    return @"   SELECT  target_id,
                                target_name,
                                sys_name
                        FROM	v_targets";
		}

        protected override List<TargetDTO> GetResult(OracleDataReader reader)
		{
            var result = new List<TargetDTO>();
			while (reader.Read())
			{
				var periodType =
                   new TargetDTO
				   {
                       Id = reader.GetValue<int>("target_id"),
                       SysName = reader.GetValue<string>("sys_name"),
                       Name = reader.GetValue<string>("target_name")
				   };
				result.Add(periodType);
			}
			return result;
		}
	}
}