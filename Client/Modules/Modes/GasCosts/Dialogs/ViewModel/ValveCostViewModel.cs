using Common.ViewModel;

using Modes.GasCosts.Dialogs.Model;

namespace Modes.GasCosts.Dialogs.ViewModel
{
    public class ValveCostViewModel : PropertyChangedBase
    {


        public ValveCost Model { get; private set; }

      
  

        public ValveCostViewModel(ValveCost valveCost)
        {
            Model = valveCost;
        }

        public int Count
        {
            get { return Model.Count; }
            set
            {
                Model.Count = value;
                OnPropertyChanged(() => Count);
                OnPropertyChanged(() => Q);
            }
        }

        public double Q
        {
            get { return Count * Model.ValveType.RatedConsumption; }
        }
    }
}