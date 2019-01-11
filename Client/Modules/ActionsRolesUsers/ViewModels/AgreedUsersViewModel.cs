using GazRouter.ActionsRolesUsers.Dialog;
using GazRouter.Application;
using GazRouter.Common;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.Authorization;
using GazRouter.DTO.Authorization.User;
using GazRouter.DTO.Repairs.Agreed;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace GazRouter.ActionsRolesUsers.ViewModels
{
    public class AgreedUsersViewModel : LockableViewModel
    {
        public AgreedUsersViewModel()
        {
            var editPermission = Authorization2.Inst.IsEditable(LinkType.Permissions);
            AddUserCommand = new DelegateCommand(OnAddUserCommandExecuted,
                () => OnAddUserCommandCanExecuted() && editPermission);
            EditUserCommand = new DelegateCommand(OnEditUserCommandExecuted,
                () => OnEditUserCommandCanExecuted() && editPermission);
            RemoveUserCommand = new DelegateCommand(OnRemoveUserCommandExecuted,
                () => OnRemoveUserCommandCanExecuted() && editPermission);
        }
        public Action OnRemoveUser;


        public async Task LoadAgreedUsers()
        {
            Behavior.TryLock();
            try
            {
                var users = await new UserManagementServiceProxy().GetAgreedUsersAsync(null);
                Users = new ObservableCollection<AgreedUserDTO>(users);
                foreach (var user in Users)
                {
                    var AgreedUser = Users.FirstOrDefault(u => u.AgreedUserId == user.ActingAgreedUserId);
                    if (AgreedUser != null)
                    {
                        user.ActingName = AgreedUser.FIO;
                        user.ActingUserID = AgreedUser.AgreedUserId;
                    }
                    else
                    {
                        user.ActingName = string.Empty;
                        user.ActingUserID = null;
                    }
                }
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
            RADialogHelper.AddAgreedUser(async id =>
            {
                await LoadAgreedUsers();
                SelectedUser = Users.Single(e => e.AgreedUserId == id);
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
            var selected_id = SelectedUser.AgreedUserId;
            RADialogHelper.EditAgreedUser(async id =>
            {
                await LoadAgreedUsers();
                SelectedUser = Users.Single(e => e.AgreedUserId == selected_id);
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
            MessageBoxProvider.Confirm("Удалить согласующего пользователя:" + SelectedUser.FIO,
                async confirmed =>
                {
                    if (!confirmed) return;
                    Behavior.TryLock();
                    try
                    {
                        await new UserManagementServiceProxy().RemoveAgreedUserAsync(SelectedUser.AgreedUserId);
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

        private AgreedUserDTO _selectedUser;

        public AgreedUserDTO SelectedUser
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

        private ObservableCollection<AgreedUserDTO> _users;

        public ObservableCollection<AgreedUserDTO> Users
        {
            get
            {
                if (_users == null)
                {
                    LoadAgreedUsers();
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
