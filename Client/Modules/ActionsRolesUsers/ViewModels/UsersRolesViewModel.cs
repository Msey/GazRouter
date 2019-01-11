using System.Collections.ObjectModel;
using System.Linq;
using GazRouter.ActionsRolesUsers.Dialog;
using GazRouter.Common;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.Authorization;
using GazRouter.DTO.Authorization.Role;
using GazRouter.DTO.Authorization.User;
using Microsoft.Practices.Prism.Commands;

namespace GazRouter.ActionsRolesUsers.ViewModels
{
    public class UsersRolesViewModel : LockableViewModel
    {
        public UsersRolesViewModel()
        {
            var editPermission = Authorization2.Inst.IsEditable(LinkType.Permissions);
            AddRoleToUserCommand = new DelegateCommand(OnAddRoleToUserCommandExecuted, 
                    ()=> OnAddRoleToUserCommandCanExecuted() && editPermission);
                RemoveRoleFromUserCommand = new DelegateCommand(OnRemoveRoleFromUserCommandExecuted, 
                    () => OnRemoveRoleFromUserCommandCanExecuted() && editPermission);
        }

        public void ClearRoles()
        {
            SelectedUser = null;
            UserRoles.Clear();
        }

        public void RefreshCommands()
        {
            AddRoleToUserCommand.RaiseCanExecuteChanged();
            RemoveRoleFromUserCommand.RaiseCanExecuteChanged();
        }

        #region SelectedUser

        private UserDTO _selectedUser;

        public UserDTO SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                _selectedUser = value;

                if (SelectedUser != null)
                {
                    LoadUserRoles();
                }

                RefreshCommands();
            }
        }

        #endregion

        #region AddRoleToUserCommand

        public DelegateCommand AddRoleToUserCommand { get; private set; }

        private void OnAddRoleToUserCommandExecuted()
        {
            RADialogHelper.AddRoleToUser(id => LoadUserRoles(), SelectedUser, UserRoles.ToList());
        }

        private async void LoadUserRoles()
        {
             Behavior.TryLock("Загрузка ролей");
            try
            {
                var roles = await new UserManagementServiceProxy().GetUserRolesAsync(SelectedUser.Id);
                UserRoles = new ObservableCollection<RoleDTO>(roles);
            }
            finally 
            {
               Behavior.TryUnlock();
            }
        }

        private bool OnAddRoleToUserCommandCanExecuted()
        {
            return SelectedUser != null;
        }

        #endregion AddRoleToUserCommand

        public DelegateCommand RemoveRoleFromUserCommand { get; }

        private async void OnRemoveRoleFromUserCommandExecuted()
        {
            RoleDTO role = SelectedRole;
            var userRoleParameterSet = new UserRoleParameterSet
            {
                UserId = SelectedUser.Id,
                RoleId = role.Id
            };
            Behavior.TryLock();
            try
            {
                await new UserManagementServiceProxy().RemoveUserRoleAsync(userRoleParameterSet);
                UserRoles.Remove(role);
            }
            finally
            {
                Behavior.TryUnlock();
            }
           /* new UserManagementDataProvider().RemoveUserRole(
                userRoleParameterSet,
                exception =>
                    {
                        if (exception != null) return false;

                        UserRoles.Remove(role);

                        return true;
                    }, Behavior);*/
        }

        private bool OnRemoveRoleFromUserCommandCanExecuted()
        {
            return SelectedUser != null && SelectedRole != null;
        }

        #region SelectedRole

        private RoleDTO _selectedRole;

        public RoleDTO SelectedRole
        {
            get { return _selectedRole; }
            set
            {
                _selectedRole = value;
                OnPropertyChanged(() => SelectedRole);
                RefreshCommands();
            }
        }

        #endregion SelectedRole

        #region UserRoles

        private ObservableCollection<RoleDTO> _userRoles;

        public ObservableCollection<RoleDTO> UserRoles
        {
            get { return _userRoles; }
            set
            {
                _userRoles = value;
                OnPropertyChanged(() => UserRoles);
            }
        }

        #endregion UserRoles
    }
}