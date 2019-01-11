using System.Linq;
using GazRouter.Balances.Commercial.Common;
using GazRouter.Balances.Commercial.Plan.CorrectionDocs;
using GazRouter.Balances.Commercial.Plan.Corrections;
using GazRouter.Balances.Commercial.Plan.Irregularity;
using GazRouter.DTO.Dictionaries.Targets;
using Microsoft.Practices.Prism.Regions;

namespace GazRouter.Balances.Commercial.Plan
{
    [RegionMemberLifetime(KeepAlive = false)]
    public class PlanViewModel : PlanFactViewModelBase
    {
        public PlanViewModel()
            : base(Target.Plan)

        {
            Irregularity = new IrregularityViewModel();
            Load();
        }
        

        public IrregularityViewModel Irregularity { get; set; }

        public CorrectionDocsViewModel CorrectionDocs { get; set; }

        public CorrectionsViewModel Corrections { get; set; }


        #region LOAD



        protected override void OnLoadComplete()
        {
            CorrectionDocs = new CorrectionDocsViewModel(_data, _isEditPermission, ShowCorrectionSummary);
            OnPropertyChanged(() => CorrectionDocs);
        }

        #endregion

        protected override void OnSelectedItemChanged()
        {
            Irregularity = new IrregularityViewModel(SelectedDate.Year, SelectedDate.Month, SelectedItem);
            OnPropertyChanged(() => Irregularity);
            Corrections = new CorrectionsViewModel(SelectedItem, _data?.CorrectionDocs);
            OnPropertyChanged(() => Corrections);
        }

        private void ShowCorrectionSummary(int docId)
        {
            var vm =
                new CorrectionsSummaryViewModel(
                    _treeBuilder.OwnerItems.OfType<PlanOwnerItem>()
                        .Where(o => o.CorrectionList != null && o.CorrectionList.Any(c => c.DocId == docId))
                        .ToList(), docId);
            var v = new CorrectionsSummaryView { DataContext = vm };
            v.ShowDialog();
        }
    }
}
