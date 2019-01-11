using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.Dictionaries.Targets;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Bindings.EntityPropertyBindings
{
    public class GetEntityPropertyBindingListQuery : QueryReader<GetEntityPropertyBindingParameterSet, List<SimpleEntityPropertyBindingDTO>>
    {
        public GetEntityPropertyBindingListQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override string GetCommandText(GetEntityPropertyBindingParameterSet parameters)
        {
            return String.Format(@"
         SELECT  
                b.entity_id,
                b.binding_id,
				b.Source_id,
				b.Property_Type_id,
				b.period_type_id,
				b.ext_key,
                e.entity_type_id
           FROM V_BINDINGS b 
            Left join V_ENTITIES e on b.Entity_id= e.entity_id 
            WHERE b.Source_id = :source_id {0}
   ", string.IsNullOrEmpty(parameters.ExtKey) ? @" and b.ext_key is not null" :  @" and b.ext_key = :extKey ");
        }

        protected override List<SimpleEntityPropertyBindingDTO> GetResult(OracleDataReader reader, GetEntityPropertyBindingParameterSet parameters)
        {
            var result = new List<SimpleEntityPropertyBindingDTO>();
            while(reader.Read())
            {
                result.Add(new SimpleEntityPropertyBindingDTO
                           {
                               Id = reader.GetValue<Guid>("binding_id"),
                               EntityId = reader.GetValue<Guid>("entity_id"),
                               SourceId = reader.GetValue<int>("Source_id"),
                               ExtKey = reader.GetValue<string>("ext_key"),
                               PropertyTypeId = reader.GetValue<PropertyType>("Property_Type_id"),
                               PeriodTypeId = reader.GetValue<PeriodType>("period_type_id"),
                               EntityTypeId = reader.GetValue<EntityType>("entity_type_id")
                           });
            }
            return result;
        }

        protected override void BindParameters(OracleCommand command, GetEntityPropertyBindingParameterSet parameters)
        {
            if (!string.IsNullOrEmpty(parameters.ExtKey))
            {
                command.AddInputParameter("extKey", parameters.ExtKey);
            }
            command.AddInputParameter("source_id", parameters.SourceId);
        }
    }
}