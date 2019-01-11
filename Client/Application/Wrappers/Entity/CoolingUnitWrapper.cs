using System.ComponentModel.DataAnnotations;
using System.Linq;
using GazRouter.DTO.Dictionaries.CoolingUnitTypes;
using GazRouter.DTO.ObjectModel.CoolingUnit;

namespace GazRouter.Application.Wrappers.Entity
{
    public class CoolingUnitWrapper : EntityWrapperBase<CoolingUnitDTO>
    {
        public CoolingUnitWrapper(CoolingUnitDTO dto, bool displaySystem)
            : base(dto, displaySystem)
        {
            AddProperty("Тип",
                Enumerable.Single<CoolingUnitTypeDTO>(ClientCache.DictionaryRepository.CoolingUnitTypes, v => v.Id == dto.CoolingUnitTypeId).Name);
        }

        
        [Display(Name = "Тип", Order = 10)]
        public string CoolingUnitTypeName
        {
            get
            {
                return Enumerable.Single<CoolingUnitTypeDTO>(ClientCache.DictionaryRepository.CoolingUnitTypes, v => v.Id == _dto.CoolingUnitTypeId).Name;
            }
        }
        
    }
}