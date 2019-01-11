using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using GazRouter.Application;
using GazRouter.Common.ViewModel;
using JetBrains.Annotations;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
namespace GazRouter.ActionsRolesUsers.ViewModels
{
    [RegionMemberLifetime(KeepAlive = false)]
    [UsedImplicitly]
    public class ActionsRolesUsersViewModel : LockableViewModel, IPageTitle
    {
        public ActionsRolesUsersViewModel()
        {
            UsersViewModel = new UsersViewModel();
            UsersViewModel.PropertyChanged += UsersViewModel_PropertyChanged;
            AgreedUsersViewModel = new AgreedUsersViewModel();
            AgreedUsersViewModel.PropertyChanged += UsersViewModel_PropertyChanged;
            UsersRolesViewModel = new UsersRolesViewModel();
            UsersViewModel.OnRemoveUser += UsersRolesViewModel.ClearRoles;
            RolesViewModel = new RolesViewModel3();
            ActiveSessionsViewModel = new ActiveSessionsViewModel();

            TSListViewModel = new TargetSigningListViewModel();

        }

        public string PageTitle => "Настройки";
        public UsersViewModel UsersViewModel { get; set; }
        public UsersRolesViewModel UsersRolesViewModel { get; set; }
        public AgreedUsersViewModel AgreedUsersViewModel { get; set; }
        public TargetSigningListViewModel TSListViewModel { get; set; }
        public RolesViewModel3 RolesViewModel { get; set; }
        public ActiveSessionsViewModel ActiveSessionsViewModel { get; set; }
        #region HyperlinkClick
        private int _selectedTabIndex;
        public int SelectedTabIndex
        {
            get { return _selectedTabIndex; }
            set
            {
                _selectedTabIndex = value;
                OnPropertyChanged(() => SelectedTabIndex);
            }
        }
        public DelegateCommand<string> HyperlinkClick { get; set; }
        #endregion
        private void UsersViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedUser")
            {
                UsersRolesViewModel.SelectedUser = UsersViewModel.SelectedUser;
            }
        }
    }
    public class ActionContext : PropertyChangedBase
    {
        private bool _isChanged;
        public Visibility Visibility { get; set; }
        public ActionContext ParentContext { get; set; }
        public string Name { get; set; }

        public bool IsChanged
        {
            get { return _isChanged; }
            set
            {
                _isChanged = value;
                OnPropertyChanged(() => IsChanged);

                if (ParentContext != null)
                {
                    ParentContext.IsChanged = _isChanged;
                }

                foreach (var cellContextBase in Values.Values)
                {
                    var cellContext = cellContextBase as HeaderCellContext;
                    if (cellContext != null)
                        cellContext.RaiseCheckedChanged();
                }
            }
        }

        public List<ActionContext> Actions { get; set; }
        public Dictionary<string, CellContextBase> Values { get; set; }
        public bool IsAllowedByDefault { get; set; }

        public bool IsEnabled
        {
            get { return !IsAllowedByDefault; }
        }

        public int ActionId { get; set; }
    }
    public class HeaderCellContext : CellContextBase
    {
        public HeaderCellContext(ActionContext parentContext, int roleId)
            : base(parentContext, roleId)
        {

        }

        public override bool? IsChecked
        {
            get
            {
                if (ParentContext.IsAllowedByDefault)
                    return true;
                if (ParentContext.Actions == null)
                    return false;
                bool allChecked = true;
                bool allUncheked = true;
                foreach (var cell in ParentContext.Actions.SelectMany(row => row.Values.Values.Where(cell => cell.RoleId == RoleId) ))
                {
                    if (cell.IsChecked ?? false)
                        allUncheked = false;
                    else
                        allChecked = false;
                }

                if (allUncheked)
                    return false;
                if (allChecked)
                    return true;
                return null;
            }
            set
            {
                if (value == null)
                    return;
                foreach (ActionContext row in ParentContext.Actions)
                {
                    if (!row.IsEnabled)
                        return;
                    foreach (CellContextBase cell in row.Values.Values.Where(cell => cell.RoleId == RoleId))
                    {
                        cell.IsChecked = value;
                    }
                }
                OnPropertyChanged(() => IsChecked);
            }
        }

        public void RaiseCheckedChanged()
        {
            OnPropertyChanged(() => IsChecked);
        }
    }
    public class CellContext : CellContextBase
    {
        public CellContext(ActionContext parentContext, bool isChecked, int roleId)
            : base(parentContext, roleId)
        {
           
            _isChecked = isChecked;

        }

        protected bool _isChecked;
        public override bool? IsChecked
        {
            get { return _isChecked; }
            set
            {
                if (_isChecked == value)
                    return;

                _isChecked = value ?? false;
                IsChanged = true;

                OnPropertyChanged(() => IsChecked);
                
            }
        }
    }
}