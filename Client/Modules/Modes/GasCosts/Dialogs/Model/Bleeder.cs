using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace GazRouter.Modes.GasCosts.Dialogs.Model
{
    public class Bleeder
    {
        #region Public Properties

        [Display(Name = "Длина дренажной линии, для которой соответствует режим критического истечения газа")]
        public double CriticalLength { get; set; }

        [Display(Name = "Диаметр свечного крана, м")]
        public double Diameter { get; set; }

        #endregion

        public override string ToString()
        {
            return Diameter.ToString(CultureInfo.InvariantCulture);
        }

        protected bool Equals(Bleeder other)
        {
            return CriticalLength.Equals(other.CriticalLength) && Diameter.Equals(other.Diameter);
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
            return Equals((Bleeder)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (CriticalLength.GetHashCode() * 397) ^ Diameter.GetHashCode();
            }
        }
    }
}