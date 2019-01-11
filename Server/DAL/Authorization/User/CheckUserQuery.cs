﻿using System;
using System.IO;
using System.Xml.Serialization;
using GazRouter.DAL.Core;
using GazRouter.DTO.Authorization.User;
using GazRouter.DTO.Dictionaries.EntityTypes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Authorization.User
{
    public class CheckUserQuery : QueryReader<CheckUserParameters, UserDTO>
    {
        public CheckUserQuery(ExecutionContext context)
            : base(context)
        {}

        protected override string GetCommandText(CheckUserParameters parameters)
        {
            return @"   SELECT  t1.user_id, 
                                t1.login, 
                                t1.description, 
                                t1.name, 
                                t1.phone, 
                                t1.site_id, 
                                t1.settings, 
                                t1.site_level, 
                                NVL(en.entity_name, 'подразделение не задано') AS site_name, 
                                en.entity_type_id
                        FROM v_users t1 
                        LEFT JOIN v_entities en ON t1.site_id = en.entity_id
                        WHERE UPPER(t1.login) = :login and t1.password = :pwd";
        }

        protected override void BindParameters(OracleCommand command, CheckUserParameters parameters)
        {
            command.AddInputParameter("login", parameters.UserName.ToUpper());
            command.AddInputParameter("pwd", parameters.Password);
        }

        protected override UserDTO GetResult(OracleDataReader reader, CheckUserParameters parameters)
        {
            UserDTO user = null;
            if (reader.Read())
            {
                user =
                    new UserDTO
                    {
                        Id = reader.GetValue<int>("user_id"),
                        UserName = reader.GetValue<string>("name"),
                        Login = reader.GetValue<string>("login"),
                        Description = reader.GetValue<string>("description"),
                        Phone = reader.GetValue<string>("phone"),
                        SiteId = reader.GetValue<Guid?>("site_id"),
                        SiteLevel = reader.GetValue<int>("site_level"),
                        SiteName = reader.GetValue<string>("site_name"),
                        EntityType = reader.GetValue<EntityType?>("entity_type_id")
                    };

                var settingsStr = reader.GetValue<string>("settings");
                if (settingsStr != null)
                {
                    var xmlSerializer = new XmlSerializer(typeof(UserSettings));
                    TextReader tr = new StringReader(settingsStr);
                    user.UserSettings = (UserSettings)xmlSerializer.Deserialize(tr);
                }
            }

            return user;
        }
    }

    public class CheckUserParameters
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}