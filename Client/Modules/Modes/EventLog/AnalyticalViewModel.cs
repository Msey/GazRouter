using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.DataProviders.EventLog;
using GazRouter.DTO.EventLog.EventAnalytical;
using Microsoft.Practices.Prism.Commands;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Media.Imaging;
using Telerik.Windows.Documents.Fixed.Model.Editing;
using Telerik.Windows.Documents.Fixed.FormatProviders.Pdf;
using Telerik.Windows.Documents.Fixed.Model;
using Telerik.Windows.Documents.Fixed.FormatProviders.Pdf.Export;
using GazRouter.Application;

namespace GazRouter.Modes.EventLog
{
    public class AnalyticalViewModel : EventLogTabBase
    {
        public AnalyticalViewModel()
        {
            _listDatePeriod = new List<Period>
                {
                    new Period {Name = "Сутки", Type = PeriodType.Day},
//                    new Period {Name = "Неделя", Type = PeriodType.Week},
                    new Period {Name = "30 дней", Type = PeriodType.Month},
//                    new Period {Name = "Квартал", Type = PeriodType.Quart}
                };
            _selectedDatePeriod = _listDatePeriod[_listDatePeriod.Count - 1];
            _items = new List<EventAnalyticalDTO>();
            ExportPDFCommand = new DelegateCommand<FrameworkElement>(ExportPDF, x => true);
        }

        public override string Header
        {
            get { return "Аналитика"; }
        }

        public DelegateCommand<FrameworkElement> ExportPDFCommand { get; set; }

        public void ExportPDF(FrameworkElement content)
        {
            var dlg = new SaveFileDialog() { Filter = "Pdf files (*.pdf)|*.pdf|All files (*.*)|*.*", FilterIndex = 1, /*DefaultFileName = Header*/ };
            if (dlg.ShowDialog() == true)
            {
                var document = new RadFixedDocument();
                var page = new RadFixedPage();

                using (var ms = new System.IO.MemoryStream())
                {
                    ExportExtensions.ExportToImage(content, ms, new PngBitmapEncoder());
                    var source = new Telerik.Windows.Documents.Fixed.Model.Resources.ImageSource(ms, ImageQuality.High);
                    page.Size = new Size(source.Width, source.Height);
                    new FixedContentEditor(page).DrawImage(source);

                }
                document.Pages.Add(page);

                using (var stream = dlg.OpenFile())
                {
                    new PdfFormatProvider().Export(document, stream);//ExportSettings.ImageQuality = ImageQuality.High;
                    stream.Flush();
                }
            }
        }

        public override async void Refresh()
        {
            DateTime endDate = DateTime.Today.AddDays(1).AddMilliseconds(-1);
            DateTime startDate = DateTime.MinValue;
            switch (SelectedDatePeriod.Type)
            {
                case PeriodType.Day:
                    startDate = DateTime.Now.AddDays(-1);
                    break;
             /*   case PeriodType.Week:
                    startDate = DateTime.Now.AddDays(-7);
                    break;*/
                case PeriodType.Month:
                    startDate = endDate.AddMonths(-1);
                    break;
               /* case PeriodType.Quart:
                    startDate = endDate.AddMonths(-3);
                    break;*/
            }
            Behavior.TryLock();
            List<EventAnalyticalDTO> list;
            var parameterSet = new EventAnalyticalParameterSet { SiteId = UserProfile.Current.Site.Id, DateBegin = startDate, DateEnd = endDate };
            try
            {
                list = await new EventLogServiceProxy().EventAnalyticalAckListAsync(parameterSet);
            }
            finally 
            {
                Behavior.TryUnlock();
            }
            
            Fill(list);
            Behavior.TryLock();
            try
            {
                list = await new EventLogServiceProxy().EventAnalyticalListAsync(parameterSet);
            }
            finally 
            {
                Behavior.TryUnlock();
            }

            FillDiagram(list);
        }

        private void FillDiagram(List<EventAnalyticalDTO> list)
        {
            
            ItemsDiagram = list;
            if (ItemsDiagram.Any())
                AxisMaxTotalValue = ItemsDiagram.Max(p => p.Total);
        }

        private void Fill(List<EventAnalyticalDTO> list)
        {
         
            Items = list;
            AxisMaxValue = Items.Count > 0 ? Items.Max(p => p.Max) : 0;
        }

        public List<EventAnalyticalDTO> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                OnPropertyChanged(() => Items);
            }
        }

        private List<EventAnalyticalDTO> _items;

        public List<EventAnalyticalDTO> ItemsDiagram
        {
            get { return _itemsDiagram; }
            set
            {
                SetProperty(ref _itemsDiagram, value);
            }
        }

        private List<EventAnalyticalDTO> _itemsDiagram;

        public List<Period> ListDatePeriod
        {
            get { return _listDatePeriod; }
            set
            {
                _listDatePeriod = value;
                OnPropertyChanged(() => ListDatePeriod);
            }
        }

        private List<Period> _listDatePeriod;

        public Period SelectedDatePeriod
        {
            get { return _selectedDatePeriod; }
            set
            {
              if (  SetProperty(ref _selectedDatePeriod, value))
                Refresh();
            }
        }

        private Period _selectedDatePeriod;

        public double AxisMaxValue
        {
            get { return _axisMaxValue; }
            set
            {
                SetProperty(ref _axisMaxValue, value + Math.Max(5, value/30));
               
            }
        }

        private double _axisMaxValue;

        public double AxisMaxTotalValue
        {
            get { return _axisMaxTotalValue; }
            set
            {
                _axisMaxTotalValue = value + Math.Max(1, value / 20);
                OnPropertyChanged(() => AxisMaxTotalValue);
            }
        }

        private double _axisMaxTotalValue;


        public List<IEventTab> TabItems { get; set; }
    }
}