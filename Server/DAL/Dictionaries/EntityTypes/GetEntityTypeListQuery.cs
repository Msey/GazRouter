using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.EntityTypes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Dictionaries.EntityTypes
{
    public class GetEntityTypeListQuery : QueryReader<List<EntityTypeDTO>>
    {
        public GetEntityTypeListQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override string GetCommandText()
        {
            return @"   SELECT      entity_type_id, 
                                    name, 
                                    description,
                                    short_name, 
                                    system_name
                        FROM        v_entity_types
                        ORDER BY    sort_order";
        }

        protected override List<EntityTypeDTO> GetResult(OracleDataReader reader)
        {
            var result = new List<EntityTypeDTO>();
            
            while (reader.Read())
            {
                var entityType = new EntityTypeDTO
                {
                    Id = reader.GetValue<int>("entity_type_id"),
                    Name = reader.GetValue<string>("name"),
                    Description = reader.GetValue<string>("description"),
                    ShortName = reader.GetValue<string>("short_name"),
                    SystemName = reader.GetValue<string>("system_name")
                };
                result.Add(entityType);
                
            }
            return result;
        }

    }
}   