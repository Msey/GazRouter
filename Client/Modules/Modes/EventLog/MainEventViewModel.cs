using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using GazRouter.Application;
using GazRouter.Application.Helpers;
using GazRouter.Common;
using GazRouter.Controls.Attachment;
using GazRouter.DataProviders;
using GazRouter.DataProviders.EventLog;
using GazRouter.DataProviders.Time;
using GazRouter.DTO.Dictionaries.EventPriorities;
using GazRouter.DTO.EventLog;
using GazRouter.DTO.EventLog.Attachments;
using GazRouter.DTO.EventLog.EventRecipient;
using GazRouter.Modes.EventLog.Dialogs;
using Microsoft.Practices.ServiceLocation;
using Telerik.Windows.Controls;
using Utils.Extensions;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;

namespace GazRouter.Modes.EventLog
{
    public class MainEventViewModel : EventLogTabBase
    {
        private readonly DispatcherTimer _timer = new DispatcherTimer();
        private readonly EventListType _type;

        private bool _addEvent;
        private List<EventDTO> _listEvent;
        private int? _tmpSelectedEventId;
        private EventDTO _selectedEvent;
        private DateType _selectedDateParameterType;
        private DateTime _shiftDate => DateTime.Now.AddDays(-UserProfile.Current.UserSettings.EventLogArchivingDelay);
        private Application.Helpers.Period _selectedPeriod;
 
        private DelegateCommand _refreshCommand;

        public MainEventViewModel(EventListType type)
        {
            _selectedPeriod = new Application.Helpers.Period(DateTime.Now.Year, DateTime.Now.Month, false);

            _type = type;
            RecepientsViewModel = new RecepientsViewModel {Type = _type};
            EventAttachmentViewModel = new EventAttachmentViewModel
            {
                ReloadParent = RefreshFromTab,
                Type = _type,
                Quote = Acknowledge
            };

            _listEvent = new List<EventDTO>();
            CreateCommands();

            _timer.Tick += TimerTick;
            _timer.Interval = TimeSpan.FromSeconds(5);
        }

        public DelegateCommand AcknowledgeCommand { get; private set; }

        public DelegateCommand AddCommand { get; private set; }

        public override bool IsActive
        {
            set
            {
                base.IsActive = value;
                if (value)
                {
                    if (!_timer.IsEnabled)
                    {
                        _timer.Start();
                    }
                }
                else
                {
                    if (_timer.IsEnabled)
                    {
                        _timer.Stop();
                    }
                }
            }
        }

        public RecepientsViewModel RecepientsViewModel { get; }

        public List<EventDTO> ListEvent
        {
            get { return _listEvent; }
            set
            {
                _listEvent = value;
                OnPropertyChanged(() => ListEvent);
            }
        }

        public EventDTO SelectedEvent
        {
            get { return _selectedEvent; }
            set
            {
                if (!SetProperty(ref _selectedEvent, value))
                {
                    return;
                }

                RefreshCommands();
                EventAttachmentViewModel.EventDTO = _selectedEvent;
                RecepientsViewModel.EventDTO = _selectedEvent;
            }
        }

        public DelegateCommand EditCommand { get; private set; }

        public override string Header
        {
            get
            {
                switch (Type)
                {
                    case EventListType.List:
                        return "Журнал";
                    case EventListType.Trash:
                        return "Корзина";
                    case EventListType.Archive:
                        return "Архив";
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public EventAttachmentViewModel EventAttachmentViewModel { get; }
        public DelegateCommand RefreshCommand => _refreshCommand ?? (_refreshCommand = new DelegateCommand(Refresh));
        public DelegateCommand RemoveCommand { get; private set; }
        public DelegateCommand TraceStartCommand { get; private set; }
        public DelegateCommand TraceStopCommand { get; private set; }
        public DelegateCommand TrashCommand { get; private set; }
        public DelegateCommand RestoreCommand { get; private set; }

        public DelegateCommand AddCommentCommand { get; private set; }

        public DelegateCommand AddFileCommand { get; private set; }

        public DelegateCommand ExportExcelCommand { get; private set; } // Команда "экспорт в MS Excel"
        // Отчет MS Excel, его дата и время и комментарий одного события для отчета:
        ExcelReport excelReport;  DateTime date; StringBuilder comment;

        public List<DateType> DateParameterTypes { get; set; }

        public DateType SelectedDateParameterType
        {
            get { return _selectedDateParameterType; }
            set
            {
                _selectedDateParameterType = value;
                OnPropertyChanged(() => SelectedDateParameterType);
                Refresh();
            }
        }

        //private void Test()
        //{
        //    var vm = new AddEditEventViewModel(id => AddRefresh());
        //    var view = new AddEditEventView { DataContext = vm };
        //    view.ShowDialog();
        //    //new EventLogDataProvider().GetEventList(null, Fill, Behavior);
        //}

        public EventListType Type => _type;

        public Application.Helpers.Period SelectedPeriod
        {
            get { return _selectedPeriod; }
            set
            {
                if(SetProperty(ref _selectedPeriod, value))
                    Refresh();
            }
        }

        public bool ShowMenu => Type == EventListType.List;

        public Visibility MainMenu => Type == EventListType.List ? Visibility.Visible : Visibility.Collapsed;

        public Visibility TrashMenu => Type == EventListType.Trash ? Visibility.Visible : Visibility.Collapsed;

        public Visibility ArchiveMenu => Type == EventListType.Archive ? Visibility.Visible : Visibility.Collapsed;

        public Visibility TrashAndArchiveMenu => Type != EventListType.List ? Visibility.Visible : Visibility.Collapsed;

        public bool MainAndArchiveMenu => Type != EventListType.Trash;

        public override async void Refresh()
        {
            if (_addEvent)
            {
                _tmpSelectedEventId = null;
                _addEvent = false;
            }
            else
            {
                if (SelectedEvent != null)
                {
                    _tmpSelectedEventId = SelectedEvent.Id;
                }
            }

            var date = DateTime.Now;

            var dispDayStart = SeriesHelper.GetCurrentDispDayStart();

            var dispShiftStart = date >= dispDayStart.AddHours(12)
                ? dispDayStart.AddHours(12)
                : dispDayStart;
            var parameterSet = new GetEventListParameterSet
            {
                QueryType = Type,
                SiteId = UserProfile.Current.Site.Id,
            };

            switch (_type)
            {
                case EventListType.List:
                    switch (SelectedDateParameterType.Type)
                    {
                        case DateTypes.All:
                            parameterSet.ArchivingDelay = UserProfile.Current.UserSettings.EventLogArchivingDelay;

                            break;

                        case DateTypes.Shift:
                            parameterSet.StartDate = dispShiftStart.ToLocalTime();
                            parameterSet.EndDate = date.ToLocalTime();

                            break;

                        case DateTypes.DispDay:
                            parameterSet.StartDate = dispDayStart.ToLocalTime();
                            parameterSet.EndDate = date.ToLocalTime();

                            break;
                    }
                    break;

                case EventListType.Archive:
                    parameterSet.StartDate = _selectedPeriod.Begin;
                    parameterSet.EndDate = _selectedPeriod.End > _shiftDate ? _shiftDate : _selectedPeriod.End;

                    break;

                case EventListType.Trash:
                    parameterSet.StartDate = _selectedPeriod.Begin;
                    parameterSet.EndDate = _selectedPeriod.End;

                    break;
                    
            }

            List<EventDTO> list;
            Behavior.TryLock();
            try
            {
                list = await new EventLogServiceProxy().GetEventListAsync(parameterSet);
            }
            finally
            {
                Behavior.TryUnlock();
            }
            Fill(list);
        }

        public bool RefreshFromTab()
        {
            Refresh();
            return true;
        }

        private void AddComment()
        {
            var viewModel = new AddEventCommentViewModel(SelectedEvent.Id, id => RefreshNonAckEventCount());
            var view = new AddEventCommentView {DataContext = viewModel};
            view.ShowDialog();
        }

        private void AddFile()
        {
            var vm = new AddEditAttachmentViewModel(async obj =>
            {
                var x = (AddEditAttachmentViewModel) obj;
                if (x.DialogResult.HasValue && x.DialogResult.Value)
                {
                    await new EventLogServiceProxy().AddEventAttachmentAsync(
                        new AddEventAttachmentParameterSet
                        {
                            Description = x.Description,
                            Data = x.FileData,
                            FileName = x.FileName,
                            EventId = SelectedEvent.Id
                        });
                    Refresh();
                }
            });
            var v = new AddEditAttachmentView {DataContext = vm};
            v.ShowDialog();
        }

        private void AddRefresh()
        {
            _addEvent = true;
            Refresh();
            EventLogMainViewModel.NotifyEventListUpdated();
        }

        private void CreateCommands()
        {
            var editPermission = Authorization2.Inst.IsEditable(LinkType.Log);
            AddCommand = new DelegateCommand(Add, () => Type == EventListType.List && editPermission);
            RemoveCommand = new DelegateCommand(Remove,
                () => SelectedEvent != null && Type == EventListType.List && CanEditDelete() && editPermission);
            EditCommand = new DelegateCommand(Edit, () => SelectedEvent != null && Type == EventListType.List &&
                                                          SelectedEvent.CreateUserId == UserProfile.Current.Id &&
                                                          SelectedEvent.CreateDate.HasValue &&
                                                          (SelectedEvent.CreateDate.Value - DateTime.Now).Minutes < 15 &&
                                                          editPermission);

            TraceStartCommand = new DelegateCommand(Check,
                () => SelectedEvent != null && SelectedEvent.Priority != EventPriority.Control && editPermission);
            TraceStopCommand = new DelegateCommand(Check,
                () => SelectedEvent != null && SelectedEvent.Priority == EventPriority.Control && editPermission);

            AcknowledgeCommand = new DelegateCommand(Acknowledge,
                () =>
                    SelectedEvent != null && SelectedEvent.IsQuote == false &&
                    Type == EventListType.List && editPermission);
            TrashCommand = new DelegateCommand(Trash, () => SelectedEvent != null && Type == EventListType.List && editPermission);
            AddCommentCommand = new DelegateCommand(AddComment,
                () => SelectedEvent != null && Type == EventListType.List);
            AddFileCommand = new DelegateCommand(AddFile, () => SelectedEvent != null && Type == EventListType.List);
            RestoreCommand = new DelegateCommand(Restore, () => SelectedEvent != null && Type == EventListType.Trash && editPermission);
            ExportExcelCommand = new DelegateCommand(ExportEventToExcelLog, () => ListEvent.Count > 0);

            DateParameterTypes = new List<DateType>
            {
                new DateType {Name = "Все", Type = DateTypes.All},
                new DateType {Name = "За смену", Type = DateTypes.Shift},
                new DateType {Name = "За дисп. сутки", Type = DateTypes.DispDay}
            };
            _selectedDateParameterType = DateParameterTypes.First();
        }

        private async void ExportEventToExcelLog()
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
                excelReport = new ExcelReport("Подробно");
                date = DateTime.Now;
                ExelReportHeader();
                foreach (var dto in ListEvent)
                {
                    CreateCommentForEvent(dto);
                    var isFirstRow = true;
                    foreach (var r in await RecepientsViewModel.LoadData(dto))
                    {
                        if (isFirstRow) isFirstRow = false;
                        else excelReport.NewRow();
                        EventInfoToExcel(dto);
                        excelReport.WriteCell(r.Recepient);
                        excelReport.WriteCell(r.AckRecepient + Environment.NewLine + r.AckDate?.ToString("dd.MM.yyyy HH:mm"));
                    }
                    excelReport.NewRow();
                }
                excelReport.Move(0, 0, "Коротко");
                ExelReportHeader(false);
                foreach (var dto in ListEvent)
                {
                    CreateCommentForEvent(dto);
                    var isFirstRow = true;
                    foreach (var a in await EventAttachmentViewModel.LoadData(dto))
                    {
                        if (isFirstRow) isFirstRow = false;
                        else comment.AppendLine();
                        comment.Append($"Пользователь: {a.UserName}").AppendLine();
                        comment.Append($"Комментарий: {a.Description}").AppendLine();
                        if (a.FileName != null) comment.Append($"Вложение: {a.FileName}").AppendLine();
                    }
                    EventInfoToExcel(dto);
                    excelReport.NewRow();
                }
                using (var stream = dialog.OpenFile())
                {
                    excelReport.Save(stream);
                }
            }
        }
        private void ExelReportHeader(bool isDetail = true)
        {
            excelReport.Write("Дата:").Write(date.Date).NewRow();
            excelReport.Write("Время:").Write(date.ToString("HH:mm")).NewRow();
            excelReport.Write("ФИО:").Write(UserProfile.Current.UserName).NewRow();
            excelReport.Write("Журнал:").Write(Header).NewRow();
            excelReport.Write($"События за период с {SelectedPeriod.Begin:d} по {SelectedPeriod.End:d}").NewRow();
            excelReport.NewRow();
            excelReport.WriteHeader("Приоритет", 120);
            excelReport.WriteHeader("Тип", 120);
            excelReport.WriteHeader("Номер", 120);
            excelReport.WriteHeader("Дата", 120);
            excelReport.WriteHeader("Событие", 450);
            excelReport.WriteHeader("ЛПУ", 250);
            excelReport.WriteHeader("Связанный объект", 250);
            excelReport.WriteHeader("Создано", 250);
            excelReport.WriteHeader("Комментарии", 450);
            if (isDetail)
            {
                excelReport.WriteHeader("Получатель", 250);
                excelReport.WriteHeader("Квитировал", 250);
            }
            excelReport.NewRow();
        }
        private void EventInfoToExcel(EventDTO dto)
        {
            excelReport.WriteCell(dto.PriorityName);
            excelReport.WriteCell(dto.TypeName);
            excelReport.WriteCell(dto.SerialNumber);
            excelReport.WriteCell(dto.EventDate.Value.ToString("dd.MM.yyyy HH:mm"));
            excelReport.WriteCell(dto.Description);
            excelReport.WriteCell(dto.SiteName);
            excelReport.WriteCell(dto.Entity.ShortPath);
            excelReport.WriteCell(dto.CreateUserName + Environment.NewLine + dto.UserEntityName +
                        (dto.EventDate.HasValue
                            ? (Environment.NewLine + dto.EventDate.Value.ToDailyDateTimeString())
                            : string.Empty));
            excelReport.WriteCell(comment);
        }
        private async void CreateCommentForEvent(EventDTO dto)
        {
            comment = new StringBuilder();
            var isFirstRow = true;
            foreach (var a in await EventAttachmentViewModel.LoadData(dto))
            {
                if (isFirstRow) isFirstRow = false; else comment.AppendLine();
                comment.Append($"Пользователь: {a.UserName}").AppendLine();
                comment.Append($"Комментарий: {a.Description}").AppendLine();
                if (a.FileName != null) comment.Append($"Вложение: {a.FileName}").AppendLine();
            }
        }
        private void Fill(List<EventDTO> data)
        {
            foreach (var item in data)
                item.TypeName = ClientCache.DictionaryRepository.EventTypes.SingleOrDefault((e) => e.Id == item.TypeId).Name ?? "Тип не известен";
            ListEvent = data;
            EventDTO tmpEvent = null;
            if (_tmpSelectedEventId.HasValue)
            {
                tmpEvent = ListEvent.FirstOrDefault(e => e.Id == _tmpSelectedEventId.Value);
            }
            SelectedEvent = tmpEvent ?? ListEvent.FirstOrDefault();
            ExportExcelCommand.RaiseCanExecuteChanged();
        }

        private async void TimerTick(object sender, EventArgs e)
        {
            var result =
                await ServiceLocator.Current.GetInstance<ITimeServiceProxy>().GetServerStateAsync(DTO.Module.EventLog);
            if (SessionInfo.ModulesStates[DTO.Module.EventLog] == result)
            {
                return;
            }
            if (SessionInfo.ModulesStates[DTO.Module.EventLog] != Guid.Empty)
            {
                Refresh();
            }
            SessionInfo.ModulesStates[DTO.Module.EventLog] = result;
        }

        private void RefreshCommands()
        {
            RemoveCommand.RaiseCanExecuteChanged();
            EditCommand.RaiseCanExecuteChanged();
            AcknowledgeCommand.RaiseCanExecuteChanged();
            TrashCommand.RaiseCanExecuteChanged();
            AddCommentCommand.RaiseCanExecuteChanged();
            AddFileCommand.RaiseCanExecuteChanged();
            RestoreCommand.RaiseCanExecuteChanged();

            TraceStartCommand.RaiseCanExecuteChanged();
            TraceStopCommand.RaiseCanExecuteChanged();
        }

        private void NotifyEventListUpdated()
        {
            EventLogMainViewModel.NotifyEventListUpdated();
        }

        private void RefreshNonAckEventCount()
        {
            EventLogMainViewModel.UpdateNotAckEventCount();
            RefreshFromTab();
        }

        private async void Restore()
        {
            Behavior.TryLock();
            try
            {
                await new EventLogServiceProxy().RestoreFromTrashAsync(new RestoreFromTrashEventParameterSet
                {
                    EventId = SelectedEvent.Id,
                    SiteId = UserProfile.Current.Site.Id
                });
            }
            finally
            {
                Behavior.TryUnlock();
            }
            RefreshNonAckEventCount();
            NotifyEventListUpdated();
        }

        private void Add()
        {
            var vm = new AddEditEventViewModel(id => AddRefresh());
            var view = new AddEditEventView {DataContext = vm};
            view.ShowDialog();
            //new EventLogDataProvider().GetEventList(null, Fill, Behavior);
        }

        private async void Delete()
        {
            Behavior.TryLock();
            try
            {
                await new EventLogServiceProxy().DeleteEventAsync(SelectedEvent.Id);
            }
            finally
            {
                Behavior.TryUnlock();
            }
            RefreshNonAckEventCount();
            NotifyEventListUpdated();
        }

        private void Remove()
        {
            if (CanEditDelete())
            {
                ShowConfirmWindow("Удалить объект?", (obj, e) =>
                {
                    if (e.DialogResult.HasValue && e.DialogResult.Value)
                    {
                        Delete();
                    }
                });
            }
            else
            {
                ShowCantEditDeleteAlert();
            }
        }

        private void ShowCantEditDeleteAlert()
        {
            MessageBoxProvider.Alert(
                "Редактировать или удалять событие может только создавший его сотрудник в течение 15 минут.",
                "Сообщение");
        }

        private void ShowConfirmWindow(string content, EventHandler<WindowClosedEventArgs> closeHandler)
        {
            RadWindow.Confirm(new DialogParameters
            {
                Content = content,
                Header = "Подтверждение удаления",
                CancelButtonContent = "Нет",
                OkButtonContent = "Да",
                Closed = closeHandler
            });
        }

        private bool CanEditDelete()
        {
            if (SelectedEvent.TypeId == 2 || SelectedEvent.TypeId == 5 || SelectedEvent.TypeId == 6) return false;
            return (!SelectedEvent.CreateDate.HasValue ||
                    (DateTime.Now - SelectedEvent.CreateDate.Value) <= new TimeSpan(0, 15, 0)) &&
                   SelectedEvent.CreateUserId == UserProfile.Current.Id;
        }

        private void Edit()
        {
            if (CanEditDelete())
            {
                var vm = new AddEditEventViewModel(id => Refresh(), SelectedEvent);
                var view = new AddEditEventView {DataContext = vm};
                view.ShowDialog();
            }
            else
            {
                ShowCantEditDeleteAlert();
            }
        }

        private async void Check()
        {
            switch (SelectedEvent.Priority)
            {
                case EventPriority.Normal:
                    Behavior.TryLock();
                    try
                    {
                        await new EventLogServiceProxy().TakeToControlEventAsync(new TakeToControlEventParameterSet
                        {
                            EventId = SelectedEvent.Id,
                            SiteId = UserProfile.Current.Site.Id
                        });
                    }
                    finally
                    {
                        Behavior.TryUnlock();
                    }
                    if (SelectedEvent.IsQuote)
                    {
                        Refresh();
                    }
                    else
                    {
                        Acknowledge();
                    }
                    break;
                case EventPriority.Control:
                {
                    var minDate = DateTime.Now.AddDays(-5);
                    if (SelectedEvent.EventDate.Value.Date < minDate.Date)
                    {
                        RadWindow.Confirm(new DialogParameters
                        {
                            Header = "Внимание!",
                            Content =
                                $@"
Событие было создано раньше {minDate:d}, если вы продолжите,
то оно будет автоматически перемещено в архив!
Вы уверены, что хотите снять событие с контроля?",
                            Closed = async (sender, e) =>
                            {
                                if (e.DialogResult.HasValue && e.DialogResult.Value)
                                {
                                    try
                                    {
                                        await new EventLogServiceProxy().BackToNormalEventAsync(
                                            new BackToNormalEventParameterSet
                                            {
                                                EventId = SelectedEvent.Id,
                                                SiteId = UserProfile.Current.Site.Id
                                            });
                                    }
                                    finally
                                    {
                                        RefreshNonAckEventCount();
                                    }
                                }
                            }
                        });
                    }
                    else
                    {
                        Behavior.TryLock();
                        try
                        {
                            await new EventLogServiceProxy().BackToNormalEventAsync(
                                new BackToNormalEventParameterSet
                                {
                                    EventId = SelectedEvent.Id,
                                    SiteId = UserProfile.Current.Site.Id
                                });
                        }
                        finally
                        {
                            Behavior.TryUnlock();
                        }

                        RefreshNonAckEventCount();
                    }
                }
                    break;
            }
        }

        private async void Acknowledge()
        {
            Behavior.TryLock();
            try
            {
                await
                    new EventLogServiceProxy().AckEventAsync(new AckEventParameterSet
                    {
                        EventId = SelectedEvent.Id,
                        SiteId = UserProfile.Current.Site.Id
                    });
            }
            finally
            {
                Behavior.TryUnlock();
            }
            RefreshNonAckEventCount();
        }

        private async void Trash()
        {
            Behavior.TryLock();
            try
            {
                await new EventLogServiceProxy().MoveToTrashAsync(new MoveToTrashEventParameterSet
                {
                    EventId = SelectedEvent.Id,
                    SiteId = UserProfile.Current.Site.Id
                });
            }
            finally
            {
                Behavior.TryUnlock();
            }

            RefreshNonAckEventCount();
            NotifyEventListUpdated();
        }

        public class DateType
        {
            public string Name { get; set; }
            public DateTypes Type { get; set; }
        }

    }
}