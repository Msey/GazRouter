using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.DistrStationOutlets;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.DistrStationOutlets
{
    public class GetDistrStationOutletByIdQuery : QueryReader<Guid, DistrStationOutletDTO>
    {
        public GetDistrStationOutletByIdQuery(ExecutionContext context) : base(context)
        {
        }

        protected override string GetCommandText(Guid parameters)
        {
            return string.Format(@" SELECT      t1.distr_station_outlet_id, 
                                                t1.entity_type_id, 
                                                t1.distr_station_outlet_name, 
                                                t1.is_virtual, 
                                                t1.distr_station_id, 
                                                t1.capacity_rated, 
                                                t1.pressure_rated, 
                                                n.entity_name full_name, 
                                                n1.entity_name short_name,
                                                t1.system_id,
                                                t1.sort_order,
                                                t1.description
                                    FROM        rd.v_distr_station_outlets t1 
                                    LEFT JOIN   v_nm_all n on t1.distr_station_outlet_id = n.entity_id
                                    LEFT JOIN   v_nm_short_all n1 on t1.distr_station_outlet_id = n1.entity_id
                                    WHERE       t1.distr_station_outlet_id = :id");
        }

        protected override void BindParameters(OracleCommand command, Guid parameters)
        {
                command.AddInputParameter(":id", parameters);
        }

        protected override DistrStationOutletDTO GetResult(OracleDataReader reader, Guid parameters)
        {
            var result = new DistrStationOutletDTO();
            if (reader.Read())
            {
                result = new DistrStationOutletDTO
                           {
                               Id = reader.GetValue<Guid>("distr_station_outlet_id"),
                               Name = reader.GetValue<string>("distr_station_outlet_name"),
                               ParentId = reader.GetValue<Guid>("distr_station_id"),
                               CapacityRated = reader.GetValue<double>("CAPACITY_RATED"),
                               PressureRated = reader.GetValue<double>("PRESSURE_RATED"),
                               Path = reader.GetValue<string>("full_name"),
                               ShortPath = reader.GetValue<string>("short_name"),
                               SystemId = reader.GetValue<int>("SYSTEM_ID"),
                               SortOrder = reader.GetValue<int>("Sort_Order"),
                               Description = reader.GetValue<string>("description")
                           };

            }
            return result;
        }
    }
}