using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using GazRouter.DTO.Dictionaries.PowerUnitTypes;

namespace GazRouter.Modes.GasCosts.Dialogs.Model
{
    public class PowerUnitType
    {
        public PowerUnitType()
        {
        }

        public PowerUnitType(PowerUnitTypeDTO dto)
        {
            Id = dto.Id;
            Name = dto.Name;
            EngineGroup = dto.EngineGroup;
            EngineGroupName = dto.EngineGroupName;
            EngineType = dto.EngineTypeName;
            FuelConsumptionRate = dto.FuelConsumptionRate;
            RatedPower = dto.RatedPower;
        }


        private bool Equals(PowerUnitType other)
        {
            return Id == other.Id;
        }

       

        [Browsable(false)]
        public int Id { get; set; }

        [Display(Name = "Норматив расхода условного топлива на выработку э/э, кг у.т./Гкал")]
        public double FuelConsumptionRate { get; set; }
        
        [Display(Name = "Номинальная электрическая мощность, кВт")]
        public double RatedPower { get; set; }
        
        [Display(Name = "Наименование")]
        public string Name { get; set; }

        [Display(Name = "Группа двигателя")]
        public EngineGroup EngineGroup { get; set; }

        [Display(Name = "Наименование группы двигателя")]
        public string EngineGroupName { get; set; }

        [Display(Name = "Тип двигателя")]
        public string EngineType { get; set; }
        

        public override string ToString()
        {
            return Name;
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
            return Equals((PowerUnitType)obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}