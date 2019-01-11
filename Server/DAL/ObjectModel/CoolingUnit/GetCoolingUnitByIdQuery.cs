using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.ObjectModel.CoolingUnit;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.CoolingUnit
{
    public class GetCoolingUnitByIdQuery : QueryReader<Guid, CoolingUnitDTO>
    {
        public GetCoolingUnitByIdQuery(ExecutionContext context)
            : base(context)
        {}

        protected override string GetCommandText(Guid parameters)
        {
            return @"   SELECT      cs.cooling_unit_id, 
                                    cs.cooling_unit_name, 
                                    cs.cooling_station_id,
                                    cs.sort_order,
                                    cs.is_virtual,
                                    cs.system_id,
                                    cs.cooling_unit_type_id,
                                    n.entity_name as path,
                                    n1.entity_name as short,
                                    cs.description
                        FROM        v_cooling_units cs
                        LEFT JOIN   v_nm_all n ON cs.cooling_unit_id = n.entity_id
                        LEFT JOIN   v_nm_short_all n1 ON cs.cooling_unit_id = n1.entity_id
                        WHERE       cs.cooling_unit_id = :id";
        }


        protected override void BindParameters(OracleCommand command, Guid parameters)
        {
            command.AddInputParameter("id", parameters);
        }


        protected override CoolingUnitDTO GetResult(OracleDataReader reader, Guid parameters)
        {
			
            if (reader.Read())
            {
				return new CoolingUnitDTO
                {
                    Id = reader.GetValue<Guid>("cooling_unit_id"),
                    Name = reader.GetValue<string>("cooling_unit_name"),
                    ParentId = reader.GetValue<Guid>("cooling_station_id"),
                    SortOrder = reader.GetValue<int>("sort_order"),
                    IsVirtual = reader.GetValue<bool>("is_virtual"),
                    SystemId = reader.GetValue<int>("system_id"),
                    CoolingUnitTypeId = reader.GetValue<int>("cooling_unit_type_id"),
					Path = reader.GetValue<string>("path"),
					ShortPath = reader.GetValue<string>("short"),
                    Description = reader.GetValue<string>("description")
                };
            }
            return null;
        }
    }
}