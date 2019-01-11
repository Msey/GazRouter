using System;
using System.Collections.Generic;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.BalanceSigns;
using GazRouter.DTO.ObjectModel.MeasStations;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.MeasStations
{

    public class GetMeasStationListQuery : QueryReader<GetMeasStationListParameterSet, List<MeasStationDTO>>
	{
		public GetMeasStationListQuery(ExecutionContext context)
			: base(context)
		{
		}

        protected override string GetCommandText(GetMeasStationListParameterSet parameters)
        {
            var sb = new StringBuilder(@"   SELECT      ms.meas_station_id, 
                                                        ms.meas_station_name, 
                                                        ms.site_id, 
                                                        ms.neighbour_enterprise_id, 
                                                        ms.is_virtual, 
                                                        ms.balance_sign_id, 
                                                        ms.balance_sign_name, 
                                                        n.entity_name full_name, 
                                                        n1.entity_name short_name,
                                                        ms.system_id, 
                                                        ms.sort_order,
                                                        ms.description,
                                                        ms.bal_name,
                                                        ms.is_intermediate,
                                                        bgo.group_id AS own_bal_group_id,
                                                        bgo.name AS own_bal_group_name,
                                                        bg.group_id AS bal_group_id,
                                                        bg.name AS bal_group_name                                                        
                                                        
                                            FROM        rd.v_meas_stations ms
                                            INNER JOIN  v_sites s ON s.site_id = ms.site_id
                                            LEFT JOIN   v_bl_groups bgo ON bgo.group_id = ms.bal_group_id                                            
                                            LEFT JOIN   v_bl_groups bg ON bg.group_id = NVL(ms.bal_group_id, s.bal_group_id)                                            
                                            LEFT JOIN   v_nm_all n ON ms.meas_station_id = n.entity_id
                                            LEFT JOIN   v_nm_short_all n1 ON ms.meas_station_id = n1.entity_id
                                            WHERE       1=1");

            if (parameters != null)
            {
                if (parameters.SystemId.HasValue) sb.Append(" AND ms.system_id  = :systemId");
                if (parameters.SiteId.HasValue) sb.Append(" AND ms.site_id  = :siteId");
                if (parameters.HideVirtual) sb.Append(" AND ms.is_virtual = 0");
                if (parameters.EnterpriseId.HasValue) sb.Append(" AND s.enterprise_id = :entId");
            }

            sb.Append(" ORDER BY ms.sort_order, ms.meas_station_name");

            return sb.ToString();
        }

        protected override void BindParameters(OracleCommand command, GetMeasStationListParameterSet parameters)
        {
            if (parameters == null) return;

            if (parameters.SystemId.HasValue)
                command.AddInputParameter("systemId", parameters.SystemId);

            if (parameters.SiteId.HasValue )
                command.AddInputParameter("siteId", parameters.SiteId);

            if (parameters.EnterpriseId.HasValue)
                command.AddInputParameter("entId", parameters.EnterpriseId);
        }

        protected override List<MeasStationDTO> GetResult(OracleDataReader reader, GetMeasStationListParameterSet parameters)
		{
			var result = new List<MeasStationDTO>();
			while (reader.Read())
			{
				result.Add(new MeasStationDTO
				{
                    Id = reader.GetValue<Guid>("meas_station_id"),
                    Name = reader.GetValue<string>("meas_station_name"),
                    ParentId = reader.GetValue<Guid>("site_id"),
                    NeighbourEnterpriseId = reader.GetValue<Guid?>("neighbour_enterprise_id"),
					IsVirtual = reader.GetValue<bool>("is_virtual"),
					BalanceSignId = reader.GetValue<Sign>("balance_sign_id"),
                    BalanceSignName = reader.GetValue<string>("balance_sign_name"),
                    IsIntermediate = reader.GetValue<bool>("is_intermediate"),
                    Path = reader.GetValue<string>("full_name"),
                    ShortPath = reader.GetValue<string>("short_name"),
					SystemId = reader.GetValue<int>("system_id"),
					SortOrder = reader.GetValue<int>("sort_order"),
                    Description = reader.GetValue<string>("description"),
                    BalanceName = reader.GetValue<string>("bal_name"),
                    BalanceGroupId = reader.GetValue<int?>("bal_group_id"),
                    BalanceGroupName = reader.GetValue<string>("bal_group_name"),
                    OwnBalanceGroupId = reader.GetValue<int?>("own_bal_group_id"),
                    OwnBalanceGroupName = reader.GetValue<string>("own_bal_group_name"),
                    
				});

			}
			return result;
		}
	}
}