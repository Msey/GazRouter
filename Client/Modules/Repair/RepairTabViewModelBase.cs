using GazRouter.DTO.Appearance.Versions;
using GazRouter.Flobus.VM;
using GazRouter.Flobus.VM.Model;
using JetBrains.Annotations;

namespace GazRouter.Repair
{
    public abstract class RepairTabViewModelBase : FloControlViewModelBase
    {
        protected RepairTabViewModelBase([NotNull] RepairMainViewModel repairMainViewModel)
        {
            MainViewModel = repairMainViewModel;
        }

        public RepairMainViewModel MainViewModel { get; set; }

        protected override void AfterModelLoaded(SchemeViewModel viewModel)
        {
        }

        protected override void CloseLoadSchemeFormCallback(SchemeVersionItemDTO schemeDTO)
        {
        }
    }
}