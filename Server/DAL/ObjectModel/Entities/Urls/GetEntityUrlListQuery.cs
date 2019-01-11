using System;
using System.Collections.Generic;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.Entities.Urls;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.Entities.Urls
{
    public class GetEntityUrlListQuery : QueryReader<Guid?, List<EntityUrlDTO>>
    {
        public GetEntityUrlListQuery(ExecutionContext context) : base(context)
        {
        }


        protected override void BindParameters(OracleCommand command, Guid? parameters)
        {
           command.AddInputParameter("entid", parameters);
        }

        protected override string GetCommandText(Guid? parameters)
        {

			var q = @"  SELECT      entity_ext_urls_id,
                                    entity_id,
                                    description,
                                    url
 
                        FROM        v_entity_ext_urls
                        WHERE       1=1";

            var sb = new StringBuilder(q);
            if (parameters.HasValue)
            {
                sb.Append(" AND entity_id = :entid");
            }

            return sb.ToString();
        }

        protected override List<EntityUrlDTO> GetResult(OracleDataReader reader, Guid? parameters)
        {
            var result = new List<EntityUrlDTO>();
            while (reader.Read())
            {
                result.Add(
                    new EntityUrlDTO
                    {
                        UrlId = reader.GetValue<int>("entity_ext_urls_id"),
                        EntityId = reader.GetValue<Guid>("entity_id"),
						Description = reader.GetValue<string>("description"),
                        Url = reader.GetValue<string>("url")
                    });
            }

            return result;
        }
    }
}
