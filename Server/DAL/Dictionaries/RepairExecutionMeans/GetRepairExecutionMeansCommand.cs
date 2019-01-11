using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.RepairExecutionMeans;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Dictionaries.RepairExecutionMeans
{
	public class GetRepairExecutionMeansQuery : QueryReader<List<RepairExecutionMeansDTO>>
	{
        public GetRepairExecutionMeansQuery(ExecutionContext context)
			: base(context)
		{
		}

		protected override string GetCommandText()
		{
            return @"   SELECT  execution_means_id,
	                            execution_means_name
                        FROM	v_execution_means";
		}

        protected override List<RepairExecutionMeansDTO> GetResult(OracleDataReader reader)
		{
            var result = new List<RepairExecutionMeansDTO>();
			while (reader.Read())
			{
				var exMeans =
                   new RepairExecutionMeansDTO
				   {
                       Id = reader.GetValue<int>("execution_means_id"),
                       Name = reader.GetValue<string>("execution_means_name"),
				   };
                result.Add(exMeans);
			}
			return result;
		}
	}
}