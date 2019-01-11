using System;
using GazRouter.DTO.ManualInput.InputStates;
using GazRouter.DTO.SeriesData.EntityValidationStatus;

namespace GazRouter.DTO.ManualInput
{
    public class SiteStatusDTO
    {
        public Guid SiteId { get; set; }
        public string SiteName { get; set; }
        public int AlarmNumber { get; set; }
        public int ErrorNumber { get; set; }
        public ManualInputState? Status { get; set; }

        public string StatusText
        {
            get{
                switch (Status)
                {
                    case ManualInputState.Input:
                        return "Ввод";
                        
                    case ManualInputState.Approved:
                        return "Подтверждено";
                        
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

    }
}