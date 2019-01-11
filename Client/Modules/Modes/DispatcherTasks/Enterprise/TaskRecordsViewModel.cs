using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.DispatcherTask;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.DispatcherTasks.TaskRecords;
using GazRouter.Modes.DispatcherTasks.Common.RecordComments;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;
using GazRouter.Modes.DispatcherTasks.Dialogs.AddTaskRecord;
using AddTaskRecordView = GazRouter.Modes.DispatcherTasks.Dialogs.AddTaskRecord.AddTaskRecordView;

namespace GazRouter.Modes.DispatcherTasks.Enterprise
{
    public class TaskRecordsViewModel : LockableViewModel
    {
        private readonly TaskItem _task;

        public TaskRecordsViewModel(TaskItem task)
        {
            _task = task;

            RefreshCommand = new DelegateCommand(() => LoadList());
            
            AddCommand = new DelegateCommand(Add, () => _task != null && _task.IsChangeable);
            EditCommand = new DelegateCommand(Edit, () => SelectedRecord != null && _task.IsChangeable);
            DeleteCommand = new DelegateCommand(Delete, () => SelectedRecord != null && _task.IsChangeable);

            SetControlCommand = new DelegateCommand(SetControl,
                () => SelectedRecord != null && _task.IsApprovedForSite && !SelectedRecord.Dto.IsSpecialControl);

            ResetControlCommand = new DelegateCommand(SetControl,
                () => SelectedRecord != null && _task.IsApprovedForSite && SelectedRecord.Dto.IsSpecialControl);

            SetExecutedCommand = new DelegateCommand(SetExecuted,
                () => SelectedRecord != null && _task.IsApprovedForSite && !SelectedRecord.IsCompleted);

            ResetExecutedCommand = new DelegateCommand(ResetExecuted,
                () => SelectedRecord != null && _task.IsApprovedForSite && SelectedRecord.IsCompleted);

            

            LoadList();
        }


        public RecordCommentsViewModel CommentsViewModel { get; set; }


        public List<RecordItem> RecordList { get; set; }


        private RecordItem _selectedRecord;
        public RecordItem SelectedRecord
        {
            get { return _selectedRecord; }
            set
            {
                if (SetProperty(ref _selectedRecord, value))
                {
                    RaiseCommands();
                    CommentsViewModel = new RecordCommentsViewModel(_selectedRecord?.Dto);
                    OnPropertyChanged(() => CommentsViewModel);
                }
            }
        }


        public DelegateCommand RefreshCommand { get; set; }

        public async void LoadList(Guid? recordId = null)
        {
            if (_task == null) return;

            Lock();
            
            var list = await new DispatcherTaskServiceProxy().GetTaskRecordCPDDListAsync(
                new GetTaskRecordsCpddParameterSet
                {
                    IsCpdd = false,
                    TaskVersionId = _task.Dto.LastVersionId
                });

            RecordList =
                list.Select(r => new RecordItem(r, _task.IsApprovedForSite)).ToList();

            OnPropertyChanged(() => RecordList);

            SelectedRecord = recordId.HasValue
                ? RecordList.SingleOrDefault(r => r.Dto.Id == recordId)
                : RecordList.FirstOrDefault();

            Unlock();
        }

        public DelegateCommand AddCommand { get; set; }
        protected void Add()
        {
            var exceptProps =
                RecordList.Select(r => new Tuple<Guid, PropertyType>(r.Dto.Entity.Id, r.Dto.PropertyTypeId)).ToList();
            var viewModel = new AddTaskRecordViewModel(_task.Dto, () => LoadList(), exceptProps);
            var view = new AddTaskRecordView { DataContext = viewModel };
            view.ShowDialog();
        }


        public DelegateCommand EditCommand { get; set; }
        protected void Edit()
        {
            var exceptProps =
                RecordList.Where(r => r != SelectedRecord)
                    .Select(r => new Tuple<Guid, PropertyType>(r.Dto.Entity.Id, r.Dto.PropertyTypeId))
                    .ToList();

            var viewModel = new AddTaskRecordViewModel(SelectedRecord.Dto, () => LoadList(SelectedRecord.Dto.Id), exceptProps);
            var view = new AddTaskRecordView { DataContext = viewModel };
            view.ShowDialog();
        }

        
        public DelegateCommand DeleteCommand { get; protected set; }
        private void Delete()
        {
            MessageBoxProvider.Confirm(
                "Удаление строки задания, нужно Ваше подтверждение. Удалить?",
                async r =>
                {
                    if (r)
                    {
                        await new DispatcherTaskServiceProxy().RemoveTaskRecordAsync(SelectedRecord.Dto.Id);
                        LoadList();
                    }
                },
                "Удаление строки задания",
                "Удалить",
                "Отмена");
        }


        public DelegateCommand SetControlCommand { get; set; }

        public DelegateCommand ResetControlCommand { get; set; }

        private async void SetControl()
        {
            if (SelectedRecord.Dto.IsSpecialControl)
                await new DispatcherTaskServiceProxy().ResetToControlTaskRecordAsync(SelectedRecord.Dto.Id);
            else
                await new DispatcherTaskServiceProxy().SetToControlTaskRecordAsync(SelectedRecord.Dto.Id);

            LoadList(SelectedRecord.Dto.Id);
        }


        public DelegateCommand SetExecutedCommand { get; set; }
        private async void SetExecuted()
        {
            await new DispatcherTaskServiceProxy().TaskRecordExecutedAsync(SelectedRecord.Dto.Id);
            LoadList(SelectedRecord.Dto.Id);
        }


        public DelegateCommand ResetExecutedCommand { get; set; }
        private async void ResetExecuted()
        {
            await new DispatcherTaskServiceProxy().TaskRecordResetExecutedAsync(SelectedRecord.Dto.Id);
            LoadList(SelectedRecord.Dto.Id);
        }






        protected void RaiseCommands()
        {
            AddCommand.RaiseCanExecuteChanged();
            EditCommand.RaiseCanExecuteChanged();
            DeleteCommand.RaiseCanExecuteChanged();
            SetControlCommand.RaiseCanExecuteChanged();
            ResetControlCommand.RaiseCanExecuteChanged();
            SetExecutedCommand.RaiseCanExecuteChanged();
            ResetExecutedCommand.RaiseCanExecuteChanged();
        }






    }
}