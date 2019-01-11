using System;
using System.Collections.Generic;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.GasCostItemGroups;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.GasCosts;
using GazRouter.DTO.ObjectModel;
using Oracle.ManagedDataAccess.Client;
using Utils.Extensions;

namespace GazRouter.DAL.GasCosts
{
    public class GetGasCostListQuery : QueryReader<GetGasCostListParameterSet, List<GasCostDTO>>
    {
        public GetGasCostListQuery(ExecutionContext context) : base(context)
        {
        }
        protected override string GetCommandText(GetGasCostListParameterSet parameters)
        {
            var sb = new StringBuilder(
                @"  SELECT      ac.aux_cost_id, 
                                ac.aux_item_id,
                                i.aux_item_group_id, 
                                ac.aux_date,
                                ac.aux_cost_import_id, 
                                ac.entity_id, 
                                ac.calculated_volume, 
                                ac.measured_volume, 
                                ac.raw_data, 
                                ac.series_id,
                                ac.target_id,
                                e.entity_name, 
                                e.entity_type_id, 
                                s.entity_name AS entity_short_path, 
                                ac.site_id, 
                                ac.change_user_name, 
                                ac.change_date, 
                                ac.change_user_site_name,
                                NVL(e.bal_group_id, site.bal_group_id) AS bal_group_id
                    FROM        v_aux_costs ac
                    INNER JOIN  v_aux_items i ON i.aux_item_id = ac.aux_item_id
                    INNER JOIN  v_sites site ON site.site_id = ac.site_id       
                    LEFT JOIN   v_entities e ON ac.entity_id = e.entity_id
			        LEFT JOIN   v_nm_short_all s ON ac.entity_id = s.entity_id 
                    WHERE       1=1");

            if (parameters.Target != Target.None)
                sb.Append(" AND ac.target_id = :targetId");

            if (parameters.SiteId.HasValue)
                sb.Append(" AND ac.site_id = :siteId");

            if (parameters.SystemId.HasValue)
                sb.Append(" AND site.system_id = :systemId");

            if (parameters.StartDate.HasValue && parameters.EndDate.HasValue)
                sb.Append(@" and AUX_DATE between :begin and :end");

            if (parameters.SeriesId.HasValue)
                sb.Append(" AND series_id = :serieId");

            if (parameters.BalanceGroupId.HasValue)
                sb.Append(" AND NVL(e.bal_group_id, site.bal_group_id) = :groupId");

            return sb.ToString() ;
        }
        protected override void BindParameters(OracleCommand command, GetGasCostListParameterSet parameters)
        {
            if (parameters.Target != Target.None)
                command.AddInputParameter("targetId", parameters.Target);

            if (parameters.SiteId.HasValue)
                command.AddInputParameter("siteId", parameters.SiteId.Value);

            if (parameters.SystemId.HasValue)
                command.AddInputParameter("systemId", parameters.SystemId.Value);

            if (parameters.StartDate.HasValue && parameters.EndDate.HasValue)
            {
                command.AddInputParameter("begin", parameters.StartDate.Value);
                command.AddInputParameter("end", parameters.EndDate.Value);
            }

            if (parameters.SeriesId.HasValue)
                command.AddInputParameter("serieId", parameters.SeriesId);

            if (parameters.BalanceGroupId.HasValue)
                command.AddInputParameter("groupId", parameters.BalanceGroupId);
        }
        protected override List<GasCostDTO> GetResult(OracleDataReader reader, GetGasCostListParameterSet parameters)
        {
            var result = new List<GasCostDTO>();
            while (reader.Read())
            {
                result.Add(new GasCostDTO
                {
                    Id = reader.GetValue<int>("aux_cost_id"),
                    CostType = (CostType)reader.GetValue<int>("aux_item_id"),
                    Date = reader.GetValue<DateTime>("aux_date"),
                    ImportId = reader.GetValue<int>("aux_cost_import_id"),
                    Entity = new CommonEntityDTO
                    {
                        Id = reader.GetValue<Guid>("entity_id"),
                        EntityType = (EntityType)reader.GetValue<int>("entity_type_id"),
                        Name = reader.GetValue<string>("entity_name"),
                        ShortPath = reader.GetValue<string>("entity_short_path"),
                    },
                    CalculatedVolume = reader.GetValue<double?>("calculated_volume"),
                    MeasuredVolume = reader.GetValue<double?>("measured_volume"),
                    InputString = reader.GetValue<string>("raw_data"),
                    Target = (Target)reader.GetValue<int>("target_id"),
                    SiteId = reader.GetValue<Guid>("site_id"),
                    ChangeUserName = reader.GetValue<string>("change_user_name"),
                    ChangeUserSiteName = reader.GetValue<string>("change_user_site_name"),
                    ChangeDate = reader.GetValue<DateTime?>("change_date"),
                    SeriesId = reader.GetValue<int>("series_id"),
                    BalanceGroupId = reader.GetValue<int>("bal_group_id"),
                    ItemGroup = reader.GetValue<GasCostItemGroup>("aux_item_group_id")
                });
            }
            return result;
        }
    }
}