using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Appearance.Versions;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Appearance.Versions
{
    public class GetSchemeVersionListQuery : QueryReader<List<SchemeVersionItemDTO>>
    {
        public GetSchemeVersionListQuery(ExecutionContext context)
            : base(context)
        {
        }



        protected override string GetCommandText()
        {
            return @"SELECT sv.scheme_version_id, sv.is_publihed, sv.create_date, s.scheme_name, u.name, sys.SYSTEM_NAME, rem,rem_date,rem_user   
                     FROM v_SCHEME_VERSIONS sv 
                     join v_schemes s on sv.scheme_id = s.scheme_id 
                     join v_systems sys on s.system_id = sys.system_id 
                     join v_users u on sv.create_user_id = u.user_id  
                     order by create_date desc";
        }

        protected override List<SchemeVersionItemDTO> GetResult(OracleDataReader reader)
        {
            var result = new List<SchemeVersionItemDTO>();
            while (reader.Read())
            {
                result.Add(new SchemeVersionItemDTO
                               {
                                   Id = reader.GetValue<int>("scheme_version_id"),
                                   IsPublished = reader.GetValue<bool>("is_publihed"),
                                   SchemeName = reader.GetValue<string>("scheme_name"),
                                   SystemName = reader.GetValue<string>("SYSTEM_NAME"),
                                   CreateDate = reader.GetValue<DateTime>("create_date"),
                                   CreatorName = reader.GetValue<string>("name"),
                                   Comment =  reader.GetValue<string>("rem"),
                                   CommentAuthor = reader.GetValue<string>("rem_user"),
                                   CommentDate = reader.GetValue<DateTime>("rem_date")
                               });

            }
            return result;
        }
    }
}