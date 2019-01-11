using System.Windows.Media;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.GasCosts;

namespace GazRouter.Modes.GasCosts.Summary
{
    public abstract class GasCostsSummaryItemBase
    {
        public GasCostsSummaryItemBase(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }

        public double Norm { get; set; }

        public double Plan { get; set; }

        public double Fact { get; set; }

        public double FactTotalToDate { get; set; }

        public SolidColorBrush FactColor
        {
            get
            {

                Color color = Colors.Black;
                if (Fact > Plan)
                    color = Color.FromArgb(0xff, 0xff, 0x8c, 0x00);

                if (Fact > Norm)
                    color = Color.FromArgb(0xff, 0xdc, 0x14, 0x3c);

                return new SolidColorBrush(color);

            }
        }

        public SolidColorBrush PlanColor
        {
            get
            {

                Color color = Colors.Black;

                if (Plan > Norm)
                    color = Color.FromArgb(0xff, 0xdc, 0x14, 0x3c);

                return new SolidColorBrush(color);

            }
        }

        public virtual void AddGasCost(GasCostDTO costDTO)
        {
            switch (costDTO.Target)
            {
                case Target.Fact:
                    Fact += costDTO.Volume;
                    break;
                case Target.Plan:
                    Plan += costDTO.Volume;
                    break;                            
                case Target.Norm:
                    Norm += costDTO.Volume;
                    break;
            }
        }
        public virtual void AddFactTotalToDateGasCost(GasCostDTO costDTO)
        {
            //if (costDTO.Target == Target.Fact)
                FactTotalToDate += costDTO.Volume;
        }
    }
}