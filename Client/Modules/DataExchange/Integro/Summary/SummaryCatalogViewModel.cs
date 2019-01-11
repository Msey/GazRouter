using DataExchange.ASDU;
using DataExchange.Integro.Summary;
using GazRouter.Application;
using GazRouter.Application.Helpers;
using GazRouter.Common;
using GazRouter.Common.ViewModel;
using GazRouter.DataExchange.CustomSource;
using GazRouter.DataExchange.ExchangeLog;
using GazRouter.DataExchange.Integro.ASSPOOTI;
using GazRouter.DataProviders.DataExchange;
using GazRouter.DataProviders.Integro;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.DataExchange.Asdu;
using GazRouter.DTO.DataExchange.ExchangeTask;
using GazRouter.DTO.DataExchange.Integro;
using GazRouter.DTO.DataExchange.Integro.Enum;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.ObjectModel.Aggregators;
using GazRouter.DTO.ObjectModel.DistrStations;
using GazRouter.DTO.ObjectModel.MeasStations;
using GazRouter.DTO.ObjectModel.ReducingStations;
using GazRouter.DTO.ObjectModel.Sites;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using Telerik.Windows.Controls;
using Utils.Extensions;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;
using UriBuilder = GazRouter.DataProviders.UriBuilder;
using ItemBase = DataExchange.ASDU.ItemBase;
using DataExchange.Integro.ASSPOOTI;
using GazRouter.DTO.DataExchange.ExchangeLog;
using GazRouter.DTO.Dictionaries.Integro;

namespace DataExchange.Integro.Summary
{
    public class SummaryCatalogViewModel : LockableViewModel
    {
        private SiteDTO _selectedSite;
        private ItemBase _selectedItem;

        public SummaryCatalogViewModel()
        {
            IsEditable = Authorization2.Inst.IsEditable(LinkType.AsduMapping);

            _selectedSystem = new SystemItem { Id = (int)MappingSourceType.ASDU_ESG, Name = "АСДУ ЕСГ", SourceType = MappingSourceType.ASSPOOTI};
        }

        public SummaryCatalogViewModel(string linkType)
        {
            if (LinkType.AsduMapping.ToString() == linkType) { 
                _selectedSystem = new SystemItem { Id = (int)MappingSourceType.ASDU_ESG, Name = "АСДУ ЕСГ", SourceType = MappingSourceType.ASDU_ESG};
                IsEditable = Authorization2.Inst.IsEditable(LinkType.AsduMapping);
            }
            else if (LinkType.AsspootiMapping.ToString() == linkType) { 
                _selectedSystem = new SystemItem { Id = (int)MappingSourceType.ASSPOOTI, Name = "АССПОТИ", SourceType = MappingSourceType.ASSPOOTI};
                IsEditable = Authorization2.Inst.IsEditable(LinkType.AsspootiMapping);
            }

            InitCommands();
            Init();
        }

        public bool IsEditable { get; set; }

        #region Commands

        private void InitCommands()
        {
            RefreshLogCommand = new DelegateCommand(RefreshLog);
            //
            RefreshSummariesCommand = new DelegateCommand(RefreshSummaries);
            EditSummaryCommand = new DelegateCommand(EditSummary, () => IsEditable && SelectedSummary != null );
            AddSummaryCommand = new DelegateCommand(AddSummary, () => IsEditable);
            RemoveSummaryCommand = new DelegateCommand(RemoveSummary, () => IsEditable && SelectedSummary != null);
            SaveExporSummaryCommand = new DelegateCommand(SaveExporSummary, () => IsEditable && SelectedSummary != null);
            ExportSummaryCommand = new DelegateCommand(ExportSummary, () => IsEditable && SelectedSummary != null);
            PreViewSummaryCommand = new DelegateCommand(PreViewExportSummary, () => IsEditable && SelectedSummary != null);
            MappingSummaryCommand = new DelegateCommand(MappingSummary, () => IsEditable && SelectedSummary != null);
            ShowDataCommand = new DelegateCommand(ShowDataLogSummary, () => IsEditable && SelectedExchangeLog != null);
            //LoadDescriptorCommand = new DelegateCommand(LoadDescriptor, () => SelectedSummary != null && (SelectedSystem.Id == (int)MappingSystemType.ASSPOOTI || (SelectedSystem.Id == (int)MappingSystemType.ASDU_ESG && !MappingDescriptorList.Any())));
            //LoadSummaryCommand = new DelegateCommand(LoadFromFileSummary, () => SelectedSummary != null);
            //LinkParamCommand = new DelegateCommand(LinkParam, () => IsLinkEnabled);
            //UnLinkParamCommand = new DelegateCommand(UnLinkParam, () => IsUnLinkEnabled);
            //AddParamCommand = new DelegateCommand(AddLinkParam, () => IsAddLinkEnabled);
            //LogListCommand = new DelegateCommand(OpenLogList, () => IsAddLinkEnabled);
        }

        public DelegateCommand RefreshSummariesCommand { get; private set; }
        public DelegateCommand AddSummaryCommand { get; private set; }
        public DelegateCommand EditSummaryCommand { get; private set; }
        public DelegateCommand RemoveSummaryCommand { get; private set; }
        public DelegateCommand ExportSummaryCommand { get; private set; }
        public DelegateCommand PreViewSummaryCommand { get; private set; }
        public DelegateCommand SaveExporSummaryCommand { get; private set; }
        public DelegateCommand RefreshLogCommand { get; private set; }
        //
       // public DelegateCommand LoadSummaryCommand { get; private set; }

        public DelegateCommand MappingSummaryCommand { get; private set; }
        public DelegateCommand ShowDataCommand { get; private set; }
        //public DelegateCommand LoadDescriptorCommand { get; private set; }
        //public DelegateCommand LinkParamCommand { get; private set; }
        //public DelegateCommand UnLinkParamCommand { get; private set; }
        //public DelegateCommand AddParamCommand { get; private set; }
        //public DelegateCommand LogListCommand { get; private set; }

        #endregion

        public async void Init()
        {
             //systems = new ObservableCollection<SystemItem> { new SystemItem { Id = (int)MappingSystemType.ASDU_ESG, Name = "АСДУ ЕСГ" } };
            _selectedDate = DateTime.Today;
            //SelectedSystem = systems.FirstOrDefault(f => f.Id == (int)MappingSystemType.ASDU_ESG);
            RefreshSummaries();
        }

        #region Prop



        public List<SummaryItem> SummariesList { get; set; }

        //public List<ExchangeTaskDTO> TaskList { get; set; }

        private SummaryItem _selectedSummary;

        public SummaryItem SelectedSummary
        {
            get { return _selectedSummary; }
            set
            {
                if (SetProperty(ref _selectedSummary, value))
                {
                    //LoadSummeryParam(value);
                    RefreshLog();
                    RefreshCommands();
                    if (SelectedSystem != null && SelectedSystem.SourceType == MappingSourceType.ASSPOOTI)
                    {
                        SetSessionTime();
                    }
                }
            }
        }

        private void LoadSummeryParam(SummaryItem value)
        {
            try
            {
                Behavior.TryLock();
                if (value != null)
                {
                    if (!string.IsNullOrEmpty(value.Dto.Descriptor))
                    {
                        var index = value.Dto.Descriptor.IndexOf(";");
                        if (index > 0)
                        {
                            //var fileName = value.Dto.Descriptor.Substring(index+1, value.Dto.Descriptor.Length - index-1);
                            var fileName = value.Dto.Descriptor.Substring(0, index);
                            LoadDescriptor(fileName);
                            //GetLinkParam();
                        }
                    }
                }
            }
            finally
            {
                Behavior.TryUnlock();
            }

        }

        private DateTime selDate = SeriesHelper.GetCurrentSession(); //new DateTime(DateTime.Now.Year,DateTime.Now.Month, DateTime.Now.Day);
        public DateTime SelDate
        {
            get { return selDate; }
            set
            {
                SetProperty(ref selDate, value);
            }
        }

        private ObservableCollection<SystemItem> systems;
        public ObservableCollection<SystemItem> Systems
        {
            get { return systems; }
            set { SetProperty(ref systems, value); }
        }

        private SystemItem _selectedSystem;
        public SystemItem SelectedSystem
        {
            get { return _selectedSystem; }
            set
            {
                if (SetProperty(ref _selectedSystem, value))
                {
                    RefreshSummaries();
                    //IsReadOnlyDesc = (value.Id == (int)MappingSystemType.ASSPOOTI);
                    //IsDescriptorVisibil = (value.Id == (int)MappingSystemType.ASSPOOTI);
                }
            }
        }


        private bool CanLoadSummary()
        {
            return true;
        }

        #endregion

        private async void SetSummaryExchTasks()
        {
            var paramSummary = new GetSummaryParameterSet() { SystemId = SelectedSystem.Id };
            var summaries = await new IntegroServiceProxy().GetSummariesListByParamsAsync(paramSummary); // IntegroServiceProxy
            var paramTask = new GetExchangeTaskListParameterSet() { SourceId = (int)SelectedSystem.SourceType };
            var taskList = await new DataExchangeServiceProxy().GetExchangeTaskListAsync(paramTask);

            var summaryTasks =
                from s in summaries
                join t in taskList on s.ExchangeTaskId equals t.Id into ps
                from p in ps.DefaultIfEmpty()
                select new SummaryItem(s, p);
            SummariesList = summaryTasks.ToList();
            foreach (var s in SummariesList)
            {
                if (s.TaskDto != null)
                {
                    var h = string.IsNullOrEmpty(s.TaskDto.ExcludeHours) ? 0 : GetHourFromFileNameMask(s.TaskDto.ExcludeHours);
                    s.SummaryHour = h > 24 ? 0 : h;
                }
            }
            
            OnPropertyChanged(() => SummariesList);
        }
        private int GetHourFromFileNameMask(string excludeHours)
        {
            int result = 0;
            if (!int.TryParse(excludeHours, out result))
                         result = 0;
            return result;
        }

        private async void RefreshSummaries()
        {
            if (SelectedSystem == null)
                return;

            try
            {
                Behavior.TryLock();
                //var parameters = new GetSummaryParameterSet() { SystemId = SelectedSystem.Id };
                //var summaries = await new IntegroServiceProxy().GetSummariesListByParamsAsync(parameters); // IntegroServiceProxy
                //SummariesList = summaries.Select(s => new SummaryItem(s)).ToList();
                SetSummaryExchTasks();
            }
            finally
            {
                Behavior.TryUnlock();
            }
        }

        private void AddSummary()
        {
            var viewModel = new AddEditSummaryViewModel(RefreshSummaries, SelectedSystem);
            var view = new AddEditSummaryView { DataContext = viewModel };
            view.ShowDialog();
        }

        private void EditSummary()
        {
            var summExch = new SummaryExchTaskDTO() { Summary = SelectedSummary.Dto, ExchangeTask = SelectedSummary.TaskDto };
            var viewModel = new AddEditSummaryViewModel(RefreshSummaries, summExch, SelectedSystem);
            var view = new AddEditSummaryView { DataContext = viewModel };
            view.ShowDialog();
        }

        private void RemoveSummary()
        {
            var dp = new DialogParameters
            {
                Closed = async (s, e) =>
                {
                    if (e.DialogResult.HasValue && e.DialogResult.Value)
                    {
                        await new IntegroServiceProxy().DeleteSummaryAsync(SelectedSummary.Dto.Id);
                        RefreshSummaries();
                    }
                },
                Content = "Вы уверены, что хотите удалить сводку?",
                Header = "Удаление сводки",
                OkButtonContent = "Да",
                CancelButtonContent = "Нет"
            };

            RadWindow.Confirm(dp);
        }

        private void SaveExporSummary()
        {
            var summaryId = SelectedSummary.Dto.Id;
            var dt = ((DateTime)SelDate).ToLocal();
            //var seriesId = 
            //HtmlPage.Window.Navigate(GetExchangeHandlerUri(summaryId, dt, 0, (int)SelectedSummary.Dto.PeriodType, SelectedSystem.Id));
            SaveExporSummary(summaryId,dt, SelectedSummary.Dto.PeriodType, SelectedSystem.SourceType);
        }

        public static void SaveExporSummary(Guid summaryId, DateTime dt, PeriodType periodType, MappingSourceType sourceType)
        {
            HtmlPage.Window.Navigate(GetExchangeHandlerUri(summaryId, dt, 0, (int)periodType, (int)sourceType));
        }

        public static Uri GetExchangeHandlerUri(Guid summaryId, DateTime dt, int seriesId, int periodTypeId, int systemTypeId, bool xmlOnly = false)
        {
            var result = new System.UriBuilder(HtmlPage.Document.DocumentUri.OriginalString.ToLower().Replace(@"/default.aspx", string.Empty).TrimEnd('/'))
            {
                Query = $"summaryId={summaryId}&dt={dt.Ticks}&seriesId={seriesId}&periodTypeId={periodTypeId}&systemTypeId={systemTypeId}"
            };
            result.Path += $"/{DateTime.Now.Ticks}.axml";
            return result.Uri;
        }

        private async void PreViewExportSummary()
        {
            var offset = (int)TimeZoneInfo.Local.BaseUtcOffset.TotalHours - Settings.ServerTimeUtcOffset;
            ExportSummaryParams parameters = new ExportSummaryParams()
            {
                PeriodDate = offset != 0 ? SelDate.AddHours(-offset) : SelDate, // уточнить
                SummaryId = SelectedSummary.Dto.Id,
                SystemId = SelectedSystem.Id,
                PeriodType = SelectedSummary.Dto.PeriodType,
                GetResult = true
            };

            var vm = new PreViewSummaryViewModel(parameters,SelectedSummary);
            var v = new PreViewSummaryView { DataContext = vm };
            v.ShowDialog();
        }
        private async void ExportSummary()
        {
            try
            {
                Behavior.TryLock();
                //// Уточнить.
                //var offset = (int)TimeZoneInfo.Local.BaseUtcOffset.TotalHours - Settings.ServerTimeUtcOffset;
                //ExportSummaryParams parameters = new ExportSummaryParams()
                //{
                //    PeriodDate = SelectedSummary.Dto.PeriodType == GazRouter.DTO.Dictionaries.PeriodTypes.PeriodType.Twohours ? 
                //            (offset != 0 ? SelDate.AddHours(-offset) : SelDate) : new DateTime(SelDate.Year, SelDate.Month, SelDate.Day), 
                //    SummaryId = SelectedSummary.Dto.Id,
                //    SystemId = SelectedSystem.Id,
                //    PeriodType = SelectedSummary.Dto.PeriodType,
                //};
                //var summaries = await new IntegroServiceProxy().ExportSummaryAsync(parameters);
                //if (summaries.ResultType == ExportResultType.Error)
                //{
                //    MessageBox.Show($"Файл {summaries.ExportFileName} выгружен с ошибкой. Ошибка :{summaries.Description}");
                //    //throw new ServerException(new Exception(summaries.Description));
                //}
                //else if(summaries.ResultType == ExportResultType.ValidationError)
                //{
                //    MessageBox.Show(String.Format("Файл '{0}' не прошел валидацию. Ошибка: {1} ", summaries.ExportFileName, summaries.Description));
                //}
                //else
                //    MessageBox.Show(String.Format("Файл '{0}' успешно сформирован. ", summaries.ExportFileName));
                var summaries = await ExportSummary(SelectedSummary.Dto.Id, SelDate, SelectedSummary.Dto.PeriodType, SelectedSystem.SourceType); if (summaries.ResultType == ExportResultType.Error)
                {
                    MessageBox.Show($"Файл {summaries.ExportFileName} выгружен с ошибкой. Ошибка :{summaries.Description}");
                    //throw new ServerException(new Exception(summaries.Description));
                }
                else if (summaries.ResultType == ExportResultType.ValidationError)
                {
                    MessageBox.Show(String.Format("Файл '{0}' не прошел валидацию. Ошибка: {1} ", summaries.ExportFileName, summaries.Description));
                }
                else
                    MessageBox.Show(String.Format("Файл '{0}' успешно сформирован. ", summaries.ExportFileName));
            }
            finally
            {
                Behavior.TryUnlock();
            }
        }

        public static async Task<ExportResult> ExportSummary(Guid summaryID, DateTime dt, PeriodType periodType, MappingSourceType sourceType)
        {
            // Уточнить.
            var offset = (int)TimeZoneInfo.Local.BaseUtcOffset.TotalHours - Settings.ServerTimeUtcOffset;
            ExportSummaryParams parameters = new ExportSummaryParams()
            {
                PeriodDate = periodType == GazRouter.DTO.Dictionaries.PeriodTypes.PeriodType.Twohours ?
                        (offset != 0 ? dt.AddHours(-offset) : dt) : new DateTime(dt.Year, dt.Month, dt.Day),
                SummaryId = summaryID,
                SystemId = (int)sourceType,
                PeriodType = periodType,
            };
            return await new IntegroServiceProxy().ExportSummaryAsync(parameters);
           
        }

        private void LoadDescriptor(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return;

            System.IO.FileStream fileStream = null;
            if (string.IsNullOrEmpty(fileName))
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == true)
                {
                    fileStream = openFileDialog.File.OpenRead();
                }
            }
            else if ((File.Exists(fileName)))
            {
                fileStream = File.OpenRead(fileName);
            }
            if (fileStream != null)
            {
                //MappingDescriptorList = new ObservableCollection<MappingDescriptorDto>(MappingHelper.ParsingDescriptorFile(fileStream));
            }
        }
        //private async void LoadFromFileSummary()
        //{
        //    OpenFileDialog openFileDialog = new OpenFileDialog();
        //    //openFileDialog.Filter = FILE_FILTER;
        //    if (openFileDialog.ShowDialog() == true)
        //    {
        //        try
        //        {

        //            //SelectedSummary.Dto.Descriptor = openFileDialog.File.Name + ";";
        //            System.IO.FileStream fileStream = openFileDialog.File.OpenRead();
        //            var parameters = MappingHelper.ParsingSummaryFile(fileStream, SelectedSummary);
        //            if (parameters == null || !parameters.Any())
        //                return;
        //            List<AddEditSummaryPParameterSet> lex = new List<AddEditSummaryPParameterSet>();
        //            //foreach (var item in MappingParamList)
        //            //{
        //            //    var ex = parameters.FirstOrDefault(f =>
        //            //                      f.ParametrGid == item.ParameterDescriptorName &&
        //            //                      f.ContentList.Any(a1 => a1.EntityId == item.EntityId && a1.PropertyTypeId == item.PropertyTypeId));
        //            //    if (ex != null)
        //            //    {
        //            //        lex.Add(ex);
        //            //        parameters.Remove(ex);
        //            //    }
        //            //}
        //            SummatyLoadResult res = await new IntegroServiceProxy().AddSummaryParamListAsync(parameters);
        //            res.DublicateEntity = lex;
        //        }
        //        catch (Exception e)
        //        {
        //            throw new Exception($"Ошибка чтения файла:{e.Message} {e.StackTrace}");
        //        }
        //        //LoadSummeryParam(SelectedSummary);
        //        //UpdateSummary();
        //    }
        //}

        private async void UpdateSummary()
        {
            var param = new AddEditSummaryParameterSet
            {
                Id = SelectedSummary.Dto.Id,
                Name = SelectedSummary.Dto.Name,
                Descriptor = SelectedSummary.Dto.Descriptor,
                TransformFileName = SelectedSummary.Dto.TransformFileName,
                PeriodType = SelectedSummary.Dto.PeriodType,
                SystemId = SelectedSummary.Dto.SystemId,

            };
            await new IntegroServiceProxy().AddEditSummaryAsync(param);
        }

        private void MappingSummary()
        {
            //var viewModel = new AddEditSummaryViewModel(RefreshSummaries, SelectedSystem.Id);
            //var view = new AddEditSummaryView { DataContext = viewModel };
            //view.ShowDialog();

            var vm = new MappingSummaryAsduEsgViewModel(SelectedSummary);
            var v = new MappingSummaryAsduEsgView { DataContext = vm };
            v.ShowDialog();
        }

        // Загрузка списка свойств по выбранному объекту

        private void RefreshCommands()
        {
            EditSummaryCommand.RaiseCanExecuteChanged();
            RemoveSummaryCommand.RaiseCanExecuteChanged();
            ExportSummaryCommand.RaiseCanExecuteChanged();
            SaveExporSummaryCommand.RaiseCanExecuteChanged();
            PreViewSummaryCommand.RaiseCanExecuteChanged();
            //LoadDescriptorCommand.RaiseCanExecuteChanged();
            //LoadSummaryCommand.RaiseCanExecuteChanged();
            MappingSummaryCommand.RaiseCanExecuteChanged();
        }

        #region LOG


        private DateTime _selectedDate;
        public DateTime SelectedDate
        {
            get { return _selectedDate; }
            set
            {
                if (SetProperty(ref _selectedDate, value))
                    RefreshLog();
            }
        }

        private List<ExchangeLogDTO> exchangeLogItems;
        public List<ExchangeLogDTO> ExchangeLogItems
        {
            get { return exchangeLogItems; }
            set
            {
                exchangeLogItems = value;
                if (value != null)
                    LogItems = new ObservableCollection<ExchangeLogDTO>(value);

            }
        }

        public ObservableCollection<ExchangeLogDTO> LogItems { get; set; }


        private ExchangeLogDTO _selectedExchangeLog;
        public ExchangeLogDTO SelectedExchangeLog
        {
            get { return _selectedExchangeLog; }
            set
            {
                SetProperty(ref _selectedExchangeLog, value);
                ShowDataCommand.RaiseCanExecuteChanged();
            }
        }

        private void ShowDataLogSummary()
        {
            var vm = new PreViewSummaryViewModel(SelectedExchangeLog, SelectedSummary);
            var v = new PreViewSummaryView { DataContext = vm };
            v.ShowDialog();
        }

        private async void RefreshLog()
        {
            if (LogItems != null)
                LogItems.Clear();
            if (SelectedSummary == null || SelectedSummary.TaskDto == null)
                return;
            try {
                Lock();
                ExchangeLogItems = await new DataExchangeServiceProxy().GetExchangeLogAsync(
                        new GetExchangeLogParameterSet
                        {
                            StartDate = _selectedDate.Date,
                            EndDate = _selectedDate.Date.AddDays(1),
                            ExchangeTaskId = SelectedSummary.TaskDto.Id
                        });
                OnPropertyChanged(() => LogItems);
            }
            finally
            {
                Unlock();
            }
        }

        private void SetSessionTime()
        {
            if (SelectedSummary == null)
                return;
            var now = DateTime.Now;
            SelDate = new DateTime(now.Year, now.Month, now.Day, SelectedSummary.SummaryHour,0,0);
        }
        #endregion
    }
}


