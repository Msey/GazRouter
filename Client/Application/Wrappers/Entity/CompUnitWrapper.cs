using System.ComponentModel.DataAnnotations;
using System.Linq;
using GazRouter.DTO.Dictionaries.CompUnitSealingTypes;
using GazRouter.DTO.Dictionaries.CompUnitTypes;
using GazRouter.DTO.Dictionaries.SuperchargerTypes;
using GazRouter.DTO.ObjectModel.CompUnits;

namespace GazRouter.Application.Wrappers.Entity
{
    public sealed class CompUnitWrapper : EntityWrapperBase<CompUnitDTO>
    {
        public CompUnitWrapper(CompUnitDTO dto, bool displaySystem) : base(dto, displaySystem)
        {
            AddProperty("№ ГПА", dto.CompUnitNum.ToString());
            AddProperty("Тип ГПА",
                Enumerable.Single<CompUnitTypeDTO>(ClientCache.DictionaryRepository.CompUnitTypes, c => c.Id == dto.CompUnitTypeId).Name);
            AddProperty("Тип нагнетателя",
                Enumerable.Single<SuperchargerTypeDTO>(ClientCache.DictionaryRepository.SuperchargerTypes, c => c.Id == dto.SuperchargerTypeId).Name);
            
            AddProperty("Объем контура нагнетателя, м³", dto.InjectionProfileVolume.ToString("0.###"));
            AddProperty("Расход газа на работу пускового турбодетандера, м³", dto.TurbineStarterConsumption.ToString("0.###"));
            AddProperty("Расход газа на холодную прокрутку, м³", dto.DryMotoringConsumption.ToString("0.###"));
            AddProperty("Расход импульсного газа на работу ЗРА при пуске агрегата, м³", dto.StartValveConsumption.ToString("0.###"));
            AddProperty("Расход импульсного газа на работу ЗРА при останове агрегата, м³", dto.StopValveConsumption.ToString("0.###"));

            AddProperty("Тип уплотнений",
                dto.SealingType.HasValue
                    ? Enumerable.Single<CompUnitSealingTypeDTO>(ClientCache.DictionaryRepository.CompUnitSealingTypes, st => st.SealingType == dto.SealingType).Name
                    : "");
            AddProperty("Кол-во уплотнений, шт.", dto.SealingCount.ToString());
            AddProperty("Расход газа через уплотнение, стравливаемый в атмосферу через свечу, м³/ч", dto.BleedingRate.ToString("0.###"));
            AddProperty("Наличие утилизационного теплообменника", dto.HasRecoveryBoiler ? "да" : "нет");
        }

        [Display(Name = "Тип ГПА", Order = 10)]
        public string CompUnitTypeName
        {
            get { return Enumerable.Single<CompUnitTypeDTO>(ClientCache.DictionaryRepository.CompUnitTypes, c => c.Id == _dto.CompUnitTypeId).Name; }
        }

        [Display(Name = "Тип нагнетателя", Order = 20)]
        public string SuperchargerTypeName
        {
            get { return Enumerable.Single<SuperchargerTypeDTO>(ClientCache.DictionaryRepository.SuperchargerTypes, c => c.Id == _dto.SuperchargerTypeId).Name; }
        }
        
        
        [Display(Name = "Объем контура нагнетателя, м³", Order = 30)]
        public double InjectionProfileVolume
        {
            get { return _dto.InjectionProfileVolume; }
        }


        [Display(Name = "Расход газа на работу турбодетандера, м³", 
            Description = "Расход газа на работу пускового турбодетандера, м³", Order = 40)]
        public double TurbineStarterConsumption
        {
            get { return _dto.TurbineStarterConsumption; }
        }


        [Display(Name = "Тип уплотнений",
            Description = "Тип уплотнений", Order = 45)]
        public string SealingTypeName
        {
            get
            {
                return _dto.SealingType.HasValue
                    ? Enumerable.Single<CompUnitSealingTypeDTO>(ClientCache.DictionaryRepository.CompUnitSealingTypes, st => st.SealingType == _dto.SealingType).Name
                    : "";
            }
        }

        [Display(Name = "Кол-во уплотнений, шт.",
            Description = "Количество уплотнений, шт.", Order = 46)]
        public int SealingCount
        {
            get { return _dto.SealingCount; }
        }


        [Display(Name = "Расход газа через уплотнение, м³/ч", 
            Description = "Расход газа через уплотнение, стравливаемый в атмосферу через свечу, м³/ч", Order = 50)]
        public double BleedingRate
        {
            get { return _dto.BleedingRate; }
        }

        [Display(Name = "Наличие утилизационного теплообменника", Order = 60)]
        public bool HasRecoveryBoiler
        {
            get { return _dto.HasRecoveryBoiler; }
        }
        
        [Display(Name = "Виртуальный", Order = 70)]
        public bool IsVirtual
        {
            get { return _dto.IsVirtual; }
        }
    }
}