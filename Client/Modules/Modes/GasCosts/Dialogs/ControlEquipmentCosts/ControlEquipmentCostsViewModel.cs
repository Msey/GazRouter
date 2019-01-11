using System;
using System.Collections.Generic;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.GasCosts;
using GazRouter.Modes.GasCosts.DefaultDataDialog;
using GazRouter.Modes.GasCosts.Dialogs.ViewModel;

namespace GazRouter.Modes.GasCosts.Dialogs.ControlEquipmentCosts
{
    public class ControlEquipmentCostsViewModel : CalcViewModelBase<ControlEquipmentCostsModel>
    {
        #region Constructors and Destructors

        public ControlEquipmentCostsViewModel(GasCostDTO gasCost, Action<GasCostDTO> callback, List<DefaultParamValues> defaultParamValues, bool ShowDayly)
            : base(gasCost, callback, defaultParamValues)
        {
            this.ShowDayly = ShowDayly;
            if (!IsEdit)
            {
                // Если вводится фактическое значение и выбран текущий месяц, 
                // то устанавливать дату в текущий день
                if (TargetId == Target.Fact && IsCurrentMonth)
                    EventDate = EventDate.AddDays(DateTime.Now.Day - EventDate.Day);
            }
            PerformCalculate();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Кол-во приборов данного типа в работе
        /// </summary>
        public int Count
        {
            get
            {
                return Model.Count;
            }
            set
            {
                Model.Count = value;
                OnPropertyChanged(() => Count);
                PerformCalculate();
            }
        }

        /// <summary>
        ///     Расход газа прибором данного типа, м³/ч, в соответствии с паспортными данными
        /// </summary>
        public double Q
        {
            get
            {
                return Model.Q;
            }
            set
            {
                Model.Q = value;
                OnPropertyChanged(() => Q);
                PerformCalculate();
            }
        }

        /// <summary>
        /// Время работы прибора
        /// </summary>
        public int Time
        {
            get
            {
                return Model.Time;
            }
            set
            {
                Model.Time = value;
                OnPropertyChanged(() => Time);
                PerformCalculate();
            }
        }

        /// <summary>
        /// Время работы прибора
        /// </summary>
        public string Type
        {
            get
            {
                return Model.Type;
            }
            set
            {
                Model.Type = value;
                OnPropertyChanged(() => Type);
                PerformCalculate();
            }
        }

        /// <summary>
        /// Т.к. расход газа по данной статье не нормируется и не планируется, 
        /// то нужно разрешить ввод только фактических данных
        /// </summary>
        public bool IsInputAllowed
        {
            get { return TargetId == Target.Fact; }
        }

        #endregion

        #region Methods

        protected override void SetValidationRules()
        {
            AddValidationFor(() => Type)
                .When(() => string.IsNullOrEmpty(Type))
                .Show("Введите тип(марку) прибора");


            AddValidationFor(() => Count)
                .When(() => Count <= 0)
                .Show("Недопустимое значение. Должно быть больше 0.");

            AddValidationFor(() => Time)
                .When(() => Time <= 0 || Time > RangeInHours)
                .Show(string.Format("Недопустимое значение (допустимый диапазон 1 - {0})", RangeInHours));
            
            AddValidationFor(() => Q)
                .When(() => Q <= 0 || Q > 100)
                .Show("Недопустимое значение (допустимый диапазон 1 - 100)");
        }

        #endregion
    }
}