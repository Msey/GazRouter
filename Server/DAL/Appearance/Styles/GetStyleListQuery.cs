using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Appearance.Styles;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Appearance.Styles
{
    public class GetStyleListQuery : QueryReader<int, List<StyleDTO>>
    {
        public GetStyleListQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override void BindParameters(OracleCommand command, int parameters)
        {
            command.AddInputParameter("versionId", parameters);
        }

        protected override string GetCommandText(int schemeVersionId)
        {
            return @"SELECT entity_id,scheme_version_id,color,the_size FROM rd.V_STYLES where scheme_version_id = :versionId";
        }

        protected override List<StyleDTO> GetResult(OracleDataReader reader, int parameters)
        {
            var result = new List<StyleDTO>();
            while (reader.Read())
            {
                result.Add(new StyleDTO
                               {
                                   EntityId = reader.GetValue<Guid>("entity_id"),
                                   Color = reader.GetValue<int>("color"),
                                   Size = reader.GetValue<int>("the_size"),
                                   SchemeVersionId = reader.GetValue<int>("scheme_version_id"),
                               });

            }
            return result;
        }
    }
}