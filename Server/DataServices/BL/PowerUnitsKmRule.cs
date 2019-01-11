using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.DTO.Dictionaries;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.InconsistencyTypes;
using GazRouter.DTO.ObjectModel.Pipelines;
using GazRouter.DTO.ObjectModel.PowerUnits;

namespace GazRouter.DataServices.BL
{
    /// <summary>
    /// Километры установки электроагрегатов на линейной части должны быть в диапазоне между началом и концом газопровода, на котором они установлены
    /// </summary>
    public class PowerUnitsKmRule : RuleBase
    {
        private readonly List<PowerUnitDTO> _powerUnits;
        private readonly List<PipelineDTO> _pipelines;

        public PowerUnitsKmRule(DictionaryRepositoryDTO dictionaries, List<PowerUnitDTO> powerUnits, List<PipelineDTO> pipelines)
            : base(dictionaries)
        {
            _powerUnits = powerUnits;
            _pipelines = pipelines;
        }

        public override List<Guid> Validate()
        {
            var errors = new List<Guid>();

            foreach (var powerUnit in _powerUnits)
            {
                if (powerUnit.ParentEntityType == EntityType.Pipeline)
                {
                    var km = powerUnit.Kilometr;
                    var pipeline = _pipelines.Single(p => p.Id == powerUnit.ParentId);
                    var kmStart = pipeline.KilometerOfStartPoint;
                    var kmEnd = kmStart + pipeline.Length;

                    if ((decimal) kmStart > (decimal) km || (decimal) kmEnd < (decimal) km)
                        errors.Add(powerUnit.Id);
                }
            }

            return errors;
        }

        public override InconsistencyType InconsistencyType
        {
            get { return InconsistencyType.KmPowerUnitsError; }
        }
    }
}