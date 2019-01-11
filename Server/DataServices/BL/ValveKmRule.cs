using System;
using System.Collections.Generic;
using GazRouter.DTO.Dictionaries;
using GazRouter.DTO.Dictionaries.InconsistencyTypes;
using GazRouter.DTO.ObjectModel.Pipelines;
using GazRouter.DTO.ObjectModel.Valves;

namespace GazRouter.DataServices.BL
{
    /// <summary>
    ///     Километры установки кранов на газопроводе должны быть в диапазоне между началом и концом газопровода, на котором
    ///     они установлены
    /// </summary>
    public class ValveKmRule : RuleBase
    {
        private readonly List<ValveDTO> _valves;
        private readonly Dictionary<Guid, PipelineDTO> _pipelines;

        public ValveKmRule(DictionaryRepositoryDTO dictionaries, List<ValveDTO> valves,
            Dictionary<Guid, PipelineDTO> pipelines)
            : base(dictionaries)
        {
            _valves = valves;
            _pipelines = pipelines;
        }

        public override InconsistencyType InconsistencyType => InconsistencyType.KmValveError;

        public override List<Guid> Validate()
        {
            var errors = new List<Guid>();
            foreach (var valve in _valves)
            {
                var pipeline = _pipelines[valve.ParentId.Value];
                var km = valve.Kilometer;
                var kmStartPipeline = pipeline.KilometerOfStartPoint;
                var kmEndPipeline = pipeline.KilometerOfStartPoint + pipeline.Length;

                if ((decimal) km < (decimal) kmStartPipeline || (decimal) km > (decimal) kmEndPipeline)
                {
                    errors.Add(valve.Id);
                }
            }
            return errors;
        }
    }
}