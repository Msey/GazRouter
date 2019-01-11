using System.Collections.Generic;
using System.Runtime.Serialization;
using GazRouter.DTO.ObjectModel.CompShops;
using GazRouter.DTO.ObjectModel.CompStations;
using GazRouter.DTO.ObjectModel.MeasLine;
using GazRouter.DTO.ObjectModel.MeasStations;
using GazRouter.DTO.ObjectModel.Pipelines;
using GazRouter.DTO.SeriesData.Series;

namespace GazRouter.DTO.DataLoadMonitoring
{
    [DataContract]
    public class GasSupplyDataSetDTO
    {
        //[DataMember]
        //public List<MeasLineDTO> MeasureLines { get; set; }
        //[DataMember]
        //public List<MeasStationDTO> MeasureStations { get; set; }
        //[DataMember]
        //public List<CompStationDTO> CompStations { get; set; }
        //[DataMember]
        //public List<CompShopDTO>  CompShops { get; set; }
        [DataMember]
        public  List<PipelineDTO> Pipelines { get; set; }
        [DataMember]
        public List<GasSupplyValue> Values { get; set; }
        [DataMember]
        public SeriesDTO ValuesSerie { get; set; } 
    }
}
