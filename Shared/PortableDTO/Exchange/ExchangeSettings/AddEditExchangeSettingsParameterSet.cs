using System;
using GazRouter.DTO.Dictionaries.PeriodTypes;

namespace GazRouter.DTO.Exchange.ExchangeSettings
{
    public class AddEditExchangeSettingsParameterSet
    {
        public PeriodType PeriodTypeId { get; set; }

        public int Id { get; set; }
        
        public int? SourceId { get; set; }

        public string SettingData { get; set; }

        public string Description { get; set; }

        public string FtpAddress { get; set; }

        public string FtpLogin { get; set; }

        public string FtpPassword { get; set; }

        public string Email { get; set; }

        public string FolderPath { get; set; }

        public DateTime SheduleStart { get; set; }

        public string FileMask { get; set; }
        public int? TrasnportType { get; set; }
        
        public bool IsTransform { get; set; }
    }
}