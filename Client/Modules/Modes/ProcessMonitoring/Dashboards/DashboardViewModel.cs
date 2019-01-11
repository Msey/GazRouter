using System;
using System.Threading.Tasks;
using GazRouter.Application.Helpers;
using GazRouter.Common;
using GazRouter.DataProviders.Dashboards;
using GazRouter.DataProviders.SeriesData;
using GazRouter.DTO.Dashboards.Dashboard;
using GazRouter.DTO.Dashboards.DashboardContent;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.SeriesData.PropertyValues;
using GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement;
using GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Container;
using GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Model;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using Utils.Extensions;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.Application;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Controls;
using GazRouter.Common.ViewModel;
using GazRouter.Modes.Infopanels;
using GazRouter.Modes.Infopanels.Tree;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Prism.Events;
using Utils.Units;
namespace GazRouter.Modes.ProcessMonitoring.Dashboards
{    
    /// <summary>
    /// 
    /// содержит старые значения по обновлению
    /// 
    /// </summary>
    public class DashboardViewModel : LockableViewModel
    {
#region constructor
        public DashboardViewModel(PanelPermission permission)
        {
            _permission = permission;
            Timestamp = SeriesHelper.GetLastCompletedSession();
            Data = new DashboardData { KeyDate = Timestamp };
            TreeVisibility = true;
            // 
            InitCommands();
        }
#endregion
#region commands
        public DelegateCommand SaveLayoutCommand { get; private set; }
        public DelegateCommand RefreshLayoutCommand { get; private set; }
        private DelegateCommand<ToTrendCommandParameter> _gotoTrendCommand;
        public DelegateCommand<ToTrendCommandParameter> ToTrendCommand
        {
            get { return _gotoTrendCommand; }
            set
            {
                _gotoTrendCommand = value;
                OnPropertyChanged(() => ToTrendCommand);
            }
        }
        private void InitCommands()
        {
            RefreshLayoutCommand = new DelegateCommand(() =>
            {
                LoadLayout();
                IsEditAllowed = _permission.IsPanelEditable(_selectedDashboard.Id);
            });
            SaveLayoutCommand = new DelegateCommand(async () => { await SaveLayout(); });
        }
        #endregion
        #region variables
        public Func<string, string> SaveMessage;
        private readonly PanelPermission _permission;
        private DashboardDTO _selectedDashboard;
        private string _savedLayout;
        private  DashboardElementView _currentDashboardElementView;
#endregion
#region property
        private DashboardLayout _layout;
        public DashboardLayout Layout
        {
            get { return _layout; }
            set { SetProperty(ref _layout, value); }
        }

        private DashboardData _data;
        public DashboardData Data
        {
            get { return _data; }
            set
            {
                SetProperty(ref _data, value);
            }
        }
        private DateTime _timestamp;
        public DateTime Timestamp
        {
            get { return _timestamp; }
            set
            {
                if (SetProperty(ref _timestamp, value)) LoadData();
            }
        }
        private bool _isEditAllowed;
        public bool IsEditAllowed
        {
            get { return _isEditAllowed; }
            set { SetProperty(ref _isEditAllowed, value); }
        }

        private bool _treeVisibility;
        public bool TreeVisibility
        {
            get { return _treeVisibility; }
            set {
                SetProperty(ref _treeVisibility, value);
                ServiceLocator.Current.GetInstance<IEventAggregator>()
                    .GetEvent<VisualStateChangedEvent>().Publish(value);
                AdjustCanvasSize();
            }
        }
#endregion
#region events
        private bool _isEditMode;
        public bool IsEditMode
        {
            get { return _isEditMode; }
            set
            {
                if (!SetProperty(ref _isEditMode, value)) return;
                SaveLayoutCommand.RaiseCanExecuteChanged();
                //
                OnEditModeChanged();
            }
        }
        public async Task<DashboardElementView> GetReportView(ItemBase itemBase)
        {
            itemBase.IsChanged = IsChanged;
            itemBase.Save = SaveLayout;
            //
            _selectedDashboard = ((DashboardItem)itemBase).Dto;
            _isEditMode = false;
            OnPropertyChanged(()=>IsEditMode);
            IsEditAllowed = _permission.IsPanelEditable(_selectedDashboard.Id);
            await UpdateLayout();
            var dashboardElementView = new DashboardElementView { DataContext = this };
            _currentDashboardElementView = dashboardElementView;

            return dashboardElementView;
        }
        private async void OnEditModeChanged()
        {            
            if (_isEditMode) return;
            // 
            if (IsChanged())
                MessageBoxProvider.Confirm(SaveMessage(_selectedDashboard.DashboardName),
                    async result =>
                    {
                        if (result) await SaveLayout();
                        await UpdateLayout();
                    });
            else
                await UpdateLayout();
        }
        private bool IsChanged()
        {
            if (Layout == null) return false;
            //            
            var layoutSerialize = DashboardLayout.Serialize(Layout);
            var currentHash     = layoutSerialize?.GetHashCode();
            var savedLayout     = _savedLayout?.GetHashCode();
            if (savedLayout     == null) return false;
            // 
            var result = currentHash != savedLayout;
            return result;
        }
        private Task SaveLayout()
        {
            var content = DashboardLayout.Serialize(Layout);
            _savedLayout = content;
            return new DashboardServiceProxy().UpdateDashboardContentAsync(
                new DashboardContentDTO
                {
                    DashboardId = _selectedDashboard.Id,
                    Content = content
                });
        }
        private async Task UpdateLayout()
        {
            Layout = await LoadLayout2();
            Data   = await LoadData2();
            _savedLayout = DashboardLayout.Serialize(Layout);            
        }
        private async void LoadLayout(Action action = null)
        {
            Lock();
            var content = await new DashboardServiceProxy()
                .GetDashboardContentAsync(_selectedDashboard.Id);
            if (!string.IsNullOrEmpty(content.Content))
            {

                

                //                _savedLayout = content.Content;
                Layout = DashboardLayout.Deserialize(content.Content);
                OnPropertyChanged(() => Layout);
                IsEditMode = false;
                LoadData(action);
            }
            else
            {
                Layout = new DashboardLayout();
                OnPropertyChanged(() => Layout);
                action?.Invoke();
            }
            Unlock();
        }
        private async Task<DashboardLayout> LoadLayout2()
        {
            var layout = new DashboardLayout();
            var content = await new DashboardServiceProxy().GetDashboardContentAsync(_selectedDashboard.Id);
            if (string.IsNullOrEmpty(content.Content)) return layout;
            //
            _savedLayout = content.Content;
            layout = DashboardLayout.Deserialize(content.Content);
            return layout;
        }
        private async void LoadData(Action action = null)
        {
            if (!(Layout?.GetRelatedEntityList().Count > 0)) return;
            //
            Lock();
            var measurings = await new SeriesDataServiceProxy().GetEntityPropertyValueListAsync(
                new GetEntityPropertyValueListParameterSet
                {
                    StartDate = Timestamp.AddDays(-1).AddSeconds(10).ToLocal(),
                    EndDate = Timestamp.ToLocal(),
                    EntityIdList = Layout?.GetRelatedEntityList(),
                    PeriodType = PeriodType.Twohours
                });
            Data = new DashboardData
            {
                KeyDate = Timestamp,
                Measurings = GetMeasuringsConverted(measurings)
            };
            OnPropertyChanged(() => Data);
            action?.Invoke();
            Unlock();
        }
        private async Task<DashboardData> LoadData2()
        {
            if (!(Layout?.GetRelatedEntityList().Count > 0)) return null;
            //
            var measurings = await new SeriesDataServiceProxy().GetEntityPropertyValueListAsync(
                new GetEntityPropertyValueListParameterSet
                {
                    StartDate = Timestamp.AddDays(-1).AddSeconds(10).ToLocal(),
                    EndDate = Timestamp.ToLocal(),
                    EntityIdList = Layout?.GetRelatedEntityList(),
                    PeriodType = PeriodType.Twohours
                });
            return new DashboardData { KeyDate = Timestamp, Measurings = GetMeasuringsConverted(measurings) };
        }
        public void AdjustCanvasSize()
        {
            _currentDashboardElementView?.DashboardElementContainer?.AdjustCanvasSize();
        }
#endregion
#region change_pressure
        PressureUnit previousPessureUnit, currentPessureUnit;
        public void OnDimensionChanged()
        {
            currentPessureUnit = UserProfile.Current.UserSettings.PressureUnit;

            if (previousPessureUnit != PressureUnit.Undefined)
            {
                if (currentPessureUnit != previousPessureUnit)
                {
                    LoadLayout(null);
                    IsEditAllowed = _permission.IsPanelEditable(_selectedDashboard.Id);
                    RefreshLayoutCommand.RaiseCanExecuteChanged();
                }
            }
            previousPessureUnit = currentPessureUnit;
        }
        private Dictionary<Guid, Dictionary<PropertyType, List<BasePropertyValueDTO>>>
            GetMeasuringsConverted(Dictionary<Guid, Dictionary<PropertyType, List<BasePropertyValueDTO>>> measurings)
        {
            foreach (var measuring in measurings.Values)
                foreach (var val in measuring)
                    val.Value.OfType<PropertyValueDoubleDTO>().ForEach(v => v.Value = UserProfile.ToUserUnits(v.Value, val.Key));
            return measurings;
        }
#endregion
    }
}
#region trash
#endregion