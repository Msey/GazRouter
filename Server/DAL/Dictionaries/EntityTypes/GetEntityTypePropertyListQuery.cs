using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.EntityTypeProperties;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Dictionaries.EntityTypes
{
    /// <summary>
    /// Возвращает список свойств для каждого типа сущности
    /// </summary>
    public class GetEntityTypePropertyListQuery : QueryReader<EntityType?, List<EntityTypePropertyDTO>>
    {
        public GetEntityTypePropertyListQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override string GetCommandText(EntityType? parameters)
        {
            var q = @"  SELECT      entity_type_id, 
                                    property_type_id,
                                    is_mandatory,
                                    is_input                                 
                        FROM        v_entity_type_properties
                        WHERE       1=1";

            var sb = new StringBuilder(q);

            if (parameters.HasValue)
                sb.Append(" AND entity_type_id = :entid");

            sb.Append(" ORDER BY sort_order");

            return sb.ToString();
        }

        protected override void BindParameters(OracleCommand command, EntityType? parameters)
        {
            command.AddInputParameter("entid", parameters);
        }

        protected override List<EntityTypePropertyDTO> GetResult(OracleDataReader reader, EntityType? parameters)
        {
            var result = new List<EntityTypePropertyDTO>();
            
            while (reader.Read())
            {
                result.Add(
                    new EntityTypePropertyDTO
                        {
                            EntityType = reader.GetValue<EntityType>("entity_type_id"),
                            PropertyType = reader.GetValue<PropertyType>("property_type_id"),
                            IsMandatory = reader.GetValue<bool>("is_mandatory"),
                            IsInput = reader.GetValue<bool>("is_input"),
                        });
            }
            return result;
        }
    }
}