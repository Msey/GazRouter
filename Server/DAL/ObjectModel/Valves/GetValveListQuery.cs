using System;
using System.Collections.Generic;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.ValvePurposes;
using GazRouter.DTO.ObjectModel.Valves;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.Valves
{

    public class GetValveListQuery : QueryReader<GetValveListParameterSet, List<ValveDTO>>
    {
        public GetValveListQuery(ExecutionContext context)
            : base(context)
        { }

        protected override string GetCommandText(GetValveListParameterSet parameters)
        {
            var q = @"  SELECT      v.valve_id AS id, 
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
                                    pipes.pipeline_name,
                                    rd.P_ENTITY.GetSiteID(v.valve_id) AS site_id                        
                        FROM        v_valves v
                        LEFT JOIN   v_nm_all n ON v.valve_id = n.entity_id
                        LEFT JOIN   v_nm_short_all n1 ON v.valve_id = n1.entity_id
                        LEFT JOIN   v_comp_shops shops ON v.comp_shop_id = shops.comp_shop_id
                        LEFT JOIN   v_comp_stations stations ON shops.comp_station_id = stations.comp_station_id
                        LEFT JOIN   v_pipelines pipes ON pipes.pipeline_id = v.pipeline_id
                        LEFT JOIN   v_systems sys ON sys.system_id = v.system_id
                        
                        WHERE       1=1";

            var sb = new StringBuilder(q);
            if (parameters != null)
            {
                if (parameters.ValveId.HasValue)
                    sb.Append(" AND v.valve_id = :valveid");

                if (parameters.PipelineId.HasValue)
                    sb.Append(" AND v.pipeline_id = :pipelineid");

                if (parameters.SystemId.HasValue)
                    sb.Append(" AND v.system_id = :systemid");

                if (parameters.SiteId.HasValue)
                    sb.Append(" AND rd.P_ENTITY.GetSiteID(v.valve_id) = :siteid");

                if (parameters.IsControlPoint.HasValue)
                    sb.Append(" AND v.is_control_point = :control");
            }
            sb.Append(" ORDER BY v.pipeline_id, v.kilometer");

            return sb.ToString();
        }

        protected override void BindParameters(OracleCommand command, GetValveListParameterSet parameters)
        {
            if (parameters == null) return;

            if (parameters.ValveId.HasValue)
                command.AddInputParameter("valveid", parameters.ValveId);

            if (parameters.PipelineId.HasValue)
                command.AddInputParameter("pipelineId", parameters.PipelineId);

            if (parameters.SystemId.HasValue)
                command.AddInputParameter("systemid", parameters.SystemId);

            if (parameters.SiteId.HasValue)
                command.AddInputParameter("siteid", parameters.SiteId);

            if (parameters.IsControlPoint.HasValue)
                command.AddInputParameter("control", parameters.IsControlPoint);
        }

        protected override List<ValveDTO> GetResult(OracleDataReader reader, GetValveListParameterSet parameters)
        {
            var valves = new List<ValveDTO>();
            while (reader.Read())
            {
                valves.Add(new ValveDTO
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
                    SiteId = reader.GetValue<Guid>("site_id"),
                    IsControlPoint = reader.GetValue<bool>("is_control_point"),
                });
            }
            return valves;
        }
    }
}
