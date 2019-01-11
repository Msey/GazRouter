using GazRouter.DTO.ObjectModel.Sites;
using GazRouter.DTO.SeriesData.Series;

namespace GazRouter.DTO.DataLoadMonitoring
{
    public class EntityPropertyValueParameterSet
    {
        //серия
        public SeriesDTO DataSeries { get; set; }
        //ЛПУ
        public SiteDTO Site { get; set; }
    }
}
