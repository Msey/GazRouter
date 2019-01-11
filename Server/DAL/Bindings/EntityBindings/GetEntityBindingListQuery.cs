using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Bindings.EntityBindings;
using GazRouter.DTO.Dictionaries.EntityTypes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Bindings.EntityBindings
{
    public class GetEntityBindingListQuery : QueryReader<Guid, List<BindingDTO>>
    {
        public GetEntityBindingListQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override string GetCommandText(Guid parameters)
        {
            var q = @"  SELECT      b.entity_binding_id,
                                    b.entity_id,                                                        
                                    b.ext_entity_id,
                                    b.source_id,
                                    s.source_name,
                                    b.is_active
                        FROM        v_entity_bindings b
			            INNER JOIN  v_sources s ON b.source_id= s.source_id
                        WHERE       1=1
                        AND         b.entity_id = :entityid
			            ORDER BY    s.source_name";

            return q;
        }
        
       

        protected override List<BindingDTO> GetResult(OracleDataReader reader, Guid parameters)
        {
            
            var entities = new List<BindingDTO>();
            while (reader.Read())
            {
                var temp = reader.GetValue<bool?>("is_active");
                entities.Add(new BindingDTO
                {
                    Id = reader.GetValue<Guid>("entity_binding_id"),
                    Name = reader.GetValue<string>("entity_name"),
                    Path = reader.GetValue<string>("path"),
                    EntityId = reader.GetValue<Guid>("entity_id"),
                    EntityType = (EntityType?)reader.GetValue<int?>("entity_type_id"),
                    ExtEntityId = reader.GetValue<string>("ext_entity_id"),
                    IsActive = temp.HasValue && temp.Value
                });
            }
            return entities;
        
        }

        protected override void BindParameters(OracleCommand command, Guid parameters)
        {
            command.AddInputParameter("entityid", parameters);
        }
    }
}