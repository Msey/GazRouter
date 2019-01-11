using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.ObjectModel.Sites;
using GazRouter.DTO.SeriesData;
using GazRouter.DTO.SeriesData.EntityValidationStatus;
using GazRouter.DTO.SeriesData.ValueMessages;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.SeriesData.ValueMessages
{
    /// <summary>
    /// Данный запрос возвращает обобщенный статус по каждому объекту в виде:
    /// entity_id, message_type_id (1 - ошибка, 3 - тревога).
    /// 
    /// Обобщенный статус расчитывается следующим образом:
    /// 1) если по значениям свойств объекта есть хотябы одно сообщение типа 1-ошибка,
    /// для объекта будет выставлен статус 1-ошибка;
    /// 2) если по значениям свойств объекта нет ошибок, но есть тревоги, 
    /// то для объекта будет выставлен статус 3-тревога;
    /// 3) если по значениям свойств объекта нет вообще никаких сообщений,
    /// то строка для этого объекта будет отсутсвовать в конечной выборке.
    /// </summary>
    public class GetEntityValidationStatusListQuery : QueryReader<GetEntityValidationStatusListParameterSet, List<EntityValidationStatusDTO>>
    {
        public GetEntityValidationStatusListQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override string GetCommandText(GetEntityValidationStatusListParameterSet parameters)
        {
            var q = @"  WITH
                        message_count AS (
                                SELECT      vm.entity_id, 
                                            m.message_type_id, 
                                            COUNT(*) AS cnt 
                                FROM        v_value_messages vm
                                INNER JOIN  v_messages m ON vm.message_id = m.message_id
                                WHERE       1=1
                                    AND     vm.series_id = :serie
                                GROUP BY    vm.entity_id, 
                                            m.message_type_id),

                        smr AS (
                                SELECT      entity_id, 
                                            message_type_id 
                                FROM        message_count
                                WHERE       message_type_id = 1

                                UNION       

                                SELECT      entity_id, 
                                            message_type_id 
                                FROM        message_count
                                WHERE       message_type_id = 3
                                    AND     entity_id NOT IN (SELECT entity_id FROM message_count WHERE message_type_id = 1))


                        SELECT      s.entity_id, 
                                    s.message_type_id,
                                    e.entity_type_id,
                                    e2s.site_id
                        FROM        smr s
                        INNER JOIN  v_entity_2_site e2s ON s.entity_id = e2s.entity_id
                        INNER JOIN  v_entities e ON e.entity_id = s.entity_id
                        WHERE       1=1";

            var sb = new StringBuilder(q);

            if (parameters.SiteId.HasValue)
                sb.Append(" AND e2s.site_id = :site");

            if (parameters.EntityType.HasValue)
                sb.Append(" AND e.entity_type_id = :etype");

            return sb.ToString();
        }

        protected override void BindParameters(OracleCommand command, GetEntityValidationStatusListParameterSet parameters)
        {
            command.AddInputParameter("serie", parameters.SerieId);

            if (parameters.SiteId.HasValue)
                command.AddInputParameter("site", parameters.SiteId);

            if (parameters.EntityType.HasValue)
                command.AddInputParameter("etype", parameters.EntityType);

        }

        
        protected override List<EntityValidationStatusDTO> GetResult(
            OracleDataReader reader, GetEntityValidationStatusListParameterSet parameters)
        {
            var result = new List<EntityValidationStatusDTO>();

            while (reader.Read())
            {
                var msgType = reader.GetValue<PropertyValueMessageType>("message_type_id");
                result.Add(
                    new EntityValidationStatusDTO
                    {
                        EntityId = reader.GetValue<Guid>("entity_id"),
                        SiteId = reader.GetValue<Guid>("site_id"),
                        EntityType = reader.GetValue<EntityType>("entity_type_id"),
                        Status = msgType == PropertyValueMessageType.Error ? EntityValidationStatus.Error : EntityValidationStatus.Alarm
                    });
            }

            return result;
        }
    }
}
