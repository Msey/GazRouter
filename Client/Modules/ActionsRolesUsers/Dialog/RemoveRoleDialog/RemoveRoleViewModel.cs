using System;
using System.Collections.Generic;
using System.ServiceModel;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.Authorization;
using GazRouter.DTO.Authorization.Role;
using GazRouter.DTO.Infrastructure.Faults;
using Microsoft.Practices.Prism.Commands;

namespace GazRouter.ActionsRolesUsers.Dialog.RemoveRoleDialog
{
    public class RemoveRoleViewModel : DialogViewModel<Action<int>>
    {
        private List<RoleDTO> _roles;

        private RoleDTO _selectedRole;

        private bool _isLoadingRoles;

        public RemoveRoleViewModel(Action<int> actionBeforeClosing)
            : base(actionBeforeClosing)
        {
            LoadRoles();
            DeleteCommand = new DelegateCommand(OnDeleteCommandExecute, OnDeleteCommandCanExecute);
        }

        public DelegateCommand DeleteCommand { get; }

        public List<RoleDTO> Roles
        {
            get { return _roles; }
            set
            {
                _roles = value;
                OnPropertyChanged(() => Roles);
            }
        }

        public RoleDTO SelectedRole
        {
            get { return _selectedRole; }
            set
            {
                _selectedRole = value;
                OnPropertyChanged(() => SelectedRole);
                DeleteCommand.RaiseCanExecuteChanged();
            }
        }

        public bool IsLoadingRoles
        {
            get { return _isLoadingRoles; }
            set
            {
                _isLoadingRoles = value;
                OnPropertyChanged(() => IsLoadingRoles);
            }
        }

        protected override void InvokeCallback(Action<int> closeCallback)
        {
            closeCallback(SelectedRole.Id);
        }

        private async void LoadRoles()
        {
            try
            {
                Behavior.TryLock();
                var roles = await new UserManagementServiceProxy().GetRolesAsync();
                IsLoadingRoles = false;
                Roles = roles;
            }
            finally
            {
                Behavior.TryUnlock();
            }
        }

        private bool OnDeleteCommandCanExecute()
        {
            return SelectedRole != null;
        }

        private async void OnDeleteCommandExecute()
        {
            if (SelectedRole == null)
            {
                return;
            }

            Behavior.TryLock();
            try
            {
                await new UserManagementServiceProxy().RemoveRoleAsync(SelectedRole.Id);
            }
            catch (FaultException<FaultDetail> ex) when (ex.Detail.FaultType == FaultType.IntegrityConstraint)
            {
                MessageBoxProvider.Alert(ex.ToString(), "Ошибка");
            }
            finally
            {
                Behavior.TryUnlock();
            }
            DialogResult = true;
        }
    }
}