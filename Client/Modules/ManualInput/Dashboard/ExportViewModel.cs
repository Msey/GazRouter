using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Browser;
using GazRouter.Application;
using GazRouter.Application.Helpers;
using GazRouter.Common.ViewModel;
using GazRouter.Controls;
using GazRouter.Controls.InputStory;
using GazRouter.DataProviders.DataExchange;
using GazRouter.DataProviders.ManualInput;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DataProviders.SeriesData;
using GazRouter.DataProviders.Integro;
using GazRouter.DTO.DataExchange.ASUTPImport;
using GazRouter.DTO.DataExchange.ExchangeLog;
using GazRouter.DTO.DataExchange.ExchangeTask;
using GazRouter.DTO.DataExchange.Integro;
using GazRouter.DTO.Dictionaries.ExchangeTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.Integro;
using GazRouter.DTO.ManualInput.InputStates;
using GazRouter.DTO.SeriesData.EntityValidationStatus;
using GazRouter.DTO.SeriesData.Series;
using Telerik.Windows.Controls;
using Utils.Extensions;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;
using UriBuilder = GazRouter.DataProviders.UriBuilder;
using DataExchange.Integro.Summary;
using GazRouter.DataExchange.ASTRA;

namespace GazRouter.ManualInput.Dashboard
{
    public class ExportViewModel : LockableViewModel
    {
        private int _lastSerieID;
        private PeriodType _lastPeriod;
        private DateTime _lastDate;

        public readonly List<int> DataSourceIDs = new List<int>()
        {
           1, // АСДУ ЕСГ
           3, // АСТРА
           4, // АССПООТИ
           //8, // АСУ ТП
        };

        public List<ExportTaskViewModel> Items { get; private set; }

        public List<ExchangeLogDTO> Details => SelectedItem?.Logs;

        public bool ShowDetails => !(SelectedItem == null || SelectedItem.Logs.Count < 2);

        public bool ShowExportMenu => TestCommand();

        private ExportTaskViewModel _selectedItem;
        public ExportTaskViewModel SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if(SetProperty(ref _selectedItem, value))
                {
                    OnPropertyChanged(() => Details);
                    OnPropertyChanged(()=> ShowDetails);
                    UpdateCommands();
                }
            }
        }
        public DelegateCommand RunCommand { get; set; }
        public DelegateCommand SaveCommand { get; set; }

        public ExportViewModel()
        {
            Items = new List<ExportTaskViewModel>();
            RunCommand = new DelegateCommand(Run, TestCommand);
            SaveCommand = new DelegateCommand(Save, TestCommand);
        }

        public async void LoadTasks(DateTime date, PeriodType period)
        {
            _lastPeriod = period;
            if (!UserProfile.Current.Site.IsEnterprise) return;

            Lock();
            Items.Clear();
            try
            {
                var taskList = await new DataExchangeServiceProxy().GetExchangeTaskListAsync(
                new GetExchangeTaskListParameterSet
                {
                    ExchangeTypeId = ExchangeType.Export,
                    PeriodTypeId = period
                });
                Items = taskList.Select(item=> new ExportTaskViewModel(item)).ToList();

                var summaries = await new IntegroServiceProxy().GetSummariesListByParamsAsync(new GetSummaryParameterSet());
                Type T = typeof(MappingSourceType);
                var ErrantTasks = new List<ExportTaskViewModel>();
                foreach (var SummaryTask in Items.Where(i=> Enum.IsDefined(T,i.DataSourceID)))
                {
                    var Summary = summaries.FirstOrDefault(s => s.ExchangeTaskId == SummaryTask.ID);
                    if (Summary == null)
                        ErrantTasks.Add(SummaryTask);
                    else
                        SummaryTask.SummaryID = Summary.Id;
                }
                Items = Items.Except(ErrantTasks).ToList();
            }
            finally
            {
                OnPropertyChanged(() => Items);
                Unlock();
            }
            LoadTaskLogs(date);
        }

        public async void LoadTaskLogs(DateTime date)
        {
            _lastDate = date;
            _lastSerieID = (await new SeriesDataServiceProxy().AddSerieAsync(
                new AddSeriesParameterSet
                {
                    KeyDate = date.ToLocal(),
                    Day = date.Day,
                    Month = date.Month,
                    Year = date.Year,
                    PeriodTypeId = _lastPeriod,
                })).Id;

            var LogItems = await new DataExchangeServiceProxy().GetExchangeLogAsync(new GetExchangeLogParameterSet()
            {
                StartDate = null,
                EndDate = null,
                SerieId =_lastSerieID,
            });

            foreach(var Item in Items)
            {
                Item.SetLogs(LogItems.Where(l=>l.ExchangeTaskId==Item.ID).ToList());
            }
            OnPropertyChanged(() => Details);
            OnPropertyChanged(() => ShowDetails);
        }
        
        private void Save()
        {
            if (_selectedItem != null)
            {
                switch (_selectedItem.DataSourceID)
                {
                    case 3:
                        {
                            AstraViewModel.SaveAs(_selectedItem.ID, _lastDate, _lastPeriod, !_selectedItem.IsTransform);
                        }
                        break;

                    case 1:
                        {
                            SummaryCatalogViewModel.SaveExporSummary(_selectedItem.SummaryID, _lastDate, _lastPeriod, MappingSourceType.ASDU_ESG);
                            break;
                        }
                    case 4:
                        {
                            SummaryCatalogViewModel.SaveExporSummary(_selectedItem.SummaryID, _lastDate, _lastPeriod, MappingSourceType.ASSPOOTI);
                            break;
                        }
                    default:
                        //HtmlPage.Window.Navigate(UriBuilder.GetSpecificExchangeHandlerUri(_selectedItem.ID,
                        //               _lastPeriod == PeriodType.Day ? _lastDate.Date : _lastDate.ToLocal(), periodType: _lastPeriod, xmlOnly: !_selectedItem.IsTransform));
                        break;
                }
            }
        }

        private async void Run()
        {
            Lock();
            try
            {               
                switch (_selectedItem.DataSourceID)
                {
                    case 3:
                        {
                            await AstraViewModel.Run(_selectedItem.ID, _lastDate, _lastPeriod);
                        }
                        break;

                    case 1:
                        {
                            var result = await SummaryCatalogViewModel.ExportSummary(_selectedItem.SummaryID, _lastDate, _lastPeriod, MappingSourceType.ASDU_ESG);
                            break;
                        }
                    case 4:
                        {
                            var result = await SummaryCatalogViewModel.ExportSummary(_selectedItem.SummaryID, _lastDate, _lastPeriod, MappingSourceType.ASSPOOTI);
                            break;
                        }
                    default:
                        //await
                        //new DataExchangeServiceProxy().RunExchangeTaskAsync(new RunExchangeTaskParameterSet()
                        //{
                        //    Id = _selectedItem.ID,
                        //    TimeStamp = _lastPeriod == PeriodType.Day ? _lastDate.Date : _lastDate.ToLocal(),
                        //    PeriodTypeId = _lastPeriod
                        //});
                        break;
                }
                LoadTaskLogs(_lastDate);
            }
            finally
            {
                Unlock();
            }
        }

        private bool TestCommand()
        {
            return _selectedItem!=null && DataSourceIDs.Contains(_selectedItem.DataSourceID);
        }
        private void UpdateCommands()
        {
            SaveCommand.RaiseCanExecuteChanged();
            RunCommand.RaiseCanExecuteChanged();
            OnPropertyChanged(() => ShowExportMenu);
        }
    }

    public class ExportTaskViewModel: Common.ViewModel.ViewModelBase
    {
        private readonly ExchangeTaskDTO _Context;

        public int ID => _Context.Id;
        public int DataSourceID => _Context.DataSourceId;
        public string SourceName => _Context.DataSourceName;
        public string ExchangeTaskName => _Context.Name;
        public bool IsTransform => _Context.IsTransform;

        public Guid SummaryID;

        private bool? _Status;
        public bool? Status
        {
            get { return _Status; }
            set
            {
                SetProperty(ref _Status, value);
            }
        }

        private DateTime? _StartTime;
        public DateTime? StartTime
        {
            get { return _StartTime; }
            set
            {
                SetProperty(ref _StartTime, value);
            }
        }

        private string _Comment;
        public string Comment
        {
            get { return _Comment; }
            set
            {
                SetProperty(ref _Comment, value);
            }
        }

        private List<ExchangeLogDTO> _Logs;
        public List<ExchangeLogDTO> Logs => _Logs;

        public ExportTaskViewModel(ExchangeTaskDTO Context)
        {
            if(Context== null) throw new ArgumentNullException("Context");
            _Context = Context;
            _Logs = new List<ExchangeLogDTO>();
        }

        public void SetLogs(List<ExchangeLogDTO> Logs)
        {
            _Logs.Clear();
            if (Logs != null && Logs.Count > 0)
            {
                _Logs = Logs;
                var LastLog = Logs[0];
                if (Logs.Count > 1)
                {
                    LastLog = Logs.First(l => l.StartTime == Logs.Max(log => log.StartTime));
                }
                StartTime = LastLog.StartTime;
                Status = LastLog.IsOk;
                Comment = LastLog.ProcessingError;
            }
            else
            {
                StartTime = null;
                Status = null;
                Comment = null;
            }
            OnPropertyChanged(()=> Logs);
        }
    }
}
