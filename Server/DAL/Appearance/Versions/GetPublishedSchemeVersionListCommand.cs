using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Appearance.Versions;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Appearance.Versions
{
    public class GetPublishedSchemeVersionListQuery : QueryReader<List<SchemeVersionItemDTO>>
    {
        public GetPublishedSchemeVersionListQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override string GetCommandText()
        {
            return @"   SELECT      sv.scheme_version_id, 
                                    sv.is_publihed, 
                                    sv.create_date, 
                                    s.scheme_name, 
                                    u.name, 
                                    sys.system_id,
                                    sys.system_name
                        
                        FROM        rd.v_scheme_versions sv
                        JOIN        rd.v_schemes s ON sv.scheme_id = s.scheme_id 
                        JOIN        v_systems sys ON s.system_id = sys.system_id 
                        JOIN        rd.v_users u ON sv.create_user_id = u.user_id
                        WHERE       (sv.scheme_id, sv.create_date) IN
                        (
                            SELECT      t1.scheme_id, max(t1.create_date)
                            FROM        rd.v_scheme_versions t1
                            WHERE       t1.is_publihed = 1
                            GROUP BY    t1.scheme_id
                        )";
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
                    SystemId = reader.GetValue<int>("system_id"),
                    SystemName = reader.GetValue<string>("system_name"),
                    CreateDate = reader.GetValue<DateTime>("create_date"),
                    CreatorName = reader.GetValue<string>("name"),
                });

            }
            return result;
        }
    }
}