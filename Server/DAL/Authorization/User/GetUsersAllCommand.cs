using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using GazRouter.DAL.Core;
using GazRouter.DTO.Authorization.User;
using GazRouter.DTO.Dictionaries.EntityTypes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Authorization.User
{
    public class GetUsersAllQuery : QueryReader<List<UserDTO>>
    {
        public GetUsersAllQuery(ExecutionContext context)
            : base(context)
        {}

        protected override string GetCommandText()
        {
            return @"   select      t1.user_id, 
                                    t1.login, 
                                    t1.description, 
                                    t1.name, 
                                    t1.phone, 
                                    t1.SITE_ID, 
                                    t1.SETTINGS, 
                                    t1.SITE_LEVEL, 
                                    en.entity_name AS Site_Name, 
                                    en.entity_type_id
                        from        v_users t1 
                        join        v_entities en on t1.SITE_ID = en.entity_id
                        order by    t1.name";
        }

        protected override List<UserDTO> GetResult(OracleDataReader reader)
        {
            var users = new List<UserDTO>();
            while (reader.Read())
            {
                var user =
                    new UserDTO
                    {
                        Id = reader.GetValue<int>("user_id"),
                        UserName = reader.GetValue<string>("name"),
                        Login = reader.GetValue<string>("login"),
                        Description = reader.GetValue<string>("description"),
                        Phone = reader.GetValue<string>("phone"),
                        SiteId = reader.GetValue<Guid?>("SITE_ID"),
                        SiteLevel = reader.GetValue<int>("SITE_LEVEL"),
                        SiteName = reader.GetValue<string>("Site_Name"),
                        EntityType = reader.GetValue<EntityType?>("entity_type_id")
                    };

                var settingsStr = reader.GetValue<string>("SETTINGS");
                if (settingsStr !=  null)
                {
                    var xmlSerializer = new XmlSerializer(typeof(UserSettings));
                    TextReader tr = new StringReader(settingsStr);
                    user.UserSettings = (UserSettings)xmlSerializer.Deserialize(tr);
                }

                users.Add(user);
            }
            return users;
        }
    }
}