using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.CompUnitSealingTypes;
using GazRouter.DTO.Dictionaries.EngineClasses;
using GazRouter.DTO.ObjectModel.CompUnits;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.CompUnits
{
	public class GetCompUnitByIdQuery : QueryReader<Guid, CompUnitDTO>
	{
        public GetCompUnitByIdQuery(ExecutionContext context)
            : base(context)
		{
		}

        protected override string GetCommandText(Guid parameters)
		{
            return @"   SELECT      t1.comp_unit_id, 
                                    t1.comp_unit_num, 
                                    t1.comp_unit_name, 
                                    t1.is_virtual,
                                    t1.comp_unit_type_id, 
                                    t1.comp_shop_id, 
                                    t1.supercharger_type_id, 
                                    n.entity_name AS full_name, 
                                    n1.entity_name AS short_name,
                                    t1.injection_profile_volume, 
                                    t1.turbine_starter_consumption, 
                                    t1.dry_motoring_consumption,
                                    t1.bleeding_rate,
                                    t1.comp_unit_sealing_type_id,
                                    t1.comp_unit_sealing_count,
                                    t1.valve_consumption_details,
                                    t1.injection_profile_piping,
                                    t1.start_valve_consumption, 
                                    t1.stop_valve_consumption, 
                                    t1.system_id,
                                    t1.description,
                                    s.comp_station_id,
                                    s.engine_class_id
                        FROM        rd.v_comp_units t1 
                        INNER JOIN  v_comp_shops s ON t1.comp_shop_id = s.comp_shop_id
                        LEFT JOIN   v_nm_all n ON t1.comp_unit_id = n.entity_id
                        LEFT JOIN   v_nm_short_all n1 ON t1.comp_unit_id = n1.entity_id
                        WHERE       t1.comp_unit_id = :id";
		}

        protected override void BindParameters(OracleCommand command, Guid parameters)
        {
            command.AddInputParameter("id", parameters);
        }

        protected override CompUnitDTO GetResult(OracleDataReader reader, Guid parameters)
		{
            CompUnitDTO result = null;
            if (reader.Read())
            {
                result = new CompUnitDTO
                {
                    Id = reader.GetValue<Guid>("comp_unit_id"),
                    Name = reader.GetValue<string>("comp_unit_name"),
                    ParentId = reader.GetValue<Guid>("comp_shop_id"),
                    CompStationId = reader.GetValue<Guid>("comp_station_id"),
                    EngineClass = reader.GetValue<EngineClass>("engine_class_id"),
                    CompUnitTypeId = reader.GetValue<int>("comp_unit_type_id"),
                    SuperchargerTypeId = reader.GetValue<int>("supercharger_type_id"),
                    IsVirtual = reader.GetValue<bool>("is_virtual"),
                    Path = reader.GetValue<string>("full_name"),
                    ShortPath = reader.GetValue<string>("short_name"),
                    InjectionProfileVolume = reader.GetValue<double>("injection_profile_volume"),
                    TurbineStarterConsumption = reader.GetValue<double>("turbine_starter_consumption"),
                    DryMotoringConsumption = reader.GetValue<double>("dry_motoring_consumption"),
                    BleedingRate = reader.GetValue<double>("bleeding_rate"),
                    SealingType = reader.GetValue<CompUnitSealingType?>("comp_unit_sealing_type_id"),
                    SealingCount = reader.GetValue<int>("comp_unit_sealing_count"),
                    ValveConsumptionDetails = reader.GetValue<string>("valve_consumption_details"),
                    InjectionProfilePiping = reader.GetValue<string>("injection_profile_piping"),
                    StartValveConsumption = reader.GetValue<double>("start_valve_consumption"),
                    StopValveConsumption = reader.GetValue<double>("stop_valve_consumption"),
                    SystemId = reader.GetValue<int>("system_id"),
                    Description = reader.GetValue<string>("description"),
                    CompUnitNum = reader.GetValue<int>("comp_unit_num"),
                };
			}
		    return result;
		}
	}
}