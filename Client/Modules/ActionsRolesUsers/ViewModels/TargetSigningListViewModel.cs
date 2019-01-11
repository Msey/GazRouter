using GazRouter.ActionsRolesUsers.Dialog;
using GazRouter.ActionsRolesUsers.Dialog.AddAgreedUserDialog;
using GazRouter.Application;
using GazRouter.Common;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.Authorization;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Authorization.TargetingList;
using GazRouter.DTO.Authorization.User;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.ObjectModel.Sites;
using GazRouter.DTO.Repairs.Agreed;
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

namespace GazRouter.ActionsRolesUsers.ViewModels
{
    public class TargetSigningListViewModel : LockableViewModel
    {
        public TargetSigningListViewModel()
        {
            var editPermission = Authorization2.Inst.IsEditable(LinkType.Permissions);

            AddUserCommand = new DelegateCommand(AddUser);
            EditUserCommand = new DelegateCommand(EditUser, () => SelectedUser != null);
            DelUserCommand = new DelegateCommand(DelUser, () => SelectedUser != null);

            FillEntityTypes();
            LoadSites();

            LoadUsers();
        }

        public void RefreshCommands()
        {
            EditUserCommand.RaiseCanExecuteChanged();
            DelUserCommand.RaiseCanExecuteChanged();
        }


        private bool _isCpdd = false;
        public bool IsCpdd
        {
            get { return _isCpdd; }
            set
            {
                if (SetProperty(ref _isCpdd, value))
                {
                    OnPropertyChanged(() => IsCpdd);
                    LoadUsers();
                }
            }
        }

        #region Types

        public List<EntityType> AllowedTypes => new List<EntityType>
        {
            EntityType.Pipeline,
            EntityType.DistrStation,
            EntityType.CompShop
        };
        public List<EntityTypeWrapper> AllowedTypesWrapped;

        private EntityTypeWrapper _selectedType;
        public EntityTypeWrapper SelectedType
        { 
            get { return _selectedType; }
            set
            {
                if (SetProperty(ref _selectedType, value))
                {
                    OnPropertyChanged(() => SelectedType);
                    LoadUsers();
                }
            }
        }

        private List<EntityTypeWrapper> _entityTypeList;
        public List<EntityTypeWrapper> EntityTypeList
        {
            get { return _entityTypeList; }
            set
            {
                if (SetProperty(ref _entityTypeList, value))
                {
                    OnPropertyChanged(() => EntityTypeList);
                }
            }
        }

        private void FillEntityTypes()
        {
            var types = ClientCache.DictionaryRepository.EntityTypes.Where(t => AllowedTypes.Contains(t.EntityType)).ToList();
            var res = types.Select(t => new EntityTypeWrapper(t)).ToList();
            AllowedTypesWrapped = types.Select(t => new EntityTypeWrapper(t)).ToList();
            res.Insert(0, new EntityTypeWrapper(null));

            EntityTypeList = res;
        }

        #endregion

        #region Sites

        private bool _showAllSitesAllowed = true;
        public bool ShowAllSitesAllowed
        {
            get { return _showAllSitesAllowed; }
            set
            {
                _showAllSitesAllowed = value;
                OnPropertyChanged(() => _showAllSitesAllowed);
            }
        }
        public List<SiteDTO> SiteList { get; set; }
        private SiteDTO _selectedSite;
        /// <summary>
        /// Выбранное ЛПУ
        /// </summary>
        public SiteDTO SelectedSite
        {
            get { return _selectedSite; }
            set
            {
                if (SetProperty(ref _selectedSite, value))
                {
                    OnPropertyChanged(() => SelectedSite);
                    LoadUsers();
                }
            }
        }

        private async void LoadSites()
        {
            if (UserProfile.Current.Site.IsEnterprise)
            {
                ShowAllSitesAllowed = true;
                SiteList = await new ObjectModelServiceProxy().GetSiteListAsync(
                new GetSiteListParameterSet
                {
                    EnterpriseId = UserProfile.Current.Site.Id
                });
            }
            else
            {
                ShowAllSitesAllowed = false;
                SiteList = await new ObjectModelServiceProxy().GetSiteListAsync(
                new GetSiteListParameterSet
                {
                    SiteId = UserProfile.Current.Site.Id
                });
                foreach (var site in SiteList)
                {
                    if (site.Id == UserProfile.Current.Site.Id)
                    {
                        SelectedSite = site;
                        break;
                    }
                }
            }
            OnPropertyChanged(() => SiteList);
        }

        #endregion

        #region Users

        private TargetingListDTO _selectedUser;
        public TargetingListDTO SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                if (SetProperty(ref _selectedUser, value))
                {
                    OnPropertyChanged(() => SelectedUser);
                    RefreshCommands();
                }
            }
        }

        private List<TargetingListDTO> _users;

        public List<TargetingListDTO> Users
        {
            get { return _users; }
            set
            {
                if (SetProperty(ref _users, value))
                {
                    OnPropertyChanged(() => Users);
                }
            }
        }

        private async void LoadUsers()
        {
            var users  = await new UserManagementServiceProxy().GetTargetingListAsync(
                new GetTargetingListParameterSet
                {
                    EntityTypeId = SelectedType?.Dto.Id,
                    SiteId = SelectedSite?.Id,
                    IsCpdd = IsCpdd
                });
            foreach (var u in users)
            {
                u.EntityName = AllowedTypesWrapped.First(t => u.EntityTypeId == t.Dto.Id).Name;
            }
            Users = users;
        }

        #endregion

        #region Users actions
        public DelegateCommand AddUserCommand { get; }
        public DelegateCommand EditUserCommand { get; }
        public DelegateCommand DelUserCommand { get; }

        
        private void AddUser()
        {
            if (IsCpdd)
            {
                var viewModel = new AddTargetingUserCpddDialogModel(id => LoadUsers(), AllowedTypesWrapped);
                var view = new AddTargetingUserCpddDialog { DataContext = viewModel };
                view.ShowDialog();
            }
            else
            {
                var viewModel = new AddTargetingUserDialogModel(id => LoadUsers(), AllowedTypesWrapped);
                var view = new AddTargetingUserDialog { DataContext = viewModel };
                view.ShowDialog();
            }
        }

        private void EditUser()
        {
            if (IsCpdd)
            {
                var viewModel = new AddTargetingUserCpddDialogModel(id => LoadUsers(), AllowedTypesWrapped, SelectedUser);
                var view = new AddTargetingUserCpddDialog { DataContext = viewModel };
                view.ShowDialog();
            }
            else
            {
                var viewModel = new AddTargetingUserDialogModel(id => LoadUsers(), AllowedTypesWrapped, SelectedUser);
                var view = new AddTargetingUserDialog { DataContext = viewModel };
                view.ShowDialog();
            }
        }

        private void DelUser()
        {
            if (SelectedUser.IsCpdd)
            {
                MessageBoxProvider.Confirm(string.Format("Удалить пользователя: {0}\nДепартамент: {1}\nТип объекта: {2}", SelectedUser.FIO, SelectedUser.Position, SelectedUser.EntityName),
                    async confirmed =>
                    {
                        if (!confirmed) return;
                        Behavior.TryLock();
                        try
                        {
                            await new UserManagementServiceProxy().RemoveUserFromTargetingListAsync(
                                new DeleteTargetingListParametersSet()
                                {
                                    Id = SelectedUser.Id,
                                    IsCpdd = true
                                });
                            LoadUsers();
                        }
                        finally
                        {
                            Behavior.TryUnlock();
                        }
                        Users.Remove(SelectedUser);
                    });
            }
            else
            {
                MessageBoxProvider.Confirm(string.Format("Удалить пользователя: {0}\nЛПУ: {1}\nТип объекта: {2}", SelectedUser.FIO, SelectedUser.SiteName, SelectedUser.EntityName),
                    async confirmed =>
                    {
                        if (!confirmed) return;
                        Behavior.TryLock();
                        try
                        {
                            await new UserManagementServiceProxy().RemoveUserFromTargetingListAsync(
                                new DeleteTargetingListParametersSet()
                                {
                                    Id = SelectedUser.Id,
                                    IsCpdd = false
                                });
                            LoadUsers();
                        }
                        finally
                        {
                            Behavior.TryUnlock();
                        }
                        Users.Remove(SelectedUser);
                    });
            }
        }

        #endregion

    }

    public class EntityTypeWrapper
    {
        public EntityTypeWrapper(EntityTypeDTO typeDto)
        {
            Dto = typeDto;
        }
        public EntityTypeDTO Dto { get; }

        public virtual string Name => Dto?.ShortName ?? "Все";
    }

    public class IntWrapper
    {
        public IntWrapper(int i)
        {
            Value = i;
        }
        public int Value { get; }

        public virtual string Name => Value.ToString();
    }
}
