using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.OperConsumerType;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Dictionaries.OperConsumerType
{
    public class OperConsumerTypesQuery : QueryReader<List<OperConsumerTypeDTO>>
    {
        public OperConsumerTypesQuery(ExecutionContext context)
			: base(context)
		{
		}

		protected override string GetCommandText()
		{
		    return
		        @"select OPER_CONSUMER_TYPE_ID, oper_consumer_type_name
                  from V_OPER_CONSUMER_TYPES";
		}

		protected override List<OperConsumerTypeDTO> GetResult(OracleDataReader reader)
		{
			var result = new List<OperConsumerTypeDTO>();
			while (reader.Read())
			{
				var operConsumerType =
				   new OperConsumerTypeDTO
				   {
					   Id = reader.GetValue<int>("OPER_CONSUMER_TYPE_ID"),
					   Name = reader.GetValue<string>("oper_consumer_type_name")
				   };
				result.Add(operConsumerType);
			}
			return result;
		}
	}
}
