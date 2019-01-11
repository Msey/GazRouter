using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using GazRouter.DTO.Dictionaries.Diameters;

namespace GazRouter.Controls.Dialogs.PipingVolumeCalculator
{

    public class PipeSection
    {
        [Display(Name = "Диаметр (условный), мм")]
        public double Diameter { get; set; }

        [Display(Name = "Длина, м")]
        public double Length { get; set; }

        [Display(Name = "Диаметр (Внешний), мм")]
        public double ExternalDiameter { get; set; }

        [Display(Name = "Толщина стенок, мм")]
        public double WallThickness { get; set; }

        /// <summary>
        /// Геометрический объем, м³
        /// </summary>
        public double Volume 
        {
            get { return Math.Round(Math.PI * Math.Pow((ExternalDiameter > 0 && WallThickness >0 ? ExternalDiameter - (2 * WallThickness): Diameter) / 2000, 2) * Length, 3); }
        }


        public override string ToString()
        {
            return Diameter.ToString(CultureInfo.InvariantCulture);
        }

        protected bool Equals(PipeSection other)
        {
            return Diameter == other.Diameter;
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
            return Equals((PipeSection)obj);
        }

        public override int GetHashCode()
        {
            return Diameter.GetHashCode();
        }
    }
}