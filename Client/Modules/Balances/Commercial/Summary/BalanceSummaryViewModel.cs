using System.Collections.Generic;
using GazRouter.Common.ViewModel;
using GazRouter.DTO.Dictionaries.Targets;

namespace GazRouter.Balances.Commercial.Summary
{
    public class BalanceSummaryViewModel : ViewModelBase
    {
        private BalanceSummary _summary;
        private readonly Target _target;

        public BalanceSummaryViewModel(Target target)
        {
            _target = target;
        }

        public void Update(BalanceSummary summary)
        {
            _summary = summary;
            OnPropertyChanged(() => Tree);
        }

        public IEnumerable<SummaryItemBase> Tree => _summary?.Tree;

        public bool ShowPlan => true;

        public bool ShowFact => _target == Target.Fact;

        public bool ShowDelta => _target == Target.Fact;

        public string ValueFormat { get; set; }

        public string DeltaFormat => $"+{ValueFormat};-{ValueFormat};#";

    }
}
