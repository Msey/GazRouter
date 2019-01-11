using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.GasCosts;
using GazRouter.Application.Helpers;
using GazRouter.DTO.ObjectModel.MeasPoint;
using GazRouter.Modes.GasCosts.DefaultDataDialog;
using GazRouter.Modes.GasCosts.Dialogs.ViewModel;
using Utils.Units;

namespace GazRouter.Modes.GasCosts.Dialogs.ChemicalAnalysisCosts
{
    public class ChemicalAnalysisCostsViewModel : CalcViewModelBase<ChemicalAnalysisCostsModel>
    {

        private MeasPointDTO _measPoint;

        #region Constructors and Destructors

        public ChemicalAnalysisCostsViewModel(GasCostDTO gasCost, Action<GasCostDTO> callback, List<DefaultParamValues> defaultParamValues, bool ShowDayly)
            : base(gasCost, callback, defaultParamValues)
        {
            this.ShowDayly = ShowDayly;
            if (!IsEdit)
            {
                // Если вводится фактическое значение и выбран текущий месяц, 
                // то устанавливать дату в текущий день
                if (TargetId == Target.Fact && IsCurrentMonth)
                    EventDate = EventDate.AddDays(DateTime.Now.Day - EventDate.Day);

                var defaultValues = DefaultParamValues.Single(d => d.Target == TargetId);
                PressureAir = defaultValues.PressureAir;
                CarbonDioxideContent = defaultValues.CarbonDioxideContent;
                NitrogenContent = defaultValues.NitrogenContent;
                Mode = 1;
            }

            LoadMeasPointData();
            SetValidationRules();
            PerformCalculate();
        }

        public async void LoadMeasPointData()
        {
            _measPoint = await new ObjectModelServiceProxy().GetMeasPointByParentAsync(Entity.Id);
            if (_measPoint != null)
            {
                Q = _measPoint.ChromatographConsumptionRate;
                Time = _measPoint.ChromatographTestTime;
            }
            
            OnPropertyChanged(() => IsInvalidData);
        }

        /// <summary>
        /// Выбранный объект, является газопроводом
        /// </summary>
        public bool IsPipeline
        {
            get { return Entity.EntityType == EntityType.Pipeline; }
        }
        
        /// <summary>
        /// Объект "точка измерения параметров газа" не найден 
        /// или введены некорректные данные по расходу хроматографа или времени испытаний
        /// </summary>
        public bool IsInvalidData => !IsPipeline &&
                                     (_measPoint == null || _measPoint.ChromatographConsumptionRate == 0 ||
                                      _measPoint.ChromatographTestTime == 0);

        #endregion

        #region Public Properties

        public int MeasCount
        {
            get
            {
                return Model.MeasCount;
            }
            set
            {
                Model.MeasCount = value;
                OnPropertyChanged(() => MeasCount);
                PerformCalculate();
            }
        }

        /// <summary>
        ///     Расход анализируемого газа в хроматографе, м³/ч, в соответствии с паспортными данными
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

        public double Time
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

        /// <summary> Список способов отбора проб </summary>
        public Dictionary<string, int> ModeList
        {
            get
            {
                return new Dictionary<string, int>
                {
                    {"Проведение разового анализа", 1},
                    {"Работа приборов на потоке", 2}
                };
            }
        }
        /// <summary> Способ отбора проб </summary>
        public int Mode
        {
            get { return Model.Mode; }
            set
            {
                Square = 0;
                OpeningDegree = 0;
                PurgeTime = 0;
                Model.Mode = value;
                OnPropertyChanged(() => Mode);
                PerformCalculate();
                IsModeOne = Mode == 1;
            }
        }


        /// <summary>
        /// Выбран первый способ отбора пробы
        /// </summary>
        public bool IsModeOne
        {
            get { return Model.IsModeOne; }
            set
            {
                Model.IsModeOne = value;
                OnPropertyChanged(() => IsModeOne);
                ClearValidations();
                SetValidationRules();
                PerformCalculate();
            }
        }

        /// <summary>
        /// Давление газа, кг/см2
        /// </summary>
        public Pressure P
        {
            get { return Model.P; }
            set
            {
                Model.P = value;
                OnPropertyChanged(() => P);
                PerformCalculate();
            }
        }
        /// <summary>
        /// Температура газа, Гр.С
        /// </summary>
        public Temperature T
        {
            get { return Model.T; }
            set
            {
                Model.T = value;
                OnPropertyChanged(() => T);
                PerformCalculate();
            }
        }

        /// <summary>
        ///     Продолжительность продувки, c
        /// </summary>
        public double PurgeTime
        {
            get
            {
                return Model.PurgeTime;
            }
            set
            {
                Model.PurgeTime = value;
                OnPropertyChanged(() => PurgeTime);
                PerformCalculate();
            }
        }

        /// <summary>
        ///     Количество анализов
        /// </summary>
        public int TestCount
        {
            get
            {
                return Model.TestCount;
            }
            set
            {
                Model.TestCount = value;
                OnPropertyChanged(() => TestCount);
                PerformCalculate();
            }
        }

        /// <summary>
        ///     Геометрический объем пробоотборника, м³
        /// </summary>
        public double Volume
        {
            get
            {
                return Model.Volume;
            }
            set
            {
                Model.Volume = value;
                OnPropertyChanged(() => Volume);
                PerformCalculate();
            }
        }

        /// <summary>
        ///     Площадь продувочного сечения вентиля
        /// </summary>
        public double Square
        {
            get
            {
                return Model.Square;
            }
            set
            {
                Model.Square = value;
                OnPropertyChanged(() => Square);
                PerformCalculate();
            }
        }

        /// <summary>
        ///     Расход газа на прибор, м³/ч
        /// </summary>
        public double Qdev
        {
            get
            {
                return Model.Qdev;
            }
            set
            {
                Model.Qdev = value;
                OnPropertyChanged(() => Qdev);
                PerformCalculate();
            }
        }

        /// <summary>
        ///     Степень открытия вентиля ВИ-160
        /// </summary>
        public double OpeningDegree
        {
            get
            {
                return Model.OpeningDegree;
            }
            set
            {
                Model.OpeningDegree = value;
                OnPropertyChanged(() => OpeningDegree);
                PerformCalculate();
                if (OpeningDegree <= 1.00 && OpeningDegree > 0.0)
                    Square = Math.Round(0.6767*OpeningDegree + 0.0084, 2)*Math.Pow(10.0, -5);
            }
        }

        /// <summary>
        ///   Содержание азота, мол.доля %
        /// </summary>
        public double NitrogenContent
        {
            get
            {
                return Model.NitrogenContent;
            }
            set
            {
                Model.NitrogenContent = value;
                OnPropertyChanged(() => NitrogenContent);
                PerformCalculate();
            }
        }

        /// <summary>
        /// Содержание двуокиси углерода, мол.доля %
        /// </summary>
        public double CarbonDioxideContent
        {
            get
            {
                return Model.CarbonDioxideContent;
            }
            set
            {
                Model.CarbonDioxideContent = value;
                OnPropertyChanged(() => CarbonDioxideContent);
                PerformCalculate();
            }
        }

        /// <summary>
        /// Давление атмосферное, мм р.ст.
        /// </summary>
        public double PressureAir
        {
            get { return Model.PressureAir.MmHg; }
            set
            {
                Model.PressureAir = Pressure.FromMmHg(value);
                OnPropertyChanged(() => PressureAir);
                PerformCalculate();
            }
        }

        /// <summary>
        /// Температура воздуха, Гр.С
        /// </summary>
        public Temperature TemperatureAir
        {
            get { return Model.TemperatureAir; }
            set
            {
                Model.TemperatureAir = value;
                OnPropertyChanged(() => TemperatureAir);
                PerformCalculate();
            }
        }
        #endregion

        #region Methods

        protected override void SetValidationRules()
        {
            AddValidationFor(() => MeasCount)
                .When(() => MeasCount <= 0)
                .Show("Недопустимое значение. Должно быть больше 0.");

            AddValidationFor(() => Time)
                .When(() => Time <= 0 || Time > 60)
                .Show("Недопустимое значение (интервал допустимых значений 1 - 60)");

            AddValidationFor(() => Q)
                .When(() => Q < 0 || Q > 10)
                .Show("Недопустимое значение (интервал допустимых значений 0 - 10)");
            
            AddValidationFor(() => P)
               .When(() => ValueRangeHelper.PressureRange.IsOutsideRange(P))
               .Show(ValueRangeHelper.PressureRange.ViolationMessage);

            AddValidationFor(() => T)
                .When(() => ValueRangeHelper.TemperatureRange.IsOutsideRange(T))
                .Show(ValueRangeHelper.TemperatureRange.ViolationMessage);

            AddValidationFor(() => Square)
            .When(() => Square <= 0)
            .Show("Недопустимое значение. Должно быть больше 0.");

            AddValidationFor(() => OpeningDegree)
            .When(() => OpeningDegree < 0 || OpeningDegree > 1)
            .Show("Недопустимое значение. Интервал допустимых значений от 0 до 1.");

            AddValidationFor(() => PressureAir)
            .When(() => PressureAir < ValueRangeHelper.PressureAirRange.Min || PressureAir > ValueRangeHelper.PressureAirRange.Max)
            .Show(ValueRangeHelper.PressureAirRange.ViolationMessage);


            AddValidationFor(() => NitrogenContent)
            .When(() => NitrogenContent < ValueRangeHelper.ContentRange.Min || NitrogenContent > ValueRangeHelper.ContentRange.Max)
            .Show(ValueRangeHelper.ContentRange.ViolationMessage);

            AddValidationFor(() => CarbonDioxideContent)
                .When(() => CarbonDioxideContent < ValueRangeHelper.ContentRange.Min || CarbonDioxideContent > ValueRangeHelper.ContentRange.Max)
                .Show(ValueRangeHelper.ContentRange.ViolationMessage);

            if (Mode == 1)
            {
                AddValidationFor(() => PurgeTime)
                .When(() => PurgeTime <= 0)
                .Show("Недопустимое значение. Должно быть больше 0.");

                AddValidationFor(() => Volume)
                .When(() => Volume <= 0)
                .Show("Недопустимое значение. Должно быть больше 0.");

                AddValidationFor(() => TestCount)
               .When(() => TestCount <= 0)
               .Show("Недопустимое значение. Должно быть больше 0.");
                
                AddValidationFor(() => TemperatureAir)
                .When(() => ValueRangeHelper.TemperatureRange.IsOutsideRange(TemperatureAir))
                .Show(ValueRangeHelper.TemperatureRange.ViolationMessage);
            }

            if (Mode == 2)
            {
                AddValidationFor(() => PurgeTime)
                .When(() => PurgeTime <= 0 || PurgeTime > RangeInHours)
                .Show($"Недопустимое значение (интервал допустимых значений 1 - {RangeInHours})");

                AddValidationFor(() => Qdev)
                .When(() => Qdev < 0)
                .Show("Недопустимое значение. Должно быть больше 0.");
            }
        }

        #endregion
    }
}