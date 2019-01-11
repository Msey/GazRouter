using System.ComponentModel.DataAnnotations;
using GazRouter.DTO.Dictionaries.PhisicalTypes;
using GazRouter.DTO.ObjectModel.DistrStationOutlets;

namespace GazRouter.Application.Wrappers.Entity
{
    public class DistrStationOutletWrapper : EntityWrapperBase<DistrStationOutletDTO>
    {
        public DistrStationOutletWrapper(DistrStationOutletDTO dto, bool displaySystem) : base(dto, displaySystem)
        {
            AddProperty("Производительность проектная, тыс.м³/ч", dto.CapacityRated.HasValue ? dto.CapacityRated.Value.ToString("0.###") : "");
            AddProperty("Давление выхода проектное, " + UserProfile.UserUnitName(PhysicalType.Pressure),
                dto.PressureRated.HasValue ? UserProfile.ToUserUnits(dto.PressureRated.Value, PhysicalType.Pressure).ToString("0.##") : "");
            AddProperty("Единственное подключение", dto.ConsumerName);
        }

        [Display(Name = "Производительность проектная, тыс.м³/ч", Order = 10)]
        public double? CapacityRated
        {
            get { return _dto.CapacityRated; }
        }

        [Display(Name = "Давление выхода проектное, кг/см²", Order = 20)]
        public double? PressureRated
        {
            get { return _dto.PressureRated; }
        }
    }
}