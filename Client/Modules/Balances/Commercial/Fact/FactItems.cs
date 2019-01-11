using GazRouter.Balances.Commercial.Common;
using GazRouter.Common.ViewModel;
using GazRouter.DTO.Balances.GasOwners;
using GazRouter.DTO.Dictionaries.BalanceItems;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.ObjectModel;
using Microsoft.Practices.Prism.Commands;

namespace GazRouter.Balances.Commercial.Fact
{
    public class FactOwnerItem : OwnerItem
    {
        public FactOwnerItem(GasOwnerDTO owner, EntityDTO entity, BalanceItem balItem, ItemActions actions)
            : base(owner, entity, balItem, actions)
        {
            RedistrCommand = new DelegateCommand(() => actions.RedistrAction(this), () => FactBase > 0 && !FactCorrected.HasValue);
            SwapCommand = new DelegateCommand(() => actions.SwapAction(this), () => FactBase > 0 && !FactCorrected.HasValue);
            UnswapCommand = new DelegateCommand(() => actions.UnswapAction(this), () => FactCorrected.HasValue);
        }

        public override Target Target => Target.Fact;

        private double? _factBase;
        public override double? FactBase
        {
            get { return _factBase; }
            set
            {
                if (SetProperty(ref _factBase, value))
                {
                    OnPropertyChanged(() => FactCorrectedDelta);
                    OnPropertyChanged(() => FactSummarized);
                    OnPropertyChanged(() => PlanFactDelta);
                    RedistrCommand.RaiseCanExecuteChanged();

                    NotifyValueChanged(BalValueType.FactBase);
                }
            }
        }

        private double? _factCorrected;
        public override double? FactCorrected
        {
            get { return _factCorrected; }
            set
            {
                if (SetProperty(ref _factCorrected, value))
                {
                    OnPropertyChanged(() => FactCorrectedDelta);
                    OnPropertyChanged(() => FactSummarized);
                    OnPropertyChanged(() => PlanFactDelta);

                    NotifyValueChanged(BalValueType.FactBase);
                }
            }
        }

        public override void UpdateActions()
        {
            RedistrCommand.RaiseCanExecuteChanged();
            SwapCommand.RaiseCanExecuteChanged();
            UnswapCommand.RaiseCanExecuteChanged();
        }

        public override bool IsReadOnly => FactCorrected.HasValue;

        public DelegateCommand RedistrCommand { get; set; }

        public DelegateCommand SwapCommand { get; set; }
        public DelegateCommand UnswapCommand { get; set; }

        public override bool IsContextMenuEnabled => true;
    }


    public class FactSummaryItem : SummaryItem
    {
        public FactSummaryItem(EntityDTO entity, BalanceItem balItem, bool isInOut, ItemActions actions)
            : base(entity, balItem, isInOut, actions) {}

        public FactSummaryItem(string alias, ItemActions actions)
            : base(alias, actions) {}

        protected override void OnChildValueChanged(BalValueType type)
        {
            switch (type)
            {
                case BalValueType.FactBase:
                    OnPropertyChanged(() => FactBase);
                    OnPropertyChanged(() => FactSummarized);
                    OnPropertyChanged(() => FactCorrectedDelta);
                    OnPropertyChanged(() => PlanFactDelta);
                    break;

                case BalValueType.FactCorrected:
                    OnPropertyChanged(() => FactCorrected);
                    OnPropertyChanged(() => FactSummarized);
                    OnPropertyChanged(() => FactCorrectedDelta);
                    OnPropertyChanged(() => PlanFactDelta);
                    break;
            }

            NotifyValueChanged(type);
        }
    }
    
}
