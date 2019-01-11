using System.ComponentModel.DataAnnotations;
namespace GazRouter.Modes.GasCosts.Dialogs.Model
{
    public class ValveShifting
    {
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }

        [Display(Name = "Наименование")]
        public string Name { get; set; }

        [Display(Name = "Кол-во переключений")]
        public uint Count { get; set; }

        [Display(Name = "Нормативный расход газа на одно переключение")]
        public double RatedConsumption { get; set; }

        public double Q
        {
            get { return Count * RatedConsumption; }
        }

        public override string ToString()
        {
            return Name;
        }

        protected bool Equals(ValveShifting other)
        {
            return string.Equals(Name, other.Name) && Id.Equals(other.Id);
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
            return Equals((ValveShifting)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ Id.GetHashCode();
            }
        }
    }
}