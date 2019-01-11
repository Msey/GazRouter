using System;
using System.Collections.Generic;
using System.ComponentModel;
using GazRouter.DAL.Core;
using GazRouter.DTO.Bindings.EntityBindings;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Bindings.EntityBindings
{
	public class GetEntityBindingsPagedListQuery : QueryReader<GetEntityBindingsPageParameterSet, EntityBindingsPageDTO>
	{
		public GetEntityBindingsPagedListQuery(ExecutionContext context)
			: base(context)
		{
		}

		protected override string GetCommandText(GetEntityBindingsPageParameterSet parameters)
		{
			const string queryTemplate = @"SELECT entity_id,
        entity_name,
		entity_name_lat,
        entity_type_id,
		path,
	
		ext_entity_id,
		Entity_binding_id,
        is_active,
        cnt
  FROM  (
         SELECT  e.entity_id,
                 e.entity_name,
				 e.name entity_name_lat,
                 e.entity_type_id,
				 n.Entity_name path,
				b.Entity_binding_id,
				
				b.ext_entity_id,
                b.is_active,
                 count(*) over() cnt,
                 row_number() over(order by {1}) rn
           FROM  v_entities e
			left join V_NM_SHORT_ALL n on e.entity_id = N.ENTITY_ID
			Left join V_ENTITY_BINDINGS b on b.Entity_id= e.entity_id AND b.Source_id = :sourceId
           WHERE (1=1)
           {0}
           AND e.entity_type_id = :entityTypeId
			{2}
        )
  WHERE rn BETWEEN :pageNumber * :pageSize + 1 AND ((:pageNumber + 1) * :pageSize)  ";

			var commandText = String.Format(queryTemplate, GetRegExpCondition(parameters.NamePart), string.Format(" {0} {1} ", GetSortColumnName(parameters.SortBy), parameters.SortOrder.ToString("g")), GetShowAllConditionString(parameters.ShowAll));
			return commandText;
		}

	    private static string GetRegExpCondition(string namePart)
	    {
            return !string.IsNullOrEmpty(namePart) ? " AND regexp_like(e.entity_name,:namePart,'i') " : string.Empty;
	    }

	    private static string GetShowAllConditionString(bool showAll)
		{
			return showAll ? string.Empty : "AND b.Source_id is not null";
		}
		 
		private static string GetSortColumnName(SortBy column)
		{
			switch (column)
			{
				case SortBy.Name:
					return "e.entity_name";
				case SortBy.Type:
					return "e.entity_type_id";
				default:
					throw new InvalidEnumArgumentException();
			}
		}

        protected override EntityBindingsPageDTO GetResult(OracleDataReader reader, GetEntityBindingsPageParameterSet parameters)
		{
            
			double count = 0;
			var entities = new List<BindingDTO>();
			while (reader.Read())
			{
				entities.Add(new BindingDTO
				{
					Id = reader.GetValue<Guid>("entity_binding_id"),
					Name = reader.GetValue<string>("entity_name"),
					Path = reader.GetValue<string>("path"),
					EntityId = reader.GetValue<Guid>("entity_id"),
					IsActive = reader.GetValue<bool>("is_active"),
					ExtEntityId = reader.GetValue<string>("ext_entity_id"),
				});
				count = reader.GetValue<int>("cnt");
			}

			return new EntityBindingsPageDTO
			{
				TotalCount = (int)Math.Floor(count),
				Entities = entities
			};
		}

		protected override void BindParameters(OracleCommand command, GetEntityBindingsPageParameterSet parameters)
		{
            
			command.AddInputParameter("pageNumber", parameters.PageNumber);
            command.AddInputParameter("pageSize", parameters.PageSize);
            command.AddInputParameter("sourceId", parameters.SourceId);
            command.AddInputParameter("entityTypeId", parameters.EntityType);
            if (!string.IsNullOrEmpty(parameters.NamePart))
				command.AddInputParameter("namePart", string.Format("{0}", parameters.NamePart));
		}
	}
}