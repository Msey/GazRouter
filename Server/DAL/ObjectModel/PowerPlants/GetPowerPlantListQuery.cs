using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.PowerPlants;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.PowerPlants
{
	public class GetPowerPlantListQuery : QueryReader<int?, List<PowerPlantDTO>>
	{
		public GetPowerPlantListQuery(ExecutionContext context) : base(context)
		{
		}

        protected override string GetCommandText(int? parameters)
        {
            return string.Format(@" SELECT      bp.power_plant_id, 
                                                bp.power_plant_name, 
                                                bp.comp_station_id, 
                                                n.entity_name full_name, 
                                                n1.entity_name short_name, 
                                                bp.system_id, 
                                                bp.sort_order,
                                                bp.description
                                    FROM        rd.v_power_plants bp
                                    LEFT JOIN   v_nm_all n on bp.power_plant_id = n.entity_id
                                    LEFT JOIN   v_nm_short_all n1 on bp.power_plant_id = n1.entity_id
                                    {0}
                                    ORDER BY    bp.sort_order, bp.power_plant_name", 
                                    parameters.HasValue ? "where bp.SYSTEM_ID = :systemId" : string.Empty);
        }

	    protected override void BindParameters(OracleCommand command, int? parameters)
        {
            command.AddInputParameter("systemId", parameters);
        }

        protected override List<PowerPlantDTO> GetResult(OracleDataReader reader, int? parameters)
		{
			var result = new List<PowerPlantDTO>();
            while (reader.Read())
            {
                result.Add(new PowerPlantDTO
                    {
                        Id = reader.GetValue<Guid>("power_plant_ID"),
                        Name = reader.GetValue<string>("power_plant_NAME"),
                        ParentId = reader.GetValue<Guid>("COMP_STATION_ID"),
                        Path = reader.GetValue<string>("full_name"),
                        ShortPath = reader.GetValue<string>("short_name"),
						SystemId = reader.GetValue<int>("SYSTEM_ID"),
						SortOrder = reader.GetValue<int>("Sort_Order"),
                        Description = reader.GetValue<string>("description")
                    });

            }
		    return result;
		}
	}
}