using System;
using System.Collections.Generic;
using GazRouter.Controls.Tree;
using GazRouter.DTO.ObjectModel.Boilers;
using GazRouter.DTO.ObjectModel.Inconsistency;
using GazRouter.DTO.ObjectModel.Pipelines;
using GazRouter.DTO.ObjectModel.PowerUnits;
using GazRouter.DTO.ObjectModel.Valves;

namespace GazRouter.ObjectModel.Model.Pipelines
{
    public class ValidatedTreeViewModelPipeline : TreeViewModelPipeline
    {
        public Dictionary<Guid, List<InconsistencyDTO>> Inconsistencies { get; set; }

        protected override EntityNode CreatePipelineNodeInternal(PipelineDTO pipeline)
        {
            List<InconsistencyDTO> errors;
            if (!Inconsistencies.TryGetValue(pipeline.Id, out errors))
            {
                errors = new List<InconsistencyDTO>();
            }
            return new ValidatedEntityNode(pipeline, errors)
            {
                TextTooltip = NodeTooptip(pipeline.BeginEntityId.HasValue, pipeline.EndEntityId.HasValue),
                SortOrder = pipeline.SortOrder
            };
        }

        protected override EntityNode CreatePowerUnitNode(PowerUnitDTO powerUnit)
        {
            List<InconsistencyDTO> errors;
            if (!Inconsistencies.TryGetValue(powerUnit.Id, out errors))
            {
                errors = new List<InconsistencyDTO>();
            }
            var node = new ValidatedEntityNode(powerUnit, errors)
            {
                SortOrder = powerUnit.SortOrder
            };
            return node;
        }

        protected override EntityNode CreateBoilerNode(BoilerDTO boiler)
        {
            List<InconsistencyDTO> errors;
            if (!Inconsistencies.TryGetValue(boiler.Id, out errors))
            {
                errors = new List<InconsistencyDTO>();
            }
            var node = new ValidatedEntityNode(boiler, errors)
            {
                SortOrder = boiler.SortOrder
            };
            return node;
        }

        protected override EntityNode CreateValveNode(ValveDTO linarValve)
        {
            List<InconsistencyDTO> errors;
            if (!Inconsistencies.TryGetValue(linarValve.Id, out errors))
            {
                errors = new List<InconsistencyDTO>();
            }
            var node = new ValidatedEntityNode(linarValve, errors)
            {
                SortOrder = linarValve.SortOrder
            };
            return node;
        }
        
    }
}