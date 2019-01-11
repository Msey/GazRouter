using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.DTO.Dictionaries;
using GazRouter.DTO.Dictionaries.PipelineEndType;
using GazRouter.DTO.ObjectModel.PipelineConns;
using GazRouter.DTO.ObjectModel.Pipelines;

namespace GazRouter.DataServices.BL
{
    public abstract class KmConnectionRule : RuleBase
    {
        private readonly Dictionary<Guid,PipelineDTO> _pipelines;
        private readonly List<PipelineConnDTO> _pipelineConns;
        private readonly PipelineEndType _pipelineEndType;

        protected KmConnectionRule(DictionaryRepositoryDTO dictionaries, Dictionary<Guid, PipelineDTO> pipelines, List<PipelineConnDTO> pipelineConns, PipelineEndType pipelineEndType)
            : base(dictionaries)
        {
            _pipelines = pipelines;
            _pipelineConns = pipelineConns;
            _pipelineEndType = pipelineEndType;
        }

        public override List<Guid> Validate()
        {
            var errors = new List<Guid>();

            foreach (var pipeline in _pipelines.Values)
            {
                var pipelineConns =
                    _pipelineConns.Where(
                        c => c.DestPipelineId.HasValue && c.PipelineId == pipeline.Id && c.EndTypeId == _pipelineEndType)
                        .ToList();
                foreach (var pipelineConn in pipelineConns)
                {
                    var pipelineDest = _pipelines[pipelineConn.DestPipelineId.Value];

                    if (pipelineConn.Kilometr.HasValue)
                    {
                        var km = pipelineConn.Kilometr;
                        var kmStartPipelineDest = pipelineDest.KilometerOfStartPoint;
                        var kmEndPipelineDest = pipelineDest.KilometerOfStartPoint + pipelineDest.Length;

                        if ((decimal) km >= (decimal) kmStartPipelineDest && (decimal) km <= (decimal) kmEndPipelineDest)
                        {
                        }

                        else
                        {
                            errors.Add(pipeline.Id);
                        }
                    }
                    else
                    {
                        errors.Add(pipeline.Id);
                    }
                }
            }
            return errors;
        } 
    }
}