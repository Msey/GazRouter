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
using GazRouter.ActionsRolesUsers.Dialog.AddUserDialog;
using GazRouter.DTO.Repairs.Agreed;
using System.Collections.ObjectModel;

namespace GazRouter.ActionsRolesUsers.Dialog.AddAgreedUserDialog
{
    public class AddEditAgreedUserViewModel : AddEditViewModelBase<AgreedUserDTO, int>
    {
        #region command
        private DelegateCommand _userSearchDialogCommand;
        public DelegateCommand UserSearchDialogCommand
        {
            get
            {
                return _userSearchDialogCommand ?? (
                  _userSearchDialogCommand = new DelegateCommand(
                     ShowUserSearchDialog,
                      () => true));
            }
        } 

        private DelegateCommand _actingUserSearchDialogCommand;
        public DelegateCommand ActingUserSearchDialogCommand
        {
            get
            {
                return _actingUserSearchDialogCommand ?? (
                  _actingUserSearchDialogCommand = new DelegateCommand(
                     ShowActingUserSearchDialog,
                      () => true));
            }
        }
        private UserSearchViewModel _userSearchviewModel;
        private void ShowUserSearchDialog()
        {
            _userSearchviewModel = new UserSearchViewModel(Callback);
            var userSearchDialog = new UserSearchDialog
            {
                DataContext = _userSearchviewModel
            };
            userSearchDialog.ShowDialog();
        }

        private AgreedUserSearchViewModel _actingUserSearchviewModel;
        private void ShowActingUserSearchDialog()
        {
            _actingUserSearchviewModel = new AgreedUserSearchViewModel(Callback2);
            var userSearchDialog = new UserSearchDialog
            {
                DataContext = _actingUserSearchviewModel
            };
            userSearchDialog.ShowDialog();
        }
        private void Callback()
        {
            if (_userSearchviewModel.DialogResult ?? false)
            {
                SelectedUser = _userSearchviewModel.SelectedUser;
                FullName = _userSearchviewModel.SelectedUser.UserName;
            }
        }
        private void Callback2()
        {
            if (_actingUserSearchviewModel.DialogResult ?? false)
                SelectedActingUser = _actingUserSearchviewModel.SelectedUser;
        }
        #endregion

        #region Fields        
        private int Id;
        private string _fullName;
        #endregion

        #region Constructors and Destructors
        public AddEditAgreedUserViewModel(Action<int> actionBeforeClosing)
            : base(actionBeforeClosing)
        {
            StartDate = DateTime.Now;
            EndDate = DateTime.Now.AddMonths(1);
            SetValidationRules();
        }
        public AddEditAgreedUserViewModel(Action<int> actionBeforeClosing, AgreedUserDTO userDto)
            : base(actionBeforeClosing, userDto)
        {
            Id = userDto.AgreedUserId;
            SelectedUser = new UserDTO();
            SelectedUser.Id = userDto.UserID;
            SelectedUserPosition = userDto.Position;
            StartDate = userDto.StartDate;
            EndDate = userDto.EndDate;
            FullName = userDto.FIO;

            Position_r = userDto.Position_r;
            FIO_r = userDto.FIO_R;
            IsHead = userDto.IsHead;

            if (userDto.ActingUserID != null)
            {
                SelectedActingUser = new AgreedUserDTO();
                SelectedActingUser.AgreedUserId = (int)userDto.ActingUserID;
                SelectedActingUser.FIO = userDto.ActingName;
            }
            SetValidationRules();
        }
        #endregion
        #region Public Properties

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
                FIO_r = _fullName;
                SaveCommand.RaiseCanExecuteChanged();
            }
        }
        
        #endregion

        #region Properties
        protected override string CaptionEntityTypeName
        {
            get
            {
                return " пользователя";
            }
        }
        
        private UserDTO _selectedUser;
        public UserDTO SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                if (SetProperty(ref _selectedUser, value))
                {
                    SelectedUserPosition = _selectedUser.Description;
                    OnPropertyChanged(() => SelectedUserPosition);
                }
            }
        }

        private string _selectedUserPosition;
        public string SelectedUserPosition
        {
            get { return _selectedUserPosition; }
            set {
                if (SetProperty(ref _selectedUserPosition, value))
                {
                    Position_r = _selectedUserPosition;
                }
            }
        }

        private string _fio_r;
        public string FIO_r
        {
            get { return _fio_r; }
            set
            {
                if (SetProperty(ref _fio_r, value))
                {

                }
            }
        }

        private string _position_r;
        public string Position_r
        {
            get { return _position_r; }
            set {
                if (SetProperty(ref _position_r, value))
                {

                }
            }
        }

        private bool _isHead;
        public bool IsHead
        {
            get { return _isHead; }
            set
            {
                if (SetProperty(ref _isHead, value))
                {

                }
            }
        }

        private AgreedUserDTO _selectedActingUser;
        public AgreedUserDTO SelectedActingUser
        {
            get { return _selectedActingUser; }
            set { SetProperty(ref _selectedActingUser, value); }
        }
        
        private DateTime _start_date;
        public DateTime StartDate
        {
            get { return _start_date; }
            set { SetProperty(ref _start_date, value); }
        }

        private DateTime _end_date;
        public DateTime EndDate
        {
            get { return _end_date; }
            set { SetProperty(ref _end_date, value); }
        }
        #endregion

        #region Methods

        protected override Task<int> CreateTask
        {
            get
            {
                int? actingId = null;
                if (SelectedActingUser != null) actingId = SelectedActingUser.AgreedUserId;
                var addUserParameterSet = new AddAgreedUserParameterSet
                {
                    UserId = SelectedUser.Id,
                    UserName = FullName,
                    Position = SelectedUserPosition,
                    StartDate = StartDate,
                    EndtDate = EndDate,
                    ActingId = actingId,
                    UserName_R = FIO_r,
                    Position_R = Position_r,
                    IsHead = IsHead
                };
                return new UserManagementServiceProxy().AddAgreedUserAsync(addUserParameterSet);
            }
        }
        protected override Task UpdateTask
        {
            get
            {
                int? actingId = null;
                if (SelectedActingUser != null) actingId = SelectedActingUser.AgreedUserId;
                return new UserManagementServiceProxy().EditAgreedUserAsync(
                    new EditAgreedUserParameterSet
                    {
                        AgreedUserId = Id,
                        UserId = SelectedUser.Id,
                        UserName = FullName,
                        Position = SelectedUserPosition,
                        StartDate = StartDate,
                        EndtDate = EndDate,
                        ActingId = actingId,
                        UserName_R = FIO_r,
                        Position_R = Position_r,
                        IsHead = IsHead
                    });
            }
        }
        protected override bool OnSaveCommandCanExecute()
        {
            return true;
        }
        private void SetValidationRules()
        {
            if (IsAdd) AddValidationFor(() => FullName).When(() => SelectedUser == null).Show("Не выбран пользователь.");
            AddValidationFor(() => FullName).When(() => string.IsNullOrEmpty(FullName)).Show("Не описано полное имя пользователя.");
            AddValidationFor(() => FIO_r).When(() => string.IsNullOrEmpty(FIO_r)).Show("Не описано ФИО в формате \"Кому\"");
            AddValidationFor(() => SelectedUserPosition).When(() => string.IsNullOrEmpty(SelectedUserPosition)).Show("Не описана должность пользователя.");
            AddValidationFor(() => Position_r).When(() => string.IsNullOrEmpty(Position_r)).Show("Не описана должность в формате \"Кому\"");
        }
        #endregion
    }
}