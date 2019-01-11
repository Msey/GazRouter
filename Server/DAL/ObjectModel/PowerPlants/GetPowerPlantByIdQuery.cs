using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.PowerPlants;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.PowerPlants
{
    public class GetPowerPlantByIdQuery : QueryReader<Guid, PowerPlantDTO>
	{
        public GetPowerPlantByIdQuery(ExecutionContext context)
            : base(context)
		{
		}
                
        protected override string GetCommandText(Guid parameters)
        {
            return @"   SELECT      bp.power_plant_id, 
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
                        WHERE       bp.power_plant_id = :id";
        }

        protected override void BindParameters(OracleCommand command, Guid parameters)
        {
            command.AddInputParameter("id", parameters);
        }

        protected override PowerPlantDTO GetResult(OracleDataReader reader, Guid parameters)
		{
            PowerPlantDTO result = null;
			while (reader.Read())
			{
				result = 
                    new PowerPlantDTO
				    {
                        Id = reader.GetValue<Guid>("power_plant_ID"),
                        Name = reader.GetValue<string>("power_plant_NAME"),
                        ParentId = reader.GetValue<Guid>("COMP_STATION_ID"),
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