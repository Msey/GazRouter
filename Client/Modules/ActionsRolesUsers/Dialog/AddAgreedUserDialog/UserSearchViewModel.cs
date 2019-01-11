using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.Authorization;
using GazRouter.DTO.Authorization.User;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
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

namespace GazRouter.ActionsRolesUsers.Dialog.AddAgreedUserDialog
{
    public class UserSearchViewModel : DialogViewModel
    {

        #region constructor
        public UserSearchViewModel(Action closeCallback) : base(closeCallback)
        {
            SearchAttribs = new List<string> { "Ф.И.О.",  "логин"};
            SearchAttribItem = SearchAttribs[0];
            Users = new ObservableCollection<UserDTO>();
        }
        #endregion
        #region variables
        public List<string> SearchAttribs { get; }
        public ObservableCollection<UserDTO> Users { get; }
                

        private string _searchAttribItem;
        public string SearchAttribItem
        {
            get { return _searchAttribItem; }
            set
            {
                if (value == null)
                {
                    SearchAttribItem = SearchAttribs[0];
                    return;
                }
                SetProperty(ref _searchAttribItem, value);
            }
        }
        private string _searchString = string.Empty;
        public string SearchString
        {
            get { return _searchString; }
            set
            {
                SetProperty(ref _searchString, value);
            }
        }
        private UserDTO _selectedUser;
        public UserDTO SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                SetProperty(ref _selectedUser, value);
                _okCommand.RaiseCanExecuteChanged();
            }
        }

        public bool IsUser => true;
        public bool IsAgreedUser => false;

        #endregion
        #region commands
        private DelegateCommand _okCommand;
        public DelegateCommand OkCommand =>
            _okCommand ?? (_okCommand = new DelegateCommand(() => DialogResult = true, () => SelectedUser != null));
        private DelegateCommand _searchCommand;
        public DelegateCommand SearchCommand =>
            _searchCommand ?? (_searchCommand = new DelegateCommand(Search, () => true));
        // - 
        private DelegateCommand _searchCommand2;
        public DelegateCommand SearchCommand2 =>
            _searchCommand2 ?? (_searchCommand2 = new DelegateCommand(DeprecatedSearch, () => true));
        #endregion
        #region methods
        private async void Search()
        {
            Behavior.TryLock();
            try
            {
                SelectedUser = null;
                Users.Clear();
                var users = await Searching(SearchString.ToLower(), SearchAttribItem.Equals(SearchAttribs[0]));
                Users.AddRange(users);
                if (Users.Count == 0) MessageBoxProvider.Alert("Учетные записи пользователей не найдены!", "Сообщение");
            }
            finally
            {
                Behavior.TryUnlock();
            }
        }
        private async void DeprecatedSearch()
        {
            Behavior.TryLock();
            try
            {
                SelectedUser = null;
                Users.Clear();
                var users = await Searching(SearchString.ToLower(), SearchAttribItem.Equals(SearchAttribs[0]));
                Users.AddRange(users);
                if (Users.Count == 0) MessageBoxProvider.Alert("Учетные записи пользователей не найдены!", "Сообщение");
            }
            finally
            {
                Behavior.TryUnlock();
            }
        }

        private async Task<List<UserDTO>> Searching(string search_string, bool search_by_acc)
        {
            Behavior.TryLock();
            List<UserDTO> found_users = new List<UserDTO>();
            try
            {
                var users = await new UserManagementServiceProxy().GetUsersAsync();
                
                if (!search_by_acc)
                    found_users.AddRange(users.Where(u => u.Login.ToLower().Contains(search_string)));
                else
                    found_users.AddRange(users.Where(u => u.UserName.ToLower().Contains(search_string)));
            }
            finally
            {
                Behavior.TryUnlock();
            }
            return found_users;
        }
        #endregion
    }
}