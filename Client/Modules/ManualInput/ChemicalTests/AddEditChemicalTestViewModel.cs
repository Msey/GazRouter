using System;
using System.Threading.Tasks;
using GazRouter.Application.Helpers;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.ManualInput;
using GazRouter.DTO.ManualInput.ChemicalTests;
using GazRouter.DTO.ManualInput.ValveSwitches;
using GazRouter.Modes.GasCosts.Imports;
using Utils.Extensions;
using Utils.Units;


namespace GazRouter.ManualInput.ChemicalTests
{
    public class AddEditChemicalTestViewModel : AddEditViewModelBase<ValveSwitchDTO, int>
    {
        private Guid _measPointId;
        private int _testId;
        private DateTime _testDate;
        private Temperature? _dewPoint;
        private Temperature? _dewPointHydrocarbon;
        private double? _contentnNitrogen;
        private double? _concentrationSourSulfur;
        private double? _concentrationHydrogenSulfide;
        private double? _contentCarbonDioxid;
        private double? _density;
        private double? _combHeatLow;


        
        public AddEditChemicalTestViewModel(Action<int> actionBeforeClosing, ChemicalTestDTO dto, bool isEdit)
            : base(actionBeforeClosing)
        {
            IsEdit = isEdit;
            _measPointId = dto?.MeasPointId ?? Guid.Empty;
            _testId = dto?.ChemicalTestId ?? 0;

            if (isEdit)
                TestDate = dto?.TestDate ?? DateTime.Now;
            else
                TestDate = DateTime.Now;
                
            

            DewPoint = dto?.DewPoint != null ? Temperature.FromCelsius(dto.DewPoint.Value) : (Temperature?) null;
            DewPointHydrocarbon = dto?.DewPointHydrocarbon != null ? Temperature.FromCelsius(dto.DewPointHydrocarbon.Value) : (Temperature?) null;
            ContentNitrogen = dto?.ContentNitrogen;
            ConcentrationSourSulfur = dto?.ConcentrSourSulfur;
            ConcentrationHydrogenSulfide = dto?.ConcentrHydrogenSulfide;
            ContentCarbonDioxid = dto?.ContentCarbonDioxid;
            Density = dto?.Density;
            CombHeatLow = dto?.CombHeatLowJoule;
            
            SetValidationRules();

            SaveCommand.RaiseCanExecuteChanged();
        }
        
        
        

        /// <summary>
        /// Дата переключения
        /// </summary>
        public DateTime TestDate
        {
            get { return _testDate; }
            set
            {
                if (SetProperty(ref _testDate, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }


        public DateTime TestDateRangeStart
        {
            get { return SeriesHelper.GetCurrentSessionPeriod().Item1; }
        }

        public DateTime TestDateRangeEnd
        {
            get
            {
                return SeriesHelper.GetCurrentSessionPeriod().Item2 < DateTime.Now
                    ? SeriesHelper.GetCurrentSessionPeriod().Item2
                    : DateTime.Now;
            }
        }


        /// <summary>
        /// Т точки росы по влаге, Гр.С 
        /// </summary>
        public Temperature? DewPoint
        {
            get { return _dewPoint; }
            set
            {
                _dewPoint = value;
                OnPropertyChanged(() => DewPoint);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }


        /// <summary>
        /// Т точки росы по углеводородам, Гр.С 
        /// </summary>
        public Temperature? DewPointHydrocarbon
        {
            get { return _dewPointHydrocarbon; }
            set
            {
                _dewPointHydrocarbon = value;
                OnPropertyChanged(() => DewPointHydrocarbon);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        

        /// <summary>
        /// Молярная доля азота, % 
        /// </summary>
        public double? ContentNitrogen
        {
            get { return _contentnNitrogen; }
            set
            {
                _contentnNitrogen = value;
                OnPropertyChanged(() => ContentNitrogen);
            }
        }
        

        /// <summary>
        /// Концентрация меркаптановой серы, г/м3 
        /// </summary>
        public double? ConcentrationSourSulfur
        {
            get { return _concentrationSourSulfur; }
            set
            {
                _concentrationSourSulfur = value;
                OnPropertyChanged(() => ConcentrationSourSulfur);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }


        /// <summary>
        /// Концентрация сероводорода, г/м3 
        /// </summary>
        public double? ConcentrationHydrogenSulfide
        {
            get { return _concentrationHydrogenSulfide; }
            set
            {
                _concentrationHydrogenSulfide = value;
                OnPropertyChanged(() => ConcentrationHydrogenSulfide);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }


        /// <summary>
        /// Молярная доля углекислого газа, % 
        /// </summary>
        public double? ContentCarbonDioxid
        {
            get { return _contentCarbonDioxid; }
            set
            {
                _contentCarbonDioxid = value;
                OnPropertyChanged(() => ContentCarbonDioxid);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }


        /// <summary>
        /// Плотность, кг/м3 
        /// </summary>
        public double? Density
        {
            get { return _density; }
            set
            {
                _density = value;
                OnPropertyChanged(() => Density);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }


        /// <summary>
        /// Теплота сгорания низшая, МДж/м3 
        /// </summary>
        public double? CombHeatLow
        {
            get { return _combHeatLow; }
            set
            {
                _combHeatLow = value;
                OnPropertyChanged(() => CombHeatLow);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }




        protected override bool OnSaveCommandCanExecute()
        {
            ValidateAll();
            return !HasErrors;
        }
        

        private void SetValidationRules()
        {
            AddValidationFor(() => TestDate)
                .When(() => TestDate < TestDateRangeStart || TestDate > TestDateRangeEnd)
                .Show(
                    $"Недопустимое время. Должно быть в интервале между {TestDateRangeStart:dd.MM.yyyy HH:mm} и {TestDateRangeEnd:dd.MM.yyyy HH:mm}");

            AddValidationFor(() => DewPoint)
                .When(
                    () => DewPoint.HasValue && ValueRangeHelper.TemperatureRange.IsOutsideRange(DewPoint ?? Temperature.Zero))
                   
                .Show(ValueRangeHelper.TemperatureRange.ViolationMessage);

            AddValidationFor(() => DewPointHydrocarbon)
                .When(
                    () => DewPointHydrocarbon.HasValue && ValueRangeHelper.TemperatureRange.IsOutsideRange(DewPointHydrocarbon ?? Temperature.Zero))
                    .Show(ValueRangeHelper.TemperatureRange.ViolationMessage);



            AddValidationFor(() => ContentNitrogen)
                .When(
                    () =>
                        ContentNitrogen < ValueRangeHelper.ContentRange.Min ||
                        ContentNitrogen > ValueRangeHelper.ContentRange.Max)
                .Show(ValueRangeHelper.ContentRange.ViolationMessage);


            AddValidationFor(() => ContentCarbonDioxid)
                .When(
                    () =>
                        ContentCarbonDioxid < ValueRangeHelper.ContentRange.Min ||
                        ContentCarbonDioxid > ValueRangeHelper.ContentRange.Max)
                .Show(ValueRangeHelper.ContentRange.ViolationMessage);



            AddValidationFor(() => ConcentrationSourSulfur)
                .When(
                    () =>
                        ConcentrationSourSulfur < ValueRangeHelper.ConcentrationRange.Min ||
                        ConcentrationSourSulfur > ValueRangeHelper.ConcentrationRange.Max)
                .Show(ValueRangeHelper.ConcentrationRange.ViolationMessage);

            AddValidationFor(() => ConcentrationHydrogenSulfide)
                .When(
                    () =>
                        ConcentrationHydrogenSulfide < ValueRangeHelper.ConcentrationRange.Min ||
                        ConcentrationHydrogenSulfide > ValueRangeHelper.ConcentrationRange.Max)
                .Show(ValueRangeHelper.ConcentrationRange.ViolationMessage);




            AddValidationFor(() => Density)
                .When(
                    () =>
                        Density < ValueRangeHelper.DensityRange.Min ||
                        Density > ValueRangeHelper.DensityRange.Max)
                .Show(ValueRangeHelper.DensityRange.ViolationMessage);

            AddValidationFor(() => CombHeatLow)
                .When(
                    () =>
                        CombHeatLow < ValueRangeHelper.CombHeatJouleRange.Min ||
                        CombHeatLow > ValueRangeHelper.CombHeatJouleRange.Max)
                .Show(ValueRangeHelper.CombHeatJouleRange.ViolationMessage);
            


        }
        

        protected override string CaptionEntityTypeName
        {
            get { return " результата химического анализа"; }
        }


        
        protected override async void CreateNew()
        {
            var testId = await new ManualInputServiceProxy().AddChemicalTestAsync(
                new AddChemicalTestParameterSet
                {
                    MeasPointId = _measPointId,
                    TestDate = TestDate.ToLocal(),
                    DewPoint = DewPoint?.Celsius,
                    DewPointHydrocarbon = DewPointHydrocarbon?.Celsius,
                    ContentNitrogen = ContentNitrogen,
                    ConcentrSourSulfur = ConcentrationSourSulfur,
                    ConcentrHydrogenSulfide = ConcentrationHydrogenSulfide,
                    ContentCarbonDioxid = ContentCarbonDioxid,
                    Density = Density,
                    CombHeatLowJoule = CombHeatLow
                });
            GasCostImportHelper.SaveChemicalAnalysisCosts(TestDate, _measPointId, testId);
            DialogResult = true;
        }

        protected override Task UpdateTask
        {
            get
            {
                return new ManualInputServiceProxy().EditChemicalTestAsync(
                    new EditChemicalTestParameterSet
                    {
                        ChemicalTestId = _testId,
                        TestDate = TestDate.ToLocal(),
                        DewPoint = DewPoint?.Celsius,
                        DewPointHydrocarbon = DewPointHydrocarbon?.Celsius,
                        ContentNitrogen = ContentNitrogen,
                        ConcentrSourSulfur = ConcentrationSourSulfur,
                        ConcentrHydrogenSulfide = ConcentrationHydrogenSulfide,
                        ContentCarbonDioxid = ContentCarbonDioxid,
                        Density = Density,
                        CombHeatLowJoule = CombHeatLow
                    });
            }
        }
        
    }
    
}
