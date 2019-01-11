using System;
using System.Collections.Generic;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.AggregatorTypes;
using GazRouter.DTO.ObjectModel.Aggregators;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.Aggregators
{
	public class GetAggregatorListQuery : QueryReader<GetAggregatorListParameterSet, List<AggregatorDTO>>
	{
		public GetAggregatorListQuery(ExecutionContext context) : base(context)
		{
		}

        protected override string GetCommandText(GetAggregatorListParameterSet parameters)
        {
            var sql = @"   SELECT       a.aggr_id, 
                                        a.aggr_name, 
                                        a.description, 
                                        a.aggr_type_id, 
                                        a.aggr_type_name,
                                        e.bal_group_id
                            FROM        v_aggregators a
                            INNER JOIN  v_entities e ON e.entity_id = a.aggr_id
                            WHERE       1=1";

            var sb = new StringBuilder(sql);

            if (parameters != null)
            {
                if (parameters.AggregatorType.HasValue) sb.Append(@" AND aggr_type_id = :aggrType");
                if (parameters.SiteId.HasValue) sb.Append(@" AND site_id = :siteid");
                if (parameters.RegionId.HasValue) sb.Append(@" AND region_id = :regionid");
                if (parameters.SystemId.HasValue) sb.Append(@" AND system_id = :systemid");
            }

            sb.Append(@" ORDER BY aggr_type_id, aggr_name");

            return sb.ToString();
        }

	    protected override void BindParameters(OracleCommand command, GetAggregatorListParameterSet parameters)
	    {
            if (parameters != null)
            {
                if (parameters.AggregatorType.HasValue) command.AddInputParameter("aggrType", parameters.AggregatorType);
                if (parameters.SiteId.HasValue) command.AddInputParameter("siteid", parameters.SiteId);
                if (parameters.RegionId.HasValue) command.AddInputParameter("regionid", parameters.RegionId);
                if (parameters.SystemId.HasValue) command.AddInputParameter("systemid", parameters.SystemId);
            }
        }

	    protected override List<AggregatorDTO> GetResult(OracleDataReader reader, GetAggregatorListParameterSet parameters)
		{
			var result = new List<AggregatorDTO>();
            while (reader.Read())
            {
                result.Add(new AggregatorDTO
                {
                    Id = reader.GetValue<Guid>("aggr_id"),
                    Name = reader.GetValue<string>("aggr_name"),
                    Description = reader.GetValue<string>("description"),
                    AggregatorType = reader.GetValue<AggregatorType>("aggr_type_id"),
                    AggregatorTypeName = reader.GetValue<string>("aggr_type_name"),
                    BalanceGroupId = reader.GetValue<int?>("bal_group_id")
                });
            }
		    return result;
		}
	}
}