using System.ComponentModel.DataAnnotations;
using System.Linq;
using GazRouter.DTO.Dictionaries.BoilerTypes;
using GazRouter.DTO.ObjectModel.Boilers;
using Microsoft.Practices.ServiceLocation;

namespace GazRouter.Application.Wrappers.Entity
{
    public class BoilerWrapper : EntityWrapperBase<BoilerDTO>
    {
        public BoilerWrapper(BoilerDTO dto, bool displaySystem)
            : base(dto, displaySystem)
        {
            AddProperty("Тип котлоагрегата",
                Enumerable.Single<BoilerTypeDTO>(ClientCache.DictionaryRepository.BoilerTypes, v => v.Id == dto.BoilerTypeId).Name);
            AddProperty("Коэф. внутрикотельных потерь", dto.HeatLossFactor.ToString("0.###"));
            AddProperty("Присоединенная нагрузка системы теплоснабжения, Гкал/ч", dto.HeatSupplySystemLoad.ToString("0.###"));
        }


        [Display(Name = "Тип", Order = 10)]
        public string BoilerTypeName
        {
            get
            {
                return Enumerable.Single<BoilerTypeDTO>(ClientCache.DictionaryRepository.BoilerTypes, v => v.Id == _dto.BoilerTypeId).Name;
            }
        }
        

        [Display(Name = "Километр установки", Order = 30)]
        public double Kilometer
        {
            get { return _dto.Kilometr; }
        }

        [Display(Name = "Коэф. внутрикотельных потерь", Order = 40)]
        public double? HeatLossFactor
        {
            get { return _dto.HeatLossFactor; }
        }

        [Display(Name = "Присоединенная нагрузка системы теплоснабжения, Гкал/ч", Order = 50)]
        public double? HeatSupplySystemLoad
        {
            get { return _dto.HeatSupplySystemLoad; }
        }

        
    }
}