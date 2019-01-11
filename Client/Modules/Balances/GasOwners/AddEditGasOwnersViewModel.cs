using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.Balances;
using GazRouter.DTO.Balances.BalanceGroups;
using GazRouter.DTO.Balances.GasOwners;
using GazRouter.DTO.Dictionaries.GasTransportSystems;


namespace GazRouter.Balances.GasOwners
{
    public class AddEditGasOwnersViewModel : AddEditViewModelBase<GasOwnerDTO, int>
    {
        
        public AddEditGasOwnersViewModel(Action<int> actionBeforeClosing, GasOwnerDTO model)
            : base(actionBeforeClosing, model)
        {
            Name = model.Name;
            Description = model.Description;
            IsLocalContract = model.IsLocalContract;
	        
            OwnerSystemList =
                ClientCache.DictionaryRepository.GasTransportSystems.Select(
                    s => new SystemToOwner(s, model.SystemList.Contains(s.Id)))
                    .ToList();
        }

        public AddEditGasOwnersViewModel(Action<int> actionBeforeClosing)
            : base(actionBeforeClosing)
        {
            OwnerSystemList =
                ClientCache.DictionaryRepository.GasTransportSystems.Select(s => new SystemToOwner(s, false))
                    .ToList();
        }


        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (SetProperty(ref _name, value))
                    RefreshCommands();
            }
        }


        private string _description;
		public string Description
		{
			get { return _description; }
			set
            {
                if(SetProperty(ref _description, value))
				    RefreshCommands();
			}
		}

        public bool IsLocalContract { get; set; }


        public List<SystemToOwner> OwnerSystemList { get; set; }

        

        private void RefreshCommands()
		{
			SaveCommand.RaiseCanExecuteChanged();
		}

		protected override bool OnSaveCommandCanExecute()
		{
			return !string.IsNullOrEmpty(_name);
		}


        protected override Task UpdateTask => Update();

        protected override Task<int> CreateTask => Create();


        private async Task<int> Create()
        {
            var id = await new BalancesServiceProxy().AddGasOwnerAsync(
                new AddGasOwnerParameterSet
                {
                    Name = Name,
                    Description = Description,
                    IsLocalContract = IsLocalContract
                });

            foreach (var sys in OwnerSystemList.Where(s => s.IsChanged))
            {

                await new BalancesServiceProxy().SetGasOwnerSystemAsync(
                    new SetGasOwnerSystemParameterSet
                    {
                        GasOwnerId = id,
                        SystemId = sys.System.Id,
                        IsActive = sys.IsActive
                    });

            }

            return id;
        }


        private async Task Update()
        {
            await new BalancesServiceProxy().EditGasOwnerAsync(
                new EditGasOwnerParameterSet
                {
                    Name = Name,
                    Description = Description,
                    Id = Model.Id,
                    IsLocalContract = IsLocalContract
                });
            

            foreach (var sys in OwnerSystemList.Where(s => s.IsChanged))
            {
                await new BalancesServiceProxy().SetGasOwnerSystemAsync(
                    new SetGasOwnerSystemParameterSet
                    {
                        GasOwnerId = Model.Id,
                        SystemId = sys.System.Id,
                        IsActive = sys.IsActive
                    });
                
            }

            DialogResult = true;
        }


        protected override string CaptionEntityTypeName => "поставщика";


    }


    public class SystemToOwner : PropertyChangedBase
    {
        private readonly bool _initialState;
        private bool _isActive;

        public SystemToOwner(GasTransportSystemDTO system, bool isActive)
        {
            System = system;
            _isActive = isActive;
            _initialState = isActive;
        }

        public GasTransportSystemDTO System { get; set; }

        public bool IsActive
        {
            get { return _isActive; }
            set { SetProperty(ref _isActive, value); }
        }

        public bool IsChanged => _isActive != _initialState;
    }
}
