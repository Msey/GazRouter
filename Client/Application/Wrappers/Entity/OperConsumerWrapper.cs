using System.Linq;
using GazRouter.DTO.ObjectModel.OperConsumers;


namespace GazRouter.Application.Wrappers.Entity
{
    public class OperConsumerWrapper : EntityWrapperBase<OperConsumerDTO>
    {
        public OperConsumerWrapper(OperConsumerDTO dto, bool displaySystem) : base(dto, displaySystem)
        {
            AddProperty("Тип", dto.OperConsumerTypeName);
            AddProperty("Прямое подключение", dto.IsDirectConnection ? "Да" : "Нет");
            if (!dto.IsDirectConnection) AddProperty("ГРС", dto.DistrStationName);
            AddProperty("Регион", ClientCache.DictionaryRepository.Regions.Single(r => r.Id == dto.RegionId).Name);
            AddProperty("Балансовая группа", dto.OwnBalanceGroupName);
        }
    }
}