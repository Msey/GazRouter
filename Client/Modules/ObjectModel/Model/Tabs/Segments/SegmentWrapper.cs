using GazRouter.Application;
using GazRouter.Common.Cache;
using GazRouter.DTO.ObjectModel.Segment;
using Microsoft.Practices.ServiceLocation;
using System.Linq;
using GazRouter.DTO.Dictionaries.PhisicalTypes;

namespace GazRouter.ObjectModel.Model.Tabs.Segments
{
    public class SegmentWrapper
    {
        private BaseSegmentDTO _dto;

        public SegmentWrapper(BaseSegmentDTO dto)
        {
            _dto = dto;
        }


        public double KilometerOfStartPoint
        {
            get { return _dto.KilometerOfStartPoint; }
        }

        
        public double KilometerOfEndPoint
        {
            get { return _dto.KilometerOfEndPoint; }
        }

        public string Description
        {
            get
            {
                if (_dto is DiameterSegmentDTO) 
                    return ((DiameterSegmentDTO)_dto).DiameterName;

                if (_dto is SiteSegmentDTO) 
                    return ((SiteSegmentDTO)_dto).SiteName;

                if (_dto is PressureSegmentDTO)
                    return string.Format("{0:0.##} {1}",
                        UserProfile.ToUserUnits(((PressureSegmentDTO) _dto).Pressure, PhysicalType.Pressure), 
                        UserProfile.UserUnitName(PhysicalType.Pressure));

                if (_dto is GroupSegmentDTO) 
                    return ((GroupSegmentDTO)_dto).PipelineGroupName;

                if (_dto is RegionSegmentDTO)
                {
                   return ((RegionSegmentDTO)_dto).RegionName;
                }

                return "Неизвестный тип сегмента. Необходимо описать его в классе SegmentWrapper";
            }
        }
    }
}
