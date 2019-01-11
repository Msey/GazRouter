using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Bindings.EntityBindings;
using GazRouter.DTO.Dictionaries.EntityTypes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Bindings.EntityBindings
{
    public class GetAllEntityBindingsListQuery : QueryReader<GetEntityBindingsPageParameterSet, List<BindingDTO>>
    {
        public GetAllEntityBindingsListQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override string GetCommandText(GetEntityBindingsPageParameterSet parameters)
        {
            const string queryTemplate = @"
         SELECT  e.entity_id,
                 e.entity_name,
				 e.name entity_name_lat,
                 e.entity_type_id,
				 n.Entity_name path,
				b.Entity_binding_id,
				b.ext_entity_id,
                b.is_active
           FROM  v_entities e
			left join V_NM_SHORT_ALL n on e.entity_id = N.ENTITY_ID
			Left join V_ENTITY_BINDINGS b on b.Entity_id= e.entity_id AND b.Source_id = :sourceId
            where b.Source_id is not null
                ";

            return String.Format(queryTemplate);
        }


        protected override List<BindingDTO> GetResult(OracleDataReader reader, GetEntityBindingsPageParameterSet parameters)
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

        protected override void BindParameters(OracleCommand command, GetEntityBindingsPageParameterSet parameters)
        {
            command.AddInputParameter("sourceId", parameters.SourceId);
        }
    }
}