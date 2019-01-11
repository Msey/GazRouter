using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.MeasPoint;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.MeasPoint
{

    public class GetMeasPointByIdQuery : QueryReader<Guid, MeasPointDTO>
    {
        public GetMeasPointByIdQuery(ExecutionContext context)
            : base(context)
        {}

        protected override string GetCommandText(Guid parameters)
		{
            return @"   SELECT      mp.meas_point_id, 
                                    mp.meas_point_name, 
                                    mp.meas_line_id, 
                                    mp.comp_shop_id,
                                    mp.distr_station_id, 
                                    mp.meas_line_name, 
                                    mp.comp_shop_name, 
                                    mp.distr_station_name, 
                                    mp.chromatogr_consumption_rate,
                                    mp.chromatogr_test_time,
                                    n.entity_name full_name, 
                                    n1.entity_name short_name,
                                    mp.system_id,
                                    mp.description,
                                    mp.site_id
                        FROM        rd.v_meas_points mp
                        LEFT JOIN   v_nm_all n ON mp.meas_point_id = n.entity_id
                        LEFT JOIN   v_nm_short_all n1 ON mp.meas_point_id = n1.entity_id                         
                        WHERE       mp.meas_point_id = :id";
		}

        protected override void BindParameters(OracleCommand command, Guid parameters)
        {
            command.AddInputParameter("id", parameters);
        }

        protected override MeasPointDTO GetResult(OracleDataReader reader, Guid parameters)
        {
            var newpoint = new MeasPointDTO();
            while (reader.Read())
            {
                newpoint = new MeasPointDTO
                {
					Id = reader.GetValue<Guid>("meas_point_id"),
					Name = reader.GetValue<string>("meas_point_name"),
					CompShopId = reader.GetValue<Guid?>("comp_shop_id"),
					CompShopName = reader.GetValue<string>("comp_shop_name"),
                    DistrStationId = reader.GetValue<Guid?>("distr_station_id"),
					DistrStationName = reader.GetValue<string>("distr_station_name"),
					MeasLineId = reader.GetValue<Guid?>("meas_line_id"),
					MeasLineName = reader.GetValue<string>("meas_line_name"),
                    ChromatographConsumptionRate = reader.GetValue<double>("chromatogr_consumption_rate"),
                    ChromatographTestTime = reader.GetValue<int>("chromatogr_test_time"),
                    Path = reader.GetValue<string>("full_name"),
                    ShortPath = reader.GetValue<string>("short_name"),
					SystemId = reader.GetValue<int>("system_id"),
                    Description = reader.GetValue<string>("description"),
                    SiteId = reader.GetValue<Guid>("site_id")
                };
				if (newpoint.CompShopId.HasValue) newpoint.ParentId = newpoint.CompShopId.Value;
				else if (newpoint.DistrStationId.HasValue) newpoint.ParentId = newpoint.DistrStationId.Value;
				else if (newpoint.MeasLineId.HasValue) newpoint.ParentId = newpoint.MeasLineId.Value;
            }

            return newpoint;
        }
    }
}
