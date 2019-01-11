using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Application;
using GazRouter.Application.Helpers;
using GazRouter.Common;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.DispatcherTask;
using GazRouter.DTO.DispatcherTasks.TaskRecords;
using GazRouter.Modes.DispatcherTasks.Common;
using GazRouter.Modes.DispatcherTasks.Common.AttachmentsView;
using GazRouter.Modes.DispatcherTasks.Common.RecordComments;
using GazRouter.Modes.DispatcherTasks.Common.TaskListType;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;


namespace GazRouter.Modes.DispatcherTasks.Site
{
    [RegionMemberLifetime(KeepAlive = false)]
    public class SiteViewModel : LockableViewModel
    {
        public SiteViewModel()
		{
            _refresher = new TaskAutoRefresher(AutoRefresh);
            _selectedTaskListType = TaskListType.Current;
            _selectedPeriod = new Period(DateTime.Now.Year, DateTime.Now.Month, false);

            RefreshCommand = new DelegateCommand(Refresh);
            AckCommand = new DelegateCommand(Ack, () => SelectedTask != null && !SelectedTask.IsCompleted && !SelectedTask.IsAck);
            ExecuteCommand = new DelegateCommand(Execute, () => SelectedTask != null && !SelectedTask.IsCompleted);

            Refresh();
            IsAutoRefreshOn = IsolatedStorageManager.Get<bool?>("TasksAutoRefreshOn") ?? true;
            _refresher.Update();
        }



        #region TASK LIST TYPE

        public IEnumerable<TaskListType> TaskListTypes
        {
            get
            {
                yield return TaskListType.Current;
                yield return TaskListType.Archive;
            }
        }

        private TaskListType _selectedTaskListType;
        public TaskListType SelectedTaskListType
        {
            get { return _selectedTaskListType; }
            set
            {
                if (SetProperty(ref _selectedTaskListType, value))
                {
                    OnPropertyChanged(() => IsArchiveSelected);
                    Refresh();
                }
            }
        }

        public bool IsArchiveSelected => _selectedTaskListType == TaskListType.Archive;

        #endregion

        #region PERIOD

        private Period _selectedPeriod;
        public Period SelectedPeriod
        {
            get { return _selectedPeriod; }
            set
            {
                if (SetProperty(ref _selectedPeriod, value))
                {
                    Refresh();
                }
            }
        }

        #endregion


        public List<TaskRecordItem> TaskList { get; set; }


        private TaskRecordItem _selectedTask;

        public TaskRecordItem SelectedTask
        {
            get { return _selectedTask; }
            set
            {
                if (SetProperty(ref _selectedTask, value))
                {
                    UpdateCommands();

                    CommentsViewModel = new RecordCommentsViewModel(_selectedTask?.Dto);
                    OnPropertyChanged(() => CommentsViewModel);

                    AttachmentsViewModel = new AttachmentsViewModel(_selectedTask?.Dto?.TaskId, IsArchiveSelected);
                    OnPropertyChanged(() => AttachmentsViewModel);
                }
            }
        }


        public RecordCommentsViewModel CommentsViewModel { get; set; }

        public AttachmentsViewModel AttachmentsViewModel { get; set; }


        public DelegateCommand RefreshCommand { get; set; }

        private async void Refresh()
        {
            Lock();

            var list = await new DispatcherTaskServiceProxy().GetTaskRecordPDSListAsync(
                new GetTaskRecordsPdsParameterSet
                {
                    SiteId = UserProfile.Current.Site.Id,
                    IsArchive = IsArchiveSelected,
                    BeginDate = IsArchiveSelected ? SelectedPeriod.Begin : (DateTime?)null,
                    EndDate = IsArchiveSelected ? SelectedPeriod.End : (DateTime?)null
                });

            TaskList = list.Select(t => new TaskRecordItem(t)).ToList();

            OnPropertyChanged(() => TaskList);

            Unlock();
        }


        #region AUTO REFRESH

        private bool _isAutoRefreshOn;
        public bool IsAutoRefreshOn
        {
            get { return _isAutoRefreshOn; }
            set
            {
                if (SetProperty(ref _isAutoRefreshOn, value))
                {
                    IsolatedStorageManager.Set("TasksAutoRefreshOn", _isAutoRefreshOn);
                    _refresher.Update();
                }
            }
        }

        private readonly TaskAutoRefresher _refresher;
        
        private async void AutoRefresh()
        {
            if (TaskList == null) return;

            var list = await new DispatcherTaskServiceProxy().GetTaskRecordPDSListAsync(
                new GetTaskRecordsPdsParameterSet
                {
                    SiteId = UserProfile.Current.Site.Id,
                    IsArchive = IsArchiveSelected,
                    BeginDate = IsArchiveSelected ? SelectedPeriod.Begin : (DateTime?)null,
                    EndDate = IsArchiveSelected ? SelectedPeriod.End : (DateTime?)null
                });

            var countDifference = list.Count != TaskList.Count;

            foreach (var task in list)
            {
                if (countDifference || TaskList.All(t => t.Dto.Id != task.Id))
                {
                    var selection = SelectedTask?.Dto?.Id;
                    TaskList = TaskList = list.Select(t => new TaskRecordItem(t)).ToList();
                    OnPropertyChanged(() => TaskList);
                    if (selection.HasValue)
                        SelectedTask = TaskList.SingleOrDefault(t => t.Dto.Id == selection);

                    break;
                }
            }
        }

        #endregion


        public DelegateCommand AckCommand { get; set; }
        private async void Ack()
        {
            await new DispatcherTaskServiceProxy().SetACKAsync(SelectedTask.Dto.Id);
            Refresh();
        }


        public DelegateCommand ExecuteCommand { get; set; }

        private async void Execute()
        {
            await new DispatcherTaskServiceProxy().TaskRecordExecutedAsync(SelectedTask.Dto.Id);
            Refresh();
        }

        private void UpdateCommands()
        {
            AckCommand.RaiseCanExecuteChanged();
            ExecuteCommand.RaiseCanExecuteChanged();
        }



    }
}
