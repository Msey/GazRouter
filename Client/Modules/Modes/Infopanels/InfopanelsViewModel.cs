using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using GazRouter.Common;
using GazRouter.Common.ViewModel;
using GazRouter.DTO.Dashboards;
using GazRouter.Modes.ExcelReports;
using GazRouter.Modes.Infopanels.Behaviors;
using GazRouter.Modes.Infopanels.Tree;
using GazRouter.Modes.ProcessMonitoring.Dashboards;
using GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
namespace GazRouter.Modes.Infopanels
{
    /// <summary>
    /// 
    /// функции:
    /// 
    /// 1. OnDashboardChanged
    /// 2. GetView
    /// 3. ActivateRegion
    /// 4. Update
    /// 5. Folders
    /// 6. switch view - messages - будет выполнен переход, с сохранением или без текущего Item!
    /// 
    /// 7. cancelation token!
    /// 
    /// </summary>
    [RegionMemberLifetime(KeepAlive = false)]
    public class InfopanelsViewModel : LockableViewModel, IConfirmNavigationRequest
    {
#region constructor
        public InfopanelsViewModel()
        {
            DashboardRegionName = DashboardRegionConst;
            //
            _permission = new PanelPermission(Authorization2.Inst.IsEditable(LinkType.Infopanel));
            _regions    = (RegionManager)ServiceLocator.Current.GetInstance<IRegionManager>();
            //
            Tree               = new DashboardTreeViewModel(_permission, OnDashboardChanged);
            DashboardViewModel = new DashboardViewModel(_permission) { SaveMessage = GetSaveMessage };
            _reportViewModel   = new ReportViewModel {SaveMessage = GetSaveMessage};
            //
            ServiceLocator.Current.GetInstance<IEventAggregator>()
                .GetEvent<VisualStateChangedEvent>()
                .Subscribe(Tree.ChangeTreeVisibility);
            Tree.ChangeTreeVisibility(true);
            _navigationState = new NavigationState();
        }
#endregion
#region variables
        public const string DashboardRegionConst = "DashboardContentRegion";
        public DashboardTreeViewModel Tree { get; }
        private DashboardViewModel DashboardViewModel { get; }
        private readonly ReportViewModel _reportViewModel;
        //
        public string DashboardRegionName { get; }
        private readonly PanelPermission _permission;
        private readonly RegionManager _regions;
        private ItemBase _currentItemBase;
        private readonly NavigationState _navigationState;        
#endregion
#region events
        /// <summary> OnDashboardChanged
        /// 
        /// последовательность: сохранение происходит перед переключением на другой дашборд! 
        /// 1. проверка несохраненных значений - сообщение
        /// 2. сохранение
        /// 3. переключение на новый объект
        /// 4. установка itemBase как текущего  
        /// 
        /// проверка сохраненного значения нужно делать до вызова переключения
        /// на элемент дерева
        /// 
        /// </summary>
        /// <param name="itemBase"></param>
        private void OnDashboardChanged(ItemBase itemBase)
        {
            Lock();
            if (_currentItemBase != null && _currentItemBase.IsChanged())
                MessageBoxProvider.Confirm(GetSaveMessage(_currentItemBase.Name),
                    async b =>
                    {
                        if (b) await _currentItemBase.Save();
                        LoadView(itemBase);
                    });
            else
                LoadView(itemBase);
        }
        public void ConfirmNavigationRequest(NavigationContext navigationContext, 
                                             Action<bool> continuationCallback)
        {
            if (_currentItemBase == null || !_currentItemBase.IsChanged()) return;
            //
            if (_navigationState.IsInProgress()) return;
            //
            continuationCallback(false);
            _navigationState.Activate();
            //
            MessageBoxProvider.Confirm(GetSaveMessage(_currentItemBase.Name), async isConfirmToSave =>
            {
                if (isConfirmToSave)
                    await _currentItemBase.Save();
                navigationContext.NavigationService.RequestNavigate(navigationContext.Uri, result => continuationCallback(true));
                _navigationState.Deactivate();
            });
        }
#region locomotive
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
        }
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }
        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }
#endregion
#endregion
#region methods
        private async void LoadView(ItemBase itemBase)
        {
            var infoType = GetInfopanelType(itemBase);
            var view = (UserControl)_regions.Regions[DashboardRegionName].GetView(nameof(EmptyView));
            switch (infoType)
            {
                case InfopanelItemType.Dash:
                    view = await DashboardViewModel.GetReportView(itemBase);
                    break;
                case InfopanelItemType.Excel:
                    view = await GetExcelView(itemBase);
                    break;
                case InfopanelItemType.Folder:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            _currentItemBase = itemBase;
            ActivateRegion(view);
            Unlock();
        }
        public void ActivateRegion(UserControl view)
        {
            if (!_regions.Regions[DashboardRegionName].Views.Contains(view))
                _regions.Regions[DashboardRegionName].Add(view);
            _regions.Regions[DashboardRegionName].Activate(view);
        }
        private static InfopanelItemType GetInfopanelType(ItemBase itemBase)
        {
            if (itemBase is DashboardItem) return InfopanelItemType.Dash;
            if (itemBase is ReportItem) return InfopanelItemType.Excel;
            if (itemBase is FolderItem) return InfopanelItemType.Folder;
            throw new Exception("Неизвестный тип в инфопанелях!");
        }
        public string GetSaveMessage(string name)
        {
            return $"Инфопанель \"{_currentItemBase.Name}\" имеет несохраненные изменения.\nСохранить изменения?";
        }
#endregion
#region excel
        public void InitReportView()
        {
            var reportSheet = new ReportSheet { DataContext = _reportViewModel };

            var viewsCollection = _regions.Regions[DashboardRegionConst].Views;
            var regionNames = viewsCollection.Select(e => e.GetType().Name)
                                                                .Distinct().ToDictionary(k => k);
            if (!regionNames.ContainsKey(nameof(ReportSheet)))
                _regions.Regions[DashboardRegionConst].Add(reportSheet, nameof(ReportSheet));
        }
        private async Task<UserControl> GetExcelView(ItemBase itemBase)
        {
            var view = (UserControl)_regions.Regions[DashboardRegionName].GetView(nameof(ReportSheet));

            await  _reportViewModel.SetItem(itemBase);
            _reportViewModel.IsEditable = _permission.IsPanelEditable(itemBase.Id);
            return view;
        }
#endregion
    }
}
#region trash
// var viewModel = (ReportViewModel)view.DataContext;
#endregion
