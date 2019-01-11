using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.Balances;
using GazRouter.DTO.Balances.DistrNetworks;
using GazRouter.DTO.Dictionaries.Regions;

namespace GazRouter.Balances.DistrNetworks
{
    public class AddEditDistrNetworkViewModel : AddEditViewModelBase<DistrNetworkDTO, int>
    {
        private int _sortOrder;


        public AddEditDistrNetworkViewModel(Action<int> actionBeforeClosing, DistrNetworkDTO model)
            : base(actionBeforeClosing, model)
        {
            Name = model.Name;
            _selectedRegion = RegionList.Single(r => r.Id == model.RegionId);
        }

        public AddEditDistrNetworkViewModel(Action<int> actionBeforeClosing, int sortOrder)
            : base(actionBeforeClosing)
        {
            _sortOrder = sortOrder;
            _selectedRegion = RegionList.First();
        }

        public List<RegionDTO> RegionList => ClientCache.DictionaryRepository.Regions;

        private RegionDTO _selectedRegion;

        public RegionDTO SelectedRegion
        {
            get { return _selectedRegion; }
            set { SetProperty(ref _selectedRegion, value); }
        }

        protected override string CaptionEntityTypeName => "ГРО";

        protected override bool OnSaveCommandCanExecute()
        {
            return !string.IsNullOrEmpty(Name);
        }

        protected override Task UpdateTask => new BalancesServiceProxy().EditDistrNetworkAsync(
            new EditDistrNetworkParameterSet
            {
                Id = Model.Id,
                Name = Name,
                RegionId = _selectedRegion.Id
            });

        protected override Task<int> CreateTask => new BalancesServiceProxy().AddDistrNetworkAsync(
            new AddDistrNetworkParameterSet
            {
                Name = Name,
                RegionId = _selectedRegion.Id
            });
    }
}
