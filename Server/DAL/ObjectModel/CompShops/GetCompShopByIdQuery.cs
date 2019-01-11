using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.EngineClasses;
using GazRouter.DTO.ObjectModel.CompShops;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.CompShops
{
	public class GetCompShopByIdQuery : QueryReader<Guid, CompShopDTO>
	{
        public GetCompShopByIdQuery(ExecutionContext context)
            : base(context)
		{
		}

        protected override string GetCommandText(Guid parameters)
		{
            return
@"
SELECT
    t1.comp_shop_id, 
    t1.comp_shop_name, 
    t1.comp_station_id,
    st.comp_station_name, 
    t1.pipeline_id, 
    t1.pipeline_name,
    t1.kilometer_conn,
    t1.engine_class_id, 
    t1.is_virtual, 
    t1.piping_volume, 
    t1.piping_volume_in, 
    t1.piping_volume_out,
    n.entity_name AS full_name, 
    n1.entity_name AS short_name,
    t1.system_id,
    t1.description

FROM
    rd.v_comp_shops t1 
    INNER JOIN  rd.v_comp_stations st   ON st.comp_station_id = t1.comp_station_id
    LEFT JOIN   v_nm_all n              ON t1.comp_shop_id = n.entity_id
    LEFT JOIN   v_nm_short_all n1       ON t1.comp_shop_id = n1.entity_id

WHERE
    t1.comp_shop_id = :id";
		}

        protected override void BindParameters(OracleCommand command, Guid parameters)
        {
            command.AddInputParameter("id", parameters);
        }

        protected override CompShopDTO GetResult(OracleDataReader reader, Guid parameters)
		{
            CompShopDTO result = null;
            while (reader.Read())
            {
                result = new CompShopDTO
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
                    Description = reader.GetValue<string>("description")
                };
            }
            return result;
		}
	}
}