using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.DispatcherTask;
using GazRouter.DTO.Dictionaries.StatusTypes;
using GazRouter.DTO.ObjectModel.Sites;
using Microsoft.Practices.Prism.Commands;
using System.Windows.Input;
using GazRouter.Application;
using GazRouter.Application.Helpers;
using GazRouter.Common;
using GazRouter.Common.Cache;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Dictionaries.AnnuledReasons;
using GazRouter.DTO.DispatcherTasks.Tasks;
using GazRouter.DTO.DispatcherTasks.TaskStatuses;
using GazRouter.Modes.DispatcherTasks.Common;
using GazRouter.Modes.DispatcherTasks.Common.TaskListType;
using GazRouter.Modes.DispatcherTasks.Dialogs.AddMultiRecordTask;
using GazRouter.Modes.DispatcherTasks.Dialogs.AddTask;
using Microsoft.Practices.ServiceLocation;
using Telerik.Windows.Controls;
using AddMultiRecordTaskView = GazRouter.Modes.DispatcherTasks.Dialogs.AddMultiRecordTask.AddMultiRecordTaskView;
using AddTaskView = GazRouter.Modes.DispatcherTasks.Dialogs.AddTask.AddTaskView;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;

namespace GazRouter.Modes.DispatcherTasks.Enterprise
{
    public class TasksViewModel : LockableViewModel
    {
        private readonly Action _selectedTaskChanged;
        

        public TasksViewModel(Action selectedTaskChanged)
        {
            _selectedTaskListType = TaskListType.Current;
            _selectedPeriod = new Period(DateTime.Now.Year, DateTime.Now.Month, false);

            _selectedTaskChanged = selectedTaskChanged;

            RefreshCommand = new DelegateCommand(() => Refresh());
            AddCommand = new DelegateCommand(Add, () => !IsArchiveSelected);
            AddMultiCommand = new DelegateCommand(AddMulti, () => !IsArchiveSelected);
            EditCommand = new DelegateCommand(Edit, () => SelectedTask != null && SelectedTask.IsDeletable);
            DeleteCommand = new DelegateCommand(Delete, () => SelectedTask != null && SelectedTask.IsDeletable);
            CloneCommand = new DelegateCommand(Clone, () => SelectedTask != null);
            //ExportExcelCommand = new DelegateCommand(ExportToExcel, () => TaskList?.Count > 0);
            SetStatusCommand = new DelegateCommand<StatusTypeItem>(SetStatus);
            
            LoadSiteList();
            Refresh();

            _refresher = new TaskAutoRefresher(AutoRefresh);
            _isAutoRefreshOn = IsolatedStorageManager.Get<bool?>("TasksAutoRefreshOn") ?? true;
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


        #region SITE LIST
        public List<SiteDTO> SiteList { get; set; }

        private SiteDTO _selectedSite;

        public SiteDTO SelectedSite
        {
            get { return _selectedSite; }
            set
            {
                if (SetProperty(ref _selectedSite, value))
                {
                    Refresh();
                }
            }
        }

        private async void LoadSiteList()
        {
            Lock();

            SiteList = await new ObjectModelServiceProxy().GetSiteListAsync(
                new GetSiteListParameterSet
                {
                    EnterpriseId = UserProfile.Current.Site.Id
                });

            OnPropertyChanged(() => SiteList);

            Unlock();
        }

        #endregion


        #region TASK LIST
        public List<TaskItem> TaskList { get; set; }
        

        private TaskItem _selectedTask;
        public TaskItem SelectedTask
        {
            get { return _selectedTask; }
            set
            {
                if (SetProperty(ref _selectedTask, value))
                {
                    UpdateCommands();
                    OnPropertyChanged(() => SetStatusItemList);
                    _selectedTaskChanged?.Invoke();
                }
            }
        }
        #endregion


        public DelegateCommand RefreshCommand { get; }
        private async void Refresh(Guid? taskId = null)
        {
            Lock();

            var tasks = await new DispatcherTaskServiceProxy().GetTaskListAsync(
                new GetTaskListParameterSet
                {
                    SiteId = SelectedSite?.Id,
                    IsArchive = IsArchiveSelected,
                    PeriodStart = IsArchiveSelected ? SelectedPeriod.Begin : (DateTime?)null,
                    PeriodEnd = IsArchiveSelected ? SelectedPeriod.End : (DateTime?)null,
                });

            TaskList = tasks.OrderByDescending(p => p.TaskNumber).Select(dto => new TaskItem(dto)).ToList();
            SelectedTask = taskId.HasValue
                ? TaskList.SingleOrDefault(i => i.Dto.Id == taskId) ?? TaskList.FirstOrDefault()
                : TaskList.FirstOrDefault();
            OnPropertyChanged(() => TaskList);

            Unlock();
        }


        #region AUTO REFRESH

        private readonly TaskAutoRefresher _refresher;

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

        private async void AutoRefresh()
        {
            var tasks = await new DispatcherTaskServiceProxy().GetTaskListAsync(
                new GetTaskListParameterSet
                {
                    SiteId = SelectedSite?.Id,
                    IsArchive = IsArchiveSelected,
                    PeriodStart = IsArchiveSelected ? SelectedPeriod.Begin : (DateTime?)null,
                    PeriodEnd = IsArchiveSelected ? SelectedPeriod.End : (DateTime?)null,
                });

            var countDifference = tasks.Count != TaskList.Count;

            foreach (var task in tasks)
            {
                var exTask = TaskList.SingleOrDefault(t => t.Dto.LastVersionId == task.LastVersionId);

                if (countDifference || exTask == null || exTask.Dto.IsComplete != task.IsComplete || exTask.Dto.IsOverdue != task.IsOverdue)
                {
                    var taskId = SelectedTask?.Dto?.Id;
                    TaskList = tasks.OrderByDescending(p => p.TaskNumber).Select(dto => new TaskItem(dto)).ToList();
                    SelectedTask = taskId.HasValue
                        ? TaskList.SingleOrDefault(i => i.Dto.Id == taskId) ?? TaskList.FirstOrDefault()
                        : TaskList.FirstOrDefault();
                    OnPropertyChanged(() => TaskList);
                    break;
                }
            }
        }

        #endregion
        


        private void UpdateCommands()
        {
            AddCommand.RaiseCanExecuteChanged();
            AddMultiCommand.RaiseCanExecuteChanged();
            EditCommand.RaiseCanExecuteChanged();
            DeleteCommand.RaiseCanExecuteChanged();
            CloneCommand.RaiseCanExecuteChanged();
            OnPropertyChanged(() => IsSetStatusAllowed);
        }

        public DelegateCommand AddCommand { get; }
        private void Add()
        {
            var viewModel = new AddTaskViewModel(async (s, d) =>
            {
                var id = await new DispatcherTaskServiceProxy().AddTaskAsync(
                    new AddTaskParameterSet
                    {
                        Subject = s,
                        Description = d
                    });
                Refresh(id);
            });
            var view = new AddTaskView { DataContext = viewModel };
            view.ShowDialog();
        }
        
        public DelegateCommand AddMultiCommand { get; }
        private void AddMulti()
        {
            var viewModel = new AddMultiRecordTaskViewModel(x => Refresh(x));
            var view = new AddMultiRecordTaskView { DataContext = viewModel };
            view.ShowDialog();
        }


        public DelegateCommand EditCommand { get; }

        private void Edit()
        {
            var viewModel = new AddTaskViewModel(SelectedTask.Dto, async (s, d) =>
            {
                await new DispatcherTaskServiceProxy().EditTaskAsync(
                    new EditTaskParameterSet
                    {
                        TaskId = SelectedTask.Dto.Id,
                        Subject = s,
                        Description = d
                    });
                Refresh(SelectedTask.Dto.Id);
            });
            var view = new AddTaskView { DataContext = viewModel };
            view.ShowDialog();
        }

        
        public DelegateCommand DeleteCommand { get; }
        private void Delete()
        {
            MessageBoxProvider.Confirm(
                $"Необходимо Ваше подтверждение. Удалить выбранное задание '{SelectedTask.Dto.Subject}'?",
                async r =>
                {
                    if (r)
                    {
                        await new DispatcherTaskServiceProxy().DeleteTaskAsync(SelectedTask.Dto.Id);
                        Refresh();
                    }
                },
                "Удаление задания",
                "Удалить",
                "Отмена");
        }



        public DelegateCommand<StatusTypeItem> SetStatusCommand { get; set; }

        private async void SetStatus(StatusTypeItem targetStatus)
        {
            if (SelectedTask == null) return;

            if (targetStatus.Type == StatusType.Annuled)
            {
                RadWindow.Prompt(new DialogParameters
                {
                    Header = "Аннулирование задания",
                    Content = "Укажите причину аннулирования:",
                    Closed = async (o, even) =>
                    {
                        if (even.DialogResult.HasValue && even.DialogResult.Value)
                        {
                            await new DispatcherTaskServiceProxy().SetTaskStatusAsync(
                                new SetTaskStatusParameterSet
                                {
                                    TaskId = SelectedTask.Dto.Id,
                                    StatusType = targetStatus.Type,
                                    AnnuledReason = AnnuledReason.CancelPDS,
                                    ReasonDescription = even.PromptResult,
                                    UserNameCpdd = string.Empty
                                });

                            Refresh(SelectedTask.Dto.Id);
                        }
                    }
                });
            }
            else
            {
                await new DispatcherTaskServiceProxy().SetTaskStatusAsync(
                    new SetTaskStatusParameterSet
                    {
                        TaskId = SelectedTask.Dto.Id,
                        StatusType = targetStatus.Type,
                        UserNameCpdd = string.Empty,
                    });

                Refresh(SelectedTask.Dto.Id);
            }
            
        }
        
        public List<SetStatusItem> SetStatusItemList
            => _selectedTask?.AllowedStatusList.Select(s => new SetStatusItem(SetStatusCommand, s)).ToList();

        public bool IsSetStatusAllowed => _selectedTask != null && SetStatusItemList != null && SetStatusItemList.Count > 0;



        public DelegateCommand CloneCommand { get; }
        
        public void Clone()
        {
            var viewModel = new AddTaskViewModel(async (s, d) =>
            {
                var id = await new DispatcherTaskServiceProxy().CloneTaskAsync(
                    new AddTaskParameterSet
                    {
                        Subject = s,
                        Description = d,
                        SourceTaskId = SelectedTask.Dto.LastVersionId
                    });

                _selectedTaskListType = TaskListType.Current;
                OnPropertyChanged(() => SelectedTaskListType);
                OnPropertyChanged(() => IsArchiveSelected);

                Refresh(id);
            });
            var view = new AddTaskView { DataContext = viewModel };
            view.ShowDialog();
        }
    }


    public class SetStatusItem
    {
        public SetStatusItem(ICommand cmd, StatusType type)
        {
            Command = cmd;
            Type = new StatusTypeItem(type);
        }

        public ICommand Command { get; set; }

        public StatusTypeItem Type { get; set; }
    }

    public class StatusTypeItem
    {
        private static IClientCache ClientCache => ServiceLocator.Current.GetInstance<IClientCache>();

        public StatusTypeItem(StatusType type)
        {
            Type = type;
        }

        public StatusType Type { get; set; }

        public string TypeName => ClientCache.DictionaryRepository.TaskStatusTypes.Single(s => s.StatusType == Type).Name;
    }
}
