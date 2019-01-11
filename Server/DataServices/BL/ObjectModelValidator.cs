using System.Collections.Generic;
using System.Linq;
using GazRouter.DataServices.Dictionaries;
using GazRouter.DAL.Core;
using GazRouter.DAL.ObjectModel.Inconsistency;
using GazRouter.DAL.ObjectModel.PipelineConns;
using GazRouter.DAL.ObjectModel.Pipelines;
using GazRouter.DAL.ObjectModel.Segment.Diameter;
using GazRouter.DAL.ObjectModel.Segment.PipelineGroup;
using GazRouter.DAL.ObjectModel.Segment.Pressure;
using GazRouter.DAL.ObjectModel.Segment.Site;
using GazRouter.DAL.ObjectModel.Valves;
using GazRouter.DTO.Dictionaries.PipelineEndType;
using GazRouter.DTO.ObjectModel.Inconsistency;
using GazRouter.DTO.ObjectModel.PipelineConns;

namespace GazRouter.DataServices.BL
{
    public static class ObjectModelValidator
    {
        public static void Validate(ExecutionContextReal context)
        {
            var pipelines = new GetPipelineListQuery(context).Execute(null).ToDictionary(k => k.Id, v => v);
            var valves = new GetValveListQuery(context).Execute(null);
            var diameterSegments = new GetDiameterSegmentListQuery(context).Execute(null);
            var siteSegments = new GetSiteSegmentListQuery(context).Execute(null);
            var pressureSegments = new GetPressureSegmentListQuery(context).Execute(null);
            var pipelineGroupSegments = new GetGroupSegmentListQuery(context).Execute(null);
            var pipelineConns = new GetPipelineConnListQuery(context).Execute(null);

            var doublePipeConns = new List<PipelineConnDTO>();
            foreach (var conn1 in pipelineConns)
            {
                foreach (var conn2 in pipelineConns)
                {
                    if (conn1.PipelineId == conn2.DestPipelineId && conn1.DestPipelineId == conn2.PipelineId)
                    {
                        if (doublePipeConns.All(c => c.PipelineId != conn1.DestPipelineId.Value))
                        {
                            doublePipeConns.Add(conn1);
                        }
                    }
                }
            }

            var validator = new RuleProcessor();
            validator.Rules.Add(new ValveDiameterRule(DictionaryRepository.Dictionaries, valves, pipelines,
                diameterSegments));
            validator.Rules.Add(new DiameterSegmentRule(DictionaryRepository.Dictionaries, pipelines, diameterSegments));
            validator.Rules.Add(new PressureSegmentRule(DictionaryRepository.Dictionaries, pipelines, pressureSegments));
            validator.Rules.Add(new SiteSegmentRule(DictionaryRepository.Dictionaries, pipelines, siteSegments));
            validator.Rules.Add(new GroupSegmentRule(DictionaryRepository.Dictionaries, pipelines, pipelineGroupSegments));
            validator.Rules.Add(new KmStartConnectionRule(DictionaryRepository.Dictionaries, pipelines, pipelineConns,
                PipelineEndType.StartType));
            validator.Rules.Add(new KmEndConnectionRule(DictionaryRepository.Dictionaries, pipelines, pipelineConns,
                PipelineEndType.EndType));
            validator.Rules.Add(new ValveKmRule(DictionaryRepository.Dictionaries, valves, pipelines));

            validator.Rules.Add(new DoubleStartConnectionRule(DictionaryRepository.Dictionaries, doublePipeConns));
            validator.Rules.Add(new DoubleEndConnectionRule(DictionaryRepository.Dictionaries, doublePipeConns));

            var errors = validator.Run();

            new DeleteAllInconsistencyCommand(context).Execute();

            foreach (var error in errors)
            {
                foreach (var entityId in error.Value)
                {
                    new AddInconsistencyCommand(context).Execute(new AddInconsistencyParameterSet
                    {
                        EntityId = entityId,
                        InconsistencyTypeId = error.Key
                    });
                }
            }
        }
    }
}