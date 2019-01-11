using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using GazRouter.DTO.Dictionaries.CoolingUnitTypes;


namespace GazRouter.Modes.GasCosts.Dialogs.Model
{
    public class CoolingUnitType
    {
        public CoolingUnitType()
        {
            
        }

        public CoolingUnitType(CoolingUnitTypeDTO dto)
        {
            Name = dto.Name;
            RatedPower = dto.RatedPower;
            FuelConsumptionRate = dto.FuelConsumptionRate;
        }

        private bool Equals(CoolingUnitType other)
        {
            return Id == other.Id;
        }

       

        [Browsable(false)]
        public int Id { get; set; }

        [Display(Name = "Норматив расхода условного топлива ГТУ, кг у.т./(кВт*ч)")]
        public double FuelConsumptionRate { get; set; }
        
        [Display(Name = "Норматив располагаемой мощности ГТУ, кВт")]
        public double RatedPower { get; set; }
        
        [Display(Name = "Наименование газотурбинного привода")]
        public string Name { get; set; }
        

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
            return Equals((CoolingUnitType)obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}