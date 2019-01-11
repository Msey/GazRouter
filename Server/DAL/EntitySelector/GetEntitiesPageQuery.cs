using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PipelineTypes;
using GazRouter.DTO.EntitySelector;
using GazRouter.DTO.ObjectModel;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.EntitySelector
{
    public class GetEntitiesPageQuery : QueryReader<GetEntitesPageParameterSet, EntitiesPageDTO>
    {
        public GetEntitiesPageQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override string GetCommandText(GetEntitesPageParameterSet parameters)
        {
            const string queryTemplate = @"

                SELECT      e.entity_id,
                            e.entity_name,
                            e.name AS entity_name_lat,
                            e.entity_type_id,
                            e.hidden,
                            n1.entity_name AS short_path,
                            COUNT(*) OVER() AS cnt,
                            ROW_NUMBER() OVER(ORDER BY n1.entity_name ASC) AS rn,
                            s.site_id, 
                            s.site_name

                FROM        v_entities e
                LEFT JOIN   v_nm_short_all n1 ON e.entity_id = n1.entity_id
                LEFT JOIN   v_pipeLines pipe on e.entity_id = pipe.pipeline_id
                LEFT JOIN   v_segments_by_sites pss ON pss.pipeline_id = pipe.pipeline_id AND pss.site_id = :siteid 
                LEFT JOIN   v_sites s ON s.site_id = NVL(rd.P_ENTITY.GetSiteID(e.entity_id), pss.site_id)
                
                WHERE 1=1
                    AND e.hidden = 0";
      
            
            var sb = new StringBuilder(queryTemplate);
            
            sb.Append(GetFilterConditionString(parameters.NamePart));

            sb.Append(GetEntityTypeConditionString(parameters.EntityTypes));

            if (parameters.SiteId.HasValue)
                sb.Append(" AND s.site_id = :siteid");

            if (parameters.EnterpriseId.HasValue)
                sb.Append(" AND s.enterprise_id = :enterpriseid");

            if (parameters.PipeLineType.HasValue)
                sb.Append(" AND pipe.pipeline_type_id = :pipelinetype");
            

            return $"SELECT * FROM ({sb}) WHERE rn BETWEEN :pageNumber * :pageSize + 1 AND ((:pageNumber + 1) * :pageSize)";
        }

        private static string GetEntityTypeConditionString(List<EntityType> types)
        {
            if (types.Count == 0) return string.Empty;
            return $"AND e.entity_type_id IN ({string.Join(",", types.Select(et => et.ToString("d")))})";
        }

        private static string GetFilterConditionString(string namePart)
        {
            if (string.IsNullOrEmpty(namePart)) return string.Empty;
            
            const string atr = "n1.entity_name";
            var sb = new StringBuilder();
            foreach (var s in namePart.Trim().Split(' '))
                sb.Append($" AND UPPER({atr}) LIKE('%{s.ToUpper()}%')");
            
            return sb.ToString();
        }
        
        protected override EntitiesPageDTO GetResult(OracleDataReader reader, GetEntitesPageParameterSet parameters)
        {
            int count = 0;
            var entities = new Collection<CommonEntityWithSiteDTO>();
            while (reader.Read())
            {
                entities.Add(new CommonEntityWithSiteDTO
                {
                    Id = reader.GetValue<Guid>("entity_id"),
                    Name = reader.GetValue<string>("entity_name"),
                    ShortPath = reader.GetValue<string>("short_path"),
                    EntityType = reader.GetValue<EntityType>("entity_type_id"),
                    SiteId = reader.GetValue<Guid?>("site_id"),
                    SiteName = reader.GetValue<string>("site_name")
                });
                count = reader.GetValue<int>("cnt");
            }
            return new EntitiesPageDTO
            {
                TotalCount = count,
                Entities = entities
            };
        }

        protected override void BindParameters(OracleCommand command, GetEntitesPageParameterSet parameters)
        {
            command.AddInputParameter("pageNumber", parameters.PageNumber);
            command.AddInputParameter("pageSize", parameters.PageSize);

            if (parameters.PipeLineType.HasValue)
                command.AddInputParameter("pipelinetype", parameters.PipeLineType);

            command.AddInputParameter("siteid", parameters.SiteId);

            if (parameters.EnterpriseId.HasValue)
                command.AddInputParameter("enterpriseid", parameters.EnterpriseId);
        }
    }
}
