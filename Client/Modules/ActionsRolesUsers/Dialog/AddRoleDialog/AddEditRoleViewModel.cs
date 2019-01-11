using System;
using System.Threading.Tasks;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.Authorization;
using GazRouter.DTO.Authorization.Role;

namespace GazRouter.ActionsRolesUsers.Dialog.AddRoleDialog
{
    public class AddEditRoleViewModel : AddEditViewModelBase<RoleDTO, int>
    {
        public AddEditRoleViewModel(Action<int> actionBeforeClosing)
            : base(actionBeforeClosing)
        {
        }

        #region Description

        private string _description;

        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged(() => Description);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion Description

        protected override string CaptionEntityTypeName => " Роль";

        protected override bool OnSaveCommandCanExecute()
        {
            return !string.IsNullOrEmpty(Name) &&
                   !string.IsNullOrEmpty(Description);
        }

        protected override Task<int> CreateTask => new UserManagementServiceProxy()
            .AddRoleAsync( new AddRoleParameterSet {Description = Description, Name = Name});
    }
}