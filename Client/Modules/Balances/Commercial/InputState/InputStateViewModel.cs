using System.Collections.Generic;
using System.Linq;
using GazRouter.Balances.Commercial.Common;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.Balances;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Balances.InputStates;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.ManualInput.InputStates;
using GazRouter.DTO.ObjectModel.Sites;
using Microsoft.Practices.Prism.Commands;

namespace GazRouter.Balances.Commercial.InputState
{
    public class InputStateViewModel : ViewModelBase
    {
        private BalanceDataBase _data;
        private InputStateItem _selectedItem;

        public InputStateViewModel(BalanceDataBase data)
        {
            _data = data;
            RefreshCommand = new DelegateCommand(Refresh);
            UnlockInputCommand = new DelegateCommand(UnlockInput, () => SelectedItem?.InputState.State == ManualInputState.Approved);

            Refresh();
        }


        public DelegateCommand RefreshCommand { get; set; }
        private async void Refresh()
        {
            var states = await new BalancesServiceProxy().GetInputStateListAsync(
                new GetInputStateListParameterSet
                {
                    ContractId = _data.FactContract.Id
                });

            ItemList = new List<InputStateItem>(
                _data.Sites.Select(s =>
                    new InputStateItem
                    {
                        SiteName = s.Name,
                        InputState = states.SingleOrDefault(state => state.SiteId == s.Id) ?? new InputStateDTO()
                    }));

            OnPropertyChanged(() => ItemList);
        }
        
        public List<InputStateItem> ItemList { get; set; }


        public InputStateItem SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (SetProperty(ref _selectedItem, value))
                {
                    UnlockInputCommand.RaiseCanExecuteChanged();
                }
            }
        }


        public DelegateCommand UnlockInputCommand { get; set; }

        private async void UnlockInput()
        {
            await new BalancesServiceProxy().SetInputStateAsync(
                new SetInputStateParameterSet
                {
                    ContractId = _data.FactContract.Id,
                    SiteId = SelectedItem.InputState.SiteId,
                    State = ManualInputState.Input
                });

            Refresh();
        }

    }

    public class InputStateItem
    {
        public InputStateDTO InputState { get; set; }

        public string SiteName { get; set; }
    }
}
