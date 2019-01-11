using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Bindings.EntityPropertyBindings;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.DataExchange.TypicalExchange
{
    public class GetExchangeEntityPropertyBindingListQuery : QueryReader<GetEntityPropertyBindingListParameterSet, List<EntityPropertyBindingDTO>>
    {
        public GetExchangeEntityPropertyBindingListQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override string GetCommandText(GetEntityPropertyBindingListParameterSet parameters)
        {
            return string.Format(@"
SELECT PT.NAME property_type_name, pt.Property_Type_id, B.BINDING_ID, b.ext_Key , e.entity_id
         from V_ENTITIES e
join V_ENTITY_TYPE_PROPERTIES etp on etp.entity_type_id = e.entity_type_id
join V_PROPERTY_TYPES pt on pt.property_type_id = etp.property_type_id
join V_BINDINGS b on b.Entity_id= e.entity_id 
WHERE b.property_type_id =    pt.property_type_id  and b.period_type_id = :periodTypeId
           AND b.source_id = :source_id
");
        }


        protected override List<EntityPropertyBindingDTO> GetResult(OracleDataReader reader, GetEntityPropertyBindingListParameterSet parameters)
        {
            var result = new List<EntityPropertyBindingDTO>();
            while (reader.Read())
            {
                result.Add(new EntityPropertyBindingDTO
                                 {
                                     Id = reader.GetValue<Guid>("binding_id"),
                                     EntityId = reader.GetValue<Guid>("entity_id"),
                                     ExtKey = reader.GetValue<string>("ext_key"),
                                     PropertyId = reader.GetValue<PropertyType>("Property_Type_id"),
                                     PropertyName = reader.GetValue<string>("Property_Type_Name"),
                                 });
            }
            return result;
        }

        protected override void BindParameters(OracleCommand command, GetEntityPropertyBindingListParameterSet parameters)
        {
            command.AddInputParameter("source_id", parameters.SourceId);
            command.AddInputParameter("periodTypeId", parameters.PeriodTypeId);
        }
    }
}