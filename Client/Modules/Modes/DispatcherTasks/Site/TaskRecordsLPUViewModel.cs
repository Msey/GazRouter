using System.Collections.Generic;
using System.Collections.ObjectModel;
using GazRouter.Application;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.DispatcherTask;
using GazRouter.DTO.DispatcherTasks.TaskRecords;
using GazRouter.Controls.Converters;
using System;
using System.Windows.Controls;
using GazRouter.Common;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;
using GazRouter.DTO.DispatcherTasks.RecordNotes;
using GazRouter.Controls;

namespace GazRouter.Modes.DispatcherTasks.LPU
{
    public class TaskRecordsLPUViewModel : LockableViewModel
    {
        private TaskRecordPdsDTO _selectedTaskRecord;

        private ObservableCollection<TaskRecordPdsDTO> _tasks;

        private LPUListType _type = LPUListType.Active;

        private List<ITaskTab> _tabItems;

        private ITaskTab _selectedTabItem;

        private ListRefreshWatcher watcher = new ListRefreshWatcher();
        public void Refresh(object sender, EventArgs e) { LoadList(); }

        public TaskRecordsLPUViewModel()
        {
            _tasks = new ObservableCollection<TaskRecordPdsDTO>();
            SetExecutedCommand = new DelegateCommand(Check,
                () => SelectedTaskRecord != null && Type != LPUListType.Archive);
            RefreshCommand = new DelegateCommand(LoadList);
            ExportExcelCommand = new DelegateCommand(ExportToExcel, () => (archiveTasks.Count > 0 || activeTasks.Count >0));

            AckCommand = new DelegateCommand(AckRecord, () => SelectedTaskRecord != null && !SelectedTaskRecord.AckDate.HasValue);
            _tabItems = new List<ITaskTab>
            {
                new TaskRecordCommentsViewModel {IsSelected = true},
                new AttachmentsLPUViewModel {IsSelected = true}
            };
            SelectedTabItem = _tabItems[0];

            watcher.TimeToRefresh += Refresh;
            watcher.Run();
        }

        public TaskRecordPdsDTO SelectedTaskRecord
        {
            get { return _selectedTaskRecord; }
            set
            {
                _selectedTaskRecord = value;
                RaiseCommand();
                OnPropertyChanged(() => SelectedTaskRecord);
                OnPropertyChanged(() => AckEnabled);
                OnPropertyChanged(() => AckCaption);

                if (_selectedTaskRecord != null)
                {
                    foreach (var item in _tabItems)
                    {
                        item.EnableCommand = !_selectedTaskRecord.ExecutedDate.HasValue && Type == LPUListType.Active;
                        item.TaskRecord = _selectedTaskRecord;
                    }
                }
            }
        }

        public ObservableCollection<TaskRecordPdsDTO> Tasks
        {
            get { return _tasks; }
            set
            {
                _tasks = value;
                OnPropertyChanged(() => Tasks);
            }
        }

        public DelegateCommand AddCommand { get; set; }
        public DelegateCommand EditCommand { get; set; }
        public DelegateCommand RemoveCommand { get; set; }
        public DelegateCommand SetExecutedCommand { get; }
        public DelegateCommand RefreshCommand { get; private set; }
        public DelegateCommand ExportExcelCommand { get; private set; } // Команда формирования отчета в Ms Excel,
        ExcelReport excelReport; DateTime date;                         // отчет и его дата и время,
        List<TaskRecordPdsDTO> archiveTasks = new List<TaskRecordPdsDTO>(), activeTasks = new List<TaskRecordPdsDTO>();                // исходные данные для отчета

        public DelegateCommand AckCommand { get; set; }

        public LPUListType Type
        {
            get { return _type; }
            set
            {
                _type = value;
                LoadList();
                foreach (var item in _tabItems)
                {
                    item.LoadType = value;
                }
                OnPropertyChanged(() => Type);
                OnPropertyChanged(() => ColVisible);
                OnPropertyChanged(() => ColNotVisible);
                OnPropertyChanged(() => Header);
            }
        }

        public bool ColVisible => Type == LPUListType.Archive;

        public bool ColNotVisible => !ColVisible;

        public string Header => Type == LPUListType.Archive ? "Архив" : "Текущие задачи";

        public bool AckEnabled { get { return SelectedTaskRecord != null && SelectedTaskRecord.AckDate == null ? true : false; } }
        public string AckCaption => "Квитировать";// SelectedTaskRecord == null ? "" : SelectedTaskRecord.AckDate == null ? "Квитировать" : "Отменить квитирование";

        public List<ITaskTab> TabItems
        {
            get { return _tabItems; }
            set
            {
                _tabItems = value;
                OnPropertyChanged(() => TabItems);
            }
        }

        public ITaskTab SelectedTabItem
        {
            get { return _selectedTabItem; }
            set
            {
                _selectedTabItem = value;

                foreach (var item in TabItems)
                {
                    item.IsActive = item == _selectedTabItem;
                }

                OnPropertyChanged(() => SelectedTabItem);
            }
        }

        private PeriodDates _selectedPeriod = new PeriodDates
        {
            BeginDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month,1),
            EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddMilliseconds(-1)
        };
        public PeriodDates SelectedPeriod
        {
            get { return _selectedPeriod; }
            set
            {
                if (SetProperty(ref _selectedPeriod, value))
                    LoadList();
            }
        }
        
        public async void LoadList()
        {
            if (UserProfile.Current.Site == null)
            {
                return;
            }

            Behavior.TryLock();

            var parameterSet = new GetTaskRecordsPdsParameterSet
            {
                SiteId = UserProfile.Current.Site.Id,
                IsList = Type == LPUListType.Active
            };
            try
            {
                Tasks.Clear();
                if (Type == LPUListType.Active)
                    LoadActTasks();
                else
                    LoadArchTasks();
            }
            finally
            {
                Behavior.TryUnlock();
            }
        }

        private async void LoadArchTasks()
        {
            var parameterSet = new GetTaskRecordsPdsParameterSet
            {
                SiteId = UserProfile.Current.Site.Id,
                IsList = false,
                BeginDate = SelectedPeriod.BeginDate,
                EndDate = SelectedPeriod.EndDate
            };
            archiveTasks.Clear();
            var result = await new DispatcherTaskServiceProxy().GetTaskRecordPDSListAsync(parameterSet);
            foreach (var taskRecordsDto in result)
            {
                Tasks.Add(taskRecordsDto);
                archiveTasks.Add(taskRecordsDto);
            }
            ExportExcelCommand.RaiseCanExecuteChanged();

            OnPropertyChanged(() => Tasks);
        }
        private async void LoadActTasks()
        {
            var parameterSet = new GetTaskRecordsPdsParameterSet
            {
                SiteId = UserProfile.Current.Site.Id,
                IsList = true
            };
            activeTasks.Clear();
            var result = await new DispatcherTaskServiceProxy().GetTaskRecordPDSListAsync(parameterSet);
            foreach (var taskRecordsDto in result)
            {
                Tasks.Add(taskRecordsDto);
                activeTasks.Add(taskRecordsDto);
            }
            ExportExcelCommand.RaiseCanExecuteChanged();

            OnPropertyChanged(() => Tasks);
        }

        private void RaiseCommand()
        {
            SetExecutedCommand.RaiseCanExecuteChanged();
        }

        private async void Check()
        {
            AckRecordNoReload();
            Behavior.TryLock();
            try
            {
                await new DispatcherTaskServiceProxy().TaskRecordExecutedAsync(SelectedTaskRecord.Id);
            }
            finally
            {
                Behavior.TryUnlock();
            }
            LoadList();
        }

        private async void AckRecord()
        {
            if (SelectedTaskRecord.AckDate == null)
            {
                AckRecordNoReload();

                LoadList();
            }

            
        }
        private async void AckRecordNoReload()
        {
            if (SelectedTaskRecord.AckDate == null)
            {
                Behavior.TryLock();
                try
                {

                    //if (SelectedTaskRecord.AckDate.HasValue)
                    //    await new DispatcherTaskServiceProxy().ResetACKAsync(SelectedTaskRecord.Id);
                    //else
                    await new DispatcherTaskServiceProxy().SetACKAsync(SelectedTaskRecord.Id);
                }
                finally
                {
                    Behavior.TryUnlock();
                }
            }
        }

        private async void ExportToExcel()
        {
            var dialog = new SaveFileDialog
            {
                DefaultExt = "xlsx",
                Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*",
                FilterIndex = 1,
                //DefaultFileName = Header
            };
            if (dialog.ShowDialog() == true)
            {
                /*
                // экспорт выбранной вкладки
                excelReport = new ExcelReport(Header);
                date = DateTime.Now;
                ReportHeader(Header);
                foreach (var ac in Tasks)
                {
                    var comm = "";
                    var comments = await new DispatcherTaskServiceProxy().GetRecordNoteListAsync(new GetRecordNoteListParameterSet
                    {
                        TaskId = ac.TaskId,
                        EntityId = ac.EntityId,
                        PropertyTypeId = ac.PropertyTypeId
                    });
                    foreach (var comment in comments)
                    {
                        if (comm != "") comm += "\n";
                        comm += comment.Note + " " + comment.CreateUserName + " " + String.Format("{0:dd.MM.yyyy HH: mm}", comment.CreateDate);
                    }
                    var att = "";
                    var list = await new DispatcherTaskServiceProxy().GetTaskAttachementListAsync(ac.TaskId);
                    foreach (var at in list)
                    {
                        if (att != "")
                            att += "\n";
                        att += String.Format("{0:dd.MM.yyyy HH: mm}", at.CreateDate) + " " +
                        at.Description + " " + at.FileName + " " + at.CreateUserName;
                    }
                    excelReport.NewRow();
                    excelReport.WriteCell(ac.TaskNum);
                    excelReport.WriteCell(ac.ObjectName);
                    excelReport.WriteCell(ac.Description);
                    excelReport.WriteCell(ac.PropertyTypeName);
                    excelReport.WriteCell(ac.TargetValue);
                    excelReport.WriteCell(new PropertyTypeToUnitNameConverter().Convert(ac.PropertyTypeId, typeof(string), null, null));
                    excelReport.WriteCell(ac.AproveUserName);
                    excelReport.WriteCell(comm);
                    excelReport.WriteCell(att);
                } */
                // Обе вкладки (текущие задания и архив) -> Ms Excel

                if (activeTasks.Count > 0)
                {
                    excelReport = new ExcelReport("Текущие задания");
                    date = DateTime.Now;
                    ReportHeader();
                    foreach (var ac in activeTasks)
                    {
                        var comm = "";
                        var comments = await new DispatcherTaskServiceProxy().GetRecordNoteListAsync(new GetRecordNoteListParameterSet
                        {
                            TaskId = ac.TaskId,
                            EntityId = ac.EntityId,
                            PropertyTypeId = ac.PropertyTypeId
                        });
                        foreach (var comment in comments)
                        {
                            if (comm != "") comm += "\n";
                            comm += comment.Note + " " + comment.CreateUserName + " " + String.Format("{0:dd.MM.yyyy HH: mm}", comment.CreateDate);
                        }
                        var att = "";
                        var list = await new DispatcherTaskServiceProxy().GetTaskAttachementListAsync(ac.TaskId);
                        foreach (var at in list)
                        {
                            if (att != "")
                                att += "\n";
                            att += String.Format("{0:dd.MM.yyyy HH: mm}", at.CreateDate) + " " +
                            at.Description + " " + at.FileName + " " + at.CreateUserName;
                        }
                        excelReport.NewRow();
                        excelReport.WriteCell(ac.TaskNum);
                        excelReport.WriteCell(ac.ObjectName);
                        excelReport.WriteCell(ac.Description);
                        excelReport.WriteCell(ac.PropertyTypeName);
                        excelReport.WriteCell(ac.TargetValue);
                        excelReport.WriteCell(new PropertyTypeToUnitNameConverter().Convert(ac.PropertyTypeId, typeof(string), null, null));
                        excelReport.WriteCell(String.Format("{0:dd.MM.yyyy HH:mm}", ac.CompletionDate));
                        excelReport.WriteCell(ac.AproveUserName);
                        excelReport.WriteCell(comm);
                        excelReport.WriteCell(att);
                    }
                }
                if (archiveTasks.Count > 0)
                {
                    excelReport.Move(0, 0, "Архив");
                    ReportHeader();
                    foreach (var ac in archiveTasks)
                    {
                        var comm = "";
                        var comments = await new DispatcherTaskServiceProxy().GetRecordNoteListAsync(new GetRecordNoteListParameterSet
                        {
                            TaskId = ac.TaskId,
                            EntityId = ac.EntityId,
                            PropertyTypeId = ac.PropertyTypeId
                        });
                        foreach (var comment in comments)
                        {
                            if (comm != "") comm += "\n";
                            comm += comment.Note + " " + comment.CreateUserName + " " + String.Format("{0:dd.MM.yyyy HH: mm}", comment.CreateDate);
                        }
                        var att = "";
                        var list = await new DispatcherTaskServiceProxy().GetTaskAttachementListAsync(ac.TaskId);
                        foreach (var at in list)
                        {
                            if (att != "")
                                att += "\n";
                            att += String.Format("{0:dd.MM.yyyy HH: mm}", at.CreateDate) + " " +
                            at.Description + " " + at.FileName + " " + at.CreateUserName;
                        }
                        excelReport.NewRow();
                        excelReport.WriteCell(ac.TaskNum);
                        excelReport.WriteCell(ac.ObjectName);
                        excelReport.WriteCell(ac.Description);
                        excelReport.WriteCell(ac.PropertyTypeName);
                        excelReport.WriteCell(ac.TargetValue);
                        excelReport.WriteCell(new PropertyTypeToUnitNameConverter().Convert(ac.PropertyTypeId, typeof(string), null, null));
                        excelReport.WriteCell(String.Format("{0:dd.MM.yyyy HH:mm}", ac.CompletionDate));
                        excelReport.WriteCell(ac.AproveUserName);
                        excelReport.WriteCell(comm);
                        excelReport.WriteCell(att);
                    }
                } 
                using (var stream = dialog.OpenFile())
                {
                    excelReport.Save(stream);
                }
            }
        }
        private void ReportHeader(string header = "")
        {
            excelReport.Write("Дата:").Write(date.Date).NewRow();
            excelReport.Write("Время:").Write(date.ToString("HH:mm")).NewRow();
            excelReport.Write("ФИО:").Write(UserProfile.Current.UserName).NewRow();
            excelReport.Write("ДЗ ЛПУ").Write(header).NewRow();
            excelReport.NewRow();
            excelReport.WriteHeader("№ ДЗ", 80);
            excelReport.WriteHeader("Объект", 450);
            excelReport.WriteHeader("Задание", 250);
            excelReport.WriteHeader("Параметр", 150);
            excelReport.WriteHeader("Текущее значение (требуемое)", 150);
            excelReport.WriteHeader("Ед.изм.", 100);
            excelReport.WriteHeader("Срок выполнения", 150);
            excelReport.WriteHeader("Утвердил ДЗ", 150);
            excelReport.WriteHeader("Комментарии", 450); // \nКомментарий / Создал / Время
            excelReport.WriteHeader("Вложенные файлы", 450); // \nДата добавления / Описание / Файл / Добавил
        }
    }
 }