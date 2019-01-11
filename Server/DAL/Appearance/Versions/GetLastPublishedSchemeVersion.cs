using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.Appearance.Versions;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Appearance.Versions
{
    public class GetLastPublishedSchemeVersion : QueryReader<SchemeVersionDTO>
    {
        public GetLastPublishedSchemeVersion(ExecutionContext context)
            : base(context)
        {
        }


        protected override string GetCommandText()
        {
            return @"
                SELECT 
                    sv.scheme_version_id, 
                    sv.scheme_id, 
                    sv.is_publihed, 
                    sv.create_date, 
                    sv.create_user_id, 
                    sv.description, 
                    s.scheme_name, 
                    u.name, 
                    sv.system_id  
                FROM v_SCHEME_VERSIONS sv 
                join v_schemes s on sv.scheme_id = s.scheme_id 
                join v_users u on sv.create_user_id = u.user_id 
                where sv.scheme_version_id in
                (
                    select max(scheme_version_id)
                    from v_SCHEME_VERSIONS
                    where is_publihed = 1
                )";
        }

        protected override SchemeVersionDTO GetResult(OracleDataReader reader)
        {
            if (reader.Read())
            {
                return new SchemeVersionDTO
                               {
                                   Id = reader.GetValue<int>("scheme_version_id"),
                                   SchemeId = reader.GetValue<int>("scheme_id"),
                                   IsPublished = reader.GetValue<bool>("is_publihed"),
                                   SchemeName = reader.GetValue<string>("scheme_name"),
                                   CreateDate = reader.GetValue<DateTime>("create_date"),
                                   CreatorId = reader.GetValue<int>("create_user_id"),
                                   CreatorName = reader.GetValue<string>("name"),
                                   SystemId = reader.GetValue<int>("system_id"),
                               };

            }
            return null;
        }
    }
}