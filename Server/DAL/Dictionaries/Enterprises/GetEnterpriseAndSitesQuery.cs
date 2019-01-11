using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.ObjectModel;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Dictionaries.Enterprises
{
	public class GetEnterpriseAndSitesQuery : QueryReader<Guid, List<CommonEntityDTO>>
	{
		public GetEnterpriseAndSitesQuery(ExecutionContext context) : base(context)
		{
		}

        protected override void BindParameters(OracleCommand command, Guid parameters)
        {
            command.AddInputParameter("id", parameters);
        }

		protected override string GetCommandText(Guid parameters)
		{
            return @"select * from (
                    select enterprise_id, enterprise_id entity_id, entity_type_id, enterprise_name ""name"" ,Sort_order from v_enterprises 
                    union 
                    select enterprise_id, site_id entity_id, entity_type_id, site_name ""name"" ,Sort_order from v_sites 
                    order by entity_type_id, Sort_order 
                    ) res
                    where res.enterprise_id = :id";
		}

        protected override List<CommonEntityDTO> GetResult(OracleDataReader reader, Guid parameters)
		{
			var result = new List<CommonEntityDTO>();
			while (reader.Read())
			{
				result.Add(new CommonEntityDTO
				{
					Id = reader.GetValue<Guid>("entity_id"),
					Name = reader.GetValue<string>("name"),
					EntityType = reader.GetValue<EntityType>("entity_type_id"),
					SortOrder = reader.GetValue<int>("Sort_order"),
				});

			}
			return result;
		}
	}
}