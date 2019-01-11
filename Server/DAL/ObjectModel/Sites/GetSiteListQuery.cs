using System;
using System.Collections.Generic;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.ObjectModel.Sites;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.Sites
{
    public class GetSiteListQuery : QueryReader<GetSiteListParameterSet, List<SiteDTO>>
	{
		public GetSiteListQuery(ExecutionContext context)
			: base(context)
		{
		}

        protected override string GetCommandText(GetSiteListParameterSet parameters)
        {
            var sb = new StringBuilder(@"   SELECT      s.site_id, 
                                                        s.site_name, 
                                                        s.enterprise_id, 
                                                        s.status, 
                                                        n.entity_name AS full_name, 
                                                        n1.entity_name AS short_name, 
                                                        s.system_id, 
                                                        s.sort_order, 
                                                        sn.neighbour_site_id AS neighbour_site_id1, 
                                                        sn1.site_id AS neighbour_site_id2,
                                                        ds.dependant_site_id,
                                                        s.description,
                                                        s.bal_group_id,
                                                        bg.name AS bal_group_name
                                            FROM        rd.v_sites s
                                            LEFT JOIN   v_nm_all n ON s.site_id = n.entity_id
                                            LEFT JOIN   v_nm_short_all n1 ON s.site_id = n1.entity_id
                                            LEFT JOIN   v_bl_groups bg ON s.bal_group_id = bg.group_id
                                            LEFT JOIN   v_site_neighbours sn ON s.site_id = sn.site_id 
                                            LEFT JOIN   v_site_neighbours sn1 ON s.site_id = sn1.neighbour_site_id
                                            LEFT JOIN   v_input_dependant_sites ds ON ds.site_id = s.site_id
                                            WHERE       1 = 1");

            if (parameters != null)
            {
                if (parameters.SystemId.HasValue) sb.Append(" AND s.system_id = :systemId");
                if (parameters.EnterpriseId.HasValue) sb.Append(" AND s.enterprise_id = :enterpriseId");
                if (parameters.SiteId.HasValue) sb.Append(" AND s.site_id = :siteId");
            }
            
            sb.Append(" ORDER BY s.sort_order, s.site_name");

            return sb.ToString();
        }

        protected override void BindParameters(OracleCommand command, GetSiteListParameterSet parameters)
        {
            if (parameters == null) return;
            if (parameters.SystemId.HasValue)  command.AddInputParameter("systemId", parameters.SystemId.Value);
            if (parameters.EnterpriseId.HasValue) command.AddInputParameter("enterpriseId", parameters.EnterpriseId.Value);
            if (parameters.SiteId.HasValue) command.AddInputParameter("siteId", parameters.SiteId.Value);
        }

        protected override List<SiteDTO> GetResult(OracleDataReader reader, GetSiteListParameterSet parameters)
		{
			var result = new List<SiteDTO>();
            SiteDTO siteDto = null;
            while (reader.Read())
            {
              var siteId = reader.GetValue<Guid>("site_id");
                if (siteDto == null || siteDto.Id != siteId)
                {
                    siteDto = new SiteDTO
                    {
                        Id = siteId,
                        Name = reader.GetValue<string>("site_name"),
                        ParentId = reader.GetValue<Guid>("enterprise_id"),
                        Path = reader.GetValue<string>("full_name"),
                        ShortPath = reader.GetValue<string>("short_name"),
                        SystemId = reader.GetValue<int>("system_id"),
                        SortOrder = reader.GetValue<int>("sort_order"),
                        Status = reader.GetValue<EntityStatus?>("status"),
                        Description = reader.GetValue<string>("description"),
                        BalanceGroupId = reader.GetValue<int?>("bal_group_id"),
                        BalanceGroupName = reader.GetValue<string>("bal_group_name")
                    };
                    result.Add(siteDto);
                }
                var neighbourSiteId1 = reader.GetValue<Guid?>("neighbour_site_id1");
                if (neighbourSiteId1.HasValue && !siteDto.NeighbourSiteIdList.Contains(neighbourSiteId1.Value))
                {
                    siteDto.NeighbourSiteIdList.Add(neighbourSiteId1.Value);
                }
                var neighbourSiteId2 = reader.GetValue<Guid?>("neighbour_site_id2");
                if (neighbourSiteId2.HasValue && !siteDto.NeighbourSiteIdList.Contains(neighbourSiteId2.Value))
                {
                    siteDto.NeighbourSiteIdList.Add(neighbourSiteId2.Value);
                }

                var dependantSiteId = reader.GetValue<Guid?>("dependant_site_id");
                if (dependantSiteId.HasValue && !siteDto.DependantSiteIdList.Contains(dependantSiteId.Value))
                    siteDto.DependantSiteIdList.Add(dependantSiteId.Value);
            }
		    return result;
		}
	}
}