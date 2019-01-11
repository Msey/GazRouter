using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GazRouter.ActionsRolesUsers.Dialog;
using GazRouter.Application;
using GazRouter.Common;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.Authorization;
using GazRouter.DTO.Authorization.User;
using Microsoft.Practices.Prism.Commands;

namespace GazRouter.ActionsRolesUsers.ViewModels
{
    public class UsersViewModel : LockableViewModel
    {
        public UsersViewModel()
        {
            var editPermission = Authorization2.Inst.IsEditable(LinkType.Permissions);
            AddUserCommand = new DelegateCommand(OnAddUserCommandExecuted, 
                ()=> OnAddUserCommandCanExecuted() && editPermission);
            EditUserCommand = new DelegateCommand(OnEditUserCommandExecuted,
                () => OnEditUserCommandCanExecuted() && editPermission);
            RemoveUserCommand = new DelegateCommand(OnRemoveUserCommandExecuted,
                () => OnRemoveUserCommandCanExecuted() && editPermission);
        }
        public Action OnRemoveUser;


        public async Task LoadUsers()
        {
            Behavior.TryLock();
            try
            {
                var users = await new UserManagementServiceProxy().GetUsersAsync();
                Users = new ObservableCollection<UserDTO>(users);
            }
            finally
            {
                Behavior.TryUnlock();
            }
        }

        public void RefreshCommands()
        {
            EditUserCommand.RaiseCanExecuteChanged();
            RemoveUserCommand.RaiseCanExecuteChanged();
        }

        #region AddUserCommand

        public DelegateCommand AddUserCommand { get; private set; }

        private void OnAddUserCommandExecuted()
        {
            RADialogHelper.AddUser(async id =>
            {
                await LoadUsers();
                SelectedUser = Users.Single(e => e.Id == id);
            });
        }

        private bool OnAddUserCommandCanExecuted()
        {
            return true;
        }

        #endregion AddUserCommand

        #region EditUserCommand

        public DelegateCommand EditUserCommand { get; }

        private void OnEditUserCommandExecuted()
        {
            RADialogHelper.EditUser(async id =>
            {
                await LoadUsers();
                SelectedUser = Users.Single(e => e.Id == id);
            }, SelectedUser);
        }

        private bool OnEditUserCommandCanExecuted()
        {
            return SelectedUser != null;
        }

        #endregion AddUserCommand

        #region RemoveUserCommand

        public DelegateCommand RemoveUserCommand { get; }

        private void OnRemoveUserCommandExecuted()
        {
            if (SelectedUser.Login.Equals(UserProfile.Current.Login))
            {
                MessageBoxProvider.Alert("Невозможно удалить текущего пользователя!", "Предупреждение");
                return;
            }
            MessageBoxProvider.Confirm("Удалить пользователя:" + SelectedUser.Login,
                async confirmed =>
                {
                    if (!confirmed) return;
                    Behavior.TryLock();
                    try
                    {
                        await new UserManagementServiceProxy().RemoveUserAsync(SelectedUser.Id);
                        OnRemoveUser?.Invoke();
                    }
                    finally
                    {
                        Behavior.TryUnlock();
                    }
                    Users.Remove(SelectedUser);
                }, "Удаление");
        }

        private bool OnRemoveUserCommandCanExecuted()
        {
            return SelectedUser != null;
        }

        #endregion RemoveUserCommand

        #region SelectedUser

        private UserDTO _selectedUser;

        public UserDTO SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                _selectedUser = value;
                RefreshCommands();
                OnPropertyChanged(() => SelectedUser);
            }
        }

        #endregion

        #region Users

        private ObservableCollection<UserDTO> _users;

        public ObservableCollection<UserDTO> Users
        {
            get
            {
                if (_users == null)
                {
                    LoadUsers();
                }
                return _users;
            }
            set
            {
                _users = value;
                OnPropertyChanged(() => Users);
            }
        }

        #endregion Users
    }
}