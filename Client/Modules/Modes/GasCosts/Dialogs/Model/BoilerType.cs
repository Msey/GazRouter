using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using GazRouter.DTO.Dictionaries.BoilerTypes;

namespace GazRouter.Modes.GasCosts.Dialogs.Model
{
    public class BoilerType
    {
        public BoilerType()
        {
        }

        public BoilerType(BoilerTypeDTO dto)
        {
            Id = dto.Id;
            Name = dto.Name;
            Group = dto.GroupName;
            IsSmall = dto.IsSmall;
            EfficiencyRated = dto.RatedEfficiencyFactor;
            HeatProductivityRated = dto.RatedHeatingEfficiency;
            HeatingArea = dto.HeatingArea;
        }


        private bool Equals(BoilerType other)
        {
            return Id == other.Id;
        }

       

        [Browsable(false)]
        public int Id { get; set; }
        
        [Display(Name = "Номинальный КПД")]
        public double EfficiencyRated { get; set; }

        [Display(Name = "Номинальная теплопроизводительность, Гкал/ч")]
        public double HeatProductivityRated { get; set; }
        
        [Display(Name = "Наименование")]
        public string Name { get; set; }

        [Display(Name = "Группа")]
        public string Group { get; set; }

        [Display(Name = "Котел малой мощности?")]
        public bool IsSmall { get; set; }

        [Display(Name = "Площадь нагрева котла, м2")]
        public double HeatingArea { get; set; }

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
            return Equals((BoilerType)obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}