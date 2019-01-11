using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.EngineClasses;
using GazRouter.DTO.ObjectModel.CompShops;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.CompShops
{
    public class GetCompShopListQuery : QueryReader<GetCompShopListParameterSet, List<CompShopDTO>>
	{
		public GetCompShopListQuery(ExecutionContext context) : base(context)
		{
		}

        protected override string GetCommandText(GetCompShopListParameterSet parameters)
		{

            var sb = new StringBuilder(
                               @"
SELECT
    t1.comp_shop_id, 
    t1.comp_shop_name,
    t1.comp_station_id,
    st.comp_station_name, 
    t1.pipeline_id,  
    t1.piping_volume,
    t1.piping_volume_in, 
    t1.piping_volume_out, 
    t1.kilometer_conn, 
    t1.engine_class_id,
    t1.is_virtual, 
    n.entity_name AS full_name, 
    n1.entity_name AS short_name,
    t1.system_id,
    t1.sort_order,
    t1.pipeline_name,
    t1.description

FROM
    rd.v_comp_shops t1
    INNER JOIN  rd.v_comp_stations st   ON st.comp_station_id = t1.comp_station_id
    LEFT  JOIN  v_nm_all n              ON t1.comp_shop_id = n.entity_id
    LEFT  JOIN  v_nm_short_all n1       ON t1.comp_shop_id = n1.entity_id
    
WHERE
    1=1");
            if (parameters != null)
            {
                if (parameters.SystemId.HasValue)
                    sb.Append(" AND t1.system_id  = :systemid");

                if (parameters.StationIdList != null && parameters.StationIdList.Any())
                {
                    sb.Append(" AND t1.comp_station_id IN ");
                    sb.Append(CreateInClause(parameters.StationIdList.Count));
                }

                if (parameters.PipelineId.HasValue)
                    sb.Append(" AND t1.pipeline_id  = :pipeid");

                if (parameters.SiteId.HasValue)
                    sb.Append(" AND st.site_id  = :siteid");

            }

            sb.Append(@" ORDER BY t1.sort_order, t1.comp_shop_name");
            
            return sb.ToString();
		}

        protected override void BindParameters(OracleCommand command, GetCompShopListParameterSet parameters)
        {
            if (parameters == null) return;

            if (parameters.SystemId.HasValue)
                command.AddInputParameter("systemid", parameters.SystemId);

            if (parameters.PipelineId.HasValue)
                command.AddInputParameter("pipeid", parameters.PipelineId);

            if (parameters.SiteId.HasValue)
                command.AddInputParameter("siteid", parameters.SiteId);

            if (parameters.StationIdList != null && parameters.StationIdList.Any())
            {
                for (var i = 0; i < parameters.StationIdList.Count; i++)
                {
                    command.AddInputParameter(string.Format("p{0}", i), parameters.StationIdList[i]);
                }
            }
        }

        protected override List<CompShopDTO> GetResult(OracleDataReader reader, GetCompShopListParameterSet parameters)
		{
            var result = new List<CompShopDTO>();
            while (reader.Read())
            {
                result.Add(new CompShopDTO
                {
                    Id = reader.GetValue<Guid>("comp_shop_id"),
                    Name = reader.GetValue<string>("comp_shop_name"),
                    ParentId = reader.GetValue<Guid>("comp_station_id"),
                    StationName = reader.GetValue<string>("comp_station_name"),
                    PipelineId = reader.GetValue<Guid>("pipeline_id"),
                    PipelineName = reader.GetValue<string>("pipeline_name"),
                    KmOfConn = reader.GetValue<double>("kilometer_conn"),
                    PipingVolume = reader.GetValue<double?>("piping_volume"),
                    PipingVolumeIn = reader.GetValue<double?>("piping_volume_in"),
                    PipingVolumeOut = reader.GetValue<double?>("piping_volume_out"),
                    EngineClass = reader.GetValue<EngineClass>("engine_class_id"),
                    IsVirtual = reader.GetValue<bool>("is_virtual"),
                    Path = reader.GetValue<string>("full_name"),
                    ShortPath = reader.GetValue<string>("short_name"),
                    SystemId = reader.GetValue<int>("system_id"),
                    SortOrder = reader.GetValue<int>("sort_order"),
                    Description = reader.GetValue<string>("description")
                });
            }
            return result;
		}
	}
}