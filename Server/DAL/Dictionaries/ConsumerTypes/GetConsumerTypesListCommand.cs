using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.ConsumerTypes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Dictionaries.ConsumerTypes
{
	public class GetConsumerTypesListQuery : QueryReader<List<ConsumerTypesDTO>>
	{
		public GetConsumerTypesListQuery(ExecutionContext context)
			: base(context)
		{
		}

		protected override string GetCommandText()
		{
		    return
		        @"select t1.consumer_type_id, t1.consumer_type_name, t1.description, t1.sort_order from rd.V_CONSUMER_TYPES t1";
		}

		protected override List<ConsumerTypesDTO> GetResult(OracleDataReader reader)
		{
			var result = new List<ConsumerTypesDTO>();
			while (reader.Read())
			{
				var periodType =
				   new ConsumerTypesDTO
				   {
					   Id = reader.GetValue<int>("consumer_type_id"),
					   Name = reader.GetValue<string>("consumer_type_name")
				   };
				result.Add(periodType);
			}
			return result;
		}
	}
}