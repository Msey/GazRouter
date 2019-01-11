using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.DTO.Dictionaries;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.InconsistencyTypes;
using GazRouter.DTO.ObjectModel.Boilers;
using GazRouter.DTO.ObjectModel.Pipelines;

namespace GazRouter.DataServices.BL
{
    /// <summary>
    /// Километры установки котлов на линейной части должны быть в диапазоне между началом и концом газопровода, на котором они установлены
    /// </summary>
    public class BoilerKmRule : RuleBase
    {
        private readonly List<BoilerDTO> _boilers;
        private readonly List<PipelineDTO> _pipelines;

        public BoilerKmRule(DictionaryRepositoryDTO dictionaries, List<BoilerDTO> boilers, List<PipelineDTO> pipelines)
            : base(dictionaries)
        {
            _boilers = boilers;
            _pipelines = pipelines;
        }

        public override List<Guid> Validate()
        {
            var errors = new List<Guid>();

            foreach (var boiler in _boilers)
            {
                if (boiler.ParentEntityType == EntityType.Pipeline)
                {
                    var pipeline = _pipelines.Single(p => p.Id == boiler.ParentId);
                    var km = boiler.Kilometr;
                    var kmStart = pipeline.KilometerOfStartPoint;
                    var kmEnd = pipeline.KilometerOfStartPoint + pipeline.Length;

                    if ((decimal) kmStart > (decimal) km || (decimal) kmEnd < (decimal) km)
                        errors.Add(boiler.Id);
                }
            }

            return errors;
        }
 

        public override InconsistencyType InconsistencyType
        {
            get { return InconsistencyType.KmBoilerError; }
        }
    }
}