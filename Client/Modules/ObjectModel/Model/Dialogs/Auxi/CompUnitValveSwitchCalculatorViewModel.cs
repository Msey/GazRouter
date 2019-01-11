using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Application;
using GazRouter.Common.ViewModel;
using GazRouter.DTO.Dictionaries.ValveTypes;
using Microsoft.Practices.Prism.Commands;


namespace GazRouter.ObjectModel.Model.Dialogs.Auxi
{
    public class CompUnitValveSwitchCalculatorViewModel : DialogViewModel
    {
        public CompUnitValveSwitchCalculatorViewModel(Action<double, double, List<ValveSwitches>> callback, List<ValveSwitches> list)
            : base(null)
        {
            ConfirmCommand = new DelegateCommand(() =>
            {
                if (callback != null)
                    callback(QStart, QStop, ValveList.Select(v => v.Vs).ToList());
                DialogResult = true;
            });


            PopulateValveList(list ?? new List<ValveSwitches>());
        }


        public List<UnitValveWrap> ValveList { get; set; }

        public List<ValveTypeDTO> ValveTypeList
        {
            get { return ClientCache.DictionaryRepository.ValveTypes; }
        }

        /// <summary>
        /// Суммарный расход газа при пуске
        /// </summary>
        public double QStart
        {
            get { return ValveList.Sum(v => v.QStart); }
        }

        /// <summary>
        /// Суммарный расход газа при останове
        /// </summary>
        public double QStop
        {
            get { return ValveList.Sum(v => v.QStop); }
        }


        public DelegateCommand ConfirmCommand { get; set; }


        private void PopulateValveList(List<ValveSwitches> vsList)
        {
            ValveList = new List<UnitValveWrap>
            {
                //Трубопроводы технологического газа
                new UnitValveWrap(vsList.SingleOrDefault(s => s.ValveId == 1) ?? new ValveSwitches(1))
                {
                    Num = "1",
                    Name = "Входной",
                    Description = "Входной газопровод ГПА",
                    GroupName = "Трубопроводы технологического газа"
                },

                new UnitValveWrap(vsList.SingleOrDefault(s => s.ValveId == 2) ?? new ValveSwitches(2))
                {
                    Num = "2",
                    Name = "Нагнетательный",
                    Description = "Выходной газопровод ГПА",
                    GroupName = "Трубопроводы технологического газа"
                },

                new UnitValveWrap(vsList.SingleOrDefault(s => s.ValveId == 3) ?? new ValveSwitches(3))
                {
                    Num = "3, 3б (бис)",
                    Name = "Обводной",
                    Description = "Трубопровод между входным и выходным газопроводами ГПА",
                    GroupName = "Трубопроводы технологического газа"
                },

                new UnitValveWrap(vsList.SingleOrDefault(s => s.ValveId == 4) ?? new ValveSwitches(4))
                {
                    Num = "4",
                    Name = "Наполнительный",
                    Description = "Обводной газопровод крана 1",
                    GroupName = "Трубопроводы технологического газа"
                },

                new UnitValveWrap(vsList.SingleOrDefault(s => s.ValveId == 5) ?? new ValveSwitches(5))
                {
                    Num = "5",
                    Name = "Выпускной",
                    Description = "Выпускной газопровод (свеча) ГПА",
                    GroupName = "Трубопроводы технологического газа"
                },

                new UnitValveWrap(vsList.SingleOrDefault(s => s.ValveId == 6) ?? new ValveSwitches(6))
                {
                    Num = "6, 6р",
                    Name = "Рециркуляционный",
                    Description = "Обводная линия группы или агрегата",
                    GroupName = "Трубопроводы технологического газа"
                },


                // Трубопроводы пускового газа
                new UnitValveWrap(vsList.SingleOrDefault(s => s.ValveId == 11) ?? new ValveSwitches(11))
                {
                    Num = "11",
                    Name = "Отсечной",
                    Description = "Входной газопровод пускового газа ГПА",
                    GroupName = "Трубопроводы пускового газа"
                },

                new UnitValveWrap(vsList.SingleOrDefault(s => s.ValveId == 10) ?? new ValveSwitches(10))
                {
                    Num = "10",
                    Name = "Выпускной (свеча)",
                    Description = "Выпускной газопровод (свеча) пускового газа ГПА",
                    GroupName = "Трубопроводы пускового газа"
                },

                new UnitValveWrap(vsList.SingleOrDefault(s => s.ValveId == 13) ?? new ValveSwitches(13))
                {
                    Num = "13",
                    Name = "Регулирующий",
                    Description = "Входной газопровод непосредственно перед пусковым устройством",
                    GroupName = "Трубопроводы пускового газа"
                },


                // Трубопроводы топливного газа
                new UnitValveWrap(vsList.SingleOrDefault(s => s.ValveId == 12) ?? new ValveSwitches(12))
                {
                    Num = "12",
                    Name = "Отсечной",
                    Description = "Входной топливный газопровод ГПА",
                    GroupName = "Трубопроводы топливного газа"
                },

                new UnitValveWrap(vsList.SingleOrDefault(s => s.ValveId == 9) ?? new ValveSwitches(9))
                {
                    Num = "9",
                    Name = "Выпускной (свеча)",
                    Description = "Выпускной топливный газопровод (свеча)",
                    GroupName = "Трубопроводы топливного газа"
                },

                new UnitValveWrap(vsList.SingleOrDefault(s => s.ValveId == 14) ?? new ValveSwitches(14))
                {
                    Num = "14",
                    Name = "Дежурный",
                    Description = "Входной газопровод дежурной горелки камеры сгорания ГПУ",
                    GroupName = "Трубопроводы топливного газа"
                },
            };

            ValveList.ForEach(
                v => v.PropertyChanged += (obj, e) =>
                {
                    if (e.PropertyName == "QStart") 
                        OnPropertyChanged(() => QStart);
                    if (e.PropertyName == "QStop")
                        OnPropertyChanged(() => QStop);
                });

            OnPropertyChanged(() => ValveList);

        }


    }


    public class ValveSwitches
    {
        public ValveSwitches()
        {
        }

        public ValveSwitches(int id)
        {
            ValveId = id;
        }

        public int ValveId { get; set; }
        
        public int ValveTypeId { get; set; }

        public int StartSwitchCnt { get; set; }

        public int StopSwitchCnt { get; set; }
    }


    public class UnitValveWrap : ViewModelBase
    {
        private readonly ValveSwitches _vs;

        public UnitValveWrap(ValveSwitches vs)
        {
            _vs = vs;
        }

        public ValveSwitches Vs 
        {
            get { return _vs; }
        }

        public string Num { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string GroupName { get; set; }

        /// <summary>
        /// Тип крана
        /// </summary>
        public ValveTypeDTO ValveType
        {
            get
            {
                return
                    ClientCache.DictionaryRepository.ValveTypes.SingleOrDefault(
                        vt => vt.Id == _vs.ValveTypeId);
            }
            set
            {
                _vs.ValveTypeId = value.Id;
                OnPropertyChanged(() => ValveType);
                OnPropertyChanged(() => QStart);
                OnPropertyChanged(() => QStop);
            }
        }

        
        /// <summary>
        /// Кол-во перестановок крана при пуске
        /// </summary>
        public int StartSwitchCount
        {
            get { return _vs.StartSwitchCnt; }
            set
            {
                _vs.StartSwitchCnt = value;
                OnPropertyChanged(() => StartSwitchCount);
                OnPropertyChanged(() => QStart);
            }
        }

        /// <summary>
        /// Кол-во перестановок крана при останове
        /// </summary>
        public int StopSwitchCount
        {
            get { return _vs.StopSwitchCnt; }
            set
            {
                _vs.StopSwitchCnt = value;
                OnPropertyChanged(() => StopSwitchCount);
                OnPropertyChanged(() => QStop);
            }
        }

        /// <summary>
        /// Расход газа на переключения при старте
        /// </summary>
        public double QStart
        {
            get { return ValveType != null ? ValveType.RatedConsumption*StartSwitchCount : 0; }
        }

        /// <summary>
        /// Расход газа на переключения при останове
        /// </summary>
        public double QStop
        {
            get { return ValveType != null ? ValveType.RatedConsumption * StopSwitchCount : 0; }
        }

    }
}