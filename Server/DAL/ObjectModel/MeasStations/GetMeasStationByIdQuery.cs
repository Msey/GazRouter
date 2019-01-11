using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.BalanceSigns;
using GazRouter.DTO.ObjectModel.MeasStations;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.MeasStations
{

	public class GetMeasStationByIdQuery : QueryReader<Guid, MeasStationDTO>
	{
        public GetMeasStationByIdQuery(ExecutionContext context)
			: base(context)
		{
		}

        protected override string GetCommandText(Guid parameters)
		{
            return @"   SELECT      ms.meas_station_id, 
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
                        WHERE       ms.meas_station_id = :id";
		}

        protected override void BindParameters(OracleCommand command, Guid parameters)
        {
            command.AddInputParameter("id", parameters);
        }

        protected override MeasStationDTO GetResult(OracleDataReader reader, Guid parameters)
		{
            MeasStationDTO result = null;
			while (reader.Read())
			{
				result = new MeasStationDTO
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
                };

			}
			return result;
		}
	}
}