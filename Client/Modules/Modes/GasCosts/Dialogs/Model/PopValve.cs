using System.ComponentModel.DataAnnotations;
using System.Globalization;
using GazRouter.DTO.Dictionaries.EmergencyValveTypes;

namespace GazRouter.Modes.GasCosts.Dialogs.Model
{
    public class PopValve
    {

        public PopValve()
        {
            
        }

        public PopValve(EmergencyValveTypeDTO dto)
        {
            Name = dto.Name;
            Diameter = dto.InnerDiameter;
        }


        #region Public Properties

        [Display(Name = "Наименование")]
        public string Name { get; set; }

        [Display(Name = "Диаметр, м")]
        public double Diameter { get; set; }

        #endregion

        public override string ToString()
        {
            return Diameter.ToString(CultureInfo.InvariantCulture);
        }

        protected bool Equals(PopValve other)
        {
            return string.Equals(Name, other.Name) && Diameter.Equals(other.Diameter);
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
            return Equals((PopValve)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ Diameter.GetHashCode();
            }
        }
    }
}