using System.Runtime.Versioning;

namespace GazRouter.Balances.Commercial.Common
{
    public class SummaryValues
    {
        public double Resources => Intake + PipeMinus; 

        public double Intake { get; set; }
        public double PipeMinus { get; set; }

        public double Distribution => Transit + Consumers + AuxCosts + BalanceLoss + OperConsumers + PipePlus;

        public double Transit { get; set; }

        public double Consumers { get; set; }

        public double AuxCosts { get; set; }

        public double BalanceLoss { get; set; }

        public double OperConsumers { get; set; }

        public double PipePlus { get; set; }

        public double BalanceDelta => Resources - Distribution;

        public bool IsBalanced => Resources == Distribution;

    }
}