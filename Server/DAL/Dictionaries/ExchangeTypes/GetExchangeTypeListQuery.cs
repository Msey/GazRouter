using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.ExchangeTypes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Dictionaries.ExchangeTypes
{
	public class GetExchangeTypeListQuery : QueryReader<List<ExchangeTypeDTO>>
	{
        public GetExchangeTypeListQuery(ExecutionContext context)
			: base(context)
		{
		}

		protected override string GetCommandText()
		{
			return @"   SELECT  exchange_type_id,
                                exchange_type_name
                        FROM	v_exchange_types";
		}

        protected override List<ExchangeTypeDTO> GetResult(OracleDataReader reader)
		{
            var result = new List<ExchangeTypeDTO>();
			while (reader.Read())
			{
				var inconsistencyType =
                   new ExchangeTypeDTO
				   {
                       Id = reader.GetValue<int>("exchange_type_id"),
                       Name = reader.GetValue<string>("exchange_type_name")
				   };
				result.Add(inconsistencyType);
			}
			return result;
		}
	}
}