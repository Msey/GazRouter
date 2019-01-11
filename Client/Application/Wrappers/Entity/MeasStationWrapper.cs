using System.ComponentModel.DataAnnotations;
using GazRouter.DTO.ObjectModel.MeasStations;

namespace GazRouter.Application.Wrappers.Entity
{
    public class MeasStationWrapper : EntityWrapperBase<MeasStationDTO>
    {
        public MeasStationWrapper(MeasStationDTO dto, bool displaySystem) : base(dto, displaySystem)
        {
            AddProperty("Знак в балансе", dto.BalanceSignName);
            AddProperty("Промежуточный", dto.IsIntermediate ? "да" : "нет");
            AddProperty("Балансовая группа", dto.OwnBalanceGroupName);
            AddProperty("Балансовое имя", dto.BalanceName);
        }
        
        [Display(Name = "Знак в балансе", Order = 10)]
        public string BalanceSignName => _dto.BalanceSignName;
    }
}