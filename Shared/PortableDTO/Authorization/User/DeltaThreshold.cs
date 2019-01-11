using GazRouter.DTO.Dictionaries.PhisicalTypes;

namespace GazRouter.DTO.Authorization.User
{
    public class DeltaThreshold
    {
        public DeltaThreshold() { }

        public DeltaThreshold(PhysicalType pt, double show, double warn, bool isPercentage = false)
        {
            PhysicalType = pt;
            ShowThreshold = show;
            WarnThreshold = warn;
            IsPercentage = isPercentage;
        }

        public PhysicalType PhysicalType { get; set; }

        public double ShowThreshold { get; set; }

        public double WarnThreshold { get; set; }

        public bool IsPercentage { get; set; }
    }
}