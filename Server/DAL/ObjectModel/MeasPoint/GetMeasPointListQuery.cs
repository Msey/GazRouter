using System;
using System.Collections.Generic;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.MeasPoint;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.MeasPoint
{

	public class GetMeasPointListQuery : QueryReader<GetMeasPointListParameterSet, List<MeasPointDTO>>
    {
        public GetMeasPointListQuery(ExecutionContext context)
            : base(context)
        {}

        protected override string GetCommandText(GetMeasPointListParameterSet parameters)
		{
            var q = @"  SELECT      mp.meas_point_id, 
                                    mp.meas_point_name, 
                                    mp.meas_line_id, 
                                    mp.comp_shop_id,
                                    mp.distr_station_id, 
                                    mp.meas_line_name, 
                                    mp.comp_shop_name, 
                                    mp.distr_station_name, 
                                    n.entity_name full_name, 
                                    n1.entity_name short_name,
                                    mp.chromatogr_consumption_rate,
                                    mp.chromatogr_test_time,
                                    mp.system_id,
                                    mp.sort_order,
                                    mp.description
                        FROM        rd.v_meas_points mp
                        LEFT JOIN   v_nm_all n on mp.meas_point_id = n.entity_id
                        LEFT JOIN   v_nm_short_all n1 on mp.meas_point_id = n1.entity_id
                        WHERE       1=1";

            var sb = new StringBuilder(q);

            if (parameters != null)
            {
                if (parameters.SystemId.HasValue)
                    sb.Append(" AND mp.system_id = :systemid");

                if (parameters.Id.HasValue)
                    sb.Append(" AND mp.meas_point_id = :id");

                if (parameters.ParentId.HasValue)
                    sb.Append(@" AND (mp.meas_line_id  = :parentid
                                        OR mp.comp_shop_id = :parentid
                                        OR mp.distr_station_id = :parentid)");
            }
            
            sb.Append(" ORDER BY mp.sort_order, mp.meas_point_name");

            return sb.ToString();
		}

        protected override void BindParameters(OracleCommand command, GetMeasPointListParameterSet parameters)
        {
            if (parameters != null)
            {
                if (parameters.SystemId.HasValue)
                    command.AddInputParameter("systemid", parameters.SystemId);

                if (parameters.Id.HasValue)
                    command.AddInputParameter("id", parameters.Id);

                if (parameters.ParentId.HasValue)
                    command.AddInputParameter("parentid", parameters.ParentId);
            }
            
        }

        protected override List<MeasPointDTO> GetResult(OracleDataReader reader, GetMeasPointListParameterSet parameters)
        {
            var measPoints = new List<MeasPointDTO>();
            while (reader.Read())
            {
                var newpoint = new MeasPointDTO
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
									  SortOrder = reader.GetValue<int>("sort_order"),
                                      Description = reader.GetValue<string>("description")
                                  };
				if (newpoint.CompShopId.HasValue) newpoint.ParentId = newpoint.CompShopId.Value;
				else if (newpoint.DistrStationId.HasValue) newpoint.ParentId = newpoint.DistrStationId.Value;
				else if (newpoint.MeasLineId.HasValue) newpoint.ParentId = newpoint.MeasLineId.Value;
	            measPoints.Add(newpoint);
            }

            return measPoints;
        }
    }
}
