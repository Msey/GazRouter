using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Alarms;
using GazRouter.DTO.Dictionaries.AlarmTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Alarms
{
    public class GetAlarmListQuery : QueryReader<GetAlarmListParameterSet, List<AlarmDTO>>
    {
        public GetAlarmListQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override string GetCommandText(GetAlarmListParameterSet parameters)
        {
            return string.Format(
                   @"   SELECT      a.alarm_id,
                                    a.entity_id,
                                    a.property_type_id,
                                    a.period_type_id,
                                    a.alarm_type_id,
                                    a.setting,
                                    a.activation_date,
                                    a.expiration_date,
                                    a.description,
                                    a.user_id,
                                    a.user_name,
                                    a.user_full_name,
                                    a.user_site_name,
                                    a.user_site_id,
                                    a.creation_date,
                                    a.status,
                                    sp.entity_name
                        FROM        v_alr_alarms a
                        LEFT JOIN  v_nm_short_all sp ON (sp.entity_id = a.entity_id)
                        WHERE       1 = 1 
                            AND     a.user_site_id = :siteid
                            {0}
                        ORDER BY    a.creation_date DESC",
                        parameters.UserId.HasValue ? "AND a.user_id = :userid" : "");
                
        }


        protected override void BindParameters(OracleCommand command, GetAlarmListParameterSet parameters)
        {
            command.AddInputParameter("siteid", parameters.SiteId);
            if (parameters.UserId.HasValue)
                command.AddInputParameter("userid", parameters.UserId.Value);
        }


        protected override List<AlarmDTO> GetResult(OracleDataReader reader, GetAlarmListParameterSet parameters)
        {
            var result = new List<AlarmDTO>();
            while (reader.Read())
            {
                result.Add(new AlarmDTO
                {
                    Id = reader.GetValue<int>("alarm_id"),
                    EntityId = reader.GetValue<Guid>("entity_id"),
                    EntityName = reader.GetValue<string>("entity_name"),
                    PropertyTypeId = reader.GetValue<PropertyType>("property_type_id"),
                    PeriodTypeId = reader.GetValue<PeriodType>("period_type_id"),
                    AlarmTypeId = reader.GetValue<AlarmType>("alarm_type_id"),
                    Setting = reader.GetValue<double>("setting"),
                    ActivationDate = reader.GetValue<DateTime>("activation_date"),
                    ExpirationDate = reader.GetValue<DateTime>("expiration_date"),
                    Description = reader.GetValue<string>("description"),
                    UserId = reader.GetValue<int>("user_id"),
                    UserLogin = reader.GetValue<string>("user_name"),
                    UserName = reader.GetValue<string>("user_full_name"),
                    UserSiteId = reader.GetValue<Guid>("user_site_id"),
                    UserSiteName = reader.GetValue<string>("user_site_name"),
                    CreationDate = reader.GetValue<DateTime>("creation_date"),
                    Status = reader.GetValue<int>("status")
                });
            }
            return result;
        }
    }
}
