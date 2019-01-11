using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.Entities;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.Entities
{
    public class GetEntityListQuery : QueryReader<GetEntityListParameterSet, List<CommonEntityDTO>>
    {
		public GetEntityListQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override string GetCommandText(GetEntityListParameterSet parameters)
        {
            var q = @"  SELECT      e.entity_id,
                                    e.entity_name,
                                    e.entity_type_id,
                                    n.entity_name AS path,
                                    n1.entity_name AS short_path,
                                    e.is_input_off
                        FROM        v_entities e
                        LEFT JOIN   v_nm_all n ON e.entity_id = n.entity_id
                        LEFT JOIN   v_nm_short_all n1 ON e.entity_id = n1.entity_id
                        LEFT JOIN   v_sites s on s.site_id = rd.P_ENTITY.GetSiteID(e.entity_id)
                        WHERE       1=1";

            var sb = new StringBuilder(q);

            if (parameters != null)
            {
                if (!parameters.ShowHidden) sb.Append(" AND e.hidden = 0");
                if (!parameters.ShowDeleted) sb.Append(" AND e.status = 0");
                if (!parameters.ShowVirtual) sb.Append(" AND e.is_virtual = 0");

                if (parameters.IsInputOff.HasValue) sb.Append(" AND e.is_input_off = :isinpoff");

                if (parameters.EntityType.HasValue) sb.Append(" AND e.entity_type_id = :enttype");
                
                if (parameters.EnterpriseId.HasValue) sb.Append(" AND s.enterprise_id = :enterpriseId");
                if (parameters.SiteId.HasValue) sb.Append(" AND s.site_id = :siteId");

                if (parameters.EntityIdList != null && parameters.EntityIdList.Any())
                {
                    sb.Append(" AND e.entity_id IN ");
                    sb.Append(CreateInClause(parameters.EntityIdList.Count));
                }

                if (!string.IsNullOrEmpty(parameters.NamePart))
                {
                    sb.Append(parameters.SearchPath
                        ? " AND regexp_like(n1.entity_name, :namePart,'i')"
                        : " AND regexp_like(e.entity_name, :namePart,'i')");
                }
            }

            return sb.ToString();
        }
        
        protected override void BindParameters(OracleCommand command, GetEntityListParameterSet parameters)
        {
            if (parameters == null) return;

            if (parameters.IsInputOff.HasValue)
                command.AddInputParameter("isinpoff", parameters.IsInputOff.Value);

            if (parameters.EntityType.HasValue)
                command.AddInputParameter("enttype", parameters.EntityType);

            if (parameters.EnterpriseId.HasValue)
                command.AddInputParameter("enterpriseId", parameters.EnterpriseId);

            if (parameters.SiteId.HasValue)
                command.AddInputParameter("siteId", parameters.SiteId);

            var i = 0;
            foreach (var parameter in parameters.EntityIdList)
            {
                command.AddInputParameter(string.Format(":p{0}", i), parameter);
                i++;
            }

            if (!string.IsNullOrEmpty(parameters.NamePart))
                command.AddInputParameter("namePart", parameters.NamePart);
        }

        protected override List<CommonEntityDTO> GetResult(OracleDataReader reader, GetEntityListParameterSet parameters)
        {
            var entities = new List<CommonEntityDTO>();
            while (reader.Read())
            {
                entities.Add(new CommonEntityDTO
                {
                    Id = reader.GetValue<Guid>("entity_id"),
                    Name = reader.GetValue<string>("entity_name"),
                    Path = reader.GetValue<string>("path"),
                    ShortPath = reader.GetValue<string>("short_path"),
                    EntityType = reader.GetValue<EntityType>("entity_type_id")
                });
            }

            return entities;
        }

        
    }
}