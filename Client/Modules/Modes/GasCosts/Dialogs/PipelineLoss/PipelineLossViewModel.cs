using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Application.Helpers;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.Dictionaries.ValvePurposes;
using GazRouter.DTO.GasCosts;
using GazRouter.DTO.ObjectModel.CompShops;
using GazRouter.DTO.ObjectModel.Sites;
using GazRouter.Flobus.VM.FloModel;
using GazRouter.Flobus.VM.Model;
using GazRouter.Modes.GasCosts.DefaultDataDialog;
using GazRouter.Modes.GasCosts.Dialogs.ViewModel;
using Utils.Units;

namespace GazRouter.Modes.GasCosts.Dialogs.PipelineLoss
{
    public class PipelineLossViewModel : CalcViewModelBase<PipelineLossModel>
    {
        private Pipeline _pipeline;

        public PipelineLossViewModel(GasCostDTO gasCost, Action<GasCostDTO> callback,
            List<DefaultParamValues> defaultParamValues, bool ShowDayly)
            : base(gasCost, callback, defaultParamValues, ShowDayly)
        {
            this.ShowDayly = ShowDayly;
            LoadPipelineInfo();

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
                CarbonDioxideContent = defaultValues.CarbonDioxideContent;
                NitrogenContent = defaultValues.NitrogenContent;
                PressureAir = defaultValues.PressureAir;
                
            }
        }
        
        private async void GetCompShopList()
        {
            var sites = await new ObjectModelServiceProxy().GetSiteListAsync(new GetSiteListParameterSet());
            var systemId = sites.Single(s => s.Id == SiteId).SystemId;

            var compShops = await new ObjectModelServiceProxy().GetCompShopListAsync(
                new GetCompShopListParameterSet
                {
                    SystemId = systemId
                });

            CompShopFromList = compShops.OrderBy(sh => sh.ShortPath).ToList();
            CompShopToList = compShops.OrderBy(sh => sh.ShortPath).ToList();

            SelectedCompShopFrom = CompShopFromList.SingleOrDefault(s => s.Id == Model.CompShopFromId) ??
                                    CompShopFromList.First();

            SelectedCompShopTo = CompShopToList.SingleOrDefault(s => s.Id == Model.CompShopToId) ?? CompShopToList.First();
            OnPropertyChanged(() => CompShopFromList);
            OnPropertyChanged(() => CompShopToList);
        }
        
        private CompShopDTO _selectedCompShopFrom;
        
        public CompShopDTO SelectedCompShopFrom
        {
            get { return _selectedCompShopFrom; }
            set
            {
                if (SetProperty(ref _selectedCompShopFrom, value))
                {
                    Model.CompShopFromId = value.Id;
                    PerformCalculate();
                }
            }
        }

        private CompShopDTO _selectedCompShopTo;
        public CompShopDTO SelectedCompShopTo
        {
            get { return _selectedCompShopTo; }
            set
            {
                if (SetProperty(ref _selectedCompShopTo, value))
                {
                    Model.CompShopToId = value.Id;
                    PerformCalculate();
                }
            }
        }

        /// <summary>
        /// Список КЦ до разрыва
        /// </summary>
        public List<CompShopDTO> CompShopFromList { get; set; }

        /// <summary>
        /// Список КЦ за разрывом
        /// </summary>
        public List<CompShopDTO> CompShopToList { get; set; }

        /// <summary>
        ///    Км подключения крана №21, находящегося после КС до разрыва по ходу движения газа
        /// </summary>
        public double Valve21Km {get; private set; }

        /// <summary>
        ///     Км подключения ближайшего линейного крана, находящегося перед местом разрыва по ходу движения газа
        /// </summary>
        public double LinValve1Km { get; private set; }

        /// <summary>
        ///    Км подключения крана №19, находящегося после КС до разрыва по ходу движения газа
        /// </summary>
        public double Valve19Km { get; private set; }

        /// <summary>
        ///     Км подключения ближайшего линейного крана, находящегося за местом разрыва по ходу движения газа
        /// </summary>
        public double LinValve2Km { get; private set; }

        /// <summary>
        ///     Километр разрыва газопровода
        /// </summary>
        public double KmBreaking
        {
            get { return Model.KmBreaking; }
            set
            {
                Model.KmBreaking = value;
                OnPropertyChanged(() => KmBreaking);
           //     RefreshSegmentInfo();
                PerformCalculate();
            }
        }

        /// <summary>
        ///   Расстояние от разрыва до КС1
        /// </summary>
        public double LengthToCompStation1
        {
            get { return Model.LengthToCompStation1; }
            set
            {
                Model.LengthToCompStation1 = value;
                OnPropertyChanged(() => LengthToCompStation1);
                PerformCalculate();
            }
        }

        /// <summary>
        ///    Расстояние от разрыва до КС2
        /// </summary>
        public double LengthToCompStation2
        {
            get { return Model.LengthToCompStation2; }
            set
            {
                Model.LengthToCompStation2 = value;
                OnPropertyChanged(() => LengthToCompStation2);
                PerformCalculate();
            }
        }

        /// <summary>
        ///     Расстояние от разрыва до ближайшего линейного крана, находящегося перед местом разрыва по ходу движения газа
        /// </summary>
        public double LengthToLinearValve1
        {
            get { return Model.LengthToLinearValve1; }
            set
            {
                Model.LengthToLinearValve1 = value;
                OnPropertyChanged(() => LengthToLinearValve1);
                PerformCalculate();
            }
        }

        /// <summary>
        ///     Расстояние от разрыва до ближайшего линейного крана, находящегося за местом разрыва по ходу движения газа
        /// </summary>
        public double LengthToLinearValve2
        {
            get { return Model.LengthToLinearValve2; }
            set
            {
                Model.LengthToLinearValve2 = value;
                OnPropertyChanged(() => LengthToLinearValve2);
                PerformCalculate();
            }
        }

        public bool NoHasLinearValve1
        {
            get { return Model.NoHasLinearValve1; }
            set
            {
                Model.NoHasLinearValve1 = value;
                OnPropertyChanged(() => NoHasLinearValve1);
                ClearValidations();
                SetValidationRules();
                PerformCalculate();
            }
        }

        public bool NoHasLinearValve2
        {
            get { return Model.NoHasLinearValve2; }
            set
            {
                Model.NoHasLinearValve2 = value;
                OnPropertyChanged(() => NoHasLinearValve2);
                ClearValidations();
                SetValidationRules();
                PerformCalculate();
            }
        }

        /// <summary>
        ///     Наименование ближайшего линейного крана, находящегося перед местом разрыва по ходу движения газа
        /// </summary>
        public string LinValve1Name { get; private set; }

        /// <summary>
        ///     Время прошедшее с момента аварии до момента полного закрытия линейного крана, находящегося до места разрыва по ходу
        ///     движения газа, с
        /// </summary>
        public int T1
        {
            get { return Model.T1; }
            set
            {
                Model.T1 = value;
                OnPropertyChanged(() => T1);
                PerformCalculate();
            }
        }

        /// <summary>
        ///     Наименование ближайшего линейного крана, находящегося за местом разрыва по ходу движения газа
        /// </summary>
        public string LinValve2Name { get; private set; }

        /// <summary>
        ///     Время прошедшее с момента аварии до момента полного закрытия линейного крана, находящегося за местом разрыва по
        ///     ходу движения газа, с
        /// </summary>
        public int T2
        {
            get { return Model.T2; }
            set
            {
                Model.T2 = value;
                OnPropertyChanged(() => T2);
                PerformCalculate();
            }
        }

        /// <summary>
        ///     Внутренний диаметр газопровода до разрыва, мм
        /// </summary>
        public double Diameter1
        {
            get { return Model.Diameter1; }
            set
            {
                Model.Diameter1 = value;
                OnPropertyChanged(() => Diameter1);
                PerformCalculate();
            }
        }

        /// <summary>
        ///     Внутренний диаметр газопровода за разрывом, мм
        /// </summary>
        public double Diameter2
        {
            get { return Model.Diameter2; }
            set
            {
                Model.Diameter2 = value;
                OnPropertyChanged(() => Diameter2);
                PerformCalculate();
            }
        }

        /// <summary>
        ///     Наименование крана №21, находящегося после КС до разрыва по ходу движения газа
        /// </summary>
        public string Valve21Name { get; private set; }

        /// <summary>
        ///     Наименование КС1, находящейся до разрыва по ходу движения газа
        /// </summary>
        public string CompStation1Name { get; private set; }

        /// <summary>
        ///     Время прошедшее с момента аварии до момента полного закрытия крана №21, с
        /// </summary>
        public int T21
        {
            get { return Model.T21; }
            set
            {
                Model.T21 = value;
                OnPropertyChanged(() => T21);
                PerformCalculate();
            }
        }

        /// <summary>
        ///     Наименование крана №19, находящегося перед КС после разрыва по ходу движения газа
        /// </summary>
        public string Valve19Name { get; private set; }

        /// <summary>
        ///     Наименование КС2, находящейся после разрыва по ходу движения газа
        /// </summary>
        public string CompStation2Name { get; private set; }

        /// <summary>
        ///     Время прошедшее с момента аварии до момента полного закрытия крана №19, с
        /// </summary>
        public int T19
        {
            get { return Model.T19; }
            set
            {
                Model.T19 = value;
                OnPropertyChanged(() => T19);
                PerformCalculate();
            }
        }

        /// <summary>
        ///     Плотность газа, кг/м³
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
        ///     Содержание азота, мол.доля %
        /// </summary>
        public double NitrogenContent
        {
            get { return Model.NitrogenContent; }
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
            get { return Model.CarbonDioxideContent; }
            set
            {
                Model.CarbonDioxideContent = value;
                OnPropertyChanged(() => CarbonDioxideContent);
                PerformCalculate();
            }
        }

        /// <summary>
        ///     Давление газа в начале участка газопровода до разрыва, кг/см²
        /// </summary>
        public Pressure PressureIn
        {
            get { return Model.PressureIn; }
            set
            {
                Model.PressureIn = value;
                OnPropertyChanged(() => PressureIn);
                PerformCalculate();
            }
        }

        /// <summary>
        ///     Давление газа в конце участка газопровода до разрыва, кг/см²
        /// </summary>
        public Pressure PressureOut
        {
            get { return Model.PressureOut; }
            set
            {
                Model.PressureOut = value;
                OnPropertyChanged(() => PressureOut);
                PerformCalculate();
            }
        }

        /// <summary>
        ///     Температура газа в начале участка газопровода до разрыва, Гр.С
        /// </summary>
        public Temperature TemperatureIn
        {
            get { return Model.TemperatureIn; }
            set
            {
                Model.TemperatureIn = value;
                OnPropertyChanged(() => TemperatureIn);
                PerformCalculate();
            }
        }

        /// <summary>
        ///     Температура газа в конце участка газопровода до разрыва, Гр.С
        /// </summary>
        public Temperature TemperatureOut
        {
            get { return Model.TemperatureOut; }
            set
            {
                Model.TemperatureOut = value;
                OnPropertyChanged(() => TemperatureOut);
                PerformCalculate();
            }
        }

        /// <summary>
        ///     Давление атмосферное, мм рт.ст.
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

        public int N
        {
            get { return Model.N; }
            set
            {
                Model.N = value;
                OnPropertyChanged(() => N);
                PerformCalculate();
            }
        }

        /// <summary>
        /// Признак закрытия перемычек между нитками для многониточного газопровода
        /// </summary>
        public bool DoesBridgesClose
        {
            get { return Model.DoesBridgesClose; }
            set
            {
                Model.DoesBridgesClose = value;
                OnPropertyChanged(() => DoesBridgesClose);
                PerformCalculate();
            }
        }

        

        /// <summary>
        ///     Производительность газопровода в нормальном режиме, млн.м3/сут.
        /// </summary>
        public double Q
        {
            get { return Model.Q; }
            set
            {
                Model.Q = value;
                OnPropertyChanged(() => Q);
                PerformCalculate();
            }
        }

        protected override void SetValidationRules()
        {
            if (_pipeline != null)
            {
                AddValidationFor(() => KmBreaking)
                    .When(() => KmBreaking < _pipeline.KmBegining || KmBreaking > _pipeline.KmEnd)
                    .Show(
                        $"Недопустимое значение (интервал допустимых значений от {_pipeline.KmBegining} до {_pipeline.KmEnd})");
            }

            AddValidationFor(() => SelectedCompShopFrom)
                .When(() => SelectedCompShopFrom == null)
                .Show("Не выбран цех");

            AddValidationFor(() => SelectedCompShopTo)
                .When(() => SelectedCompShopTo == null)
                .Show("Не выбран цех");

            if (!NoHasLinearValve1)
                AddValidationFor(() => LengthToLinearValve1)
                    .When(() => LengthToLinearValve1 <= 0)
                    .Show("Недопустимое значение. Должно быть больше 0.");

            if (!NoHasLinearValve2)
                AddValidationFor(() => LengthToLinearValve2)
                    .When(() => LengthToLinearValve2 <= 0)
                    .Show("Недопустимое значение. Должно быть больше 0.");

            AddValidationFor(() => LengthToCompStation2)
                .When(() => LengthToCompStation2 <= 0)
                .Show("Недопустимое значение. Должно быть больше 0.");

            AddValidationFor(() => LengthToCompStation1)
                .When(() => LengthToCompStation1 <= 0)
                .Show("Недопустимое значение. Должно быть больше 0.");

            AddValidationFor(() => T1)
                .When(() => T1 < 1 || T1 > 600)
                .Show("Недопустимое значение (интервал допустимых значений от 1 до 600)");

            AddValidationFor(() => T2)
                .When(() => T2 < 1 || T2 > 600)
                .Show("Недопустимое значение (интервал допустимых значений от 1 до 600)");

            AddValidationFor(() => T21)
                .When(() => T21 < 1 || T21 > 600)
                .Show("Недопустимое значение (интервал допустимых значений от 1 до 600)");

            AddValidationFor(() => T19)
                .When(() => T19 < 1 || T19 > 600)
                .Show("Недопустимое значение (интервал допустимых значений от 1 до 600)");

            AddValidationFor(() => PressureIn)
                .When(() => ValueRangeHelper.PressureRange.IsOutsideRange(PressureIn))
                .Show(ValueRangeHelper.PressureRange.ViolationMessage);

            AddValidationFor(() => PressureOut)
                .When(() => ValueRangeHelper.PressureRange.IsOutsideRange(PressureOut))
                .Show(ValueRangeHelper.PressureRange.ViolationMessage);

            AddValidationFor(() => TemperatureIn)
                .When(() => ValueRangeHelper.TemperatureRange.IsOutsideRange(TemperatureIn))
                .Show(ValueRangeHelper.TemperatureRange.ViolationMessage);

            AddValidationFor(() => TemperatureOut)
                .When(() => ValueRangeHelper.TemperatureRange.IsOutsideRange(TemperatureOut))
                .Show(ValueRangeHelper.TemperatureRange.ViolationMessage);

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
                .When(
                    () =>
                        NitrogenContent < ValueRangeHelper.ContentRange.Min ||
                        NitrogenContent > ValueRangeHelper.ContentRange.Max)
                .Show(ValueRangeHelper.ContentRange.ViolationMessage);

            AddValidationFor(() => CarbonDioxideContent)
                .When(
                    () =>
                        CarbonDioxideContent < ValueRangeHelper.ContentRange.Min ||
                        CarbonDioxideContent > ValueRangeHelper.ContentRange.Max)
                .Show(ValueRangeHelper.ContentRange.ViolationMessage);

            AddValidationFor(() => Q)
                .When(() => Q <= 0)
                .Show("Недопустимое значение. Должно быть больше 0.");

            AddValidationFor(() => N)
                .When(() => N < 1 || N > 15)
                .Show("Недопустимое значение (интервал допустимых значений от 1 до 15)");

            AddValidationFor(() => Diameter1)
                .When(() => Diameter1 <= 0)
                .Show("Недопустимое значение. Должно быть больше 0.");

            AddValidationFor(() => Diameter2)
                .When(() => Diameter2 <= 0)
                .Show("Недопустимое значение. Должно быть больше 0.");
        }

        private async void LoadPipelineInfo()
        {
            _pipeline = await PipelineLoader.LoadAsync(Entity.Id);
            GetCompShopList();
            SetValidationRules();
            ValidateAll();
        }

        private void RefreshSegmentInfo()
        {            
            if (_pipeline != null && KmBreaking >= _pipeline.KmBegining && KmBreaking <= _pipeline.KmEnd)
            {
                var valves = _pipeline.Valves.ToList();
                if (valves.Count < 2)
                {
                    return;
                }

                for (var i = 0; i < valves.Count; i++)
                {
                    if (i == valves.Count - 1)
                    {
                        break;
                    }

                    // здесь нужно сделать проверку, если разрыв попадает на кран

                    if (valves[i].Km < KmBreaking && KmBreaking < valves[i + 1].Km)
                    {
                        LinValve1Name = valves[i].Name;
                        //Model.KmOfLinearValve1 = valves[i].Km;
                        OnPropertyChanged(() => LinValve1Name);
                        LinValve2Name = valves[i + 1].Name;
                        //Model.KmOfLinearValve2 = valves[i + 1].Km;
                        OnPropertyChanged(() => LinValve2Name);
                    }
                }

                // Поиск 21 крана
                var valve21 =
                    _pipeline.Valves.LastOrDefault(
                        v => v.Dto.ValvePurposeId == ValvePurpose.OutletProtectiveCompShop && v.Km < KmBreaking);

                if (valve21 != null)
                {
                    Valve21Name = valve21.Name;
                    //Model.KmOfCompStation1 = valve21.Km;
                    OnPropertyChanged(() => Valve21Name);
                }

                // Поиск 19 крана
                var valve19 =
                    _pipeline.Valves.FirstOrDefault(
                        v => v.Dto.ValvePurposeId == ValvePurpose.InletProtectiveCompShop && v.Km > KmBreaking);

                if (valve19 != null)
                {
                    Valve19Name = valve19.Name;
                    //Model.KmOfCompStation2 = valve19.Km;
                    OnPropertyChanged(() => Valve19Name);
                }
            }
        }
    }
}