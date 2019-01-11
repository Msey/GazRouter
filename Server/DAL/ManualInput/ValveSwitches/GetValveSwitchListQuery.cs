using System;
using System.Collections.Generic;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.PipelineTypes;
using GazRouter.DTO.Dictionaries.StatesModel;
using GazRouter.DTO.ManualInput.ValveSwitches;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ManualInput.ValveSwitches
{
    public class GetValveSwitchListQuery : QueryReader<GetValveSwitchListParameterSet, List<ValveSwitchDTO>>
    {
        public GetValveSwitchListQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override string GetCommandText(GetValveSwitchListParameterSet parameters)
        {
            var q =
                @"  SELECT      sw.switching_date,
                                sw.switching_type,
                                sw.state,
                                sw.entity_id,
                                sw.open_percent,
                                sw.change_user_name,
                                sw.change_user_site_name,
                                v.valve_name,
                                v.valve_type_id,
                                v.bypass1_type_id,
                                v.bypass2_type_id,
                                v.bypass3_type_id,
                                v.kilometer,
                                vp.valve_purpose_name,
                                cst.comp_station_id,
                                cst.comp_station_name,
                                csh.comp_shop_id,
                                csh.comp_shop_name,
                                site.site_id,
                                site.site_name,
                                p.pipeline_id,
                                p.pipeline_name,
                                p.pipeline_type_id                                    
                        
                    FROM        rd.V_VALVE_SWITCHING sw
                    JOIN        rd.v_valves v ON v.valve_id=sw.entity_id
                    JOIN        rd.v_valve_purposes vp ON vp.valve_purpose_id=v.valve_purpose_id
                    LEFT JOIN   rd.v_comp_shops csh ON csh.comp_shop_id=v.comp_shop_id
                    LEFT JOIN   rd.v_comp_stations cst ON cst.comp_station_id=csh.comp_station_id
                    LEFT JOIN   rd.v_sites site ON site.site_id = rd.P_ENTITY.GetSiteID(sw.entity_id,v.kilometer)
                    LEFT JOIN   rd.v_pipelines p on p.pipeline_id=v.pipeline_id
                        
                    WHERE       sw.switching_date BETWEEN :startdate AND :enddate";

            var sb = new StringBuilder(q);

            if (parameters.ValveId.HasValue) sb.Append(" AND sw.entity_id = :valveId");
            if (parameters.SiteId.HasValue) sb.Append(" AND site.site_id = :siteId");
            
            sb.Append(" ORDER BY sw.switching_date DESC");

            return sb.ToString();
        }

        protected override void BindParameters(OracleCommand command, GetValveSwitchListParameterSet parameters)
        {
            command.AddInputParameter("startDate", parameters.BeginDate);
            command.AddInputParameter("endDate", parameters.EndDate);
            if(parameters.ValveId.HasValue)
                command.AddInputParameter("valveId", parameters.ValveId);
            if (parameters.SiteId.HasValue)
                command.AddInputParameter("siteId", parameters.SiteId);
        }

        protected override List<ValveSwitchDTO> GetResult(OracleDataReader reader, GetValveSwitchListParameterSet parameters)
        {
            var result = new List<ValveSwitchDTO>();
            while (reader.Read())
            {
                result.Add(new ValveSwitchDTO
                {
                    Id = reader.GetValue<Guid>("entity_id"),
                    SwitchingDate = reader.GetValue<DateTime>("switching_date"),
                    SwitchType = reader.GetValue<ValveSwitchType>("switching_type"),
                    State = reader.GetValue<ValveState>("state"),
                    OpenPercent = reader.GetValue<int?>("open_percent"),
                    ValveName = reader.GetValue<string>("valve_name"),
                    ValveTypeId = reader.GetValue<int>("valve_type_id"),
                    Bypass1TypeId = reader.GetValue<int>("bypass1_type_id"),
                    Bypass2TypeId = reader.GetValue<int>("bypass2_type_id"),
                    Bypass3TypeId = reader.GetValue<int>("bypass3_type_id"),
                    PipelineType = reader.GetValue<PipelineType>("pipeline_type_id"),
                    Kilometr = reader.GetValue<double>("kilometer"),
                    ValvePurposeName = reader.GetValue<string>("valve_purpose_name"),
                    CompStationId = reader.GetValue<Guid>("comp_station_id"),
                    CompStationName = reader.GetValue<string>("comp_station_name"),
                    SiteId = reader.GetValue<Guid>("site_id"),
                    SiteName = reader.GetValue<string>("site_name"),
                    CompShopId = reader.GetValue<Guid?>("comp_shop_id"),
                    CompShopName = reader.GetValue<string>("comp_shop_name"),
                    PipelineId = reader.GetValue<Guid>("pipeline_id"),
                    PipelineName = reader.GetValue<string>("pipeline_name"),
                    ChangeUserName = reader.GetValue<string>("change_user_name"),
                    ChangeUserSite = reader.GetValue<string>("change_user_site_name")
                });
            }
            return result;
        }
    }
}
