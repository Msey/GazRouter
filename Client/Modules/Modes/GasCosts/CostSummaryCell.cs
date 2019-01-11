using System.Collections.Generic;
using GazRouter.DTO.Dictionaries.Targets;
namespace GazRouter.Modes.GasCosts
{
    public class CostSummaryCell : ICostSummarySell
    {
        public CostSummaryCell(bool isEditable)
        {
            IsEditable = isEditable;
//            ConsumptionViewModel = consumptionViewModel;
            MayContainValue = IsEditable;
        }
        private readonly Dictionary<Target, double> _values = new Dictionary<Target, double>();
        public bool MayContainValue { get; set; }
        public double this[Target target]
        {
            get { return _values.ContainsKey(target) ? _values[target] : 0; }
            set
            {
                if (_values.ContainsKey(target))
                    _values[target] = value;
                else
                {
                    _values.Add(target, value);
                }

            }
        }
        public bool IsEditable { get; private set; }
    }
    public interface ICostSummarySell
    {
        bool MayContainValue { get; }
        bool IsEditable { get; }
        double this[Target target] { get; }
    }
}