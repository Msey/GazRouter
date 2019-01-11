using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.BoilerPlants;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.BoilerPlants
{
    public class GetBoilerPlantByIdQuery : QueryReader<Guid, BoilerPlantDTO>
	{
        public GetBoilerPlantByIdQuery(ExecutionContext context)
            : base(context)
		{
		}
                
        protected override string GetCommandText(Guid parameters)
        {
            return @"   SELECT      bp.boiler_plant_id, 
                                    bp.boiler_plant_name, 
                                    bp.comp_station_id, 
                                    n.entity_name full_name, 
                                    n1.entity_name short_name, 
                                    bp.system_id, 
                                    bp.sort_order,
                                    bp.description
                        FROM        rd.v_boiler_plants bp
                        LEFT JOIN   v_nm_all n ON bp.boiler_plant_id = n.entity_id
                        LEFT JOIN   v_nm_short_all n1 ON bp.boiler_plant_id = n1.entity_id
                        WHERE       bp.boiler_plant_id = :id";
        }

        protected override void BindParameters(OracleCommand command, Guid parameters)
        {
            command.AddInputParameter("id", parameters);
        }

        protected override BoilerPlantDTO GetResult(OracleDataReader reader, Guid parameters)
		{
            BoilerPlantDTO result = null;
			while (reader.Read())
			{
				result = new BoilerPlantDTO
				{
                    Id = reader.GetValue<Guid>("boiler_plant_id"),
                    Name = reader.GetValue<string>("boiler_plant_name"),
                    ParentId = reader.GetValue<Guid>("comp_station_id"),
                    Path = reader.GetValue<string>("full_name"),
                    ShortPath = reader.GetValue<string>("short_name"),
                    SystemId = reader.GetValue<int>("system_id"),
                    SortOrder = reader.GetValue<int>("sort_order"),
                    Description = reader.GetValue<string>("description")
                };
			}
			return result;
		}
	}
}