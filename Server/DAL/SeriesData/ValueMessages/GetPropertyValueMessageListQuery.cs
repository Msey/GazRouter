using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.SeriesData;
using GazRouter.DTO.SeriesData.ValueMessages;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.SeriesData.ValueMessages
{
    public class GetPropertyValueMessageListQuery : QueryReader<GetPropertyValueMessageListParameterSet, Dictionary<Guid, Dictionary<PropertyType, List<PropertyValueMessageDTO>>>>
    {
        public GetPropertyValueMessageListQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override string GetCommandText(GetPropertyValueMessageListParameterSet parameters)
        {
            var q = @"  SELECT      m.value_message_id,
                                    m.series_id,
                                    m.key_date,
                                    m.period_type_id,
                                    m.entity_id,
                                    m.property_type_id,
                                    m.message_id,
                                    m.check_id,
                                    m.cre_mess_date,
                                    m.cre_user_name,
                                    m.cre_user_site_name,
                                    m.acept_mess_date,
                                    m.acept_user_name,
                                    m.acept_user_site_name,
                                    m.message_text,
                                    m.message_type_id,
                                    m.message_type_name,
                                    site.site_id,
                                    e.entity_type_id,
                                    e.entity_name,
                                    p.entity_name AS entity_path
                        FROM        rd.v_value_messages m
                        INNER JOIN  v_entities e ON e.entity_id = m.entity_id
                        LEFT JOIN   v_nm_short_all p ON p.entity_id = e.entity_id
                        INNER JOIN  v_entity_2_site site ON site.entity_id = m.entity_id 
                        WHERE       1=1";

            var sb = new StringBuilder(q);

            if (parameters != null)
            {

                if (parameters.EntityIdList.Any())
                    sb.Append(string.Format(" AND m.entity_id IN {0}", CreateInClause(parameters.EntityIdList.Count)));
                
                if (parameters.SiteId.HasValue) sb.Append(" AND site.site_id = :site");

                if (parameters.PropertyType.HasValue)
                    sb.Append(" AND m.property_type_id = :proptype");


                if (parameters.SerieId.HasValue)
                    sb.Append(" AND m.series_id = :serie");
                else
                {
                    sb.Append(" AND m.key_date BETWEEN :startdate AND :enddate");
                    sb.Append(" AND m.period_type_id = :pertypeid");
                }
            }

            sb.Append(" ORDER BY m.entity_id, m.property_type_id, m.key_date");

            return sb.ToString();
        }

        protected override void BindParameters(OracleCommand command, GetPropertyValueMessageListParameterSet parameters)
        {
            

            if (parameters == null) return;


            if (parameters.SiteId.HasValue) command.AddInputParameter("site", parameters.SiteId);

            for (var i = 0; i < parameters.EntityIdList.Count; i++)
            {
                command.AddInputParameter(string.Format("p{0}", i), parameters.EntityIdList[i]);
            }

            if (parameters.PropertyType.HasValue)
                command.AddInputParameter("proptype", parameters.PropertyType);


            if (parameters.SerieId.HasValue)
            {
                command.AddInputParameter("serie", parameters.SerieId);
            }
            else
            {
                command.AddInputParameter("startdate", parameters.StartDate);
                command.AddInputParameter("enddate", parameters.EndDate);
                command.AddInputParameter("pertypeid", parameters.PeriodType);
            }
        }

        
        protected override Dictionary<Guid, Dictionary<PropertyType, List<PropertyValueMessageDTO>>> GetResult(
            OracleDataReader reader, GetPropertyValueMessageListParameterSet parameters)
        {
            var result = new Dictionary<Guid, Dictionary<PropertyType, List<PropertyValueMessageDTO>>>();
            
            Guid? entityId = null;
            PropertyType? propertyTypeId = null;
            List<PropertyValueMessageDTO> msgList = null;


            while (reader.Read())
            {
                var newEntityId = reader.GetValue<Guid>("entity_id");
                var newPropertyId = reader.GetValue<PropertyType>("property_type_id");

                if (entityId == null || newEntityId != entityId)
                {
                    entityId = newEntityId;
                    propertyTypeId = newPropertyId;
                    msgList = new List<PropertyValueMessageDTO>();
                    var dict = new Dictionary<PropertyType, List<PropertyValueMessageDTO>>
                    {
                        {newPropertyId, msgList}
                    };
                    result.Add(newEntityId, dict);
                }
                else
                {
                    if (newPropertyId != propertyTypeId.Value)
                    {
                        msgList = new List<PropertyValueMessageDTO>();
                        propertyTypeId = newPropertyId;
                        result[entityId.Value].Add(newPropertyId, msgList);
                    }
                }

                var msg = new PropertyValueMessageDTO
                {
                    Id = reader.GetValue<Guid>("value_message_id"),
                    Timestamp = reader.GetValue<DateTime>("key_date"),
                    EntityId = reader.GetValue<Guid>("entity_id"),
                    EntityType = reader.GetValue<EntityType>("entity_type_id"),
                    EntityName = reader.GetValue<string>("entity_name"),
                    EntityPath = reader.GetValue<string>("entity_path"),

                    SiteId = reader.GetValue<Guid>("site_id"),
                    PropertyType = reader.GetValue<PropertyType>("property_type_id"),
                    MessageText = reader.GetValue<string>("message_text"),
                    MessageType = reader.GetValue<PropertyValueMessageType>("message_type_id"),
                    CreateDate = reader.GetValue<DateTime>("cre_mess_date"),
                    CreateUserName = reader.GetValue<string>("cre_user_name"),
                    CreateUserSite = reader.GetValue<string>("cre_user_site_name"),

                    AckDate = reader.GetValue<DateTime>("acept_mess_date"),
                    AckUserName = reader.GetValue<string>("acept_user_name"),
                    AckUserSite = reader.GetValue<string>("message_type_name")
                };
                msgList.Add(msg);

            }

            return result;
        }
    }
}
