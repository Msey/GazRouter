using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GazRouter.Application;
using GazRouter.Application.Helpers;
using GazRouter.Common.ViewModel;
using GazRouter.Controls;
using GazRouter.Controls.InputStory;
using GazRouter.DataProviders.DataExchange;
using GazRouter.DataProviders.ManualInput;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DataProviders.SeriesData;
using GazRouter.DTO.DataExchange.ASUTPImport;
using GazRouter.DTO.DataExchange.ExchangeLog;
using GazRouter.DTO.DataExchange.ExchangeTask;
using GazRouter.DTO.Dictionaries.ExchangeTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.ManualInput.InputStates;
using GazRouter.DTO.SeriesData.EntityValidationStatus;
using GazRouter.DTO.SeriesData.Series;
using Telerik.Windows.Controls;
using Utils.Extensions;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;

namespace GazRouter.ManualInput.Dashboard
{
    public class DashboardViewModel : LockableViewModel
    {
        /// <summary>
        /// Информация о выбранной серии.
        /// Нужна всегда, привязана жестко к SelectedDate
        /// </summary>
        private SeriesDTO _serie;
        
        
        public DashboardViewModel()
        {
            RefreshCommand = new DelegateCommand(Refresh);
            ResetInputStateCommand = new DelegateCommand(ResetInputState, () => (_selectedItem as SiteItem)?.Status == DataStatus.Ok);
            AutorefreshActivityCommand = new DelegateCommand(()=> { AutorefreshEnabled = !AutorefreshEnabled; });
            ReloadSerieCommand = new DelegateCommand(ReloadSerie);
            ExportTasksViewModel = new ExportViewModel();
            RunExportTaskCommand = ExportTasksViewModel.RunCommand;
            SaveExportTaskCommand = ExportTasksViewModel.SaveCommand;
            _selectedDate = SeriesHelper.GetCurrentSession(); 

            RefresInpStory();
            LoadExportTasks();
        }



        public DelegateCommand RefreshCommand { get; private set; }
        public DelegateCommand ResetInputStateCommand { get; private set; }
        public DelegateCommand AutorefreshActivityCommand { get; private set; }
        public DelegateCommand ReloadSerieCommand { get; set; }
        public DelegateCommand RunExportTaskCommand { get; }
        public DelegateCommand SaveExportTaskCommand { get; }


        public IEnumerable<PeriodType> PeriodList
        {
            get
            {
                yield return PeriodType.Twohours;
                yield return PeriodType.Day;
            }
        }


        private PeriodType _selectedPeriod = PeriodType.Twohours;

        public PeriodType SelectedPeriod
        {
            get { return _selectedPeriod; }
            set
            {
                if (SetProperty(ref _selectedPeriod, value))
                {
                    OnPropertyChanged(() => IsDaySelected);
                    refreshExportTasks = false;
                    SelectedDate = IsDaySelected ? SeriesHelper.GetCurrentDispDay() : SeriesHelper.GetCurrentSession();
                    LoadExportTasks();
                    refreshExportTasks = true;
                }
            }
        }

        public bool IsDaySelected => _selectedPeriod == PeriodType.Day;


        private bool _autorefreshEnabled;
        public bool AutorefreshEnabled
        {
            get { return _autorefreshEnabled; }
            set
            {
                if (SetProperty(ref _autorefreshEnabled, value))
                    OnPropertyChanged(() => AutorefreshEnabled);
                if(value)
                {
                    InpStoryViewModel.ActivateTimer(TimerInterval);
                }
                else
                {
                    InpStoryViewModel.DeactivateTimer();
                }
            }
        }


        private int _timerInterval = 5;
        public int TimerInterval
        {
            get { return _timerInterval; }
            set
            {
                if (SetProperty(ref _timerInterval, value))
                {
                    InpStoryViewModel.ActivateTimer(TimerInterval);
                }
            }
        }

        private DateTime _selectedDate;
        /// <summary>
        /// Выбранная дата. 
        /// Должна быть задана всегда, NULL недопустим. 
        /// Поэтому первым делом инициализируется в конструкторе.
        /// </summary>
        public DateTime SelectedDate
        {
            get { return _selectedDate; }
            set
            {
                if (SetProperty(ref _selectedDate, value))
                {
                    RefresInpStory();
                    if (refreshExportTasks) RefreshExportTasks();
                }
            }
        }
        
        public InputStoryViewModel InpStoryViewModel { get; set; }

        public ExportViewModel ExportTasksViewModel { get; }

        public bool ShowImportTaskActions => SelectedTabIndex == 0;


        private int _selectedTabIndex;
        public int SelectedTabIndex
        {
            get { return _selectedTabIndex; }
            set
            {
                if(SetProperty(ref _selectedTabIndex, value))
                {
                    OnPropertyChanged(()=> ShowImportTaskActions);
                }
            }
        }

        private void Refresh()
        {
            RefresInpStory();
            LoadExportTasks();
        }

        private void RefresInpStory()
        {
            if (InpStoryViewModel == null)
            {
                InpStoryViewModel = new InputStoryViewModel();
                InpStoryViewModel.PropertyChanged += (obj, e) => { SelectedItem = InpStoryViewModel.SelectedItem; };
            }
            InpStoryViewModel.Load(_selectedDate, _selectedPeriod, true);            
        }

        private ItemBase _selectedItem;
        public ItemBase SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (SetProperty(ref _selectedItem, value))
                    UpdateCommands();
            }
        }

        private void UpdateCommands()
        {
            ResetInputStateCommand.RaiseCanExecuteChanged();
        }

        // Сброс статуса ввода данных по ЛПУ (состояние ввод)
        private async void ResetInputState()
        {
            InpStoryViewModel?.Load(_selectedDate, _selectedPeriod, true);
            _serie = InpStoryViewModel.CurrentSerie;

            var item = _selectedItem as SiteItem;

            if (item == null || _serie==null) return;
            

            await new ManualInputServiceProxy().SetInputStateAsync(
                new SetManualInputStateParameterSet
                {
                    SerieId = _serie.Id,
                    SiteId = item.Dto.Id,
                    State = ManualInputState.Input
                });

            RefresInpStory();
        }

        // Запуск загрузки и расчета серии данных
        private void ReloadSerie()
        {
            new DataExchangeServiceProxy().AsutpImportAsync(
                new ASUTPImportParameterSet
                {
                    Timestamp = _selectedDate.ToLocal(),
                    PeriodType = _selectedPeriod
                });

            RadWindow.Alert(
                new DialogParameters
                {
                    Header = "Загрузка данных",
                    CancelButtonContent = "Закрыть",
                    OkButtonContent = "Закрыть",
                    Content = new TextBlock
                    {
                        Text = "Процесс загрузки данных запущен. Такая загрузка обычно занимает некоторое время (иногда весьма продолжительное). После завершения загрузки результат будет виден в журнале загрузок.",
                        Width = 300,
                        TextWrapping = TextWrapping.Wrap
                    }
                });
        }

        #region Export Tasks

        bool refreshExportTasks = true;

        private readonly Object SyncObj = new Object();
        private void LoadExportTasks()
        {
            lock (SyncObj)
                ExportTasksViewModel.LoadTasks(_selectedDate, _selectedPeriod);
        }

        private void RefreshExportTasks()
        {
            lock (SyncObj)
               ExportTasksViewModel.LoadTaskLogs(_selectedDate);
        }

        #endregion
    }
}