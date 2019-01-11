using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.Authorization;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Authorization.User;
using GazRouter.DTO.ObjectModel;
using Microsoft.Practices.Prism.Commands;
namespace GazRouter.ActionsRolesUsers.Dialog.AddUserDialog
{
    public class AddEditUserViewModel : AddEditViewModelBase<UserDTO, int>
    {
#region command
        private DelegateCommand _adUserSearchDialogCommand;
        public DelegateCommand AdUserSearchDialogCommand
        {
            get { return _adUserSearchDialogCommand ?? (
                    _adUserSearchDialogCommand = new DelegateCommand(
                       ShowAdUserSearchDialog,
                        () => true));  
            }
        }
        private AdUserSearchViewModel _adUserSearchviewModel;
        private void ShowAdUserSearchDialog()
        {            
            _adUserSearchviewModel = new AdUserSearchViewModel(Callback);
            var adUserSearchDialog = new AdUserSearchDialog
            {
                DataContext = _adUserSearchviewModel
            };
            adUserSearchDialog.ShowDialog();
        }
        private void Callback()
        {
            if (_adUserSearchviewModel.DialogResult ?? false)
                SelectedUser = _adUserSearchviewModel.SelectedAdUser;
        }
#endregion

#region Fields        
        private string _description;
        private string _phone;
        private Guid? _entitySelectedId;
        private string _fullName;
        private List<CommonEntityDTO> _listSite = new List<CommonEntityDTO>();
        private string _login;
        private CommonEntityDTO _selectedSite;
        private List<AdUserDTO> _adUserList;
        private AdUserDTO _selectedUser;
#endregion

#region Constructors and Destructors
        public AddEditUserViewModel(Action<int> actionBeforeClosing)
            : base(actionBeforeClosing)
        {
	        LoadList();
            SetValidationRules();
        }
        public List<AdUserDTO> AdUserList
        {
            get { return _adUserList; }
            set
            {
                SetProperty(ref _adUserList, value) ;
            }
        }
        public AddEditUserViewModel(Action<int> actionBeforeClosing, UserDTO userDto)
            : base(actionBeforeClosing, userDto)
        {
            Id = userDto.Id;            
            Login = userDto.Login;
            FullName = userDto.UserName;
            _entitySelectedId = userDto.SiteId;
            Description = userDto.Description;
            Phone = userDto.Phone;
	        LoadList();
            SetValidationRules();
        }
#endregion
        private async void LoadList()
		{
			ListSite = await new ObjectModelServiceProxy().GetCurrentEnterpriseAndSitesAsync();
            SelectedSite = ListSite.FirstOrDefault(t => t.Id == _entitySelectedId);
		}
#region Public Properties
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
                OnPropertyChanged(() => Description);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }
        public string FullName
        {
            get
            {
                return _fullName;
            }
            set
            {
                _fullName = value;
                OnPropertyChanged(() => FullName);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public string Phone
        {
            get
            {
                return _phone;
            }
            set
            {
                _phone = value;
                OnPropertyChanged(() => Phone);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public List<CommonEntityDTO> ListSite
        {
            get
            {
                return _listSite;
            }
            set
            {
                if (SetProperty(ref _listSite,value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }
        public string Login
        {
            get
            {
                return _login;
            }
            set
            {
                _login = value;
                OnPropertyChanged(() => Login);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }
        public CommonEntityDTO SelectedSite
        {
            get
            {
                return _selectedSite;
            }
            set
            {
                if (_selectedSite == value) return;
                _selectedSite = value;
                if(_selectedSite != null) SiteId = _selectedSite.Id;
                SaveCommand.RaiseCanExecuteChanged();
                OnPropertyChanged(() => SelectedSite);
            }
        }
        public Guid? SiteId { get; set; }
#endregion

#region Properties
        protected override string CaptionEntityTypeName
        {
            get
            {
                return " пользователя";
            }
        }
        public AdUserDTO SelectedUser
        {
            get { return _selectedUser; }
            set {  SetProperty(ref _selectedUser, value);  }
        }
#endregion

#region Methods
        protected override Task<int> CreateTask
        {
            get
            {
	            var addUserParameterSet = new AddUserParameterSet
		                                      {
			                                      Description = Description,
												  FullName = SelectedUser.DisplayName,
												  Login = SelectedUser.Login,
                                                  Phone = Phone,
		                                      };
	            if (SiteId.HasValue) addUserParameterSet.SiteId = SiteId.Value;
                return new UserManagementServiceProxy().AddUserAsync(addUserParameterSet);
            }
        }
        protected override Task UpdateTask
        {
            get
            {
                return new UserManagementServiceProxy().EditUserAsync(
                    new EditUserParameterSet
                    {
                        Id = Id,
                        UserName = FullName,
                        Description = Description,
                        Phone=Phone,
                        SiteId = SiteId
                    });
            }
        }
        protected override bool OnSaveCommandCanExecute()
        {
            return true;
        }
        private void SetValidationRules()
        {
            if (IsAdd) AddValidationFor(() => SelectedUser).When(() => SelectedUser == null).Show("Не выбран пользователь AD.");
//            AddValidationFor(() => FullName).When(() => string.IsNullOrEmpty(FullName)).Show("Не заполнено ФИО.");
            AddValidationFor(() => Description)
                .When(() => string.IsNullOrEmpty(Description))
                .Show("Не заполнено описание.");
            AddValidationFor(() => SelectedSite)
                .When(() => (SelectedSite == null))
                .Show("Не указано управление.");
        }
#endregion
    }
}
/*
        public Guid? EntitySelectedId
        {
            get
            {
                return _entitySelectedId;
            }
            set
            {
                if (_entitySelectedId == value)
                {
                    return;
                }
                _entitySelectedId = value;
                SiteId = _entitySelectedId;
                SaveCommand.RaiseCanExecuteChanged();
                OnPropertyChanged(() => EntitySelectedId);
            }
        }
*/
