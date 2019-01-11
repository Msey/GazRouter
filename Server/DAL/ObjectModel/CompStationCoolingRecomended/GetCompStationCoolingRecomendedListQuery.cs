using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.CompStationCoolingRecomended;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.CompStationCoolingRecomended
{
    public class GetCompStationCoolingRecomendedListQuery : QueryReader<Guid, List<CompStationCoolingRecomendedDTO>>
	{
		public GetCompStationCoolingRecomendedListQuery(ExecutionContext context)
			: base(context)
		{
		}

        protected override string GetCommandText(Guid parameters)
        {
            return @"   SELECT  comp_station_id,
                                month,
                                temperature 
                        FROM    v_t_cooling_recomended
                        WHERE   comp_station_id = :id";
        }

        protected override void BindParameters(OracleCommand command, Guid parameters)
        {
			command.AddInputParameter("id", parameters);
        }

        protected override List<CompStationCoolingRecomendedDTO> GetResult(OracleDataReader reader, Guid parameters)
		{
			var result = new List<CompStationCoolingRecomendedDTO>();
            while (reader.Read())
            {
                result.Add(new CompStationCoolingRecomendedDTO
                {
                    CompStationId = reader.GetValue<Guid>("comp_station_id"),
                    Month = reader.GetValue<int>("month"),
                    Temperature = reader.GetValue<double>("temperature")

                });
            }
		    return result;
		}
	}
}