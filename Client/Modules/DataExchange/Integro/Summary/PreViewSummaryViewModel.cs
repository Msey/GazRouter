using DataExchange.Integro.ASSPOOTI;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.Integro;
using GazRouter.DTO.DataExchange.ExchangeLog;
using GazRouter.DTO.DataExchange.Integro;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace DataExchange.Integro.Summary
{
    public class PreViewSummaryViewModel : DialogViewModel
    {

        private ExportSummaryParams exportSummaryParams;
        private SummaryItem summary;
        private ExchangeLogDTO exchangeLog;
        /// <summary>
        /// Карточка только для просмотра
        /// </summary>
        private bool isReadOnly; 
        public PreViewSummaryViewModel(ExportSummaryParams exportSummaryParams, SummaryItem summary) : base(null)
        {
            isReadOnly = false;
            this.exportSummaryParams = exportSummaryParams;
            this.summary = summary;
            Init();
        }

        public PreViewSummaryViewModel(ExchangeLogDTO exchangeLog, SummaryItem summary) : base(null)
        {
            isReadOnly = true;
            this.exchangeLog = exchangeLog;
            this.summary = summary;
            Init();
        }
        public DelegateCommand SendCommand { get; set; }


        private string summaryData;
        /// <summary>
        /// XML для отправки
        /// </summary>
        public string SummaryData
        {
            get { return summaryData; }
            set
            {
                if (SetProperty(ref summaryData, value))
                {
                    ParseToGrid(value);
                }

            }
        }

        private string summaryLog;

        public string SummaryLog
        {
            get { return summaryLog; }
            set
            {
                if (SetProperty(ref summaryLog, value))
                {
                    SummaryLogVisible = !string.IsNullOrEmpty(summaryLog);
                }

            }
        }

        private bool summaryLogVisible;

        public bool SummaryLogVisible
        {
            get { return summaryLogVisible; }
            set
            {
                if (SetProperty(ref summaryLogVisible, value))
                {
                }

            }
        }

        public ObservableCollection<IntegroExchangeDataSection> dataSectionItems;
        public ObservableCollection<IntegroExchangeDataSection> DataSectionItems
        {
            get { return dataSectionItems; }
            set
            {
                SetProperty(ref dataSectionItems, value);
            }
        }

        private void Init()
        {
            SendCommand = new DelegateCommand(SendData);
            if (!isReadOnly)
                ExportSummary();
            else
                GetExportSummaryFromLog();
        }

        private async void SendData()
        {
            if (exportSummaryParams == null)
                return;

            exportSummaryParams.GetResult = false;
            var summaries = await new IntegroServiceProxy().ExportSummaryAsync(exportSummaryParams);
            if (summaries.ResultType == ExportResultType.Error)
            {
                MessageBox.Show(String.Format("Файл '{0}' сформирован с ошибкой ", summaries.Description));
            }
            else
            {
                MessageBox.Show(String.Format("Файл '{0}' успешно сформирован. ", summaries.Description));
                CancelCommand.Execute();
            }
        }

        private void GetExportSummaryFromLog()
        {
            if (exchangeLog == null)
                return;
            GetSummaryDataFromLog();

        }

        private async void GetSummaryDataFromLog()
        {
            try
            {
                Behavior.TryLock();
                exportSummaryParams.GetFromLog = true;
                exportSummaryParams.SeriesId = exchangeLog.SerieId;
                exportSummaryParams.ExchangeTaskId = exchangeLog.ExchangeTaskId;
                exportSummaryParams.FileLogName = exchangeLog.DataContent;
                var result = await new IntegroServiceProxy().ExportSummaryAsync(exportSummaryParams);
                SummaryData = result?.ExportData;
                //if (result.ResultType == ExportResultType.Error)
                //{
                //    MessageBox.Show(String.Format("Файл '{0}' сформировался с ошибкой. ", result.Description));
                //}
                //else
                //    MessageBox.Show(String.Format("Файл '{0}' успешно сформирован. ", result.Description));
            }
            finally
            {
                Behavior.TryUnlock();
            }
        }

        private async void ExportSummary()
        {
            try
            {
                Behavior.TryLock();
                var result = await new IntegroServiceProxy().ExportSummaryAsync(exportSummaryParams);
                SummaryData = result?.ExportData;
                if (exportSummaryParams.GetResult && result!= null && result.ResultType != ExportResultType.Successful)
                    SummaryLog = string.IsNullOrEmpty(result.LogData) ? result.Description : result.LogData;
                //if (result.ResultType == ExportResultType.Error)
                //{
                //    MessageBox.Show(String.Format("Файл '{0}' сформировался с ошибкой. ", result.Description));
                //}
                //else
                //    MessageBox.Show(String.Format("Файл '{0}' успешно сформирован. ", result.Description));
            }
            finally
            {
                Behavior.TryUnlock();
            }
        }

        private void ParseToGrid(string xmlData)
        {
            XDocument doc = XDocument.Parse(xmlData);
            var dataSection = doc.Descendants("DataSection");
            var dataItems = new ObservableCollection<IntegroExchangeDataSection>();
            foreach (var dataItem in dataSection)
            {
                var newItem = new IntegroExchangeDataSection();
                newItem.Identifier = dataItem.Element("Identifier")?.Value;
                newItem.Value = dataItem.Element("Value")?.Value;
                newItem.Dimension = dataItem.Element("Dimension")?.Value;
                newItem.ParameterFullName = dataItem.Element("ParameterFullName")?.Value;
                dataItems.Add(newItem);
            }
            DataSectionItems = dataItems;
        }
    }
}
