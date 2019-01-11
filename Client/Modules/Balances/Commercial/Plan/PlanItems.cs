using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Balances.Commercial.Common;
using GazRouter.Balances.Commercial.Plan.Irregularity;
using GazRouter.DTO.Balances.Corrections;
using GazRouter.DTO.Balances.GasOwners;
using GazRouter.DTO.Balances.Values;
using GazRouter.DTO.Dictionaries.BalanceItems;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.ObjectModel;


namespace GazRouter.Balances.Commercial.Plan
{
    public class PlanSummaryItem : SummaryItem
    {
        public PlanSummaryItem(EntityDTO entity, BalanceItem balItem, bool isInOut, ItemActions actions)
            : base(entity, balItem, isInOut, actions)
        { }

        public PlanSummaryItem(string alias, ItemActions actions)
            : base(alias, actions)
        { }

        protected override void OnChildValueChanged(BalValueType type)
        {
            switch (type)
            {
                case BalValueType.PlanCorrected:
                    OnPropertyChanged(() => PlanCorrected);
                    OnPropertyChanged(() => PlanCorrectedDelta);
                    break;

                case BalValueType.PlanBase:
                    OnPropertyChanged(() => PlanBase);
                    OnPropertyChanged(() => PlanCorrectedDelta);
                    break;
            }

            NotifyValueChanged(type);
        }
    }

    public class PlanOwnerItem : OwnerItem
    {
        private List<PeriodVolume> _periodVolumeList;
        private List<CorrectionDTO> _correctionList;

        public PlanOwnerItem(GasOwnerDTO owner, EntityDTO entity, BalanceItem balItem, ItemActions actions)
            : base(owner, entity, balItem, actions) {}

        public override Target Target => Target.Plan;

        public bool HasIrregularity => PeriodVolumeList != null && PeriodVolumeList.Count > 0;

        public bool WrongIrregularity
            => HasIrregularity && PlanSummarized.HasValue && PeriodVolumeList.Sum(p => p.Volume * p.Days) != PlanSummarized.Value;

        public List<PeriodVolume> PeriodVolumeList
        {
            get { return _periodVolumeList; }
            set
            {
                if (SetProperty(ref _periodVolumeList, value))
                {
                    OnPropertyChanged(() => HasIrregularity);
                    OnPropertyChanged(() => WrongIrregularity);
                }
            }
        }

        public List<CorrectionDTO> CorrectionList
        {
            get { return _correctionList; }
            set { SetProperty(ref _correctionList, value); }
        }


        private double? _planBase;
        public override double? PlanBase
        {
            get { return _planBase; }
            set
            {
                if (SetProperty(ref _planBase, value))
                {
                    OnPropertyChanged(() => PlanCorrectedDelta);
                    OnPropertyChanged(() => PlanSummarized);
                    OnPropertyChanged(() => WrongIrregularity);

                    NotifyValueChanged(BalValueType.PlanBase);
                }
            }
        }

        private double? _planCorrected;
        public override double? PlanCorrected
        {
            get { return _planCorrected; }
            set
            {
                if (SetProperty(ref _planCorrected, value == PlanBase ? null : value))
                {
                    OnPropertyChanged(() => PlanCorrectedDelta);
                    OnPropertyChanged(() => PlanSummarized);
                    OnPropertyChanged(() => WrongIrregularity);

                    NotifyValueChanged(BalValueType.PlanCorrected);
                }
            }
        }
        

        public override void InitValues(BalanceValues values, Target target, double coef)
        {
            base.InitValues(values, target, coef);
            var val = values.GetValue(Entity.Id, Owner.Id, BalItem, coef);
            PeriodVolumeList = val?.IrregularityList?.Select(i => new PeriodVolume(i)).ToList();
            CorrectionList = val?.CorrectionList;
        }

        public override SetBalanceValueParameterSet GetSetValueParamSet(int contractId, double coef)
        {
            var pSet = base.GetSetValueParamSet(contractId, coef);
            if (pSet != null)
            {
                pSet.IrregularityList = PeriodVolumeList?.Select(p => p.ToParameterSet(coef)).ToList();
                pSet.CorrectionList = CorrectionList?.Select(c => new SetCorrectionParameterSet {DocId = c.DocId, Value = c.Value}).ToList();
            }
            return pSet;
        }
    }
}