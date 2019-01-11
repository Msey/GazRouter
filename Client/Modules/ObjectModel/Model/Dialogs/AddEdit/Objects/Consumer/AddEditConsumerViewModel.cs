using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GazRouter.DataProviders.Balances;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Balances.DistrNetworks;
using GazRouter.DTO.Dictionaries.ConsumerTypes;
using GazRouter.DTO.Dictionaries.Regions;
using GazRouter.DTO.ObjectModel.Consumers;

namespace GazRouter.ObjectModel.Model.Dialogs.AddEdit.Objects.Consumer
{
	public class AddEditConsumerViewModel : AddEditViewModelBase<ConsumerDTO, Guid>
    {
        private readonly Guid _distrStationId;
        private ConsumerTypesDTO _selectedType;

        public AddEditConsumerViewModel(Action<Guid> actionBeforeClosing, ConsumerDTO model)
            : base(actionBeforeClosing, model)
        {
            _distrStationId = model.DistrStationId;
            _selectedType = TypeList.Single(dto => dto.Id == Model.ConsumerTypeId);
            _selectedRegion = ListRegion.Single(r => r.Id == Model.RegionId);
            UseInBalance = model.UseInBalance;
            Name = Model.Name;

            LoadDistrNetworkList(model.DistrNetworkId);

            SaveCommand.RaiseCanExecuteChanged();
        }

		public AddEditConsumerViewModel(Action<Guid> actionBeforeClosing, Guid distrStationId)
            : base(actionBeforeClosing)
        {
            _distrStationId = distrStationId;
            _selectedType = TypeList.First();
            _selectedRegion = ListRegion.First();

            LoadDistrNetworkList(null);

            SaveCommand.RaiseCanExecuteChanged();
        }

	    

        public List<RegionDTO> ListRegion => ClientCache.DictionaryRepository.Regions;
        
        private RegionDTO _selectedRegion;
        public RegionDTO SelectedRegion
        {
            get { return _selectedRegion; }
            set
            {
                if (SetProperty(ref _selectedRegion, value))
                {
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }


        public List<ConsumerTypesDTO> TypeList => ClientCache.DictionaryRepository.ConsumerTypes;

        public ConsumerTypesDTO SelectedType
        {
            get { return _selectedType; }
            set
            {
                if (SetProperty(ref _selectedType, value))
                {
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public List<DistrNetworkDTO> DistrNetworkList { get; set; }

	    private DistrNetworkDTO _selectedDistrNetwork;

	    public DistrNetworkDTO SelectedDistrNetwork
	    {
	        get { return _selectedDistrNetwork; }
            set { SetProperty(ref _selectedDistrNetwork, value); }
	    }

	    private async void LoadDistrNetworkList(int? selectedNetworkId)
	    {
	        Lock();

	        DistrNetworkList = await new BalancesServiceProxy().GetDistrNetworkListAsync();
            OnPropertyChanged(() => DistrNetworkList);

	        SelectedDistrNetwork = DistrNetworkList.SingleOrDefault(n => n.Id == selectedNetworkId);

	        Unlock();
	    }


        public bool UseInBalance { get; set; }


        protected override string CaptionEntityTypeName => "потребителя";

	    

        protected override bool OnSaveCommandCanExecute()
        {
            return !string.IsNullOrEmpty(Name);
        }

        protected override Task UpdateTask => new ObjectModelServiceProxy().EditConsumerAsync(
            new EditConsumerParameterSet
            {
                Id = Model.Id,
                ParentId = _distrStationId,
                Name = Name,
                ConsumerType = SelectedType.Id,
                RegionId = SelectedRegion.Id,
                DistrNetworkId = SelectedDistrNetwork?.Id,
                UseInBalance = UseInBalance
            });

	    protected override Task<Guid> CreateTask => new ObjectModelServiceProxy().AddConsumerAsync(
	        new AddConsumerParameterSet
	        {
	            Name = Name,
	            ParentId = _distrStationId,
	            ConsumerType = SelectedType.Id,
	            RegionId = SelectedRegion.Id,
                DistrNetworkId = SelectedDistrNetwork?.Id,
                UseInBalance = UseInBalance,
                SortOrder = 0
	        }
	        );
    }
}