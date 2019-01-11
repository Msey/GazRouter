using System.ComponentModel.DataAnnotations;
using System.Linq;
using GazRouter.DTO.Dictionaries.PhisicalTypes;
using GazRouter.DTO.ObjectModel.DistrStations;

namespace GazRouter.Application.Wrappers.Entity
{
    public class DistrStationWrapper : EntityWrapperBase<DistrStationDTO>
    {
        public DistrStationWrapper(DistrStationDTO dto, bool displaySystem) : base(dto, displaySystem)
        {
            AddProperty("Регион", ClientCache.DictionaryRepository.Regions.FirstOrDefault(c => c.Id == dto.RegionId)?.Name);
            AddProperty("Участвует в балансе", dto.UseInBalance ? "да" : "нет");
            AddProperty("Сторонняя ГРС", dto.IsForeign ? "да" : "нет");
            AddProperty("Балансовая группа", dto.OwnBalanceGroupName);
            AddProperty("Производительность проектная, тыс.м³/ч", dto.CapacityRated.HasValue ? dto.CapacityRated.Value.ToString("0.###") : "");
            AddProperty("Давление входа проектное, " + UserProfile.UserUnitName(PhysicalType.Pressure),
                dto.PressureRated.HasValue ? UserProfile.ToUserUnits(dto.PressureRated.Value, PhysicalType.Pressure).ToString("0.##") : "");
        }

        [Display(Name = "Производительность проектная, тыс.м³/ч", Order = 10)]
        public double? CapacityRated
        {
            get { return _dto.CapacityRated; }
            set { _dto.CapacityRated = value; }
        }

        [Display(Name = "Давление входа проектное, кг/см²", Order = 20)]
        public double? PressureRated
        {
            get { return _dto.PressureRated; }
            set { _dto.PressureRated = value; }
        }
    }
}