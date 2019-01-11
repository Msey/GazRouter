using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.ObjectModel;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.Entities
{
    public class GetEntityQuery : QueryReader<Guid, CommonEntityDTO>
    {
        public GetEntityQuery(ExecutionContext context) : base(context)
        {
        }


        protected override void BindParameters(OracleCommand command, Guid parameters)
        {
           command.AddInputParameter("id",parameters);
        }

        protected override string GetCommandText(Guid parameters)
        {

			return @"   SELECT      e.entity_id, 
                                    e.entity_type_id, 
                                    e.entity_name, 
                                    n.entity_name AS path, 
                                    n1.entity_name AS short_path 
                        FROM        v_entities e
                        LEFT JOIN   v_nm_short_all n1 ON e.entity_id = n1.entity_id
                        LEFT JOIN   v_nm_all n ON e.entity_id = n.entity_id
                        WHERE       e.entity_id = :id";
        }

        protected override CommonEntityDTO GetResult(OracleDataReader reader, Guid parameters)
        {
            CommonEntityDTO result = null;
            if (reader.Read())
            {
                result =
                    new CommonEntityDTO
                    {
                        Id = reader.GetValue<Guid>("entity_id"),
                        EntityType = reader.GetValue<EntityType>("entity_type_id"),
						Path = reader.GetValue<string>("path"),
                        Name = reader.GetValue<string>("entity_name"),
                        ShortPath = reader.GetValue<string>("short_path")
                    };
            }

            return result;
        }
    }
}
