using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Application.Helpers;
using GazRouter.Controls.Dialogs.PipingVolumeCalculator;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.GasCosts;
using GazRouter.Flobus.VM.FloModel;
using GazRouter.Flobus.VM.Model;
using GazRouter.Modes.GasCosts.DefaultDataDialog;
using GazRouter.Modes.GasCosts.Dialogs.ViewModel;
using Microsoft.Practices.Prism.Commands;
using Utils.Units;

namespace GazRouter.Modes.GasCosts.Dialogs.RepairCosts
{
    public class RepairCostsViewModel : CalcViewModelBase<RepairCostsModel>
    {
        public RepairCostsViewModel(GasCostDTO gasCost, Action<GasCostDTO> callback,
            List<DefaultParamValues> defaultParamValues, bool ShowDayly)
            : base(gasCost, callback, defaultParamValues,ShowDayly)
        {

            this.ShowDayly = ShowDayly;
            if (!IsEdit)
            {
                // Если вводится фактическое значение и выбран текущий месяц, 
                // то устанавливать дату в текущий день
                if (TargetId == Target.Fact && IsCurrentMonth)
                {
                    EventDate = EventDate.AddDays(DateTime.Now.Day - EventDate.Day);
                }

                var defaultValues = DefaultParamValues.Single(d => d.Target == TargetId);
                Density = defaultValues.Density;
                PressureAir = defaultValues.PressureAir;
                CarbonDioxideContent = defaultValues.CarbonDioxideContent;
                NitrogenContent = defaultValues.NitrogenContent;
            }

            OpenPipingVolumeCalculator = new DelegateCommand(
                () =>
                {
                    var vm = new PipingVolumeCalculatorViewModel(
                        Model.Piping,
                        lst =>
                        {
                            Model.Piping = lst;
                            OnPropertyChanged(() => PipingVolume);
                            PerformCalculate();
                        });
                    var v = new PipingVolumeCalculatorView
                    {
                        DataContext = vm
                    };
                    v.ShowDialog();
                });

            CopyValueCommand = new DelegateCommand<object>(
                x =>
                {
                    var way = int.Parse((string) x);
                    switch (way)
                    {
                        case 1:
                            PressureInitialOut = PressureInitialIn;
                            break;
                        case 2:
                            TemperatureInitialOut = TemperatureInitialIn;
                            break;
                        case 3:
                            PressureRecoveryOut = PressureRecoveryIn;
                            break;
                        case 4:
                            TemperatureRecoveryOut = TemperatureRecoveryIn;
                            break;
                        case 5:
                            PressureMobilePumpRecoveryOut = PressureMobilePumpRecoveryIn;
                            break;
                        case 6:
                            TemperatureMobilePumpRecoveryOut = TemperatureMobilePumpRecoveryIn;
                            break;
                        case 7:
                            PressureFinalOut = PressureFinalIn;
                            break;
                        case 8:
                            TemperatureFinalOut = TemperatureFinalIn;
                            break;
                    }
                });

            LoadPipelineData();
            PerformCalculate();
        }

        /// <summary>
        ///     Открывает калькулятор геометрического объема газопроводов
        /// </summary>
        public DelegateCommand OpenPipingVolumeCalculator { get; private set; }

        /// <summary>
        ///     Копирует данные из одного поля в другое
        /// </summary>
        public DelegateCommand<object> CopyValueCommand { get; private set; }

        /// <summary>
        ///     Геометрический объем газопроводов, м³
        /// </summary>
        public double PipingVolume => Model.PipingVolume;

        /// <summary>
        ///     Выбранный газопровод
        /// </summary>
        public PipelineSection PipeSection { get; set; }

        /// <summary>
        ///     Является ли выбранный объект газопроводом
        /// </summary>
        public bool IsPipelineSelected => Entity.EntityType == EntityType.Pipeline;

        public List<double> StartKilometerList
        {
            get
            {
                if (PipeSection == null)
                {
                    return new List<double>();
                }
                return PipeSection.Pipe.SegmentKilometers.Count > 0
                    ? PipeSection.Pipe.SegmentKilometers.GetRange(0, PipeSection.Pipe.SegmentKilometers.Count - 1)
                    : PipeSection.Pipe.SegmentKilometers;
            }
        }

        public List<double> EndKilometerList
        {
            get
            {
                if (PipeSection == null)
                {
                    return new List<double>();
                }
                return PipeSection.Pipe.SegmentKilometers.Count > 0
                    ? PipeSection.Pipe.SegmentKilometers.GetRange(1, PipeSection.Pipe.SegmentKilometers.Count - 1)
                    : PipeSection.Pipe.SegmentKilometers;
            }
        }


        /// <summary>
        ///      Введённый километр начала участка
        /// </summary>
        public double KilometerStart
        {
            get { return Model.KmStart; }
            set
            {
                Model.KmStart = value;
                PipeSection.KmBegining = value;
                SetProperty(ref _KilometerStartSelectedIndex, StartKilometerList.IndexOf(value), "KilometerStartSelectedIndex");
                if (KilometerStart >= KilometerEnd && PipeSection.Pipe.SegmentKilometers.Count > 0)
                {
                    double newEnd = EndKilometerList.FirstOrDefault(ek => ek > KilometerStart);
                    if (newEnd > 0.0)
                    {
                        Model.KmEnd = PipeSection.KmEnd = newEnd;
                        OnPropertyChanged(() => KilometerEnd);
                    }
                }

                OnPropertyChanged(() => KilometerStart);
                OnPropertyChanged(() => SectionGeometry);
                OnPropertyChanged(() => SectionVolume);

                Model.SectionVolume = PipeSection.GeometricVolume;
                PerformCalculate();
            }
        }

        private int _KilometerStartSelectedIndex;
        /// <summary>
        ///     Выбранный километр начала участка
        /// </summary>
        public int KilometerStartSelectedIndex
        {
            get { return _KilometerStartSelectedIndex; }
            set
            {
                if (value > -1)
                    KilometerStart = StartKilometerList[value];
            }
        }

        /// <summary>
        ///     Введённый километр конца участка
        /// </summary>
        public double KilometerEnd
        {
            get { return Model.KmEnd; }
            set
            {
                Model.KmEnd = PipeSection.KmEnd = value;
                SetProperty(ref _KilometerEndSelectedIndex, EndKilometerList.IndexOf(value), "KilometerEndSelectedIndex");
                if (KilometerEnd <= KilometerStart && PipeSection.Pipe.SegmentKilometers.Count > 0)
                {
                    double newStart = StartKilometerList.LastOrDefault(ek => ek < KilometerEnd);
                    Model.KmStart = PipeSection.KmBegining = newStart;
                    OnPropertyChanged(() => KilometerStart);
                }

                OnPropertyChanged(() => KilometerEnd);
                OnPropertyChanged(() => SectionGeometry);
                OnPropertyChanged(() => SectionVolume);

                Model.SectionVolume = PipeSection.GeometricVolume;
                PerformCalculate();
            }
        }

        private int _KilometerEndSelectedIndex;
        /// <summary>
        ///     Выбранный километр конца участка
        /// </summary>
        public int KilometerEndSelectedIndex
        {
            get { return _KilometerEndSelectedIndex; }
            set
            {
                if (value > -1)
                {
                    KilometerEnd = EndKilometerList[value];
                }
            }
        }

        public string SectionGeometry => IsPipelineSelected && PipeSection != null ? PipeSection.GeometryString : string.Empty;

        public double SectionVolume => IsPipelineSelected && PipeSection != null ? Math.Round(PipeSection.GeometricVolume) : 0;

        /// <summary>
        ///     Кол-во продувок
        /// </summary>
        public int PurgeCount
        {
            get { return Model.PurgeCount; }
            set
            {
                Model.PurgeCount = value;
                OnPropertyChanged(() => PurgeCount);
                PerformCalculate();
            }
        }

        /// <summary>
        ///     Плотность газа
        /// </summary>
        public double Density
        {
            get { return Model.Density.KilogramsPerCubicMeter; }
            set
            {
                Model.Density = Utils.Units.Density.FromKilogramsPerCubicMeter(value);
                OnPropertyChanged(() => Density);
                PerformCalculate();
            }
        }

        /// <summary>
        ///     Давление атмосферное
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
        ///     Давление в начале участка до отключения
        /// </summary>
        public Pressure PressureInitialIn
        {
            get { return Model.PressureInitialIn; }
            set
            {
                Model.PressureInitialIn = value;
                OnPropertyChanged(() => PressureInitialIn);
                PerformCalculate();
            }
        }

        /// <summary>
        ///     Давление в конце участка до отключения
        /// </summary>
        public Pressure PressureInitialOut
        {
            get { return Model.PressureInitialOut; }
            set
            {
                Model.PressureInitialOut = value;
                OnPropertyChanged(() => PressureInitialOut);
                PerformCalculate();
            }
        }

        /// <summary>
        ///     Температура в начале участка до отключения
        /// </summary>
        public Temperature TemperatureInitialIn
        {
            get { return Model.TemperatureInitialIn; }
            set
            {
                Model.TemperatureInitialIn = value;
                OnPropertyChanged(() => TemperatureInitialIn);
                PerformCalculate();
            }
        }

        /// <summary>
        ///     Температура в конце участка до отключения
        /// </summary>
        public Temperature TemperatureInitialOut
        {
            get { return Model.TemperatureInitialOut; }
            set
            {
                Model.TemperatureInitialOut = value;
                OnPropertyChanged(() => TemperatureInitialOut);
                PerformCalculate();
            }
        }

        /// <summary>
        ///     Производится ли выработка газа
        /// </summary>
        public bool HasRecovery
        {
            get { return Model.HasRecovery; }
            set
            {
                Model.HasRecovery = value;
                OnPropertyChanged(() => HasRecovery);
                PerformCalculate();
            }
        }

        /// <summary>
        ///     Давление в начале участка после выработки
        /// </summary>
        public Pressure PressureRecoveryIn
        {
            get { return Model.PressureRecoveryIn; }
            set
            {
                Model.PressureRecoveryIn = value;
                OnPropertyChanged(() => PressureRecoveryIn);
                PerformCalculate();
            }
        }

        /// <summary>
        ///     Давление в конце участка после выработки
        /// </summary>
        public Pressure PressureRecoveryOut
        {
            get { return Model.PressureRecoveryOut; }
            set
            {
                Model.PressureRecoveryOut = value;
                OnPropertyChanged(() => PressureRecoveryOut);
                PerformCalculate();
            }
        }

        /// <summary>
        ///     Температура в начале участка после выработки
        /// </summary>
        public Temperature TemperatureRecoveryIn
        {
            get { return Model.TemperatureRecoveryIn; }
            set
            {
                Model.TemperatureRecoveryIn = value;
                OnPropertyChanged(() => TemperatureRecoveryIn);
                PerformCalculate();
            }
        }

        /// <summary>
        ///     Температура в конце участка после выработки
        /// </summary>
        public Temperature TemperatureRecoveryOut
        {
            get { return Model.TemperatureRecoveryOut; }
            set
            {
                Model.TemperatureRecoveryOut = value;
                OnPropertyChanged(() => TemperatureRecoveryOut);
                PerformCalculate();
            }
        }

        /// <summary>
        ///     Объем выработанного газа
        /// </summary>
        public double RecoveryVolume
        {
            get { return Math.Round(Model.RecoveryVolume, 3)*Coef; }
        }

        /// <summary>
        ///     Производится ли выработка газа мобильной КС
        /// </summary>
        public bool HasMobilePumpRecovery
        {
            get { return Model.HasMobilePumpRecovery; }
            set
            {
                Model.HasMobilePumpRecovery = value;
                OnPropertyChanged(() => HasMobilePumpRecovery);
                PerformCalculate();
            }
        }

        /// <summary>
        ///     Давление в начале участка после выработки мобильной КС
        /// </summary>
        public Pressure PressureMobilePumpRecoveryIn
        {
            get { return Model.PressureMobilePumpRecoveryIn; }
            set
            {
                Model.PressureMobilePumpRecoveryIn = value;
                OnPropertyChanged(() => PressureMobilePumpRecoveryIn);
                PerformCalculate();
            }
        }

        /// <summary>
        ///     Давление в конце участка после выработки мобильной КС
        /// </summary>
        public Pressure PressureMobilePumpRecoveryOut
        {
            get { return Model.PressureMobilePumpRecoveryOut; }
            set
            {
                Model.PressureMobilePumpRecoveryOut = value;
                OnPropertyChanged(() => PressureMobilePumpRecoveryOut);
                PerformCalculate();
            }
        }

        /// <summary>
        ///     Температура в начале участка после выработки мобильной КС
        /// </summary>
        public Temperature TemperatureMobilePumpRecoveryIn
        {
            get { return Model.TemperatureMobilePumpRecoveryIn; }
            set
            {
                Model.TemperatureMobilePumpRecoveryIn = value;
                OnPropertyChanged(() => TemperatureMobilePumpRecoveryIn);
                PerformCalculate();
            }
        }

        /// <summary>
        ///     Температура в конце участка после выработки мобильной КС
        /// </summary>
        public Temperature TemperatureMobilePumpRecoveryOut
        {
            get { return Model.TemperatureMobilePumpRecoveryOut; }
            set
            {
                Model.TemperatureMobilePumpRecoveryOut = value;
                OnPropertyChanged(() => TemperatureMobilePumpRecoveryOut);
                PerformCalculate();
            }
        }

        /// <summary>
        ///     Объем газа выработанный мобильной КС
        /// </summary>
        public double MobilePumpRecoveryVolume
        {
            get { return Math.Round(Model.MobilePumpRecoveryVolume, 3)*Coef; }
        }

        /// <summary>
        ///     Давление в начале участка после стравливания
        /// </summary>
        public Pressure PressureFinalIn
        {
            get { return Model.PressureFinalIn; }
            set
            {
                Model.PressureFinalIn = value;
                OnPropertyChanged(() => PressureFinalIn);
                PerformCalculate();
            }
        }

        /// <summary>
        ///     Давление в конце участка после стравливания
        /// </summary>
        public Pressure PressureFinalOut
        {
            get { return Model.PressureFinalOut; }
            set
            {
                Model.PressureFinalOut = value;
                OnPropertyChanged(() => PressureFinalOut);
                PerformCalculate();
            }
        }

        /// <summary>
        ///     Температура в начале участка после стравливания
        /// </summary>
        public Temperature TemperatureFinalIn
        {
            get { return Model.TemperatureFinalIn; }
            set
            {
                Model.TemperatureFinalIn = value;
                OnPropertyChanged(() => TemperatureFinalIn);
                PerformCalculate();
            }
        }

        /// <summary>
        ///     Температура в конце участка после стравливания
        /// </summary>
        public Temperature TemperatureFinalOut
        {
            get { return Model.TemperatureFinalOut; }
            set
            {
                Model.TemperatureFinalOut = value;
                OnPropertyChanged(() => TemperatureFinalOut);
                PerformCalculate();
            }
        }

        /// <summary>
        ///     Содержание азота, мол.доля %
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
        ///     Содержание двуокиси углерода, мол.доля %
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

        protected override void SetValidationRules()
        {
            AddValidationFor(() => PurgeCount)
                .When(() => PurgeCount < 0 || Density > 10)
                .Show("Недопустимое значение (интервал допустимых значений от 0 до 10)");

            AddValidationFor(() => Density)
                .When(() => Density < ValueRangeHelper.DensityRange.Min || Density > ValueRangeHelper.DensityRange.Max)
                .Show(ValueRangeHelper.DensityRange.ViolationMessage);

            AddValidationFor(() => PressureAir)
                .When(
                    () =>
                        PressureAir < ValueRangeHelper.PressureAirRange.Min ||
                        PressureAir > ValueRangeHelper.PressureAirRange.Max)
                .Show(ValueRangeHelper.PressureAirRange.ViolationMessage);

            AddValidationFor(() => NitrogenContent)
                .When(() => NitrogenContent < ValueRangeHelper.ContentRange.Min || NitrogenContent > ValueRangeHelper.ContentRange.Max)
                .Show(ValueRangeHelper.ContentRange.ViolationMessage);

            AddValidationFor(() => CarbonDioxideContent)
                .When(() => CarbonDioxideContent < ValueRangeHelper.ContentRange.Min || CarbonDioxideContent > ValueRangeHelper.ContentRange.Max)
                .Show(ValueRangeHelper.ContentRange.ViolationMessage);

            AddValidationFor(() => PressureInitialIn)
                .When(() => ValueRangeHelper.PressureRange.IsOutsideRange(PressureInitialIn))
                .Show(ValueRangeHelper.PressureRange.ViolationMessage);
            AddValidationFor(() => PressureInitialOut)
                .When(() => ValueRangeHelper.PressureRange.IsOutsideRange(PressureInitialOut))
                .Show(ValueRangeHelper.PressureRange.ViolationMessage);
            AddValidationFor(() => TemperatureInitialIn)
                .When(() => ValueRangeHelper.TemperatureRange.IsOutsideRange(TemperatureInitialIn))
                .Show(ValueRangeHelper.TemperatureRange.ViolationMessage);
            AddValidationFor(() => TemperatureInitialOut)
                .When(() => ValueRangeHelper.TemperatureRange.IsOutsideRange(TemperatureInitialOut))
                .Show(ValueRangeHelper.TemperatureRange.ViolationMessage);

            AddValidationFor(() => PressureRecoveryIn)
                .When(() => HasRecovery && ValueRangeHelper.PressureRange.IsOutsideRange(PressureRecoveryIn))
                .Show(ValueRangeHelper.PressureRange.ViolationMessage);
            AddValidationFor(() => PressureRecoveryOut)
                .When(() => HasRecovery && ValueRangeHelper.PressureRange.IsOutsideRange(PressureRecoveryOut))
                .Show(ValueRangeHelper.PressureRange.ViolationMessage);
            AddValidationFor(() => TemperatureRecoveryIn)
                .When(() => HasRecovery && ValueRangeHelper.TemperatureRange.IsOutsideRange(TemperatureRecoveryIn))
                .Show(ValueRangeHelper.TemperatureRange.ViolationMessage);
            AddValidationFor(() => TemperatureRecoveryOut)
                .When(() => HasRecovery && ValueRangeHelper.TemperatureRange.IsOutsideRange(TemperatureRecoveryOut))
                .Show(ValueRangeHelper.TemperatureRange.ViolationMessage);

            AddValidationFor(() => PressureMobilePumpRecoveryIn)
                .When(
                    () =>
                        HasMobilePumpRecovery &&
                        ValueRangeHelper.PressureRange.IsOutsideRange(PressureMobilePumpRecoveryIn))
                .Show(ValueRangeHelper.PressureRange.ViolationMessage);
            AddValidationFor(() => PressureMobilePumpRecoveryOut)
                .When(
                    () =>
                        HasMobilePumpRecovery &&
                        ValueRangeHelper.PressureRange.IsOutsideRange(PressureMobilePumpRecoveryOut))
                .Show(ValueRangeHelper.PressureRange.ViolationMessage);
            AddValidationFor(() => TemperatureMobilePumpRecoveryIn)
                .When(
                    () =>
                        HasMobilePumpRecovery &&
                        ValueRangeHelper.TemperatureRange.IsOutsideRange(TemperatureMobilePumpRecoveryIn))
                .Show(ValueRangeHelper.TemperatureRange.ViolationMessage);
            AddValidationFor(() => TemperatureMobilePumpRecoveryOut)
                .When(
                    () =>
                        HasMobilePumpRecovery &&
                        ValueRangeHelper.TemperatureRange.IsOutsideRange(TemperatureMobilePumpRecoveryOut))
                .Show(ValueRangeHelper.TemperatureRange.ViolationMessage);

            AddValidationFor(() => PressureFinalIn)
                .When(() => ValueRangeHelper.PressureRange.IsOutsideRange(PressureFinalIn))
                .Show(ValueRangeHelper.PressureRange.ViolationMessage);
            AddValidationFor(() => PressureFinalOut)
                .When(() => ValueRangeHelper.PressureRange.IsOutsideRange(PressureFinalOut))
                .Show(ValueRangeHelper.PressureRange.ViolationMessage);
            AddValidationFor(() => TemperatureFinalIn)
                .When(() => ValueRangeHelper.TemperatureRange.IsOutsideRange(TemperatureFinalIn))
                .Show(ValueRangeHelper.TemperatureRange.ViolationMessage);
            AddValidationFor(() => TemperatureFinalOut)
                .When(() => ValueRangeHelper.TemperatureRange.IsOutsideRange(TemperatureFinalOut))
                .Show(ValueRangeHelper.TemperatureRange.ViolationMessage);

            AddValidationFor(() => PressureRecoveryIn)
                .When(() => HasRecovery && PressureRecoveryIn > PressureInitialIn)
                .Show("Давление газа после выработки не может быть больше давления до отключения участка");
            AddValidationFor(() => PressureRecoveryOut)
                .When(() => HasRecovery && PressureRecoveryOut > PressureInitialOut)
                .Show("Давление газа после выработки не может быть больше давления до отключения участка");

            AddValidationFor(() => PressureMobilePumpRecoveryIn)
                .When(() => HasMobilePumpRecovery &&
                            ((HasRecovery && PressureMobilePumpRecoveryIn > PressureRecoveryIn)
                             || PressureMobilePumpRecoveryIn > PressureInitialIn))
                .Show("Давление газа после выработки не может быть больше давления до выработки");
            AddValidationFor(() => PressureMobilePumpRecoveryOut)
                .When(() => HasMobilePumpRecovery &&
                            ((HasRecovery && PressureMobilePumpRecoveryOut > PressureRecoveryOut)
                             || PressureMobilePumpRecoveryOut > PressureInitialOut))
                .Show("Давление газа после выработки не может быть больше давления до выработки");

            AddValidationFor(() => PressureFinalIn)
                .When(() => (HasRecovery && PressureFinalIn > PressureRecoveryIn)
                            || (HasMobilePumpRecovery && PressureFinalIn > PressureMobilePumpRecoveryIn)
                            || PressureFinalIn > PressureInitialIn)
                .Show("Давление газа после стравливания не может быть больше давления до стравливания");
            AddValidationFor(() => PressureFinalOut)
                .When(() => (HasRecovery && PressureFinalOut > PressureRecoveryOut)
                            || (HasMobilePumpRecovery && PressureFinalOut > PressureMobilePumpRecoveryOut)
                            || PressureFinalOut > PressureInitialOut)
                .Show("Давление газа после стравливания не может быть больше давления до стравливания");
        }

        protected override void PerformCalculate()
        {
            base.PerformCalculate();

            OnPropertyChanged(() => RecoveryVolume);
            OnPropertyChanged(() => MobilePumpRecoveryVolume);
        }

        private async void LoadPipelineData()
        {
            if (!IsPipelineSelected)
            {
                return;
            }

            Behavior.TryLock();
            var pl = await PipelineLoader.LoadAsync(Entity.Id);
            if (pl.SegmentKilometers.Count >= 2)
            {
                PipeSection = new PipelineSection(
                    pl,
                    pl.SegmentKilometers.First(),
                    pl.SegmentKilometers.Last());

                if (!IsEdit)
                {
                    Model.KmStart = PipeSection.KmBegining;
                    Model.KmEnd = PipeSection.KmEnd;
                }
                else
                {
                    PipeSection.KmBegining = Model.KmStart;
                    PipeSection.KmEnd = Model.KmEnd;
                }
                OnPropertyChanged(() => StartKilometerList);
                OnPropertyChanged(() => EndKilometerList);
                OnPropertyChanged(() => KilometerStart);
                OnPropertyChanged(() => KilometerEnd);
                OnPropertyChanged(() => SectionGeometry);
                OnPropertyChanged(() => SectionVolume);
                Model.SectionVolume = PipeSection.GeometricVolume;

                AddValidationFor(() => KilometerStart)
                .When(() => KilometerStart < StartKilometerList.FirstOrDefault() || KilometerStart > EndKilometerList.LastOrDefault())
                .Show($"Недопустимое значение (интервал допустимых значений от {StartKilometerList.FirstOrDefault()} до {EndKilometerList.LastOrDefault()})");
                AddValidationFor(() => KilometerEnd)
                    .When(() => KilometerEnd < StartKilometerList.FirstOrDefault() || KilometerEnd > EndKilometerList.LastOrDefault())
                    .Show($"Недопустимое значение (интервал допустимых значений от {StartKilometerList.FirstOrDefault()} до {EndKilometerList.LastOrDefault()})");
                AddValidationFor(() => KilometerStart)
                   .When(() => KilometerStart > KilometerEnd)
                   .Show("Км начала газопровода не может быть больше км конца газопровода");
                AddValidationFor(() => KilometerEnd)
                  .When(() => KilometerStart > KilometerEnd)
                  .Show("Км начала газопровода не может быть больше км конца газопровода");

                SetProperty(ref _KilometerStartSelectedIndex, StartKilometerList.IndexOf(KilometerStart), "KilometerStartSelectedIndex");
                SetProperty(ref _KilometerEndSelectedIndex, EndKilometerList.IndexOf(KilometerEnd), "KilometerEndSelectedIndex");
            }
            Behavior.TryUnlock();
        }
    }
}