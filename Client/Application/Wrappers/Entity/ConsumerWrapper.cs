using System.Linq;
using GazRouter.DTO.ObjectModel.Consumers;

namespace GazRouter.Application.Wrappers.Entity
{
    public class ConsumerWrapper : EntityWrapperBase<ConsumerDTO>
    {
        public ConsumerWrapper(ConsumerDTO dto, bool displaySystem) : base(dto, displaySystem)
        {
            AddProperty("Тип", ClientCache.DictionaryRepository.ConsumerTypes.Single(t => t.Id == dto.ConsumerTypeId).Name);
            AddProperty("Регион", ClientCache.DictionaryRepository.Regions.Single(r => r.Id == dto.RegionId).Name);
            AddProperty("ГРО", dto.DistrNetworkName);
            AddProperty("Участвует в балансе", dto.UseInBalance ? "Да" : "Нет");
        }
    }
}