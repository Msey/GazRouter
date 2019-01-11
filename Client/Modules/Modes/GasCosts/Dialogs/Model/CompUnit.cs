using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using GazRouter.DTO.Dictionaries.CompUnitSealingTypes;
using GazRouter.DTO.ObjectModel.CompUnits;

namespace GazRouter.Modes.GasCosts.Dialogs.Model
{

    public class CompUnit
    {
        public CompUnit()
        {
            
        }

        public CompUnit(CompUnitDTO dto)
        {
            Id = dto.Id;
            Name = dto.Name;
            TypeId = dto.CompUnitTypeId;
            SuperchargerTypeId = dto.SuperchargerTypeId;
            InjectionProfileVolume = dto.InjectionProfileVolume;
            DryMotoringConsumption = dto.DryMotoringConsumption;
            TurbineStarterConsumption = dto.TurbineStarterConsumption;
            HasRecoveryBoiler = dto.HasRecoveryBoiler;
            SealingType = dto.SealingType;
            SealingCount = dto.SealingCount;
            BleedingRate = dto.BleedingRate;
            StartValveConsumption = dto.StartValveConsumption;
            StopValveConsumption = dto.StopValveConsumption;
        }



        [Browsable(false)]
        public Guid Id { get; set; }

        [Display(Name = "Наименование")]
        public string Name { get; set; }

        [Display(Name = "Тип")]
        public int TypeId { get; set; }

        [Display(Name = "Тип нагнетателя")]
        public int SuperchargerTypeId { get; set; }

        [Display(Name = "Объем контура нагнетателя, м³")]
        public double InjectionProfileVolume { get; set; }

        [Display(Name = "Q газа на холодную прокрутку, м³")]
        public double DryMotoringConsumption { get; set; }

        [Display(Name = "Q газа на работу пускового турбодетандера, м³")]
        public double TurbineStarterConsumption { get; set; }

        [Display(Name = "Наличие котла-утилизатора")]
        public bool HasRecoveryBoiler { get; set; }

        [Display(Name = "Тип уплотнения")]
        public CompUnitSealingType? SealingType { get; set; }

        [Display(Name = "Кол-во уплотнений на ГПА")]
        public int SealingCount { get; set; }

        [Display(Name = "Расход газа через уплотнение, м3/ч")]
        public double BleedingRate { get; set; }

        [Display(Name = "Расход газа на переключения ЗА при пуске ГПА (в соотв. с алгоритмом), м3")]
        public double StartValveConsumption { get; set; }
        
        [Display(Name = "Расход газа на переключения ЗА при останове ГПА (в соотв. с алгоритмом), м3")]
        public double StopValveConsumption { get; set; }

        public override string ToString()
        {
            return Name;
        }

        protected bool Equals(CompUnit other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((CompUnit)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}