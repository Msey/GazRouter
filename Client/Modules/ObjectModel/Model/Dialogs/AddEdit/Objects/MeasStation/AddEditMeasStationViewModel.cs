using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GazRouter.Application;
using GazRouter.DataProviders.Balances;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Balances.BalanceGroups;
using GazRouter.DTO.Dictionaries.BalanceSigns;
using GazRouter.DTO.Dictionaries.Enterprises;
using GazRouter.DTO.ObjectModel.MeasStations;

namespace GazRouter.ObjectModel.Model.Dialogs.AddEdit.Objects.MeasStation
{
    public class AddEditMeasStationViewModel : AddEditViewModelBase<MeasStationDTO>
    {
        private readonly Guid _parentId;
		private readonly int _sortorder;
        private readonly int _gasTransportSystemId;

        public AddEditMeasStationViewModel(Action<Guid> actionBeforeClosing, MeasStationDTO model)
            : base(actionBeforeClosing, model)
        {
            _parentId = model.ParentId.Value;
            ListBalanceSigns = ClientCache.DictionaryRepository.BalanceSigns;
            ListEnterprises = ClientCache.DictionaryRepository.Enterprises.Where(e => e.Id != Settings.EnterpriseId).ToList();
            SelectedBalanceSign = ListBalanceSigns.Single(dto => dto.BalanceSign == Model.BalanceSignId);
            if (model.NeighbourEnterpriseId.HasValue)
                SelectedEnterprise = ListEnterprises.Single(e => e.Id == model.NeighbourEnterpriseId);
            IsVirtual = model.IsVirtual;
            IsIntermediate = model.IsIntermediate;
            BalanceName = model.BalanceName;
            _gasTransportSystemId = model.SystemId;

            LoadBalGroupList();
        }

		public AddEditMeasStationViewModel(Action<Guid> actionBeforeClosing, Guid parentId, int gasTransportSystemId, int sortorder)
            : base(actionBeforeClosing)
        {
            _parentId = parentId;
			_sortorder = sortorder;
		    _gasTransportSystemId = gasTransportSystemId;
		    ListBalanceSigns = ClientCache.DictionaryRepository.BalanceSigns;
            ListEnterprises = ClientCache.DictionaryRepository.Enterprises.Where(e => e.Id != Settings.EnterpriseId).ToList();
            SelectedBalanceSign = ListBalanceSigns.First();

            LoadBalGroupList();
        }


        protected override string CaptionEntityTypeName => " ГИС";

        private bool _isVirtual;
        public bool IsVirtual
        {
            get { return _isVirtual; }
            set { SetProperty(ref _isVirtual, value); }
        }


        public bool IsOrgSelectorEnabled => !_isIntermediate && _selectedBalanceSign.BalanceSign != Sign.NotUse;
        


        private bool _isIntermediate;
        public bool IsIntermediate
        {
            get { return _isIntermediate; }
            set
            {
                if (SetProperty(ref _isIntermediate, value))
                {
                    if (value) SelectedEnterprise = null;
                    OnPropertyChanged(()=> IsOrgSelectorEnabled);
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }


        #region SelectedBalanceSign

        private BalanceSignDTO _selectedBalanceSign;
        public BalanceSignDTO SelectedBalanceSign
        {
            get { return _selectedBalanceSign; }
            set
            {
                if (SetProperty(ref _selectedBalanceSign, value))
                {
                    if (_selectedBalanceSign.BalanceSign == Sign.NotUse) SelectedEnterprise = null;
                    OnPropertyChanged(() => IsOrgSelectorEnabled);
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        #endregion

        #region ListBalanceSigns

        private List<BalanceSignDTO> _listBalanceSigns = new List<BalanceSignDTO>();
        public List<BalanceSignDTO> ListBalanceSigns
        {
            get { return _listBalanceSigns; }
            private set
            {
                if (_listBalanceSigns == value) return;
                _listBalanceSigns = value;
                OnPropertyChanged(() => ListBalanceSigns);
            }
        }

        #endregion

        #region ListEnterprises

        private List<EnterpriseDTO> _listEnterprises = new List<EnterpriseDTO>();
        public List<EnterpriseDTO> ListEnterprises
        {
            get { return _listEnterprises; }
            private set
            {
                if (_listEnterprises == value) return;
                _listEnterprises = value;
                OnPropertyChanged(() => ListEnterprises);
            }
        }

        #endregion

        #region SelectedEnterprise

        private EnterpriseDTO _selectedEnterprise;
        public EnterpriseDTO SelectedEnterprise
        {
            get { return _selectedEnterprise; }
            set
            {
                if (SetProperty(ref _selectedEnterprise, value))
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
            BalGroupList = await new BalancesServiceProxy().GetBalanceGroupListAsync(_gasTransportSystemId);
            OnPropertyChanged(() => BalGroupList);
            SelectedBalGroup = BalGroupList.SingleOrDefault(g => g.Id == Model.OwnBalanceGroupId);
            OnPropertyChanged(() => SelectedBalGroup);
            Unlock();
        }
        #endregion


        public string BalanceName { get; set; }

        #region SaveCommand

        protected override bool OnSaveCommandCanExecute()
        {
            return !string.IsNullOrEmpty(Name);
        }

        #endregion SaveCommand

		protected override Task UpdateTask
		{
			get
			{
				return new ObjectModelServiceProxy().EditMeasStationAsync(
					new EditMeasStationParameterSet
					{
						Id = Model.Id,
						Name = Name,
                        BalanceName = BalanceName,
						ParentId = _parentId,
                        NeighbourEnterpriseId = SelectedEnterprise == null ? (Guid?)null : SelectedEnterprise.Id,
						IsVirtual = IsVirtual,
						BalanceSignId = SelectedBalanceSign.BalanceSign,
                        BalanceGroupId = SelectedBalGroup?.Id,
                        IsIntermediate = IsIntermediate
					});
			}
		}

		protected override Task<Guid> CreateTask
		{
			get
			{
				return new ObjectModelServiceProxy().AddMeasStationAsync(
					new AddMeasStationParameterSet
					{
						Name = Name,
                        BalanceName = BalanceName,
                        ParentId = _parentId,
                        NeighbourEnterpriseId = SelectedEnterprise == null ? (Guid?)null : SelectedEnterprise.Id,
                        IsVirtual = IsVirtual,
						BalanceSignId = SelectedBalanceSign.BalanceSign,
                        BalanceGroupId = SelectedBalGroup?.Id,
                        IsIntermediate = IsIntermediate,
                        SortOrder = _sortorder
					});
			}
		}
    }
}