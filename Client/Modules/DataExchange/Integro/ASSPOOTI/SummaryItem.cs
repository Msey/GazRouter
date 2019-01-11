using GazRouter.Common.ViewModel;
using GazRouter.DTO.DataExchange.ExchangeTask;
using GazRouter.DTO.DataExchange.Integro;
using GazRouter.DTO.DataExchange.Integro.Enum;

namespace DataExchange.Integro.ASSPOOTI
{
    public class SummaryItem : PropertyChangedBase
    {
        public SummaryDTO Dto { get; }
        public ExchangeTaskDTO TaskDto { get; }

        public SummaryItem(SummaryDTO dto, ExchangeTaskDTO taskDto)
        {
            Dto = dto;
            TaskDto = taskDto;
        }

        public string PeriodTypeName
        {
            get
            {
                string result = string.Empty;
                switch (Dto.PeriodType)
                {
                    case GazRouter.DTO.Dictionaries.PeriodTypes.PeriodType.Twohours :
                        result = "2х-часовая";
                        break;
                    case GazRouter.DTO.Dictionaries.PeriodTypes.PeriodType.Day:
                        result = "Суточная";
                        break;
                    case GazRouter.DTO.Dictionaries.PeriodTypes.PeriodType.Month:
                        result = "Месячная";
                        break;
                    default:
                        result = "Не определено";
                        break;
                }

                return result;
            }
        }

        public bool UsedInExchanged
        {
            get { return Dto.StatusId == (int)SummaryStatusTypes.Used; }
        }

        public string Descriptor
        {
            get
            {
                return Dto.Descriptor.Substring(0, Dto.Descriptor.IndexOf(";"));
            }
            set
            {
                Dto.Descriptor = string.Format("{0};{1}", Descriptor, value);
                OnPropertyChanged(() => Descriptor);
            }
        }

        public string Description
        {
            get
            {
                return Dto.Descriptor.Substring(Dto.Descriptor.IndexOf(";")+1, Dto.Descriptor.Length - Dto.Descriptor.IndexOf(";"));
            }
            set
            {
                Dto.Descriptor = string.Format("{0};{1}", value, Description);
                OnPropertyChanged(() => Description);
            }
        }

        private int summaryHour;
        public int SummaryHour
        {
            get
            {
                return summaryHour;
            }
            set
            {
                summaryHour = value;                
            }
        }
    }
}
