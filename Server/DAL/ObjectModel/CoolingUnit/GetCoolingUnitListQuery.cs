using System;
using System.Collections.Generic;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.CoolingUnit;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.CoolingUnit
{
    public class GetCoolingUnitListQuery : QueryReader<GetCoolingUnitListParameterSet, List<CoolingUnitDTO>>
    {
		public GetCoolingUnitListQuery(ExecutionContext context)
            : base(context)
        {}

        protected override string GetCommandText(GetCoolingUnitListParameterSet parameters)
        {
            var result = new StringBuilder(@"   SELECT      
                                                        cs.cooling_unit_id,
                                                        cs.cooling_unit_name, 
                                                        cs.cooling_station_id,
                                                        cs.sort_order,
                                                        cs.entity_type_id,
                                                        cs.is_virtual,
                                                        cs.system_id,
                                                        cs.cooling_unit_type_id,
                                                        n.entity_name as path,
                                                        n1.entity_name as short,
                                                        cs.description
                                    FROM        v_cooling_units cs
                                    LEFT JOIN   v_nm_all n ON cs.cooling_unit_id = n.entity_id
                                    LEFT JOIN   v_nm_short_all n1 ON cs.cooling_unit_id = n1.entity_id
                                    WHERE 1 = 1");
            if (parameters != null)
            {
                if (parameters.SiteId.HasValue)
                    result.Append(" AND rd.P_ENTITY.GetSiteID(cs.cooling_unit_id) = :siteId");
            }

            result.Append(" order by cs.sort_order, cs.cooling_unit_name");
            return result.ToString();
        }
        protected override void BindParameters(OracleCommand command, GetCoolingUnitListParameterSet parameters)
        {
            if (parameters != null)
            {
                if (parameters.SiteId.HasValue)
                    command.AddInputParameter("siteId", parameters.SiteId.Value);
            }
        }

        protected override List<CoolingUnitDTO> GetResult(OracleDataReader reader, GetCoolingUnitListParameterSet parameters)
        {
			var result = new List<CoolingUnitDTO>();
            while (reader.Read())
            {
				result.Add(new CoolingUnitDTO
                {
					Id = reader.GetValue<Guid>("cooling_unit_id"),
					Name = reader.GetValue<string>("cooling_unit_name"),
                    ParentId = reader.GetValue<Guid>("cooling_station_id"),
                    SortOrder = reader.GetValue<int>("sort_order"),
                    IsVirtual = reader.GetValue<bool>("is_virtual"),
                    SystemId = reader.GetValue<int>("system_id"),
					CoolingUnitTypeId = reader.GetValue<int>("cooling_unit_type_id"),
					Path = reader.GetValue<string>("path"),
					ShortPath = reader.GetValue<string>("short"),
                    Description = reader.GetValue<string>("description")
                });
            }
            return result;
        }
    }
}