using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Application.Helpers;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using ViewModelBase = GazRouter.Common.ViewModel.ViewModelBase;


namespace GazRouter.Controls.Dialogs.PeriodPickerEx
{
    public class PeriodPickerDropViewModel : ViewModelBase
    {
        public Period Period { get; set; }

        public PeriodPickerDropViewModel()
        {
            Period = new Period();

            YearList = new List<SelectableItem>();
            for (var i = 0; i < 4; i++)
            {
                var item = new SelectableItem {Index = DateTime.Today.Year - i};
                YearList.Add(item);
                item.PropertyChanged += (sender, args) =>
                {
                    if (!((SelectableItem)sender).IsSelected) return;

                    // Если выбран быстрый период, надо бы выделение с него снять
                    QuickPeriodList.ForEach(p => p.IsSelected = false);
                    
                    PerformPeriod();
                    OnPropertyChanged(() => DisplayString);
                };
            }


            QuarterList = new List<SelectableItem>();
            for (var i = 1; i <= 4; i++)
            {
                var item = new SelectableItem {Index = i};
                QuarterList.Add(item);
                item.PropertyChanged += (sender, args) =>
                {
                    if (!((SelectableItem)sender).IsSelected) return;

                    // Если год не выбран, то надо бы его выбрать
                    if (YearList.All(p => !p.IsSelected))
                        YearList.First().IsSelected = true;

                    PerformPeriod();
                    OnPropertyChanged(() => DisplayString);
                };
            }


            MonthList = new List<SelectableItem>();
            for (var i = 1; i <= 12; i++)
            {
                var item = new SelectableItem {Index = i};
                MonthList.Add(item);
                item.PropertyChanged += (sender, args) =>
                {
                    if (!((SelectableItem)sender).IsSelected) return;
                    
                    // Если год не выбран, то надо бы его выбрать
                    if (YearList.All(p => !p.IsSelected))
                        YearList.First().IsSelected = true;
                    
                    PerformPeriod();
                    OnPropertyChanged(() => DisplayString);
                };
            }



            QuickPeriodList = new List<SelectableItem>();
            for (var i = 1; i <= 6; i++)
            {
                var item = new SelectableItem {Index = i};
                QuickPeriodList.Add(item);
                item.PropertyChanged += (sender, args) =>
                {
                    if (!((SelectableItem) sender).IsSelected) return;
                    
                    // Если выбран быстрый период, то снимаем выбор с элементов выбора года, квартала, месяца
                    YearList.ForEach(y => y.IsSelected = false);
                    QuarterList.ForEach(y => y.IsSelected = false);
                    MonthList.ForEach(y => y.IsSelected = false);

                    // Расчет периода
                    PerformPeriod();
                    OnPropertyChanged(() => DisplayString);
                };
            }

        }

        //public PeriodDates Period { get; set; }
        public List<SelectableItem> QuickPeriodList { get; set; }
        public List<SelectableItem> YearList { get; set; }
        public List<SelectableItem> QuarterList { get; set; }
        public List<SelectableItem> MonthList { get; set; }


        private void PerformPeriod()
        {
            // Быстрые периоды
            if (QuickPeriodList.Any(i => i.IsSelected))
            {
                var item = QuickPeriodList.Single(i => i.IsSelected);
                switch (item.Index)
                {
                    case 1: // Текущий год
                        Period = PeriodHelper.ThisYear;
                        break;

                    case 2: // Текущий квартал
                        Period = PeriodHelper.ThisQuarter;
                        break;
                    
                    case 3: // Текущий месяц
                        Period = PeriodHelper.ThisMonth;
                        break;

                    case 4: // Прошлый год
                        Period = PeriodHelper.PrevYear;
                        break;

                    case 5: // Прошлый квартал
                        Period = PeriodHelper.PrevQuarter;
                        break;

                    case 6: // Прошлый месяц
                        Period = PeriodHelper.PrevMonth;
                        break;
                }
            }

            // Период выбранные в нижней части
            if (YearList.Any(i => i.IsSelected))
            {
                var year = YearList.Single(i => i.IsSelected).Index;
                
                if (QuarterList.Any(i => i.IsSelected))
                {
                    var quarter = QuarterList.Single(i => i.IsSelected).Index;
                    Period = new Period(year, quarter, true);
                    return;
                }

                if (MonthList.Any(i => i.IsSelected))
                {
                    var month = MonthList.Single(i => i.IsSelected).Index;
                    Period = new Period(year, month, false);
                    return;
                }

                Period = new Period(year);

            }
        }

        private bool _isCustom;
        public bool IsCustom
        {
            get { return _isCustom; }
            set
            {
                _isCustom = value;
                if (value)
                {
                    Period.Type = PeriodType.None;
                    QuickPeriodList.ForEach(i => i.IsSelected = false);
                    YearList.ForEach(i => i.IsSelected = false);
                    MonthList.ForEach(i => i.IsSelected = false);
                    QuarterList.ForEach(i => i.IsSelected = false);

                    PeriodBegin = DateTime.Today.AddDays(-3);
                    PeriodEnd = DateTime.Today;
                }
                
            } 
        }

        /// <summary>
        /// Начало периода (для выбора произвольного периода)
        /// </summary>
        public DateTime PeriodBegin
        {
            get { return Period.Begin; }
            set
            {
                Period = new Period(value, PeriodEnd);
                OnPropertyChanged(() => PeriodBegin);
                OnPropertyChanged(() => DisplayString);
            }
        }

        /// <summary>
        /// Конец периода (для выбора произвольного периода)
        /// </summary>
        public DateTime PeriodEnd
        {
            get { return Period.End; }
            set
            {
                Period = new Period(PeriodBegin, value);
                OnPropertyChanged(() => PeriodEnd);
                OnPropertyChanged(() => DisplayString);
            }
        }


        public string DisplayString
        {
            get
            {
                return Period.DisplayString;
            }
        }
    }


    public class SelectableItem : ViewModelBase
    {
        public int Index { get; set; }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected, value); }
        }
    }


}