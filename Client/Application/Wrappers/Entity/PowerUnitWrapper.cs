using System.ComponentModel.DataAnnotations;
using System.Linq;
using GazRouter.DTO.Dictionaries.PowerUnitTypes;
using GazRouter.DTO.ObjectModel.PowerUnits;

namespace GazRouter.Application.Wrappers.Entity
{
    public class PowerUnitWrapper : EntityWrapperBase<PowerUnitDTO>
    {
        public PowerUnitWrapper(PowerUnitDTO dto, bool displaySystem)
            : base(dto, displaySystem)
        {
            AddProperty("Тип",
                Enumerable.Single<PowerUnitTypeDTO>(ClientCache.DictionaryRepository.PowerUnitTypes, v => v.Id == dto.PowerUnitTypeId).Name);
            AddProperty("Километр установки", dto.Kilometr.ToString("0.###"));
            AddProperty("Расход газа на работу турбодетандера, м³/с", dto.TurbineConsumption.ToString());
            AddProperty("Время работы турбодетандера, с", dto.TurbineRuntime.ToString());
            AddProperty("Электроагрегат проходил капитальный ремонт", dto.OperatingTimeFactor == 1.02 ? "да" : "нет");
        }

        
        [Display(Name = "Тип", Order = 10)]
        public string PowerUnitTypeName
        {
            get
            {
                return Enumerable.Single<PowerUnitTypeDTO>(ClientCache.DictionaryRepository.PowerUnitTypes, v => v.Id == _dto.PowerUnitTypeId).Name;
            }
        }
        

        [Display(Name = "Километр установки", Order = 30)]
        public double Kilometer
        {
            get { return _dto.Kilometr; }
        }

        [Display(Name = "Расход газа на работу турбодетандера, м³/с", Order = 50)]
        public double TurbineConsumption
        {
            get { return _dto.TurbineConsumption; }
        }

        [Display(Name = "Время работы турбодетандера, с", Order = 60)]
        public int TurbineRuntime
        {
            get { return _dto.TurbineRuntime; }
        }

        [Display(Name = "Электроагрегат проходил капитальный ремонт", Order = 70)]
        public bool OperatingTimeFactor
        {
            get { return _dto.OperatingTimeFactor == 1.02; }
        }
    }
}