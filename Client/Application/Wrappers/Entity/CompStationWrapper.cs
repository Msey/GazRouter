using System.ComponentModel.DataAnnotations;
using System.Linq;
using GazRouter.DTO.ObjectModel.CompStations;

namespace GazRouter.Application.Wrappers.Entity
{
    public class CompStationWrapper : EntityWrapperBase<CompStationDTO>
    {
        public CompStationWrapper(CompStationDTO dto, bool displaySystem)
            : base(dto, displaySystem)
        {
            AddProperty("Регион", ClientCache.DictionaryRepository.Regions.FirstOrDefault(c => c.Id == dto.RegionId)?.Name);
            AddProperty("Участвует в балансе", dto.UseInBalance ? "да" : "нет");
        }

        [Display(Name = "Регион", Order = 10)]
        public string Region => ClientCache.DictionaryRepository.Regions.FirstOrDefault(c => c.Id == _dto.RegionId)?.Name;
    }
}