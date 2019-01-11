using System.ComponentModel.DataAnnotations;
namespace GazRouter.Modes.GasCosts.Dialogs.Model
{
    public class RegulatorRuntime
    {
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }
        [Display(Name = "Наименование")]
        public string Name { get; set; }
        [Display(Name = "Время работы регулятора, ч")]
        public uint Runtime { get; set; }
        [Display(Name = "Кол-во регуляторов данного типа")]
        public uint Count { get; set; }
        [Display(Name = "Нормативный расход газа, м³/ч")]
        public double RatedConsumption { get; set; }
        public double Q
        {
            get { return Runtime * Count * RatedConsumption; }
        }

        public override string ToString()
        {
            return Name;
        }
        protected bool Equals(RegulatorRuntime other)
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
            return Equals((RegulatorRuntime)obj);
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