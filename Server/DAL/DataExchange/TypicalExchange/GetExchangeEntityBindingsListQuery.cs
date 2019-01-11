using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Bindings.EntityBindings;
using GazRouter.DTO.Bindings.ExchangeEntities;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.DataExchange.TypicalExchange
{
    public class GetExchangeEntityBindingsListQuery : QueryReader<GetExchangeEntityBindingsListParameterSet, List<BindingDTO>>
    {
        public GetExchangeEntityBindingsListQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override string GetCommandText(GetExchangeEntityBindingsListParameterSet parameters)
        {
            return string.Format(@"
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
			join V_ENTITY_BINDINGS b on b.Entity_id= e.entity_id 
           WHERE b.source_id = :source_id 
        ");
        }


        protected override List<BindingDTO> GetResult(OracleDataReader reader, GetExchangeEntityBindingsListParameterSet parameters)
        {
            
            var result = new List<BindingDTO>();
            while (reader.Read())
            {
                result.Add(new BindingDTO
                                 {
                                     Id = reader.GetValue<Guid>("entity_binding_id"),
                                     Name = reader.GetValue<string>("entity_name"),
                                     Path = reader.GetValue<string>("path"),
                                     EntityId = reader.GetValue<Guid>("entity_id"),
                                     ExtEntityId = reader.GetValue<string>("ext_entity_id"),
                                     IsActive = reader.GetValue<bool>("is_active")
                                 });
            }

            return result;
        }


        protected override void BindParameters(OracleCommand command, GetExchangeEntityBindingsListParameterSet parameters)
        {
            command.AddInputParameter("source_id", parameters.SourceId);
        }
    }
}