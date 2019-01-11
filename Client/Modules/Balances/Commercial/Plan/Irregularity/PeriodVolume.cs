using GazRouter.DTO.Balances.Irregularity;

namespace GazRouter.Balances.Commercial.Plan.Irregularity
{
    public class PeriodVolume
    {
        public PeriodVolume()
        {
        }

        public PeriodVolume(IrregularityDTO dto)
        {
            StartDayNum = dto.StartDayNum;
            EndDayNum = dto.EndDayNum;
            Volume = dto.Value;
        }

        public int StartDayNum { get; set; }

        public int EndDayNum { get; set; }

        public int Days => EndDayNum - StartDayNum + 1;

        public double Volume { get; set; }

        public SetIrregularityParameterSet ToParameterSet(double coef)
        {
            return new SetIrregularityParameterSet
            {
                StartDayNum = StartDayNum,
                EndDayNum = EndDayNum,
                Value = Volume / coef
            };
        }
    }
}