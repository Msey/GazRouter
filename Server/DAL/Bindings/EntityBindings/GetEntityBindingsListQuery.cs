using System;
using System.Collections.Generic;
using System.ComponentModel;
using GazRouter.DAL.Core;
using GazRouter.DTO.Bindings.EntityBindings;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PipelineTypes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Bindings.EntityBindings
{
    public class GetEntityBindingsListQuery : QueryReader<GetEntityBindingsPageParameterSet, List<BindingDTO>>
    {
        public GetEntityBindingsListQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override string GetCommandText(GetEntityBindingsPageParameterSet parameters)
        {
            const string queryTemplate = @" SELECT      e.entity_id,
                                                        e.entity_name,
                                                        e.name entity_name_lat,
                                                        e.entity_type_id,
                                                        n.Entity_name path,
                                                        b.Entity_binding_id,
                                                        b.ext_entity_id,
                                                        b.is_active
                                            FROM        v_entities e
			                                LEFT JOIN   v_nm_short_all n ON e.entity_id = n.entity_id
                                            LEFT JOIN   v_entity_bindings b ON b.entity_id= e.entity_id AND b.source_id = :sourceid
                                            {4}
                                            WHERE       1=1
                                            {0}
                                            AND e.entity_type_id = :entityTypeId
			                                {2}
                                            {3}
                                            ORDER BY {1}";

            var commandText = String.Format(queryTemplate, GetRegExpCondition(parameters.NamePart), string.Format(" {0} {1} ", GetSortColumnName(parameters.SortBy), parameters.SortOrder.ToString("g")), GetShowAllConditionString(parameters.ShowAll), GetPipeLineTypeConditionStrig(parameters.PipelineTypeId), GetPipeLineTypeJoinStrig(parameters.PipelineTypeId));
            return commandText;
        }

        private static string GetPipeLineTypeConditionStrig(PipelineType? entityTypes)
        {
            return entityTypes.HasValue ? " AND pp.pipeline_type_id = :pipelinetype " : string.Empty;
        }
        private static string GetPipeLineTypeJoinStrig(PipelineType? entityTypes)
        {
            return entityTypes.HasValue ? " left join V_PipeLines pp on e.entity_id=pp.pipeLine_id " : string.Empty;
        }


        private static string GetRegExpCondition(string namePart)
        {
            return !string.IsNullOrEmpty(namePart) ? " AND regexp_like(e.entity_name,:namePart,'i') " : string.Empty;
        }

        private static string GetShowAllConditionString(bool showAll)
        {
            return showAll ? string.Empty : "AND (b.Source_id is not null)";
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


        protected override List<BindingDTO> GetResult(OracleDataReader reader, GetEntityBindingsPageParameterSet parameters)
        {
            
            var entities = new List<BindingDTO>();
            while (reader.Read())
            {
                var temp = reader.GetValue<bool?>("is_active");
                entities.Add(new BindingDTO
                             {
                                 Id = reader.GetValue<Guid>("entity_binding_id"),
                                 Name = reader.GetValue<string>("entity_name"),
                                 Path = reader.GetValue<string>("path"),
                                 EntityId = reader.GetValue<Guid>("entity_id"),
                                 EntityType = (EntityType?)reader.GetValue<int?>("entity_type_id"),
                                 ExtEntityId = reader.GetValue<string>("ext_entity_id"),
                                 IsActive = temp.HasValue && temp.Value
                             });
            }
            return entities;
        
        }

        protected override void BindParameters(OracleCommand command, GetEntityBindingsPageParameterSet parameters)
        {
            
            command.AddInputParameter("sourceId", parameters.SourceId);
            command.AddInputParameter("entityTypeId", parameters.EntityType);
            if (parameters.PipelineTypeId.HasValue)
            {
                command.AddInputParameter("pipelinetype", (int)parameters.PipelineTypeId.Value);
            }
            if (!string.IsNullOrEmpty(parameters.NamePart))
                command.AddInputParameter("namePart", string.Format("{0}", parameters.NamePart));
        }
    }
}