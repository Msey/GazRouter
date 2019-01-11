using System;
using System.Collections.Generic;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.OperConsumerType;
using GazRouter.DTO.ObjectModel.OperConsumers;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.OperConsumers
{
    public class GetOperConsumerListQuery : QueryReader<GetOperConsumerListParameterSet, List<OperConsumerDTO>>
	{
		public GetOperConsumerListQuery(ExecutionContext context) : base(context)
		{
		}

        protected override string GetCommandText(GetOperConsumerListParameterSet parameters)
        {
            
            var sql = @"    SELECT      oc.oper_consumer_id,
                                        oc.oper_consumer_name,
                                        oc.oper_consumer_type_id,
                                        oct.oper_consumer_type_name, 
                                        oc.site_id,
                                        s.site_name,
                                        oc.sort_order,
                                        s.system_id,
                                        oc.is_direct_connection,
                                        oc.region_id,
                                        rg.region_name,
                                        oc.distr_station_id,
                                        ds.distr_station_name,
                                        bgo.group_id AS own_bal_group_id,
                                        bgo.name AS own_bal_group_name,
                                        bg.group_id AS bal_group_id,
                                        bg.name AS bal_group_name                                        

                            FROM        v_oper_consumers oc 
                            JOIN        v_sites s ON oc.site_id = s.site_id
                            LEFT JOIN   v_bl_groups bgo ON bgo.group_id = oc.bal_group_id                                            
                            LEFT JOIN   v_bl_groups bg ON bg.group_id = NVL(oc.bal_group_id, s.bal_group_id)
                            JOIN        v_oper_consumer_types oct ON oc.oper_consumer_type_id = oct.oper_consumer_type_id
                            JOIN        v_regions rg ON oc.region_id = rg.region_id
                            LEFT JOIN   v_distr_stations ds ON oc.distr_station_id = ds.distr_station_id
                            WHERE       1=1";
            
            var sb = new StringBuilder(sql);
            if (parameters != null)
            {
                if (parameters.SiteId.HasValue)
                    sb.Append(" AND oc.site_id = :site_id");

                if (parameters.SystemId.HasValue)
                    sb.Append(" AND s.system_id = :system_id");
            }

            sb.Append(" ORDER BY oc.sort_order, oc.oper_consumer_name");

            return sb.ToString();
        }

        protected override void BindParameters(OracleCommand command, GetOperConsumerListParameterSet parameters)
        {
            if (parameters != null)
            {
                if (parameters.SiteId.HasValue)
                    command.AddInputParameter("site_id", parameters.SiteId);

                if (parameters.SystemId.HasValue)
                    command.AddInputParameter("system_id", parameters.SystemId);
            }
                
        }

        protected override List<OperConsumerDTO> GetResult(OracleDataReader reader, GetOperConsumerListParameterSet parameters)
		{
			var result = new List<OperConsumerDTO>();
			while (reader.Read())
			{
				result.Add(
                    new OperConsumerDTO
				    {
                        Id = reader.GetValue<Guid>("oper_consumer_id"),
                        Name = reader.GetValue<string>("oper_consumer_name"),
                        ParentId = reader.GetValue<Guid>("site_id"),
                        SystemId = reader.GetValue<int>("system_id"),
                        IsDirectConnection = reader.GetValue<bool>("is_direct_connection"),
					    RegionId = reader.GetValue<int>("region_id"),
					    RegionName = reader.GetValue<string>("region_name"),
                        OperConsumerTypeId = reader.GetValue<OperConsumerType>("oper_consumer_type_id"),
                        OperConsumerTypeName = reader.GetValue<string>("oper_consumer_type_name"),
                        DistrStationId = reader.GetValue<Guid?>("distr_station_id"),
                        DistrStationName = reader.GetValue<string>("distr_station_name"),
                        BalanceGroupId = reader.GetValue<int?>("bal_group_id"),
                        BalanceGroupName = reader.GetValue<string>("bal_group_name"),
                        OwnBalanceGroupId = reader.GetValue<int?>("own_bal_group_id"),
                        OwnBalanceGroupName = reader.GetValue<string>("own_bal_group_name"),
                        SortOrder = reader.GetValue<int>("sort_order")
                    });

			}
			return result;
		}
	}
}