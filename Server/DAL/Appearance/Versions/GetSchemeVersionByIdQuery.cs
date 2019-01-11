using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.Appearance.Versions;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Appearance.Versions
{
    public class GetSchemeVersionByIdQuery : QueryReader<int, SchemeVersionDTO>
    {
        public GetSchemeVersionByIdQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override void BindParameters(OracleCommand command, int parameters)
        {
            command.AddInputParameter("p1", parameters);
        }



        protected override string GetCommandText(int parameters)
        {
            return @"SELECT sv.scheme_version_id, sv.scheme_id, sv.is_publihed, sv.create_date, sv.create_user_id, sv.content, sv.description, s.scheme_name, u.name, sv.system_Id  
                     FROM v_SCHEME_VERSIONS sv 
                     join v_schemes s on sv.scheme_id = s.scheme_id 
                     join v_users u on sv.create_user_id = u.user_id 
                     where sv.scheme_version_id = :p1 ";

        }

        protected override SchemeVersionDTO GetResult(OracleDataReader reader, int parameters)
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
                                   SystemId = reader.GetValue<int>("system_Id"),
                                   Content = reader.GetValue<string>("content")
                               };

            }
            return null;
        }
    }
}