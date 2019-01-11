using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.Authorization;
using GazRouter.DTO.Authorization.Role;
using GazRouter.DTO.Authorization.User;

namespace GazRouter.ActionsRolesUsers.Dialog.AddRoleToUserDialog
{
    public class AddRoleToUserViewModel : AddEditViewModelBase<UserDTO, int>
    {
        public AddRoleToUserViewModel(Action<int> actionBeforeClosing)
            : base(actionBeforeClosing, new UserDTO())
        {
            LoadRoles();
        }

        private async void LoadRoles()
        {

            try
            {
                Behavior.TryLock();
                var roles =await new UserManagementServiceProxy().GetRolesAsync();
                IsLoadingRoles = false;
                Roles = roles;
            }
            finally
            {
                Behavior.TryUnlock();
            }
            IsLoadingRoles = false;
        }

        protected override string CaptionEntityTypeName => " Роль";


        protected override bool OnSaveCommandCanExecute()
        {
            return SelectedRole != null;
        }

        protected override Task UpdateTask => new UserManagementServiceProxy().AddUserRoleAsync(
            new UserRoleParameterSet { RoleId = SelectedRole.Id, UserId = SelectedUser.Id });

        private List<RoleDTO> _roles;

        public List<RoleDTO> Roles
        {
            get { return _roles; }
            set
            {
               if (SetProperty(ref _roles, value))
                    OnPropertyChanged(() => AvaliableRoles);
            }
        }

        public List<RoleDTO> AvaliableRoles
        {
            get
            {
                if (Roles == null || Roles.Count == 0) return null;

                if (UserRoles == null || UserRoles.Count == 0) return Roles;

                RoleDTO[] userRoles = Roles.Join(UserRoles, r1 => r1.Id, r2 => r2.Id, (r1, r2) => r1).ToArray();

                return Roles.Except(userRoles).ToList();
            }
        }

        private List<RoleDTO> _userRoles;

        public List<RoleDTO> UserRoles
        {
            get { return _userRoles; }
            set
            {
                _userRoles = value;
                OnPropertyChanged(() => UserRoles);
                OnPropertyChanged(() => AvaliableRoles);
            }
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
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion SelectedRole

        #region SelectedUser

        private UserDTO _selectedUser;

        public UserDTO SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                SetProperty(ref _selectedUser, value);
            }
        }

        #endregion SelectedUser

        private bool _isLoadingRoles;

        public bool IsLoadingRoles
        {
            get { return _isLoadingRoles; }
            set
            {
                _isLoadingRoles = value;
                OnPropertyChanged(() => IsLoadingRoles);
            }
        }
    }
}