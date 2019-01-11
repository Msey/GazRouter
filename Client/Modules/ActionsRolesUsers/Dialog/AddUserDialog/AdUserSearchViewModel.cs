using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GazRouter.Application;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.Authorization;
using GazRouter.DTO.Authorization.User;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
namespace GazRouter.ActionsRolesUsers.Dialog.AddUserDialog
{
    public class AdUserSearchViewModel : DialogViewModel
    {
#region constructor
        public AdUserSearchViewModel(Action closeCallback) : base(closeCallback)
        {
            SearchAttribs = new List<string> { "логин", "Ф.И.О.", "описание" };
            SearchAttribItem = SearchAttribs[0];
            //
            AdUsers = new ObservableCollection<AdUserDTO>();
        }
#endregion
#region variables
        public List<string> SearchAttribs { get; }
        public ObservableCollection<AdUserDTO> AdUsers { get; }

        private string _searchAttribItem;
        public string SearchAttribItem {
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
        private string _searchString;
        public string SearchString
        {
            get { return _searchString; }
            set
            {
                SetProperty(ref _searchString, value);                
            }
        }
        private AdUserDTO _selectedAdUser;
        public AdUserDTO SelectedAdUser
        {
            get { return _selectedAdUser; }
            set
            {
                SetProperty(ref _selectedAdUser, value);                
                _okCommand.RaiseCanExecuteChanged();
            }
        }
#endregion
#region commands
        private DelegateCommand _okCommand;
        public DelegateCommand OkCommand => 
            _okCommand ?? (_okCommand = new DelegateCommand(()=> DialogResult = true, ()=> SelectedAdUser != null));
        private DelegateCommand _searchCommand;
        public DelegateCommand SearchCommand => 
            _searchCommand ?? (_searchCommand = new DelegateCommand(Search, ()=> true));
        // - 
        private DelegateCommand _searchCommand2;
        public DelegateCommand SearchCommand2 => 
            _searchCommand2 ?? (_searchCommand2 = new DelegateCommand(DeprecatedSearch, ()=> true));
#endregion
#region methods
        private async void Search()
        {
            Behavior.TryLock();
            try
            {
                SelectedAdUser = null;
                AdUsers.Clear();
                AdUsers.AddRange(await new UserManagementServiceProxy().GetAdUsersTreeAsync(
                        new AdUserFilterParameterSet(SearchString, 
                                                     SearchAttribItem, 
                                                     UserProfile.Current.Site.IsEnterprise)));                
                if (AdUsers.Count == 0) MessageBoxProvider.Alert("Учетные записи пользователей не найдены!", "Сообщение");
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
                SelectedAdUser = null;
                AdUsers.Clear();
                AdUsers.AddRange(await new UserManagementServiceProxy().GetAdUsersFilteredAsync(
                        new AdUserFilterParameterSet(SearchString,
                                                     SearchAttribItem,
                                                     UserProfile.Current.Site.IsEnterprise)));
                if (AdUsers.Count == 0) MessageBoxProvider.Alert("Учетные записи пользователей не найдены!", "Сообщение");
            }
            finally
            {
                Behavior.TryUnlock();
            }
        }
#endregion
    }
}
#region trash
//                ServiceLocator.Current
//                           .GetInstance<IEventAggregator>()
//                           .GetEvent<AddLogEntryEvent>().Publish(new Tuple<string, string>("тест", "тест"));
//                (await new UserManagementServiceProxy().GetDomainsAsync())
//                    .ForEach(e =>
//                    {
//                        ServiceLocator.Current
//                            .GetInstance<IEventAggregator>()
//                            .GetEvent<AddLogEntryEvent>().Publish(new Tuple<string, string>(e, e));
//                    });
#endregion
