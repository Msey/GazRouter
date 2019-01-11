using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.DistrStations;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.DistrStations
{
    public class GetDistrStationByIdQuery : QueryReader<Guid, DistrStationDTO>
	{
        public GetDistrStationByIdQuery(ExecutionContext context)
            : base(context)
		{
		}

        protected override string GetCommandText(Guid parameters)
		{
            return @"   SELECT      ds.distr_station_id, 
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
                        WHERE       ds.distr_station_id = :id";
		}

        protected override void BindParameters(OracleCommand command, Guid parameters)
        {
            command.AddInputParameter("id", parameters);
        }

        protected override DistrStationDTO GetResult(OracleDataReader reader, Guid parameters)
		{
            DistrStationDTO result = null;
			while (reader.Read())
			{
				result = new DistrStationDTO
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
                };

			}
			return result;
		}
	}
}