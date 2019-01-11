using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.ValvePurposes;
using GazRouter.DTO.ObjectModel.Valves;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.Valves
{

    public class GetValveByIdQuery : QueryReader<Guid, ValveDTO>
    {
        public GetValveByIdQuery(ExecutionContext context)
            : base(context)
        { }

        protected override string GetCommandText(Guid parameters)
        {
            return @"   SELECT      v.valve_id AS id, 
                                    v.entity_type_id, 
                                    v.status, 
                                    v.sort_order, 
                                    v.valve_name AS name, 
                                    v.is_virtual, 
                                    v.valve_type_id, 
                                    v.pipeline_id, 
                                    v.kilometer, 
                                    v.comp_shop_id, 
                                    v.valve_purpose_id,
                                    v.is_control_point, 
                                    n.entity_name AS full_name, 
                                    n1.entity_name AS short_name,
                                    v.system_id,
                                    sys.system_name,
                                    v.sort_order, 
                                    v.bypass1_type_id, 
                                    v.bypass2_type_id, 
                                    v.bypass3_type_id,
                                    stations.comp_station_name,
                                    v.description,
                                    pipes.pipeline_name                        
                        FROM        v_valves v
                        LEFT JOIN   v_nm_all n ON v.valve_id = n.entity_id
                        LEFT JOIN   v_nm_short_all n1 ON v.valve_id = n1.entity_id
                        LEFT JOIN   v_entity_2_site_segments e2s ON v.valve_id = e2s.entity_id AND v.kilometer BETWEEN e2s.kilometer_start AND e2s.kilometer_end
                        LEFT JOIN   v_comp_shops shops ON v.comp_shop_id = shops.comp_shop_id
                        LEFT JOIN   v_comp_stations stations ON shops.comp_station_id = stations.comp_station_id
                        LEFT JOIN   v_pipelines pipes ON pipes.pipeline_id = v.pipeline_id
                        LEFT JOIN   v_systems sys ON sys.system_id = v.system_id 
                        WHERE       v.valve_id = :id";
        }

        protected override void BindParameters(OracleCommand command, Guid parameters)
        {
            command.AddInputParameter("id", parameters);
        }

        protected override ValveDTO GetResult(OracleDataReader reader, Guid parameters)
	    {
            ValveDTO valve = null;
	        while (reader.Read())
	        {
                valve = new ValveDTO
	            {
                    Id = reader.GetValue<Guid>("id"),
                    Name = reader.GetValue<string>("name"),
                    IsVirtual = reader.GetValue<bool>("is_virtual"),
                    ValveTypeId = reader.GetValue<int>("valve_type_id"),
                    ParentId = reader.GetValue<Guid>("pipeline_id"),
                    PipelineName = reader.GetValue<string>("pipeline_name"),
                    Kilometer = reader.GetValue<double>("kilometer"),
                    Bypass1TypeId = reader.GetValue<int?>("bypass1_type_id"),
                    Bypass2TypeId = reader.GetValue<int?>("bypass2_type_id"),
                    Bypass3TypeId = reader.GetValue<int?>("bypass3_type_id"),
                    ValvePurposeId = reader.GetValue<ValvePurpose>("valve_purpose_id"),
                    CompShopId = reader.GetValue<Guid?>("comp_shop_id"),
                    Path = reader.GetValue<string>("full_name"),
                    ShortPath = reader.GetValue<string>("short_name"),
                    SystemId = reader.GetValue<int>("system_id"),
                    SystemName = reader.GetValue<string>("system_name"),
                    SortOrder = reader.GetValue<int>("Sort_order"),
                    CompStationName = reader.GetValue<string>("comp_station_name"),
                    Description = reader.GetValue<string>("description"),
                    IsControlPoint = reader.GetValue<bool>("is_control_point"),

                };
	        }
	        return valve;
	    }
    }
}
