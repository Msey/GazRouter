using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.Balances;
using GazRouter.DTO.Balances.BalanceGroups;
using GazRouter.DTO.Balances.GasOwners;
using GazRouter.DTO.Dictionaries.GasTransportSystems;
using Microsoft.Practices.Prism.Commands;

namespace GazRouter.Balances.BalanceGroups
{
    public class AddEditBalanceGroupViewModel : AddEditViewModelBase<BalanceGroupDTO, int>
    {
        private int _systemId;
        private int _sortOrder;


        public AddEditBalanceGroupViewModel(Action<int> actionBeforeClosing, BalanceGroupDTO model)
            : base(actionBeforeClosing, model)
        {
            Name = model.Name;
        }

        public AddEditBalanceGroupViewModel(Action<int> actionBeforeClosing, int systemId, int sortOrder)
            : base(actionBeforeClosing)
        {
            _systemId = systemId;
            _sortOrder = sortOrder;
        }


        //private string _name;
        //public string Name
        //{
        //    get { return _name; }
        //    set
        //    {
        //        if (SetProperty(ref _name, value))
        //            RefreshCommands();
        //    }
        //}
        
        
        private void RefreshCommands()
		{
			SaveCommand.RaiseCanExecuteChanged();
		}

        
        protected override string CaptionEntityTypeName => "балансовой группы";

        protected override bool OnSaveCommandCanExecute()
        {
            return !string.IsNullOrEmpty(Name);
        }

        protected override Task UpdateTask => new BalancesServiceProxy().EditBalanceGroupAsync(
            new EditBalanceGroupParameterSet
            {
                Id = Model.Id,
                Name = Name,
                SystemId = Model.SystemId,
                SortOrder = Model.SortOrder
            });

        protected override Task<int> CreateTask => new BalancesServiceProxy().AddBalanceGroupAsync(
            new AddBalanceGroupParameterSet
            {
                Name = Name,
                SystemId = _systemId,
                SortOrder = _sortOrder
            });
    }
}
