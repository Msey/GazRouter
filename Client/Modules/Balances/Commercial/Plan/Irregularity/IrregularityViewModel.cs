using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GazRouter.Balances.Commercial.Common;
using GazRouter.Balances.Commercial.Plan.Dialogs;
using GazRouter.Common.ViewModel;
using Microsoft.Practices.Prism.Commands;

namespace GazRouter.Balances.Commercial.Plan.Irregularity
{
    public class IrregularityViewModel : ViewModelBase
    {
        private readonly int _month;
        private readonly int _year;
        private readonly PlanOwnerItem _item;
        private bool _isIrregular;

        public IrregularityViewModel()
        {
            IsIrregularityAllowed = false;
            DayVolumeList = new List<DayVolume>();

            InitCommands();
        }

        public IrregularityViewModel(int year, int month, ItemBase item)
        {
            DayVolumeList = new List<DayVolume>();
            _year = year;
            _month = month;

            _item = item as PlanOwnerItem;
            // Неравномерность можно вводить только для объектов типа владельцев газа, 
            // для которых разрешен ввод плана
            // При этом значение базового объема, должно быть введено
            IsIrregularityAllowed = _item?.PlanSummarized != null;
            if (IsIrregularityAllowed)
            {
                IsIrregular = _item?.HasIrregularity ?? false;
            }

            InitCommands();
        }

        /// <summary>
        ///     Разрешен ли ввод неравномерности для выбранной строки плана
        /// </summary>
        public bool IsIrregularityAllowed { get; }

        public bool IsIrregular
        {
            get { return _isIrregular; }
            set
            {
                if (SetProperty(ref _isIrregular, value))
                {
                    if (value)
                    {
                        Init();
                    }
                    else
                    {
                        _item.PeriodVolumeList = null;
                    }
                }
            }
        }

        public List<DayVolume> DayVolumeList { get; set; }

        public ObservableCollection<DayVolume> SelectedItemList { get; } = new ObservableCollection<DayVolume>();

        /// <summary>
        ///     Остаток, нераспределенный по дням объем
        /// </summary>
        public double? RestVolume
            => IsIrregularityAllowed ? _item.PlanSummarized - DayVolumeList.Sum(v => v.Volume) : null;

        public bool IsDivided => RestVolume == 0; //-V3024

        public List<PeriodVolume> PeriodVolumeList => _item?.PeriodVolumeList;

        /// <summary>
        ///     Команда добавления суммарного объема за выбранные дни,
        ///     Введенный объем равномерно распределяется по выбранным дням
        /// </summary>
        public DelegateCommand SetTotalVolumeCommand { get; set; }

        /// <summary>
        ///     Команда ввода заданного объема по всем выбранным дням,
        /// </summary>
        public DelegateCommand SetVolumeCommand { get; set; }

        /// <summary>
        ///     Команда Очистить
        /// </summary>
        public DelegateCommand ClearVolumeCommand { get; set; }

        public void Init()
        {
            DayVolumeList = new List<DayVolume>();

            var dayCount = DateTime.DaysInMonth(_year, _month);
            var monthVolume = _item?.PlanSummarized ?? 0;
            for (var day = 1; day <= dayCount; day++)
            {
                var val =
                    _item?.PeriodVolumeList?.FirstOrDefault(v => v.StartDayNum <= day && day <= v.EndDayNum)?.Volume ??
                    0;

                var dv = new DayVolume
                {
                    DayNum = day,
                    Volume = val,
                    MonthVolume = monthVolume,
                    AvgVolume = Math.Round(monthVolume/dayCount)
                };

                dv.PropertyChanged += (obj, args) => { OnDayVolumeChanged(); };
                DayVolumeList.Add(dv);
            }

            OnPropertyChanged(() => DayVolumeList);

            OnDayVolumeChanged();
        }

        public void MakePeriods()
        {
            if (_item == null)
            {
                return;
            }

            var pl = new List<PeriodVolume>();
            PeriodVolume prevPeriod = null;

            foreach (var dayVal in DayVolumeList)
            {
                if (prevPeriod == null || prevPeriod.Volume != dayVal.Volume)
                {
                    prevPeriod = new PeriodVolume
                    {
                        StartDayNum = dayVal.DayNum,
                        EndDayNum = dayVal.DayNum,
                        Volume = dayVal.Volume
                    };
                    pl.Add(prevPeriod);
                }

                if (prevPeriod.Volume == dayVal.Volume)
                {
                    prevPeriod.EndDayNum = dayVal.DayNum;
                }
            }
            _item.PeriodVolumeList = pl;

            OnPropertyChanged(() => PeriodVolumeList);
        }

        private void ClearVolume()
        {
            if (!SelectedItemList.Any())
            {
                return;
            }

            foreach (var i in SelectedItemList)
            {
                i.Volume = 0;
            }
        }

        private void OnDayVolumeChanged()
        {
            OnPropertyChanged(() => RestVolume);
            OnPropertyChanged(() => IsDivided);
            MakePeriods();
        }

        private void InitCommands()
        {
            SetTotalVolumeCommand = new DelegateCommand(SetTotalVolume);
            SetVolumeCommand = new DelegateCommand(SetVolume);
            ClearVolumeCommand = new DelegateCommand(ClearVolume);
        }

        private void SetVolume()
        {
            if (!SelectedItemList.Any())
            {
                return;
            }

            var vm = new SetVolumeViewModel(
                "Объем за сутки",
                vol =>
                {
                    foreach (var i in SelectedItemList)
                    {
                        i.Volume = vol;
                    }
                });

            var v = new SetVolumeView {DataContext = vm};
            v.ShowDialog();
        }

        private void SetTotalVolume()
        {
            if (!SelectedItemList.Any())
            {
                return;
            }

            var vm = new SetVolumeViewModel(
                "Суммарный объем",
                total =>
                {
                    var avg = Math.Round(total/SelectedItemList.Count, 3);

                    foreach (var i in SelectedItemList)
                    {
                        if (i != SelectedItemList.Last())
                        {
                            i.Volume = avg;
                        }
                        else
                        {
                            i.Volume = total - (SelectedItemList.Count - 1)*avg;
                        }
                    }
                });

            var v = new SetVolumeView {DataContext = vm};
            v.ShowDialog();
        }

        private string _unitsName;
        public string UnitsName
        {
            get { return _unitsName; }
            set
            {
                _unitsName = value;
                OnPropertyChanged(UnitsName);
            }
        }
    }
}