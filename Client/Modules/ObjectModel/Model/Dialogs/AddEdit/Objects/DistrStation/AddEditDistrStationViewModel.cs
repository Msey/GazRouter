using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GazRouter.Application.Helpers;
using GazRouter.DataProviders.Balances;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Balances.BalanceGroups;
using GazRouter.DTO.Dictionaries.Regions;
using GazRouter.DTO.ObjectModel.DistrStations;
using Utils.Units;

namespace GazRouter.ObjectModel.Model.Dialogs.AddEdit.Objects.DistrStation
{
    public class AddEditDistrStationViewModel : AddEditViewModelBase<DistrStationDTO>
    {
        private readonly Guid _siteId;
        private double _capacityRated;
        private bool _useInBalance;
        private bool _isForeign;
        private bool _isVirtual;
        private Pressure _pressureRated;
        private int _systemId;

        public AddEditDistrStationViewModel(Action<Guid> actionBeforeClosing, DistrStationDTO model)
            : base(actionBeforeClosing, model)
        {
	        if (model.PressureRated.HasValue) PressureRated = Pressure.FromKgh( model.PressureRated.Value);
			if (model.CapacityRated.HasValue) CapacityRated = model.CapacityRated.Value;
			Id = model.Id;
			Name = model.Name;
            UseInBalance = model.UseInBalance;
            _siteId = model.ParentId.Value;
            _isVirtual = model.IsVirtual;
            _isForeign = model.IsForeign;
            _systemId = model.SystemId;
            SelectedRegion = ListRegion.FirstOrDefault(p => Equals(p.Id, Model.RegionId));
            LoadBalGroupList();
            SetValidationRules();
        }

		public AddEditDistrStationViewModel(Action<Guid> actionBeforeClosing, Guid siteId, int systemId)
            : base(actionBeforeClosing)
        {
            _siteId = siteId;
		    _systemId = systemId;
            SelectedRegion = ListRegion.First();
            LoadBalGroupList();
            SetValidationRules();
        }

        private void SetValidationRules()
        {
            AddValidationFor(() => PressureRated)
                .When(() => ValueRangeHelper.PressureRange.IsOutsideRange(PressureRated))
                .Show(ValueRangeHelper.PressureRange.ViolationMessage);
            
            AddValidationFor(() => CapacityRated)
                .When(() => CapacityRated < 0 || CapacityRated > 2000)
                .Show("Значение должно быть в диапозоне от 0 до 2000");
        }

        public Pressure PressureRated
        {
            get { return _pressureRated; }
            set
            {
                SetProperty(ref _pressureRated, value);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public double CapacityRated
        {
            get { return _capacityRated; }
            set
            {
                SetProperty(ref _capacityRated, value); 
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        protected override string CaptionEntityTypeName => "ГРС";


        public bool UseInBalance
        {
            get { return _useInBalance; }
            set
            {
                if (SetProperty(ref _useInBalance, value))
                {
                    if (!_useInBalance) SelectedBalGroup = null;
                }
            }
        }

        public bool IsForeign
        {
            get { return _isForeign; }
            set { SetProperty(ref _isForeign, value); }
        }

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


        public bool IsVirtual
        {
            get { return _isVirtual; }
            set { SetProperty(ref _isVirtual, value); }
        }

        
        public List<RegionDTO> ListRegion => ClientCache.DictionaryRepository.Regions;

        

        #region SelectedRegion

        private RegionDTO _selectedRegion;

        public RegionDTO SelectedRegion
        {
            get { return _selectedRegion; }
            set
            {
                if (SetProperty(ref _selectedRegion, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }
        #endregion

        protected override bool OnSaveCommandCanExecute()
        {
            ValidateAll();
            return !string.IsNullOrEmpty(Name) && !HasErrors; 
        }

		protected override Task UpdateTask
		{
			get
			{
				return new ObjectModelServiceProxy().EditDistrStationAsync(
					 new EditDistrStationParameterSet
					 {
						 Id = Model.Id,
						 ParentId = _siteId,
						 Name = Name,
                         RegionId = SelectedRegion.Id,
						 CapacityRated = CapacityRated,
						 PressureRated = PressureRated.Kgh,
                         UseInBalance = UseInBalance,
                         BalanceGroupId = SelectedBalGroup?.Id,
                         IsForeign = IsForeign,
						 IsVirtual = IsVirtual
					 });
			}
		}

		protected override Task<Guid> CreateTask
		{
			get
			{
				return new ObjectModelServiceProxy().AddDistrStationAsync(
					new AddDistrStationParameterSet
					{
						Name = Name,
                        RegionId = SelectedRegion.Id,
                        ParentId = _siteId,
						CapacityRated = CapacityRated,
						PressureRated = PressureRated.Kgh,
                        UseInBalance = UseInBalance,
                        BalanceGroupId = SelectedBalGroup?.Id,
                        IsVirtual = IsVirtual,
                        IsForeign = IsForeign
					});
			}
		}
    }
}