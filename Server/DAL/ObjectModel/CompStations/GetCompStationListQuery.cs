using System;
using System.Collections.Generic;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.CompStations;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.CompStations
{
    public class GetCompStationListQuery : QueryReader<GetCompStationListParameterSet, List<CompStationDTO>>
    {
        public GetCompStationListQuery(ExecutionContext context) : base(context)
        {
        }

        protected override string GetCommandText(GetCompStationListParameterSet parameters)
        {
            var q = @"  SELECT      s.comp_station_id, 
                                    s.comp_station_name, 
                                    s.site_id, 
                                    s.region_id, 
                                    n.entity_name AS full_name, 
                                    n1.entity_name AS short_name,
                                    s.system_id, 
                                    s.sort_order,
                                    s.description,
                                    s.use_in_balance
                        FROM        v_comp_stations s
                        LEFT JOIN   v_nm_all n ON s.comp_station_id = n.entity_id
                        LEFT JOIN   v_nm_short_all n1 ON s.comp_station_id = n1.entity_id
                        WHERE 1=1";

            var sb = new StringBuilder(q);

            if (parameters != null)
            {
                if (parameters.SystemId.HasValue) sb.Append(" AND s.system_id  = :systemid");
                if (parameters.SiteId.HasValue) sb.Append(" AND s.site_id  = :siteid");

                if (parameters.HideVirtual) sb.Append(" AND s.is_virtual = 0");

                if (parameters.UseInBalance.HasValue) sb.Append(" AND s.use_in_balance = :uib");
            }

            sb.Append(@" ORDER BY s.sort_order, s.comp_station_name");
            return sb.ToString();
        }

        protected override void BindParameters(OracleCommand command, GetCompStationListParameterSet parameters)
        {
            if (parameters == null) return;
            if (parameters.SystemId.HasValue) command.AddInputParameter("systemid", parameters.SystemId);
            if (parameters.SiteId.HasValue) command.AddInputParameter("siteid", parameters.SiteId);
            if (parameters.UseInBalance.HasValue) command.AddInputParameter("uib", parameters.UseInBalance);
        }

        protected override List<CompStationDTO> GetResult(OracleDataReader reader,
            GetCompStationListParameterSet parameters)
        {
            var result = new List<CompStationDTO>();
            while (reader.Read())
            {
                result.Add(new CompStationDTO
                {
                    Id = reader.GetValue<Guid>("comp_station_id"),
                    Name = reader.GetValue<string>("comp_station_name"),
                    ParentId = reader.GetValue<Guid>("site_id"),
                    RegionId = reader.GetValue<int>("region_id"),
                    Path = reader.GetValue<string>("full_name"),
                    ShortPath = reader.GetValue<string>("short_name"),
                    SystemId = reader.GetValue<int>("system_id"),
                    SortOrder = reader.GetValue<int>("sort_order"),
                    UseInBalance = reader.GetValue<bool>("use_in_balance"),
                    Description = reader.GetValue<string>("description")
                });
            }
            return result;
        }
    }
}