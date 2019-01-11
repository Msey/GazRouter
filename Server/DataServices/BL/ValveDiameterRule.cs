using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.DTO.Dictionaries;
using GazRouter.DTO.Dictionaries.InconsistencyTypes;
using GazRouter.DTO.ObjectModel.Pipelines;
using GazRouter.DTO.ObjectModel.Segment;
using GazRouter.DTO.ObjectModel.Valves;

namespace GazRouter.DataServices.BL
{
    /// <summary>
    ///     Все краны, установленные на газопровод, должны быть такого же диаметра как и сегмент по диаметру, на который они
    ///     попадают
    /// </summary>
    public class ValveDiameterRule : RuleBase
    {
        private readonly List<ValveDTO> _valves;
        private readonly Dictionary<Guid, PipelineDTO> _pipelines;
        private readonly List<DiameterSegmentDTO> _diameterSegments;

        public ValveDiameterRule(DictionaryRepositoryDTO dictionaries, List<ValveDTO> valves,
            Dictionary<Guid, PipelineDTO> pipelines, List<DiameterSegmentDTO> diameterSegments)
            : base(dictionaries)
        {
            _valves = valves;
            _pipelines = pipelines;
            _diameterSegments = diameterSegments;
        }

        public override InconsistencyType InconsistencyType => InconsistencyType.DiameterValveError;

        public override List<Guid> Validate()
        {
            var errors = new List<Guid>();
            foreach (var valve in _valves)
            {
                var pipeline = _pipelines[valve.ParentId.Value];
                var segments = _diameterSegments.Where(ds => ds.PipelineId == pipeline.Id);
                foreach (var segment in segments)
                {
                    if (segment.KilometerOfStartPoint <= valve.Kilometer &&
                        segment.KilometerOfEndPoint >= valve.Kilometer)
                    {
                        var valveDiameter = Dictionaries.ValveTypes.Single(vt => vt.Id == valve.ValveTypeId);
                        if (segment.DiameterId != valveDiameter.Id)
                        {
                            errors.Add(valve.Id);
                        }
                    }
                }
            }
            return errors;
        }
    }
}