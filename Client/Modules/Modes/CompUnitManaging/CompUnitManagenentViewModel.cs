using GazRouter.Common.ViewModel;
using GazRouter.Modes.CompressorUnitManaging.OperatingTimeCompUnit;
using Microsoft.Practices.Prism.Regions;

namespace GazRouter.Modes.CompUnitManaging
{
    [RegionMemberLifetime(KeepAlive = false)]
    public class CompUnitManagenentViewModel : MainViewModelBase
    {
        public OperatingTimeCompUnitViewModel OperatingTimeCompUnitViewModel { get; private set; }
        
        public void Refresh()
        {
            
        }

        public CompUnitManagenentViewModel()
        {
            //OperatingTimeCompUnitViewModel = new OperatingTimeCompUnitViewModel();
            
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            Refresh();
        }
    }
}