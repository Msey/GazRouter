using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GazRouter.ActionsRolesUsers.Dialog;
using GazRouter.Common;
using GazRouter.Common.Ui.Behaviors;
using GazRouter.Common.Ui.Templates;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.Authorization;
using GazRouter.DTO.Authorization.Role;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Prism.Commands;
namespace GazRouter.ActionsRolesUsers.ViewModels
{
    public class RolesViewModel3 : LockableViewModel, ITabItem
    {
#region constructor
        public RolesViewModel3()
        {
            var editPermission = Authorization2.Inst.IsEditable(LinkType.Permissions);
            LoadRolesAndPermissions();
            Permissionses = RegisterItem.GetPermissionWrapper();
            RoleMenu = new ObservableCollection<RegisterItem>();
            InitCommands(editPermission);
            IsChanged = false;
            _changedPermissions = new List<PermissionDTO>();
        }
#endregion
#region register
        private RegisterItem _baseRegister;
        private readonly List<PermissionDTO> _changedPermissions;

        private ObservableCollection<RegisterItem> _roleMenu;
        public ObservableCollection<RegisterItem> RoleMenu
        {
            get { return _roleMenu; }
            set
            {
                _roleMenu = value;
                OnPropertyChanged(() => RoleMenu);
            }
        }

        public ObservableCollection<PermissionWrapper> Permissionses { get; set; }
#endregion
#region roles
        private RoleDTO _selectedRole;
        public RoleDTO SelectedRole
        {
            get { return _selectedRole; }
            set
            {
                _selectedRole = value;
                OnPropertyChanged(() => SelectedRole);
                //
                if (value == null)
                {
                    RoleMenu = null;
                    return;
                }
                UpdateMenuItems(_selectedRole.Id, _baseRegister);
                RoleMenu = _baseRegister.Items;
            }
        }

        private ObservableCollection<RoleDTO> _roles;
        public ObservableCollection<RoleDTO> Roles
        {
            get { return _roles; }
            set
            {
                _roles = value;
                OnPropertyChanged(() => Roles);
            }
        }
#endregion
#region commands
        private void InitCommands(bool editPermission)
        {
            AddRoleCommand = new DelegateCommand(OnAddRoleCommandExecuted,
                () => OnAddRoleCommandCanExecuted() && editPermission);
            RemoveRoleCommand = new DelegateCommand(OnRemoveRoleCommandExecuted,
                () => OnRemoveRoleCommandCanExecuted() && editPermission);
            SaveRolesCommand = new DelegateCommand(OnSaveRolesCommandExecuted,
                () => OnSaveRolesCommandCanExecuted() && editPermission);
        }
        // addRoleCommand
        public DelegateCommand AddRoleCommand { get; private set; }
        private void OnAddRoleCommandExecuted()
        {
            RADialogHelper.AddRole(id => LoadRoles());
            IsChanged = true;
        }
        private bool OnAddRoleCommandCanExecuted()
        {
            return true;
        }
        // removeRoleCommand
        public DelegateCommand RemoveRoleCommand { get; private set; }
        private void OnRemoveRoleCommandExecuted()
        {
            RADialogHelper.RemoveRole(RemoveRoleAndReload);
        }
        private void RemoveRoleAndReload(int roleId)
        {
            _changedPermissions.RemoveAll(e => e.RoleId == roleId);            
            LoadRoles();
            IsChanged = true;
        }
        private bool OnRemoveRoleCommandCanExecuted()
        {
            return true;
        }
        // saveRolesCommand
        public DelegateCommand SaveRolesCommand { get; private set; }
        private async void OnSaveRolesCommandExecuted()
        {
            Behavior.TryLock("Сохранение привилегий ролей");
            try
            {
                await new UserManagementServiceProxy().UpdatePermissionsAsync(_changedPermissions);
                _changedPermissions.Clear();
                SaveRolesCommand.RaiseCanExecuteChanged();
            }
            finally
            {
                Behavior.TryUnlock();
            }
            IsChanged = false;
        }
        private bool OnSaveRolesCommandCanExecuted()
        {
            return IsChanged && _changedPermissions.Count > 0;
        }
        //
        private bool _isChanged;
        public bool IsChanged
        {
            get { return _isChanged; }
            set
            {
                _isChanged = value;
                OnPropertyChanged(() => IsChanged);
            }
        }
#endregion
#region events
        public void Activate()
        {
            // MessageBoxProvider.Alert("RolesViewModel2", "Activate");
        }
        public void Deactivate()
        {
            // MessageBoxProvider.Alert("RolesViewModel2", "Deactivate");
        }
#endregion
#region methods
        public async void LoadRoles()
        {
            Behavior.TryLock("Загрузка ролей");
            try
            {
                var roles = await new UserManagementServiceProxy().GetRolesAsync();
                Roles = new ObservableCollection<RoleDTO>(roles);
            }
            finally
            {
                Behavior.TryUnlock();
            }
        }
        public async void LoadRolesAndPermissions()
        {
            RegisterItem.IsChanged = null;
            Behavior.TryLock("Загрузка ролей пользователя");
            try
            {
                var roles = await new UserManagementServiceProxy().GetRolesAsync();
                Roles = new ObservableCollection<RoleDTO>(roles);
                //
                _baseRegister = GetBaseRegister(Authorization2.Inst.Menu);
                _changedPermissions.Clear();
            }
            finally{ Behavior.TryUnlock(); }
            RegisterItem.IsChanged = ChangeSaveEnable;
        }
        private void ChangeSaveEnable(RegisterItem item)
        {
            IsChanged = true;
            var changedItem = _changedPermissions.FirstOrDefault(e => e.RoleId == item.RoleId && 
                                                                      e.ItemId == item.Id);
            if (changedItem == null)
                _changedPermissions.Add(new PermissionDTO
                {
                    ItemId     = item.Id,
                    RoleId     = item.RoleId,
                    Name       = item.Name,
                    Permission = (int)item.Permission
                });
            else changedItem.Permission = (int)item.Permission;
            //
            SaveRolesCommand.RaiseCanExecuteChanged();
        }
#region roleManagement
        private async void UpdateMenuItems(int roleId, RegisterItem baseRegister)
        {
            RegisterItem.IsChanged = null;
            var permissions = (await new UserManagementServiceProxy().GetPermissionsByRoleIdAsync(roleId)).ToDictionary(k => k.ItemId);
            var changedPermissions = _changedPermissions.ToLookup(k => k.RoleId);
            var changedForRole     = changedPermissions[roleId].ToDictionary(k => k.ItemId);
            Traversal(baseRegister, item => 
            {
                if (changedForRole.ContainsKey(item.Id))
                    item.Permission = RegisterItem.GetPermissionType(changedForRole[item.Id].Permission);
                else
                    item.Permission = permissions.ContainsKey(item.Id) ? 
                        RegisterItem.GetPermissionType(permissions[item.Id].Permission) :
                        PermissionType.Hidden;
                item.RoleId = roleId;
            });
            RegisterItem.IsChanged = ChangeSaveEnable;
        }
        private static RegisterItem GetBaseRegister(PermissionNode menu)
        {
            var regItems = new Dictionary<int, RegisterItem>();
            MenuTraversal(menu, item =>
            {
                var parentId = (int) item.ParentLinkType;
                var regItem = new RegisterItem(item.Name, (int)item.LinkType);
                if (regItems.ContainsKey(parentId))
                {
                    var parent = regItems[parentId];
                    regItem.SetParent(parent);
                    parent.Items.Add(regItem);
                }
                else
                {
                    if (regItems.Count > 0)
                    {
                        var parent = regItems.First().Value;
                        regItem.SetParent(parent);
                        parent.Items.Add(regItem);
                    }
                }
                regItems.Add(regItem.Id, regItem);
            });
            return regItems.First().Value;
        }
        private static void MenuTraversal(PermissionNode node, Action<PermissionNode> action)
        {
            action.Invoke(node);
            if (node is Leaf) return;            
            //
            ((Branch)node).Childs.ForEach(item => MenuTraversal(item, action));
        }
        private static void Traversal(RegisterItem data, Action<RegisterItem> action)
        {
            action.Invoke(data);
            if (data.Items == null) return;
            //
            foreach (var item in data.Items) Traversal(item, action);
        }
#endregion
#endregion
    }
}