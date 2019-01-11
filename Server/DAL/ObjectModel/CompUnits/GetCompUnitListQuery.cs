using System;
using System.Collections.Generic;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.CompUnitSealingTypes;
using GazRouter.DTO.Dictionaries.EngineClasses;
using GazRouter.DTO.ObjectModel.CompUnits;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.CompUnits
{
    public class GetCompUnitListQuery : QueryReader<GetCompUnitListParameterSet, List<CompUnitDTO>>
	{
		public GetCompUnitListQuery(ExecutionContext context) : base(context)
		{
		}

        protected override string GetCommandText(GetCompUnitListParameterSet parameters)
        {
            var q = @"  SELECT      u.comp_unit_id,
                                    u.comp_unit_num, 
                                    u.comp_unit_name, 
                                    u.is_virtual,
                                    u.comp_unit_type_id, 
                                    u.comp_shop_id, 
                                    u.supercharger_type_id, 
                                    n.entity_name AS full_name, 
                                    n1.entity_name AS short_name,
                                    u.has_recovery_boiler,
                                    u.injection_profile_volume, 
                                    u.turbine_starter_consumption, 
                                    u.dry_motoring_consumption,
                                    u.bleeding_rate,
                                    u.comp_unit_sealing_type_id,
                                    u.comp_unit_sealing_count,
                                    u.start_valve_consumption, 
                                    u.stop_valve_consumption, 
                                    u.valve_consumption_details,
                                    u.injection_profile_piping,
                                    u.system_id,
                                    u.sort_order,
                                    u.description,
                                    s.comp_station_id,
                                    s.engine_class_id,
                                    site.site_id

                        FROM        v_comp_units u 
                        INNER JOIN  v_comp_shops s ON u.comp_shop_id = s.comp_shop_id
                        LEFT JOIN   v_nm_all n ON u.comp_unit_id = n.entity_id
                        LEFT JOIN   v_nm_short_all n1 ON u.comp_unit_id = n1.entity_id
                        LEFT JOIN   v_entity_2_site site ON site.entity_id = u.comp_unit_id
                        WHERE       1=1";

            var sb = new StringBuilder(q);

            if (parameters != null)
            {
                if (parameters.SystemId.HasValue) sb.Append(" AND u.system_id  = :systemid");
                if (parameters.UnitId.HasValue) sb.Append(" AND u.comp_unit_id  = :unitid");
                if (parameters.ShopId.HasValue) sb.Append(" AND u.comp_shop_id  = :shopid");
                if (parameters.StationId.HasValue) sb.Append(" AND s.comp_station_id  = :stationid");
                if (parameters.SiteId.HasValue) sb.Append(" AND site.site_id  = :siteid");
            }

            sb.Append(" ORDER BY u.sort_order, u.comp_unit_name");

            return sb.ToString();
        }

        protected override void BindParameters(OracleCommand command, GetCompUnitListParameterSet parameters)
        {
            if (parameters == null) return;
            if (parameters.UnitId.HasValue) command.AddInputParameter("unitid", parameters.UnitId);
            if (parameters.SystemId.HasValue) command.AddInputParameter("systemid", parameters.SystemId);
            if (parameters.ShopId.HasValue) command.AddInputParameter("shopid", parameters.ShopId);
            if (parameters.StationId.HasValue) command.AddInputParameter("stationid", parameters.StationId);
            if (parameters.SiteId.HasValue) command.AddInputParameter("siteid", parameters.SiteId);
        }

        protected override List<CompUnitDTO> GetResult(OracleDataReader reader, GetCompUnitListParameterSet parameters)
		{
			var result = new List<CompUnitDTO>();
		    while (reader.Read())
			{
                result.Add(new CompUnitDTO
                {
                    Id = reader.GetValue<Guid>("comp_unit_id"),
                    Name = reader.GetValue<string>("comp_unit_name"),
                    CompUnitNum = reader.GetValue<int>("comp_unit_num"),
                    ParentId = reader.GetValue<Guid>("comp_shop_id"),
                    CompStationId = reader.GetValue<Guid>("comp_station_id"),
                    EngineClass = reader.GetValue<EngineClass>("engine_class_id"),
                    CompUnitTypeId = reader.GetValue<int>("comp_unit_type_id"),
                    SuperchargerTypeId = reader.GetValue<int>("supercharger_type_id"),
                    IsVirtual = reader.GetValue<bool>("is_virtual"),
                    HasRecoveryBoiler = reader.GetValue<bool>("has_recovery_boiler"),
                    Path = reader.GetValue<string>("full_name"),
                    ShortPath = reader.GetValue<string>("short_name"),
                    InjectionProfileVolume = reader.GetValue<double>("injection_profile_volume"),
                    TurbineStarterConsumption = reader.GetValue<double>("turbine_starter_consumption"),
                    DryMotoringConsumption = reader.GetValue<double>("dry_motoring_consumption"),
                    BleedingRate = reader.GetValue<double>("bleeding_rate"),
                    SealingType = reader.GetValue<CompUnitSealingType?>("comp_unit_sealing_type_id"),
                    SealingCount = reader.GetValue<int>("comp_unit_sealing_count"),
                    StartValveConsumption = reader.GetValue<double>("start_valve_consumption"),
                    StopValveConsumption = reader.GetValue<double>("stop_valve_consumption"),
                    ValveConsumptionDetails = reader.GetValue<string>("valve_consumption_details"),
                    InjectionProfilePiping = reader.GetValue<string>("injection_profile_piping"),
					SystemId = reader.GetValue<int>("system_id"),
					SortOrder = reader.GetValue<int>("sort_order"),
                    Description = reader.GetValue<string>("description")
                });
			}
		    return result;
		}
	}
}