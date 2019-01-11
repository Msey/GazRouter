using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using GazRouter.Common;
using GazRouter.Common.Ui.Templates;
using GazRouter.Common.ViewModel;
using GazRouter.DTO.Dashboards.Dashboard;
using GazRouter.DTO.ObjectModel.Sites;
using GazRouter.Modes.Infopanels.Tree;
using Telerik.Windows.Controls;

namespace GazRouter.Modes.Infopanels.ShareDashboard
{
    /// <summary>
    /// 
    /// сохранение происходит для всех значений в 
    /// 
    /// todo: разделить логику:  dash & folders 
    ///                          interface
    /// 
    /// логика загрузки / сохранения
    /// передать тип
    /// 
    /// todo: добавить метод 
    /// 
    /// </summary>
    public class ShareDashboardViewModel2 : AddEditViewModelBase<DashboardDTO, int>
    {
#region constructor
        public ShareDashboardViewModel2(ShareBase shareContext, Action<int> update) 
            : base(update, new DashboardDTO())
        {
            _shareContext = shareContext;
            Permissionses = RegisterItem.GetPermissionWrapper();
            LoadRoles();
        }
#endregion
#region variables
        private readonly ShareBase _shareContext;
        public override string Caption => "Предоставить доступ";
        public ObservableCollection<PermissionWrapper> Permissionses { get; set; }
        protected override string CaptionEntityTypeName { get; }

        private ObservableCollection<RolesPermissionWrapper> _rolesPermissions;
        public ObservableCollection<RolesPermissionWrapper> RolesPermissions
        {
            get { return _rolesPermissions; }
            set
            {
                _rolesPermissions = value;
                OnPropertyChanged(() => RolesPermissions);
            }
        }
#endregion
#region methods
        protected override bool OnSaveCommandCanExecute()
        {
            return true;
        }
        public async void LoadRoles()
        {
            Behavior.TryLock("Загрузка ролей");
            try
            {
                RolesPermissions = await _shareContext.GetRolesPermissions();
            }
            finally
            {
                Behavior.TryUnlock();
            }
        }
        protected override Task UpdateTask => _shareContext.Update(RolesPermissions);
#endregion
    }
    public class RolesPermissionWrapper : PropertyChangedBase
    {
        public RolesPermissionWrapper(SiteDTO dto)
        {            
            _siteDTO = dto;
        }

        private readonly SiteDTO _siteDTO;
        public Guid SiteId => _siteDTO.Id;

        public string Name => _siteDTO.Name;
        public string Description => _siteDTO.Description;

        private PermissionType _permission;
        public PermissionType Permission
        {
            get { return _permission; }
            set
            {
                _permission = value;
                OnPropertyChanged(() => Permission);
            }
        }
    }
    public class PermissionPanelsStyle : StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {
            var permission = (byte)((RolesPermissionWrapper)item).Permission;
            switch (permission)
            {
                case 0: return DeniedStyle;
                case 1: return ReadStyle;
                case 2: return WriteStyle;
                default: return null;
            }
        }
        public Style DeniedStyle { get; set; }
        public Style ReadStyle { get; set; }
        public Style WriteStyle { get; set; }
    }
}

