using System.Collections.Generic;
using System.Runtime.Serialization;
using GazRouter.DTO.Appearance.Versions;
using GazRouter.DTO.ObjectModel.CompShops;
using GazRouter.DTO.ObjectModel.CompUnits;
using GazRouter.DTO.ObjectModel.DistrStations;
using GazRouter.DTO.ObjectModel.MeasLine;
using GazRouter.DTO.ObjectModel.PipelineConns;
using GazRouter.DTO.ObjectModel.Pipelines;
using GazRouter.DTO.ObjectModel.ReducingStations;
using GazRouter.DTO.ObjectModel.Valves;
using GazRouter.DTO.ObjectModel.Segment;

namespace GazRouter.DTO.Appearance
{
    [DataContract]
    public class SchemeModelDTO
    {
        [DataMember]
        public List<PipelineDTO> PipelineList { get; set; }

        [DataMember]
        public List<DiameterSegmentDTO> DiameterSegments { get; set; }

        [DataMember]
        public List<ValveDTO> ValveList { get; set; }

        [DataMember]
        public List<DistrStationDTO> DistrStationList { get; set; }

        [DataMember]
        public List<MeasLineDTO> MeasLineList { get; set; }

        [DataMember]
        public List<CompShopDTO> CompShopList { get; set; }

        [DataMember]
        public List<CompUnitDTO> CompUnitList { get; set; }

        [DataMember]
        public List<ReducingStationDTO> ReducingStationList { get; set; }

        [DataMember]
        public List<PipelineConnDTO> PipelineConnList { get; set; }

        [DataMember]
        public SchemeVersionDTO SchemeVersion { get; set; }
    }
}