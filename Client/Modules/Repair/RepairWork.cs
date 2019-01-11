using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Common.ViewModel;
using GazRouter.DTO.Dictionaries.RepairWorksType;
using GazRouter.DTO.ObjectModel.Valves;
using Microsoft.Practices.Prism.Commands;


namespace GazRouter.Repair
{
    public class RepairWork : PropertyChangedBase
    {
        private bool _isSelected;
        private double? _kilometerEnd;
        private double? _kilometerStart;
        
        public RepairWork(List<KilometerItem> kilometerList, RepairWorkTypeDTO dto, bool isSelected, double? kmStart, double? kmEnd)
        {
            KilometerList = kilometerList;
            Dto = dto;
            _isSelected = isSelected;
            _kilometerStart = kmStart;
            _kilometerEnd = kmEnd;
        }

        
        public RepairWorkTypeDTO Dto { get; }
        
        public List<KilometerItem> KilometerList { get; set; }

        public List<KilometerItem> KilometerStartList => KilometerList.GetRange(0, KilometerList.Count - 1);

        public List<KilometerItem> KilometerEndList => KilometerList.GetRange(1, KilometerList.Count - 1);



        /// <summary>
        ///     Работа отмечена как активная
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (SetProperty(ref _isSelected, value))
                {
                    if (value && KilometerList?.Count > 0)
                    {
                        KilometerStart = KilometerList.Min(k => k.Km);
                        KilometerEnd = KilometerList.Max(k => k.Km);
                    }
                    else
                    {
                        KilometerStart = null;
                        KilometerEnd = null;
                    }
                    Changed();
                }
            }
        }

        /// <summary>
        ///     Километр начала участка проведения работ
        ///     (заполняется только для объектов типа Газопровод)
        /// </summary>
        public double? KilometerStart
        {
            get { return _kilometerStart; }
            set
            {
                if (_kilometerStart == value) return;

                _kilometerStart = value;
                
                if (KilometerList?.Count > 0)
                {

                    // проверяем, чтобы выбранный километр начала участка был не меньше километра начала газопровода
                    // если меньше, принудительно выставляем начало
                    // (это актуально только для типа работ Ремонтируется, т.к. так задается произвольный километр)
                    if (_kilometerStart < KilometerList.Min(k => k.Km))
                        _kilometerStart = KilometerList.Min(k => k.Km);
                    if (_kilometerStart >= KilometerList.Max(k => k.Km))
                        _kilometerStart = KilometerStartList.Max(k => k.Km);


                    // теперь проверяем, чтобы выбранный километр начала был не больше километра конца
                    if (_kilometerStart >= _kilometerEnd)
                    {
                        _kilometerEnd = _kilometerStart;

                        // для всех типов работ, кроме ремонтируется
                        // если километр начала больше, то просто выставляем километр конца 
                        // равным километру следующему за выбранным в списке километров
                        if (Dto.Id != WorkType.PipelineRepairing)
                        {
                            var i = KilometerList.IndexOf(KilometerList.First(k => k.Km == _kilometerStart));
                            _kilometerEnd = KilometerList[i + 1].Km;
                        }
                        OnPropertyChanged(() => KilometerEnd);
                    }
                }

                Changed();
                OnPropertyChanged(() => KilometerStart);
                OnPropertyChanged(() => Length);
            }
        }

        /// <summary>
        ///     Километр конца участка проведения работ
        ///     (заполняется только для объектов типа Газопровод)
        /// </summary>
        public double? KilometerEnd
        {
            get { return _kilometerEnd; }
            set
            {
                if (_kilometerEnd == value) return;

                _kilometerEnd = value;

                if (KilometerList?.Count > 0)
                {
                    // проверяем, чтобы выбранный километр был не больше километра конца газопровода
                    // если больше, принудительно выставляем конец
                    // (это актуально только для типа работ Ремонтируется, т.к. так задается произвольный километр)
                    if (_kilometerEnd <= KilometerList.Min(k => k.Km))
                        _kilometerEnd = KilometerEndList.Min(k => k.Km);
                    if (_kilometerEnd > KilometerList.Max(k => k.Km))
                        _kilometerEnd = KilometerList.Max(k => k.Km);

                    // теперь проверяем, чтобы выбранный километр был не меньше километра начала участка
                    if (KilometerEnd <= KilometerStart)
                    {
                        _kilometerStart = _kilometerEnd;

                        // для всех типов работ, кроме ремонтируется
                        // если километр конца меньше, то просто выставляем километр начала 
                        // равным километру предшествующему выбранному в списке километров
                        if (Dto.Id != WorkType.PipelineRepairing)
                        {
                            var i = KilometerList.IndexOf(KilometerList.First(k => k.Km == _kilometerEnd));
                            _kilometerStart = KilometerList[i - 1].Km;
                        }
                        OnPropertyChanged(() => KilometerStart);
                    }
                    
                    Changed();
                    OnPropertyChanged(() => KilometerEnd);
                    OnPropertyChanged(() => Length);
                }
            }
        }

        public double? Length => _kilometerEnd - _kilometerStart;

        public DelegateCommand KmCloneCommand  => new DelegateCommand(() => Clone(this));


        public Action Changed { get; set; }

        public Action<RepairWork> Clone { get; set; }
    }


    /// <summary>
    /// Чтобы можно было вводить произвольные км. 
    /// для работы типа ремонтируется по газопроводу
    /// </summary>
    public class RepairWorkCustomKilometer : RepairWork
    {
        public RepairWorkCustomKilometer(List<KilometerItem> kilometerList, RepairWorkTypeDTO dto, bool isSelected, double? kmStart,
            double? kmEnd) : base(kilometerList, dto, isSelected, kmStart, kmEnd)
        {
            
        }
    }




    public class RepairWorkList : List<RepairWork>
    {
        public bool HasBleedWork
        {
            get
            {
                var bleedWorkList = new List<int> { WorkType.DistrStationOvergazing, 212, 214, WorkType.PipelineOvergazing };
                return this.Any(w => bleedWorkList.Contains(w.Dto.Id) && w.IsSelected);
            }
        }
        
        public void AddRepairWork(List<KilometerItem> kilometerList, RepairWorkTypeDTO dto, bool isSelected, double? kmStart, double? kmEnd)
        {

            var work = dto.Id == WorkType.PipelineRepairing
                ? new RepairWorkCustomKilometer(kilometerList, dto, isSelected, kmStart, kmEnd)
                : new RepairWork(kilometerList, dto, isSelected, kmStart, kmEnd);

            work.Changed = () => WorksChanged();
            work.Clone = Clone;

            Add(work);
        }


        /// <summary>
        /// Вызывается когда происходят изменения в списке работ
        /// </summary>
        public Action WorksChanged { get; set; }


        // Копирует км. начала и конца выбранной работы по всем другим активным работам
        // Существенно ускоряет ввод данных по работам
        // На форме это правая колонка в таблице работ - кнопка >
        public void Clone(RepairWork work)
        {
            foreach (var w in this.Where(w => w.IsSelected))
            {
                w.KilometerStart = work.KilometerStart;
                w.KilometerEnd = work.KilometerEnd;
            }
        }
    }





    /// <summary>
    /// Классы для отображения километров в отпадающих списках
    /// в таблицы работ
    /// </summary>
    public class KilometerItem
    {
        public virtual double Km { get; }
    }

    public class PipeEdgeKilometer : KilometerItem
    {
        public PipeEdgeKilometer(double km)
        {
            Km = km;
        }

        public override double Km { get; }
    }

    public class ValveKilometer : KilometerItem
    {
        public ValveKilometer(ValveDTO valve)
        {
            Valve = valve;
        }
        public ValveDTO Valve { get; set; }

        public override double Km => Valve.Kilometer;
    }
    
}