using System;
using System.Collections.Generic;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.ObjectModel.DistrStationOutlets;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.DistrStationOutlets
{
    public class GetDistrStationOutletListQuery : QueryReader<GetDistrStationOutletListParameterSet, List<DistrStationOutletDTO>>
	{
		public GetDistrStationOutletListQuery(ExecutionContext context) : base(context)
		{
		}

        protected override string GetCommandText(GetDistrStationOutletListParameterSet parameters)
        {
            var q = @"              SELECT      t1.distr_station_outlet_id, 
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
                                                t1.status,
                                                t1.description,
                                                c.gas_consumer_id,
                                                c.gas_consumer_name
                                    FROM        rd.v_distr_station_outlets t1 
                                    LEFT JOIN   v_gas_consumers c ON t1.gas_consumer_id = c.gas_consumer_id
                                    LEFT JOIN   v_nm_all n ON t1.distr_station_outlet_id = n.entity_id
                                    LEFT JOIN   v_nm_short_all n1 ON t1.distr_station_outlet_id = n1.entity_id
                                    WHERE       1=1";

            var sb = new StringBuilder(q);

            if (parameters != null)
            {
                if (parameters.DistrStationId.HasValue)
                    sb.Append(" AND t1.distr_station_id  = :distrStationId");

                if (parameters.SystemId.HasValue)
                    sb.Append(" AND t1.system_id  = :systemId");
            }

            sb.Append(" ORDER BY t1.sort_order");

            return sb.ToString();
        }

        protected override void BindParameters(OracleCommand command, GetDistrStationOutletListParameterSet parameters)
        {
            if (parameters != null)
            {
                if (parameters.DistrStationId.HasValue)
                    command.AddInputParameter("distrStationId", parameters.DistrStationId);
                if (parameters.SystemId.HasValue)
                    command.AddInputParameter("systemId", parameters.SystemId);
            }
        }

        protected override List<DistrStationOutletDTO> GetResult(OracleDataReader reader, GetDistrStationOutletListParameterSet parameters)
		{
			var result = new List<DistrStationOutletDTO>();
			while (reader.Read())
			{
				result.Add(new DistrStationOutletDTO
				{
                    Id = reader.GetValue<Guid>("distr_station_outlet_id"),
                    Name = reader.GetValue<string>("distr_station_outlet_name"),
                    ParentId = reader.GetValue<Guid>("distr_station_id"),
                    CapacityRated = reader.GetValue<double>("capacity_rated"),
                    PressureRated = reader.GetValue<double>("pressure_rated"),
                    ConsumerId = reader.GetValue<Guid?>("gas_consumer_id"),
                    ConsumerName = reader.GetValue<string>("gas_consumer_name"),
                    Path = reader.GetValue<string>("full_name"),
                    ShortPath = reader.GetValue<string>("short_name"),
					SystemId = reader.GetValue<int>("system_id"),
					SortOrder = reader.GetValue<int>("sort_order"),
                    Status =  reader.GetValue<EntityStatus?>("status"),
                    Description = reader.GetValue<string>("description")
                });

			}
			return result;
		}
	}
}