using System.Runtime.Serialization;
using GazRouter.DTO.ObjectModel.Sites;
using GazRouter.DTO.SeriesData.Series;

namespace GazRouter.DTO.DataLoadMonitoring
{
        [DataContract]
        public class SiteDataLoadStatistics : BaseDto<int>
        {
            [DataMember]
            public SeriesDTO DataSeries { get; set; }
            [DataMember]
            public SiteDTO Site { get; set; }
            [DataMember]
            public int ValuesCount { get; set; }
        }    
}
