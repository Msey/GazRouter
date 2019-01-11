using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Balances.DistrNetworks;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Balances.DistrNetworks
{

	public class GetDistrNetworkListQuery : QueryReader<List<DistrNetworkDTO>>
	{
		public GetDistrNetworkListQuery(ExecutionContext context)
			: base(context)
		{ }

        protected override string GetCommandText()
		{
            
            return @"   SELECT      dn.distr_network_id,
                                    dn.distr_network_name,
                                    dn.region_id,
                                    r.region_name 
                        FROM        v_distr_networks dn
                        INNER JOIN  v_regions r ON r.region_id = dn.region_id";
		}

        protected override List<DistrNetworkDTO> GetResult(OracleDataReader reader)
		{

            var result = new List<DistrNetworkDTO>();
            while (reader.Read())
            {
                result.Add(
                    new DistrNetworkDTO
                    {
                        Id = reader.GetValue<int>("distr_network_id"),
                        Name = reader.GetValue<string>("distr_network_name"),
                        RegionId = reader.GetValue<int>("region_id"),
                        RegionName = reader.GetValue<string>("region_name")
                    });

            }
            return result;
        }
	}
}
