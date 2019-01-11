using System;
using System.Collections.Generic;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.DistrStations;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.DistrStations
{
    public class GetDistrStationListQuery : QueryReader<GetDistrStationListParameterSet, List<DistrStationDTO>>
	{
		public GetDistrStationListQuery(ExecutionContext context) : base(context)
		{
		}

        protected override string GetCommandText(GetDistrStationListParameterSet parameters)
        {
            var sb =
                new StringBuilder(
                    @"  SELECT      ds.distr_station_id, 
                                    ds.distr_station_name, 
                                    ds.region_id,
                                    ds.site_id, 
                                    ds.pressure_rated, 
                                    ds.capacity_rated,
                                    ds.use_in_balance, 
                                    ds.is_virtual,
                                    ds.is_foreign, 
                                    n.entity_name full_name, 
                                    n1.entity_name short_name, 
                                    ds.system_id,
                                    ds.sort_order,
                                    ds.description,
                                    bgo.group_id AS own_bal_group_id,
                                    bgo.name AS own_bal_group_name,
                                    bg.group_id AS bal_group_id,
                                    bg.name AS bal_group_name
                                    
                        FROM        rd.v_distr_stations ds
                        INNER JOIN  v_sites s ON s.site_id = ds.site_id
                        LEFT JOIN   v_bl_groups bgo ON bgo.group_id = ds.bal_group_id                                            
                        LEFT JOIN   v_bl_groups bg ON bg.group_id = NVL(ds.bal_group_id, s.bal_group_id)
                        LEFT JOIN   v_nm_all n ON ds.distr_station_id = n.entity_id
                        LEFT JOIN   v_nm_short_all n1 ON ds.distr_station_id = n1.entity_id 
                        WHERE       1=1");

            if (parameters != null)
            {
                if (parameters.StationId.HasValue) sb.Append(" AND ds.distr_station_id  = :id");
                if (parameters.SystemId.HasValue) sb.Append(" AND ds.system_id  = :systemId");
                if (parameters.SiteId.HasValue) sb.Append(" AND ds.site_id  = :siteId");
                if (parameters.EnterpriseId.HasValue) sb.Append(" AND s.enterprise_id  = :entId");

                if (parameters.HideVirtual) sb.Append(" AND ds.is_virtual = 0");

                if (parameters.UseInBalance.HasValue) sb.Append(" AND ds.use_in_balance = :uib");
            }

            sb.Append(@" ORDER BY ds.sort_order, ds.distr_station_name");
            return sb.ToString();
        }

        protected override void BindParameters(OracleCommand command, GetDistrStationListParameterSet parameters)
        {
            if (parameters == null)
                return;

            if (parameters.StationId.HasValue)
                command.AddInputParameter("id", parameters.StationId);

            if (parameters.SystemId.HasValue)
                command.AddInputParameter("systemId", parameters.SystemId);

            if (parameters.SiteId.HasValue)
                command.AddInputParameter("siteId", parameters.SiteId);

            if (parameters.UseInBalance.HasValue)
                command.AddInputParameter("uib", parameters.UseInBalance);

            if (parameters.EnterpriseId.HasValue)
                command.AddInputParameter("entId", parameters.EnterpriseId);
        }

        protected override List<DistrStationDTO> GetResult(OracleDataReader reader, GetDistrStationListParameterSet parameters)
		{
			var result = new List<DistrStationDTO>();
			while (reader.Read())
			{
				result.Add(new DistrStationDTO
				{
					Id = reader.GetValue<Guid>("distr_station_id"),
					Name = reader.GetValue<string>("distr_station_name"),
                    ParentId = reader.GetValue<Guid>("site_id"),
                    RegionId = reader.GetValue<int>("region_id"),
                    PressureRated = reader.GetValue<double>("pressure_rated"),
                    CapacityRated = reader.GetValue<double>("capacity_rated"),
                    UseInBalance = reader.GetValue<bool>("use_in_balance"),
                    IsVirtual = reader.GetValue<bool>("is_virtual"),
                    IsForeign = reader.GetValue<bool>("is_foreign"),
                    Path = reader.GetValue<string>("full_name"),
                    ShortPath = reader.GetValue<string>("short_name"),
					SystemId = reader.GetValue<int>("system_id"),
					SortOrder = reader.GetValue<int>("sort_order"),
                    Description = reader.GetValue<string>("description"),
                    BalanceGroupId = reader.GetValue<int?>("bal_group_id"),
                    BalanceGroupName = reader.GetValue<string>("bal_group_name"),
                    OwnBalanceGroupId = reader.GetValue<int?>("own_bal_group_id"),
                    OwnBalanceGroupName = reader.GetValue<string>("own_bal_group_name")
                });

			}
			return result;
		}
	}
}