using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.GasTransportSystems;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Dictionaries.GasTransportSystem
{
	public class GetGasTransportSystemListQuery : QueryReader<List<GasTransportSystemDTO>>
    {
		public GetGasTransportSystemListQuery(ExecutionContext context)
			: base(context)
        {
        }

        protected override string GetCommandText()
        {
            return @"   SELECT      s.system_id,
                                    s.system_name,
                                    s.description
                        FROM        v_systems s
                        INNER JOIN  v_enterprise_2_system e2s ON s.system_id = e2s.system_id";
        }

		protected override List<GasTransportSystemDTO> GetResult(OracleDataReader reader)
        {
			var result = new List<GasTransportSystemDTO>();
            while (reader.Read())
            {
				result.Add(new GasTransportSystemDTO
				{
				    Id = reader.GetValue<int>("system_id"), 
                    Name = reader.GetValue<string>("system_name"), 
                    Description = reader.GetValue<string>("description")
				});
            }
            return result;
        }
    }
}