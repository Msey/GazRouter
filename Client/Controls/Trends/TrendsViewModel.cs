using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GazRouter.Application;
using GazRouter.Application.Helpers;
using GazRouter.Common;
using GazRouter.Controls.Dialogs;
using GazRouter.DataProviders.SeriesData;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.SeriesData.PropertyValues;
using Microsoft.Practices.ObjectBuilder2;
using Telerik.Windows.Controls;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;
using ViewModelBase = GazRouter.Common.ViewModel.ViewModelBase;

namespace GazRouter.Controls.Trends
{
    public class TrendsViewModel : ViewModelBase
    {
        public TrendsViewModel()
        {
            TrendList = new ObservableCollection<Trend>();

            TimebarPeriod = new Period(DateTime.Today.Year, DateTime.Today.Month, false);
            
            RefreshCommand = new DelegateCommand(Refresh);
            AddTrendCommand = new DelegateCommand(AddTrend);
            DuplicateTrendCommand = new DelegateCommand(DuplicateTrend, () => SelectedTrend != null);
            RemoveTrendCommand = new DelegateCommand(RemoveTrend, () => SelectedTrend != null);

            ExportExcelCommand = new DelegateCommand(ExportTrendsToExcel);
        }

        #region DataPeriods

        private Period _timebarPeriod;
        /// <summary>
        /// Выбранный период.
        /// Выбирается вверху, с помощью элемента выбора периода
        /// </summary>
        public Period TimebarPeriod
        {
            get { return _timebarPeriod; }
            set
            {
                if (SetProperty(ref _timebarPeriod, value))
                {
                    PeriodBegin = value.Begin;
                    PeriodEnd = value.End;
                    if (value.Span.TotalDays > 31)
                    {
                        VisiblePeriodBegin = value.Begin;
                        VisiblePeriodEnd = value.Begin.AddMonths(1);
                    }
                    TrendPeriodBegin = value.Begin;
                    TrendPeriodEnd = value.Begin.AddDays(1);

                    TrendList.ForEach(t => ReloadTrend(t));
                }
            }
        }



        private DateTime _periodBegin;
        /// <summary>
        /// Начало периода
        /// </summary>
        public DateTime PeriodBegin
        {
            get { return _periodBegin; }
            set { SetProperty(ref _periodBegin, value); }
        }



        private DateTime _periodEnd;

        /// <summary>
        /// Окончание периода
        /// </summary>
        public DateTime PeriodEnd
        {
            get { return _periodEnd; }
            set { SetProperty(ref _periodEnd, value); }
        }



        private DateTime _visiblePeriodBegin;
        /// <summary>
        /// Начало периода отображаемого на Timebar.
        /// </summary>
        public DateTime VisiblePeriodBegin
        {
            get { return _visiblePeriodBegin; }
            set { SetProperty(ref _visiblePeriodBegin, value); }
        }



        private DateTime _visiblePeriodEnd;
        /// <summary>
        /// Окончание периода отображаемого на Timebar.
        /// </summary>
        public DateTime VisiblePeriodEnd
        {
            get { return _visiblePeriodEnd; }
            set { SetProperty(ref _visiblePeriodEnd, value); }
        }
        
        



        private DateTime _trendPeriodBegin;
        /// <summary>
        /// Начало периода отображаемого на тренде (выбирается с помощью элемента timebar)
        /// </summary>
        public DateTime TrendPeriodBegin
        {
            get { return _trendPeriodBegin; }
            set
            {
                if (SetProperty(ref _trendPeriodBegin, value))
                {
                    TrendList.ForEach(t => t.PeriodBegin = value);
                    UpdateTrendView();
                }
            }
        }


        private DateTime _trendPeriodEnd;
        /// <summary>
        /// Окончание периода отображаемого на тренде (выбирается с помощью элемента timebar)
        /// </summary>
        public DateTime TrendPeriodEnd
        {
            get { return _trendPeriodEnd; }
            set
            {
                if (SetProperty(ref _trendPeriodEnd, value))
                {
                    TrendList.ForEach(t => t.PeriodEnd = value);
                    UpdateTrendView();
                }
            }
        }

        #endregion


        /// <summary>
        /// Список трендов
        /// </summary>
        public ObservableCollection<Trend> TrendList { get; private set; }

        private Trend _selectedTrend;
        public Trend SelectedTrend
        {
            get { return _selectedTrend; }
            set
            {
                if (SetProperty(ref _selectedTrend, value))
                {
                    DuplicateTrendCommand.RaiseCanExecuteChanged();
                    RemoveTrendCommand.RaiseCanExecuteChanged();
                }
            }
        }
        

        #region Add & Remove Trend

        public DelegateCommand AddTrendCommand { get; private set; }


        private SelectEntityPropertyViewModel _vm;
        private void AddTrend()
        {
            _vm = new SelectEntityPropertyViewModel(() =>
            {
                AddTrend(_vm.EntitySelector.Entity,
                         _vm.EntitySelector.PropertyType,
                         _vm.EntitySelector.PeriodType ?? PeriodType.Twohours);
            });
            var view = new SelectEntityProperty { DataContext = _vm };
            view.ShowDialog();
        }
        
        public void AddTrend(CommonEntityDTO entity, PropertyType propertyType, PeriodType periodType)
        {
            var propType = ClientCache.DictionaryRepository.PropertyTypes.Single(pt => pt.PropertyType == propertyType);
            if (!propType.PhysicalType.TrendAllowed)
            {
                MessageBox.Show("Для выбранного типа параметра построение тренда невозможно.");
                return;
            }

            var trend = new Trend(ReloadTrend, UpdateTrendView, entity, propertyType, periodType, GetNewColor(), 4,
                TrendPeriodBegin, TrendPeriodEnd);
            TrendList.Add(trend);
            UpdateTrendView();
        }
        


        /// <summary>
        /// Функция создания копии тренда
        /// </summary>
        private void DuplicateTrend()
        {
            if (SelectedTrend == null) return;

            var newTrend = new Trend(ReloadTrend, UpdateTrendView, SelectedTrend.Entity, SelectedTrend.PropertyType,
                SelectedTrend.PeriodType, GetNewColor(), 4, TrendPeriodBegin, TrendPeriodEnd);
            TrendList.Add(newTrend);
            UpdateTrendView();
        }



        /// <summary>
        /// Функция удаления тренда
        /// </summary>
        private void RemoveTrend()
        {
            if (SelectedTrend == null) return;

            TrendList.Remove(SelectedTrend);
            UpdateTrendView();
        }
        
        #endregion
        




        #region ReloadTrendData

        private async void ReloadTrend(Trend trend)
        {
            try
            {
                Behavior.TryLock();

                var values = (await new SeriesDataServiceProxy().GetPropertyValueListAsync(
                    new GetPropertyValueListParameterSet
                    {
                        EntityId = trend.Entity.Id,
                        PropertyTypeId = trend.PropertyType,
                        PeriodTypeId = trend.PeriodType,
                        StartDate = TimebarPeriod.Begin,
                        EndDate = TimebarPeriod.End
                    })).OfType<PropertyValueDoubleDTO>().ToList();

                // Это нужно для корректной работы графиков в TimeBar
                if(values.All(v => v.Date != TimebarPeriod.Begin))
                    values.Add(new PropertyValueDoubleDTO { Date = TimebarPeriod.Begin, Value = double.NaN });
                if (values.All(v => v.Date != TimebarPeriod.End))
                    values.Add(new PropertyValueDoubleDTO { Date = TimebarPeriod.End, Value = double.NaN });

                trend.SetData(values);
            }
            finally 
            {
                Behavior.TryUnlock();
            }
        }



        /// <summary>
        /// Обновить данные по всем трендам
        /// </summary>
        private void Refresh()
        {
            foreach (var trends in TrendList)
            {
                ReloadTrend(trends);
            }

            UpdateTrendView();
        }


        /// <summary>
        /// Минимальное значение по всем трендам (за весь период)
        /// </summary>
        public double? Min
        {
            get
            {
                if (!TrendList.Any(t => t.IsVisible && t.Min.HasValue)) return null;
                return TrendList.Where(t => t.IsVisible && t.Min.HasValue).Min(t => t.Min.Value);
            }
        }

        /// <summary>
        /// Максимальное значение по всем трендам (за весь период)
        /// </summary>
        public double? Max
        {
            get
            {
                if (!TrendList.Any(t => t.IsVisible && t.Max.HasValue)) return null;
                return TrendList.Where(t => t.IsVisible && t.Max.HasValue).Max(t => t.Max.Value);
            }
        }





        /// <summary>
        /// Временной шаг по оси X в сутка
        /// </summary>
        public int Step
        {
            get
            {
                var span = TrendPeriodEnd - TrendPeriodBegin;
                //return (int)Math.Round(span.TotalDays/20.0) + 1;
                if (span.TotalDays < 6)  return 6;
                if (span.TotalDays < 11) return 12;
                if (span.TotalDays < 22) return 24;
                if (span.TotalDays < 31) return 48;
                return 0;
            }
        }

        /// <summary>
        /// Максимальное значение по всем трендам (за интервал тренда)
        /// </summary>
        public double? TrendMax
        {
            get
            {
                if (!TrendList.Any(t => t.IsVisible && t.TrendMax.HasValue)) return null;
                var max = TrendList.Where(t => t.IsVisible && t.TrendMax.HasValue).Max(t => t.TrendMax.Value);
                return Math.Round(max > 0 ? 1.5*max : 0.5*max, 3);
            }
        }

        
        public void UpdateTrendView()
        {
            OnPropertyChanged(() => Min);
            OnPropertyChanged(() => Max);
            OnPropertyChanged(() => TrendMax);
            OnPropertyChanged(() => Step);
        }
        
        
        #endregion

        public DelegateCommand RefreshCommand { get; private set; }
        
        public DelegateCommand ExportExcelCommand { get; private set; }

        /// <summary>
        /// Команда удаления тренда
        /// </summary>
        public DelegateCommand RemoveTrendCommand { get; set; }


        /// <summary>
        /// Команда создания копии трнеда
        /// </summary>
        public DelegateCommand DuplicateTrendCommand { get; set; }


        // Возвращает новый не использующийся цвет
        private Color GetNewColor()
        {
            foreach (var color in _palette)
            {
                if (TrendList.All(t => t.Color != color))
                    return color;
            }
            return Colors.Orange;
        }

        /// <summary>
        /// Палитрочка
        /// </summary>
        private readonly List<Color> _palette = new List<Color>
        {
            Common.GoodStyles.Colors.NiceGreen,
            Common.GoodStyles.Colors.SoftOrange,
            Common.GoodStyles.Colors.Cyan,
            Common.GoodStyles.Colors.Green,
            Common.GoodStyles.Colors.Dark,
            Common.GoodStyles.Colors.Purple
        };


        private void ExportTrendsToExcel()
        {
            var dialog = new SaveFileDialog
            {
                DefaultExt = "xlsx",
                Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*",
                FilterIndex = 1
            };
            if (dialog.ShowDialog() == false) return;

            

            var excelReport = new ExcelReport();

            excelReport.Write("Дата:").Write(DateTime.Today).NewRow();
            excelReport.Write("Время:").Write(DateTime.Now.ToString("HH:mm")).NewRow();
            excelReport.Write("ФИО:").Write(UserProfile.Current.UserName).NewRow();

            excelReport.Write($"Тренды за период с {TrendPeriodBegin.ToShortDateString()} по {TrendPeriodEnd.ToShortDateString()}").NewRow();
            excelReport.NewRow();


            // Добавить заголовок столбца с меткой времени
            excelReport.WriteHeader("Метка времени", 120);

            // Добавить столбцы со значениями тренда
            foreach (var trend in TrendList.Where(t => t.IsVisible))
            {
                excelReport.WriteHeader($"{trend.Entity.ShortPath}\n{trend.PropertyTypeDto.Name}\n{trend.UnitName}", 120);
            }


            // Есть только 2 периода сутки и 2 часа. 
            // Если все тренды суточные, то шаг 1 день, если есть часовые, то 2 часа
            var span = new TimeSpan(1, 0, 0, 0);
            if (TrendList.Any(t => t.PeriodType == PeriodType.Twohours))
                span = new TimeSpan(2, 0, 0);


            excelReport.NewRow();

            for (var ts = TrendPeriodBegin; ts <= TrendPeriodEnd && ts <= DateTime.Now; ts += span)
            {
                excelReport.WriteCell(ts.ToString("dd.MM.yyyy HH:mm"));
                foreach (var trend in TrendList.Where(t => t.IsVisible))
                {
                    var val = trend.AllData.SingleOrDefault(v => v.Date == ts)?.Value;
                    excelReport.WriteCell(val != null ? val.ToString() : "");
                }
                excelReport.NewRow();
            }
                
            using (var stream = dialog.OpenFile())
            {
                excelReport.Save(stream);
            }
        }
        
    }
}