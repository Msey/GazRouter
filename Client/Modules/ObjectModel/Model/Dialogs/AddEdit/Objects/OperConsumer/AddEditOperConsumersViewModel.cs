using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GazRouter.DataProviders.Balances;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Balances.BalanceGroups;
using GazRouter.DTO.Dictionaries.OperConsumerType;
using GazRouter.DTO.Dictionaries.Regions;
using GazRouter.DTO.ObjectModel.DistrStations;
using GazRouter.DTO.ObjectModel.OperConsumers;


namespace GazRouter.ObjectModel.Model.Dialogs.AddEdit.Objects.OperConsumer
{
    public class AddEditOperConsumersViewModel : AddEditViewModelBase<OperConsumerDTO, Guid>
    {
        private Guid _siteId;
        private int _systemId;
        
        public AddEditOperConsumersViewModel(Action<Guid> closeCallback, Guid siteId, int systemId)
            : base(closeCallback)
        {
            _siteId = siteId;
            _systemId = systemId;
            SetValidationRules();
            LoadDistrStationList();
            LoadBalGroupList();
        }

        public AddEditOperConsumersViewModel(Action<Guid> closeCallback, OperConsumerDTO dto)
            : base(closeCallback, dto)
        {
            _siteId = dto.ParentId.Value;
            _systemId = dto.SystemId;

            Name = dto.Name;
            SelectedConsumerType = ConsumerTypeList.Single(p => p.ConsumerTypes == dto.OperConsumerTypeId);
            IsDirectConnection = dto.IsDirectConnection;
            SelectedRegion = RegionList.First(p => p.Id == dto.RegionId);
            SetValidationRules();
            LoadDistrStationList();
            LoadBalGroupList();
        }

        #region Properties

        public List<OperConsumerTypeDTO> ConsumerTypeList => ClientCache.DictionaryRepository.OperConsumerTypes;
        
        private OperConsumerTypeDTO _selectedConsumerType;
        public OperConsumerTypeDTO SelectedConsumerType
        {
            get { return _selectedConsumerType; }
            set
            {
                if(SetProperty(ref _selectedConsumerType, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }


		public List<RegionDTO> RegionList => ClientCache.DictionaryRepository.Regions;

        private RegionDTO _selectedRegion;
		public RegionDTO SelectedRegion
		{
			get { return _selectedRegion; }
			set
			{
				if(SetProperty(ref _selectedRegion, value))
				    SaveCommand.RaiseCanExecuteChanged();
			}
		}

        
        private bool _isDirectConnection;
        public bool IsDirectConnection
        {
            get { return _isDirectConnection; }
            set
            {
                if (SetProperty(ref _isDirectConnection, value))
                {
                    SaveCommand.RaiseCanExecuteChanged();
                    SelectedDistrStation = null;
                }
            }
        }


        public List<DistrStationDTO> DistStationList { get; set; }

        private async void LoadDistrStationList()
        {
            var tree = await new ObjectModelServiceProxy().GetDistrStationTreeAsync(
                new GetDistrStationListParameterSet
                {
                    SiteId = _siteId
                });
            DistStationList = tree.DistrStations;
            OnPropertyChanged(() => DistStationList);

            if (Model.DistrStationId.HasValue)
                SelectedDistrStation = DistStationList.SingleOrDefault(s => s.Id == Model.DistrStationId.Value);
        }


        private DistrStationDTO _selectedDistrStation;
        public DistrStationDTO SelectedDistrStation
        {
            get { return _selectedDistrStation; }
            set
            {
                if (SetProperty(ref _selectedDistrStation, value))
                {
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        #endregion

        #region Balance group
        public List<BalanceGroupDTO> BalGroupList { get; set; }

        public BalanceGroupDTO SelectedBalGroup { get; set; }

        private async void LoadBalGroupList()
        {
            Lock();
            BalGroupList = await new BalancesServiceProxy().GetBalanceGroupListAsync(_systemId);
            OnPropertyChanged(() => BalGroupList);
            SelectedBalGroup = BalGroupList.SingleOrDefault(g => g.Id == Model.OwnBalanceGroupId);
            OnPropertyChanged(() => SelectedBalGroup);
            Unlock();
        }
        #endregion

        protected override Task<Guid> CreateTask => new ObjectModelServiceProxy().AddOperConsumerAsync(
            new AddEditOperConsumerParameterSet
            {
                ConsumerName = Name,
                SiteId = _siteId,
                ConsumerType = SelectedConsumerType.Id,
                IsDirectConnection = IsDirectConnection,
                DistrStationId = SelectedDistrStation?.Id,
                RegionId = IsDirectConnection ? SelectedRegion.Id : SelectedDistrStation.RegionId,
                BalanceGroupId = SelectedBalGroup?.Id
            });

        protected override Task UpdateTask => new ObjectModelServiceProxy().EditOperConsumerAsync(
            new AddEditOperConsumerParameterSet
            {
                Id = Model.Id,
                ConsumerName = Name,
                SiteId = _siteId,
                ConsumerType = SelectedConsumerType.Id,
                IsDirectConnection = IsDirectConnection,
                DistrStationId = SelectedDistrStation?.Id,
                RegionId = IsDirectConnection ? SelectedRegion.Id : SelectedDistrStation.RegionId,
                BalanceGroupId = SelectedBalGroup?.Id
            });

        protected override string CaptionEntityTypeName => "объекта ПЭН";

        protected override bool OnSaveCommandCanExecute()
        {
            ValidateAll();
            return !HasErrors;
        }

        private void SetValidationRules()
        {
            AddValidationFor(() => Name).When(() => string.IsNullOrEmpty(Name)).Show("Отсутствует наименование");
            AddValidationFor(() => SelectedConsumerType).When(() => SelectedConsumerType == null).Show("Не выбран тип");
            AddValidationFor(() => SelectedRegion).When(() => IsDirectConnection && SelectedRegion == null).Show("Не выбран регион");
            AddValidationFor(() => SelectedDistrStation).When(() => !IsDirectConnection && SelectedDistrStation == null).Show("Не выбрана ГРС");
        }
    }
}
