using System.Collections.Generic;
using GazRouter.DAL.Core;

using GazRouter.DTO.Dictionaries.TransportTypes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Dictionaries.TransportTypes
{
	public class GetTransportTypeListQuery : QueryReader<List<TransportTypeDTO>>
	{
        public GetTransportTypeListQuery(ExecutionContext context)
			: base(context)
		{
		}

		protected override string GetCommandText()
		{
		    return @"   SELECT  transport_type_id,
                                transport_type_name
                        FROM	v_transport_types";
		}

        protected override List<TransportTypeDTO> GetResult(OracleDataReader reader)
		{
            var result = new List<TransportTypeDTO>();
			while (reader.Read())
			{
				var periodType =
                   new TransportTypeDTO
				   {
                       Id = reader.GetValue<int>("transport_type_id"),
                       Name = reader.GetValue<string>("transport_type_name")
				   };
				result.Add(periodType);
			}
			return result;
		}
	}
}