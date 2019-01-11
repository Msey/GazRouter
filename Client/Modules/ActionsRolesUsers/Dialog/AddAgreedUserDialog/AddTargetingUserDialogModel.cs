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
using GazRouter.ActionsRolesUsers.ViewModels;
using GazRouter.DTO.Authorization.TargetingList;

namespace GazRouter.ActionsRolesUsers.Dialog.AddAgreedUserDialog
{
    public class AddTargetingUserDialogModel : AddEditViewModelBase<TargetingListDTO, int>
    {
        #region command

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

        private void Callback2()
        {
            if (_actingUserSearchviewModel.DialogResult ?? false)
                SelectedActingUser = _actingUserSearchviewModel.SelectedUser;
        }
        #endregion

        #region Fields        
        private int Id;

        protected override string CaptionEntityTypeName
        {
            get
            {
                return " пользователя";
            }
        }
        #endregion

        #region Constructors and Destructors
        public AddTargetingUserDialogModel(Action<int> actionBeforeClosing, List<EntityTypeWrapper> AllowedTypesWrapped)
            : base(actionBeforeClosing)
        {
            OrderList = orders;
            IsAdd = true;
            EntityTypeList = AllowedTypesWrapped;

            SetValidationRules();
        }
        public AddTargetingUserDialogModel(Action<int> actionBeforeClosing, List<EntityTypeWrapper> AllowedTypesWrapped, TargetingListDTO SelectedUser)
            : base(actionBeforeClosing, SelectedUser)
        {
            OrderList = orders;
            EntityTypeList = AllowedTypesWrapped;
            Id = SelectedUser.Id;
            SelectedActingUser = new AgreedUserDTO() { FIO = SelectedUser.FIO, AgreedUserId = SelectedUser.AgreedUserId };
            try
            {
                SelectedType = EntityTypeList.First(t => t.Dto.Id == SelectedUser.EntityTypeId);
            }
            catch { }
            try
            {
                SelectedOrder = orders.First(o => o.Value == SelectedUser.SortOrder);
            }
            catch { }
            SetValidationRules();
        }
        #endregion

        public string FIO { get; set; }

        private AgreedUserDTO _selectedActingUser;
        public AgreedUserDTO SelectedActingUser
        {
            get { return _selectedActingUser; }
            set
            {
                if (SetProperty(ref _selectedActingUser, value))
                {
                    OnPropertyChanged(() => SelectedActingUser);
                    FIO = SelectedActingUser.FIO;
                }
            }
        }

        private EntityTypeWrapper _selectedType;
        public EntityTypeWrapper SelectedType
        {
            get { return _selectedType; }
            set
            {
                if (SetProperty(ref _selectedType, value))
                {
                    OnPropertyChanged(() => SelectedType);
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


        private IntWrapper _selectedOrder;
        public IntWrapper SelectedOrder
        {
            get { return _selectedOrder; }
            set
            {
                if (SetProperty(ref _selectedOrder, value))
                {
                    OnPropertyChanged(() => SelectedOrder);
                }
            }
        }

        private List<IntWrapper> _oderList;
        public List<IntWrapper> OrderList
        {
            get { return _oderList; }
            set
            {
                if (SetProperty(ref _oderList, value))
                {
                    OnPropertyChanged(() => OrderList);
                }
            }
        }
        private List<IntWrapper> orders = new List<IntWrapper>
        {
            new IntWrapper(1),
            new IntWrapper(2),
            new IntWrapper(3),
            new IntWrapper(4),
            new IntWrapper(5),
            new IntWrapper(6),
            new IntWrapper(7),
            new IntWrapper(8),
            new IntWrapper(9),
        };



        #region Methods

        protected override Task<int> CreateTask
        {
            get
            {
                var addUserParameterSet = new AddEditTargetListUserParameterSet
                {
                    AgreedUserId = SelectedActingUser.AgreedUserId,
                    EntityTypeId = SelectedType.Dto.Id,
                    SortNum = SelectedOrder.Value,
                    IsCpdd = false

                };
                return new UserManagementServiceProxy().AddUserToTargetingListAsync(addUserParameterSet);
            }
        }
        protected override Task UpdateTask
        {
            get
            {
                return new UserManagementServiceProxy().EditUserInTargetingListAsync(
                    new AddEditTargetListUserParameterSet
                    {
                        Id = Id,
                        AgreedUserId = SelectedActingUser.AgreedUserId,
                        EntityTypeId = SelectedType.Dto.Id,
                        SortNum = SelectedOrder.Value,
                        IsCpdd = false
                    });
            }
        }
        protected override bool OnSaveCommandCanExecute()
        {
            return true;
        }
        private void SetValidationRules()
        {
            if (IsAdd) AddValidationFor(() => SelectedActingUser).When(() => SelectedActingUser == null).Show("Не выбран пользователь.");
            AddValidationFor(() => SelectedType).When(() => SelectedType == null).Show("Не указан тип объекта.");
            AddValidationFor(() => SelectedOrder).When(() => SelectedOrder == null).Show("Не указана очередность вывода.");
        }
        #endregion
    }
}
