using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.Authorization;
using GazRouter.DTO.Authorization.User;
using GazRouter.DTO.Repairs.Agreed;
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
    public class AgreedUserSearchViewModel : DialogViewModel
    {

        #region constructor
        public AgreedUserSearchViewModel(Action closeCallback) : base(closeCallback)
        {
            SearchAttribs = new List<string> { "Ф.И.О."};
            SearchAttribItem = SearchAttribs[0];
            Users = new ObservableCollection<AgreedUserDTO>();
        }
        #endregion
        #region variables
        public List<string> SearchAttribs { get; }
        public ObservableCollection<AgreedUserDTO> Users { get; }


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
        private AgreedUserDTO _selectedUser;
        public AgreedUserDTO SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                SetProperty(ref _selectedUser, value);
                _okCommand.RaiseCanExecuteChanged();
            }
        }

        public bool IsUser => false;
        public bool IsAgreedUser => true;

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
                var users = await Searching(SearchString.ToLower());
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
                var users = await Searching(SearchString.ToLower());
                Users.AddRange(users);
                if (Users.Count == 0) MessageBoxProvider.Alert("Учетные записи пользователей не найдены!", "Сообщение");
            }
            finally
            {
                Behavior.TryUnlock();
            }
        }

        private async Task<List<AgreedUserDTO>> Searching(string search_string)
        {
            Behavior.TryLock();
            List<AgreedUserDTO> found_users = new List<AgreedUserDTO>();
            try
            {
                var agreed_users = await new UserManagementServiceProxy().GetAgreedUsersAsync(null);

                found_users.AddRange(agreed_users.Distinct().Where(u => u.FIO != null && u.FIO.ToLower().Contains(search_string)));
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