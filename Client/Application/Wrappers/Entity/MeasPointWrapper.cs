using System.ComponentModel.DataAnnotations;
using GazRouter.DTO.ObjectModel.MeasPoint;

namespace GazRouter.Application.Wrappers.Entity
{
    public class MeasPointWrapper : EntityWrapperBase<MeasPointDTO>
    {
        public MeasPointWrapper(MeasPointDTO dto, bool displaySystem) : base(dto, displaySystem)
        {
            AddProperty("Расход анализируемого газа в хроматографе (в соответствии с паспортными данными), л/мин",
                dto.ChromatographConsumptionRate.ToString("0.###"));
            AddProperty("Время проведения анализа (в соответствии с паспортными данными), мин",
                dto.ChromatographTestTime.ToString());
        }

        [Display(Name = "Замерная линия", Order = 10)]
        public string MeasLineName
        {
            get { return _dto.MeasLineName; }
        }

        [Display(Name = "КЦ", Order = 10)]
        public string CompShopName
        {
            get { return _dto.CompShopName; }
        }

        [Display(Name = "ГРС", Order = 10)]
        public string DistrStationName
        {
            get { return _dto.DistrStationName; }
        }
    }
}