using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GazRouter.Modes.GasCosts.Dialogs.Model
{
    public class HeaterType
    {
        private bool Equals(HeaterType other)
        {
            return Id == other.Id;
        }

       

        [Browsable(false)]
        public int Id { get; set; }

        [Display(Name = "Номинальный расход газа на работу подогревателя газа, тыс.м³:")]
        public double GasConsumptionRate { get; set; }

        [Browsable(false)]
        public double? EffeciencyFactorRated { get; set; }
        
        [Display(Name = "Наименование")]
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
            return Equals((HeaterType)obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}