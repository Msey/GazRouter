using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.GasCosts;
using GazRouter.Modes.GasCosts.DefaultDataDialog;
using GazRouter.Modes.GasCosts.Dialogs.Model;
using GazRouter.Modes.GasCosts.Dialogs.ValveControlCosts;
using GazRouter.Modes.GasCosts.Dialogs.ViewModel;
using Microsoft.Practices.Prism.Commands;
namespace GazRouter.Modes.GasCosts.Dialogs.UnitStartCosts
{
    public class UnitStartCostsViewModel : CalcViewModelBase<UnitStartCostsModel>
    {
        #region Constructors and Destructors
        public UnitStartCostsViewModel(GasCostDTO gasCost, Action<GasCostDTO> callback, List<DefaultParamValues> defaultParamValues, bool ShowDayly)
            : base(gasCost, callback, defaultParamValues)
        {
            this.ShowDayly = ShowDayly;
            LoadUnitInfo();
            if (!IsEdit)
            {
                // Если вводится фактическое значение и выбран текущий месяц, 
                // то устанавливать дату в текущий день
                if (TargetId == Target.Fact && IsCurrentMonth)
                    EventDate = EventDate.AddDays(DateTime.Now.Day - EventDate.Day);
                NormalShifting = true;
            }
            LoadValveTypeList();
            SetValidationRules();
        }
        #endregion
        #region Public Properties
        /// <summary>
        /// Для ГПА, у которых время холодной прокрутки алгоритмом пуска не оговаривается, 
        /// расход пускового газа для холодной прокрутки рассчитывают по формуле Q = Qтд * t,
        /// где Qтд - расход газа на работу пускового турбодетандера, t - время холодной прокрутки.
        /// </summary>
        public bool DryMotoringConsumptionIsNull
        {
            get
            {
                return Model.Unit.DryMotoringConsumption == 0 && Model.Unit.TurbineStarterConsumption != 0;
            }
        }
        ///// <summary>
        ///// Время холодной прокрутки, с
        ///// </summary>
        //public int DryMotoringPeriod
        //{
        //    get
        //    {
        //        return Model.DryMotoringPeriod;
        //    }
        //    set
        //    {
        //        Model.DryMotoringPeriod = value;
        //        OnPropertyChanged(() => DryMotoringPeriod);
        //        PerformCalculate();
        //    }
        //}
        /// <summary>
        /// Контур нагнетателя заполнен газом?
        /// </summary>
        public bool ProfileIsNotEmpty
        {
            get
            {
                return Model.ProfileIsNotEmpty;
            }
            set
            {
                Model.ProfileIsNotEmpty = value;
                OnPropertyChanged(() => ProfileIsNotEmpty);
                PerformCalculate();
            }
        }
        /// <summary>
        /// Кол-во пусков ГПА
        /// </summary>
        public int StartCount
        {
            get
            {
                return Model.StartCount;
            }
            set
            {
                Model.StartCount = value;
                OnPropertyChanged(() => StartCount);
                PerformCalculate();
            }
        }
        /// <summary> Переключения выполняются в соотв. с алгоритмом пуска </summary>
        public bool NormalShifting
        {
            get { return Model.NormalShifting; }
            set
            {
                Model.NormalShifting = value;
                OnPropertyChanged(() => NormalShifting);
                PerformCalculate();
            }
        }
        public double ValveConsumption
        {
            get
            {
                return Model.ValveShiftings.Sum(v => v.Q);
            }
        }
        public List<ValveShiftingViewModel> ValveShiftings { get; private set; }
        public string UnitType
        {
            get
            {
                return Model.Unit.TypeId != 0 ? ClientCache.DictionaryRepository.CompUnitTypes.First(cut => cut.Id == Model.Unit.TypeId).Name : "";
            }
        }
        public string SuperchargerType
        {
            get
            {
                return Model.Unit.SuperchargerTypeId != 0 ? ClientCache.DictionaryRepository.SuperchargerTypes.First(st => st.Id == Model.Unit.SuperchargerTypeId).Name : "";
            }
        }
#endregion

#region methods
        protected override void SetValidationRules()
        {
            AddValidationFor(() => StartCount)
                .When(() => StartCount <= 0)
                .Show("Недопустимое значение. Должно быть больше 0.");
            
            //AddValidationFor(() => DryMotoringPeriod)
            //    .When(() => DryMotoringPeriod < 0)
            //    .Show("Недопустимое значение. Не может быть меньше 0.");

        }
        
        private async void LoadUnitInfo()
        {
            try
            {
                Behavior.TryLock();

                var unit = await new ObjectModelServiceProxy().GetCompUnitByIdAsync(Entity.Id);
                Model.Unit = new CompUnit(unit);
                OnPropertyChanged(() => UnitType);
                OnPropertyChanged(() => SuperchargerType);
                OnPropertyChanged(() => DryMotoringConsumptionIsNull);
                PerformCalculate();
            }
            finally 
            {
                Behavior.TryUnlock();
            }
            
        }
        /// <summary>
        /// Получение списка типов запорной арматуры
        /// </summary>
        private void LoadValveTypeList()
        {
            ValveShiftings = ClientCache.DictionaryRepository.ValveTypes
                    .Select(v => new ValveShifting { Id = v.Id, Name = v.Name, RatedConsumption = v.RatedConsumption, Count = 0 })
                    .Select(vs => new ValveShiftingViewModel(vs)).ToList();
            ValveShiftings.ForEach(
                v =>
                {
                    var vm =
                    Model.ValveShiftings.SingleOrDefault(c => Equals(c, v.Model));
                    if (vm != null) v.Model.Count = vm.Count;
                    v.PropertyChanged += ValveShiftingViewModelOnPropertyChanged;
                });
        }
        private void ValveShiftingViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "Count")
                return;
            var vm = (ValveShiftingViewModel)(sender);
            if (Model.ValveShiftings.Contains(vm.Model))
            {
                Model.ValveShiftings.Remove(vm.Model);
            }
            if (vm.Count != 0)
                Model.ValveShiftings.Add(vm.Model);
            OnPropertyChanged(() => ValveConsumption);
            PerformCalculate();
        }
#endregion
    }
}