using System.Linq;
using GazRouter.DTO.Dictionaries.CompUnitTypes;
using GazRouter.DTO.Dictionaries.SuperchargerTypes;
using GazRouter.DTO.ObjectModel.CompUnits;
using GazRouter.Flobus.UiEntities.FloModel;
using GazRouter.Flobus.UiEntities.FloModel.Measurings;

namespace GazRouter.Flobus.Model
{
    public class CompressorUnit : EntityBase<CompUnitDTO>, ISearchable
    {
        private bool _isFound;

        /// <summary>
        /// Тип ГПА
        /// </summary>
        public CompUnitTypeDTO CompressorUnitType
        {
            get { return ClientCache.DictionaryRepository.CompUnitTypes.Single(x => x.Id == Dto.CompUnitTypeId); }
        }

        /// <summary>
        /// Тип нагнетателя
        /// </summary>
        public SuperchargerTypeDTO SuperchargerType
        {
            get { return ClientCache.DictionaryRepository.SuperchargerTypes.Single(x => x.Id == Dto.SuperchargerTypeId); }
        }

        public CompressorUnitMeasuring CompressorUnitMeasuring { get; }

        public int Num { get; }

        public CompressorUnit(CompUnitDTO compUnitDTO, int num)
            : base(compUnitDTO)
        {
            CompressorUnitMeasuring = new CompressorUnitMeasuring(Dto);
            Num = num;
        }

        public bool IsFound
        {
            get { return _isFound; }
            set { SetProperty(ref _isFound, value); }
        }
    }
}
