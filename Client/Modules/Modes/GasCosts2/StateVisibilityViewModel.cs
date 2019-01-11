using System;
using System.Collections.ObjectModel;
using System.Linq;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.Dashboards;
using GazRouter.DataProviders.GasCosts;
using GazRouter.DTO.GasCosts;
using GazRouter.DTO.ObjectModel.Sites;
using Telerik.Windows.Controls;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Prism.Regions;
namespace GazRouter.Modes.GasCosts2
{
    [RegionMemberLifetime(KeepAlive = true)]
    public class StateVisibilityViewModel : LockableViewModel
    {
#region constructor
        public StateVisibilityViewModel(bool isReadOnly)
        {
            Sites      = new ObservableCollection<SiteDTO>();
            States     = new ObservableCollection<StateItem>();
            InitSites(!isReadOnly);
        }
        private async void InitSites(bool isEnabled)
        {
            _stateBuilder = new StateBuilder();
            var sitesDto = await new DashboardServiceProxy().GetEnterpriseSitesAsync();
            Sites.AddRange(sitesDto);
            //
            var costMap    = await new GasCostsServiceProxy().GetCostTypeListAsync();
            _stateItemRoot = await _stateBuilder.BuildStatesTree(costMap,
                                   ClientCache.DictionaryRepository.GasCostItemGroups,
                                   isEnabled);
            //
            States.AddRange(_stateItemRoot.Items);
            SelectedSite = Sites.First();
        }
#endregion
#region variables
        private StateBuilder _stateBuilder;
        private StateItem _stateItemRoot;
#endregion
#region property
        private ObservableCollection<StateItem> _states;
        public ObservableCollection<StateItem> States
        {
            get { return _states; }
            set { SetProperty(ref _states, value); }
        }
        
        private ObservableCollection<SiteDTO> _sites;
        public ObservableCollection<SiteDTO> Sites
        {
            get { return _sites; }
            set { SetProperty(ref _sites, value); }
        }

        private SiteDTO _selectedSite;
        public SiteDTO SelectedSite
        {
            get { return _selectedSite; }
            set
            {
                SetProperty(ref _selectedSite, value);
                LoadSiteVisibility();
            }
        }

        private bool _isReadOnly;
        public bool IsReadOnly
        {
            get { return _isReadOnly; }
            set
            {
                _isReadOnly = value;
                OnPropertyChanged(() => IsReadOnly);
            }
        }
#endregion
#region methods
        private async void SaveStateVisibility(StateItem item)
        {
            var parameter = new AddGasCostVisibilityParameterSet
            {
                SiteId   = SelectedSite.Id,
                CostType = (int) item.CostType,
            };
            if (item.Visibility) parameter.Visibility = null;
            else parameter.Visibility = 1;
            await new GasCostsServiceProxy().UpdateGasCostVisibilityAsync(parameter).ConfigureAwait(false);
        }
        private async void LoadSiteVisibility()
        {
            _stateBuilder.VisibilityChangedAction = null;
            var visibility  = await new GasCostsServiceProxy().GetGasCostsVisibilityAsync(SelectedSite.Id).ConfigureAwait(false);
            var costVisible = visibility.ToDictionary(k => k.CostType, v => v.Visibility);
            //
            var root = new StateItem("root");
            root.Items.AddRange(States);
            Traversal(root, item =>{ item.Visibility = !costVisible.ContainsKey((int) item.CostType); });
            _stateBuilder.VisibilityChangedAction    = SaveStateVisibility;
        }
#endregion
#region helpers
        public void Traversal<T>(T data, Action<T> action) where T : StateItem
        {
            action.Invoke(data);
            data.Items?.ForEach(item => Traversal((T)item, action));
        }
#endregion
    }
}
#region trash
#endregion
