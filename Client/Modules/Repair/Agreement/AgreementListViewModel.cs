using System;
using System.Linq;
using System.Collections.Generic;
using GazRouter.Common.ViewModel;
using GazRouter.Controls.Attachment;
using GazRouter.DataProviders.Repairs;
using GazRouter.DTO.Repairs.RepairWorks;
using GazRouter.DTO.Repairs.Agreed;
using Microsoft.Practices.Prism.Commands;
using GazRouter.DataProviders.Authorization;
using System.Threading.Tasks;

namespace GazRouter.Repair.Agreement
{
    public class AgreementListViewModel : LockableViewModel
    {
        private readonly int _repairId;

        private AgreedUserDTO _CurrentAgreementPerson;
        private List<AgreedUserDTO> _AgreementPersons;
        private List<AgreedUserDTO> _SubstitutablePersons;

        public List<AgreedRepairRecordDTO> RecordList { get; private set; }

        private AgreedRepairRecordDTO _SelectedRecord;
        public AgreedRepairRecordDTO SelectedRecord
        {
            get { return _SelectedRecord; }
            set
            {
                SetProperty(ref _SelectedRecord, value);
                DeleteCommand.RaiseCanExecuteChanged();
                AgreeCommand.RaiseCanExecuteChanged();
                DisagreeCommand.RaiseCanExecuteChanged();
            }
        }

        private bool _isEditingAllowed;
        public bool isEditingAllowed
        {
            get
            {
                return _isEditingAllowed;
            }
            set
            {
                SetProperty(ref _isEditingAllowed, value);
            }
        }

        private bool _isAgreeingAllowed;
        public bool isAgreeingAllowed
        {
            get
            {
                return _isAgreeingAllowed;
            }
            set
            {
                SetProperty(ref _isAgreeingAllowed, value);
            }
        }

        private readonly DelegateCommand _AddCommand;
        public DelegateCommand AddCommand => _AddCommand;

        private readonly DelegateCommand _DeleteCommand;
        public DelegateCommand DeleteCommand => _DeleteCommand;

        private readonly DelegateCommand _AgreeCommand;
        public DelegateCommand AgreeCommand => _AgreeCommand;

        private readonly DelegateCommand _DisagreeCommand;
        public DelegateCommand DisagreeCommand => _DisagreeCommand;

        public AgreementListViewModel(int repairId, bool isEditingAllowed, bool isAgreeingAllowed)
        {
            _repairId = repairId;
            _isEditingAllowed = isEditingAllowed;
            _isAgreeingAllowed = isAgreeingAllowed;

            _AddCommand = new DelegateCommand(Add, () => _isEditingAllowed);
            _DeleteCommand = new DelegateCommand(Delete, () => _isEditingAllowed && SelectedRecord != null);
            _AgreeCommand = new DelegateCommand(Agree, () => IsAgreeingAllowed(true));
            _DisagreeCommand = new DelegateCommand(Disagree, () => IsAgreeingAllowed(false));

            Refresh();
        }

        private bool IsAgreeingAllowed(bool is_already_agreed)
        {
            bool result = IsAgreeingAllowed(SelectedRecord) &&
                   (!SelectedRecord.AgreedResult.HasValue || SelectedRecord.AgreedResult.Value != is_already_agreed);
            return result;
        }

        public bool IsAgreeingAllowed(AgreedRepairRecordDTO Record)
        {
            bool result = _isAgreeingAllowed && Record != null && _CurrentAgreementPerson != null &&
                      (_CurrentAgreementPerson.AgreedUserId == Record.AgreedUserId || _SubstitutablePersons.Any(person => person.AgreedUserId == Record.AgreedUserId));
            return result;
        }


        public async Task<List<AgreedRepairRecordDTO>> GetList()
        {
            var list = await new RepairsServiceProxy().GetAgreedRepairRecordListAsync(_repairId);
            if (list == null)
                list = new List<AgreedRepairRecordDTO>();
            return list;
        }
        private async void Refresh()
        {
            Lock();

            RecordList = await GetList();
            
            _AgreementPersons = await new  UserManagementServiceProxy().GetAgreedUsersAsync(new GetAgreedUsersAllParameterSet() { TargetDate = DateTime.Now, });
            if (_AgreementPersons == null)
                _AgreementPersons = new List<AgreedUserDTO>();

            _CurrentAgreementPerson = _AgreementPersons.FirstOrDefault(person => person.UserID== Application.UserProfile.Current.Id);

            if (_CurrentAgreementPerson != null && _CurrentAgreementPerson.ActingUserID.HasValue)
                _SubstitutablePersons = _AgreementPersons.Where(person => person.UserID == _CurrentAgreementPerson.ActingUserID).ToList();
            else
                _SubstitutablePersons = new List<AgreedUserDTO>();

            OnPropertyChanged(() => RecordList);
            Unlock();
        }

        private void Add()
        {
            var CurrentAgreementPersons = RecordList.Select(record => record.AgreedUserId).ToList();
            var vm = new Dialogs.AgreementPersonsDialogViewModel(_AgreementPersons.Where(user=> !CurrentAgreementPersons.Contains(user.AgreedUserId) && user.SiteId== Application.UserProfile.Current.Site.Id).ToList(), async SelectedPersons =>
            {
                Lock();
                foreach (var Person in SelectedPersons)
                    await new RepairsServiceProxy().AddAgreedRepairRecordAsync(
                        new AddEditAgreedRepairRecordParameterSet()
                        {
                            RepairID = _repairId,
                            AgreedUserId = Person.AgreedUserId,
                        });

                Unlock();
                Refresh();
            });
            var v = new Dialogs.AgreementPersonsDialogView { DataContext = vm };
            v.ShowDialog();
        }

        private void Delete()
        {
            MessageBoxProvider.Confirm(
                "Необходимо Ваше подтверждение. Удалить запись?",
                async result =>
                {
                    if (result)
                    {
                        Lock();
                        await new RepairsServiceProxy().DeleteAgreedRepairRecordAsync(SelectedRecord.Id);
                        Refresh();
                        Unlock();
                    }
                },
                "Удаление записи",
                "Удалить");
        }

        private void Agree()
        {
            EditAgreedRepairRecor(true);
        }

        private void Disagree()
        {
            EditAgreedRepairRecor(false);
        }

        private void EditAgreedRepairRecor(bool IsAgreed)
        {
            string dialogHeaderText = "Добавить комментарий для " + (IsAgreed ? "Утверждения" : "Отклонения");
            var ViewModel = new Dialogs.AgreementCommentDialogViewModel(dialogHeaderText,async comment =>
            {
                Lock();
                await new RepairsServiceProxy().EditAgreedRepairRecordAsync(new AddEditAgreedRepairRecordParameterSet()
                {
                    Id = SelectedRecord.Id,
                    RepairID = _repairId,
                    AgreedUserId = SelectedRecord.AgreedUserId, 
                    RealAgreedUserId = SelectedRecord.AgreedUserId==_CurrentAgreementPerson.AgreedUserId? null : (int?)_CurrentAgreementPerson.AgreedUserId,
                    AgreedDate = DateTime.Now,
                    CreationDate = SelectedRecord.CreationDate,
                    Comment = comment,
                    AgreedResult = IsAgreed,
                });
                Refresh();
                Unlock();
            });
            var View = new Dialogs.AgreementCommentDialogView() { DataContext = ViewModel };
            View.ShowDialog();
        }
    }
}