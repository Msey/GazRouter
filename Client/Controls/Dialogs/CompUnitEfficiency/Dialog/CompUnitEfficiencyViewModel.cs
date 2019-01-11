using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Application;
using GazRouter.Application.Helpers;
using GazRouter.Common.ViewModel;
using GazRouter.Controls.Dialogs.CompUnitEfficiency.Charts;
using GazRouter.Controls.Dialogs.CompUnitEfficiency.Model;
using GazRouter.DataProviders.ManualInput;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DataProviders.SeriesData;
using GazRouter.DTO;
using GazRouter.DTO.Dictionaries.CompUnitTypes;
using GazRouter.DTO.Dictionaries.EngineClasses;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.Dictionaries.SuperchargerTypes;
using GazRouter.DTO.ManualInput.CompUnitTests;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.CompUnits;
using GazRouter.DTO.SeriesData.CompUnitPropertyValues;
using GazRouter.DTO.SeriesData.PropertyValues;
using Microsoft.Practices.Prism;
using Telerik.Windows.Controls;
using Utils.Extensions;
using Utils.Units;
using Pressure = Utils.Units.Pressure;

namespace GazRouter.Controls.Dialogs.CompUnitEfficiency.Dialog
{
    public class CompUnitEfficiencyViewModel : DialogViewModel
    {
        #region Constuctor

        public CompUnitEfficiencyViewModel(CommonEntityDTO selectedEntity, DateTime period)
            : base(() => { })
        {
            _unitModel = new CompUnitModel();
            _unitModel.Measurings.TemperatureAir = Temperature.FromCelsius(0);

            _selectedEntity = selectedEntity;
            _date = period == default(DateTime)?SeriesHelper.GetLastCompletedSession():period;
            
            SetValidationRules();

            if (CompUnitType != null && IsTurbine && CompUnitType.CompUnitKtAirs.Count > 0)
                _unitModel.CoefTAir =
                    CompUnitType.CompUnitKtAirs.Single(t => t.TMin < TemperatureAir.Celsius && t.TMax > TemperatureAir.Celsius).KtValue;

            ResultList = new List<ResultRow>();
            

            RefreshModel();
            
        }

        #endregion


        



        #region Methods

        


        private async void RefreshModel()
        {
            if (SelectedEntity == null) return;

            _compUnitDto = await new ObjectModelServiceProxy().GetCompUnitByIdAsync(SelectedEntity.Id);
            OnPropertyChanged(() => SuperchargerType);

            var compUnitTests =
                await new ManualInputServiceProxy().GetCompUnitTestListAsync(new GetCompUnitTestListParameterSet
                {
                    CompUnitId = _compUnitDto.Id
                });
            CompUnitTest = compUnitTests.Any() ? compUnitTests.First() : null;

            OnPropertyChanged(() => CompUnitTest);
            OnPropertyChanged(() => IsCompUnitTest);
            OnPropertyChanged(() => CompUnitTestDate);

            if (CompUnitTest != null)
            {
                _unitModel.CompRatioPoints = CompUnitTest.ChartPoints.Where(
                    p => p.LineType == 1).Select(p => new Point {X = p.X, Y = p.Y}).ToList();

                _unitModel.PolytrEfficiencyPoints = CompUnitTest.ChartPoints.Where(
                        p => p.LineType == 2).Select(p => new Point { X = p.X, Y = p.Y }).ToList();

                _unitModel.PowerInPoints = CompUnitTest.ChartPoints.Where(
                        p => p.LineType == 3).Select(p => new Point { X = p.X, Y = p.Y }).ToList();
            }

            else
            {
                _unitModel.CompRatioPoints = SuperchargerType.ChartPoints.Where(
                    p => p.LineType == 1).Select(p => new Point { X = p.X, Y = p.Y }).ToList();
                

                _unitModel.PolytrEfficiencyPoints = SuperchargerType.ChartPoints.Where(
                        p => p.LineType == 2).Select(p => new Point { X = p.X, Y = p.Y }).ToList();
                

                _unitModel.PowerInPoints = SuperchargerType.ChartPoints.Where(
                        p => p.LineType == 3).Select(p => new Point { X = p.X, Y = p.Y }).ToList();
                
            }

            _unitModel.CompRatioPoints.Sort();
            _unitModel.PolytrEfficiencyPoints.Sort();
            _unitModel.PowerInPoints.Sort();

            if (_unitModel.CompRatioPoints.Count > 0)
            {
                _unitModel.Qmin = _unitModel.CompRatioPoints[0].X;
                _unitModel.Qmax = _unitModel.CompRatioPoints[_unitModel.CompRatioPoints.Count - 1].X;
            }
            else
            {
                RadWindow.Alert(new DialogParameters
                {
                    Header = "Ошибка",
                    Content = "Газодинамические характеристики отсутствуют"
                });
            }

            if (SuperchargerType.RpmRated.HasValue)
                _unitModel.RpmRated = SuperchargerType.RpmRated.Value;

            if (SuperchargerType.NCbnRated.HasValue)
                _unitModel.PolytropicEfficiencyRated = SuperchargerType.NCbnRated.Value;

            if (CompUnitType.KTechStateFuel.HasValue)
                _unitModel.CoefTechStateFuelRated = CompUnitType.KTechStateFuel.Value;

            if (SuperchargerType.KaRated.HasValue)
                _unitModel.KaRated = SuperchargerType.KaRated.Value;

            _unitModel.CoefTechStateByPowerRated = CompUnitType.KTechStatePow;
            _unitModel.NGtuRated = CompUnitType.RatedEfficiency;
            _unitModel.PowerRated = CompUnitType.RatedPower;

            _unitModel.MotorisierteEfficiencyFactor = CompUnitType.MotorisierteEfficiencyFactor;
            _unitModel.ReducerEfficiencyFactor = CompUnitType.ReducerEfficiencyFactor;

            OnPropertyChanged(() => CanCalculate);

            RefreshMeasurings();

        }


        private async void RefreshMeasurings()
        {
            if (!IsUnitSelected) return;

            try
            {
                Behavior.TryLock();

                var values = await new SeriesDataServiceProxy().GetCompUnitPropertyValueListAsync(
                    new GetCompUnitPropertyValuesParameterSet
                    {
                        CompUnitId = _compUnitDto.Id,
                        Timestamp = Date
                    });

                _unitModel.Measurings.PIn = Pressure.FromKgh(values.GetOrDefault(PropertyType.PressureSuperchargerInlet)?.Value ?? 0);
                OnPropertyChanged(() => PressureSuperchargerInlet);

                _unitModel.Measurings.POut = Pressure.FromKgh(values.GetOrDefault(PropertyType.PressureSuperchargerOutlet)?.Value ?? 0);
                OnPropertyChanged(() => PressureSuperchargerOutlet);
                OnPropertyChanged(() => CompressionRatio);

                _unitModel.Measurings.TIn = Temperature.FromCelsius(values.GetOrDefault(PropertyType.TemperatureSuperchargerInlet)?.Value ?? 0);
                OnPropertyChanged(() => TemperatureSuperchargerInlet);

                _unitModel.Measurings.TOut = Temperature.FromCelsius(values.GetOrDefault(PropertyType.TemperatureSuperchargerOutlet)?.Value ?? 0);
                OnPropertyChanged(() => TemperatureSuperchargerOutlet);

                _unitModel.Measurings.FuelGasConsumptionMeasured = values.GetOrDefault(PropertyType.FuelGasConsumption)?.Value ?? 0;
                OnPropertyChanged(() => FuelGasConsumption);

                _unitModel.Measurings.PumpingMeasured = values.GetOrDefault(PropertyType.Pumping)?.Value ?? 0;
                OnPropertyChanged(() => Pumping);

                _unitModel.Measurings.Rpm = values.GetOrDefault(PropertyType.RpmSupercharger)?.Value ?? 0;
                OnPropertyChanged(() => RpmSupercharger);


                _unitModel.Measurings.CombustionHeatLow =
                    CombustionHeat.FromKCal(values.GetOrDefault(PropertyType.CombustionHeatLow)?.Value ?? 0);
                OnPropertyChanged(() => CombHeatLow);

                _unitModel.Measurings.Density = Utils.Units.Density.FromKilogramsPerCubicMeter(values.GetOrDefault(PropertyType.Density)?.Value ?? 0);
                OnPropertyChanged(() => Density);
                

                _unitModel.Measurings.PressureAir = Pressure.FromMmHg(values.GetOrDefault(PropertyType.PressureAir)?.Value ?? 0);
                OnPropertyChanged(() => PressureAir);

                _unitModel.Measurings.TemperatureAir = Temperature.FromCelsius(values.GetOrDefault(PropertyType.TemperatureAir)?.Value ?? 0);
                OnPropertyChanged(() => TemperatureAir);

                OnPropertyChanged(() => IsTurbine);
                OnPropertyChanged(() => IsMotorisierte);
                OnPropertyChanged(() => CanCalculate);

                Calculate();

            }
            finally 
            {
                Behavior.TryUnlock();
            }
        }

        
        private static double ExtractValue(Dictionary<PropertyType, PropertyValueDoubleDTO> valueDict, PropertyType propType)
        {
            var prop = valueDict.GetValueOrNull(propType);
            return prop?.Value ?? 0;
        }
        


        private void Calculate()
        {
            ValidateAll();


            if (!CanCalculate) return;

            var res = false;
            if (IsTurbine)
            {
                res = _unitModel.CalculateTurbine();
            }

            if (IsMotorisierte)
            {
                res = _unitModel.CalculateMotorieserte();
            }

            OnPropertyChanged(() => SuperchargerConditionFactor);
            OnPropertyChanged(() => FuelConditionFactor);
            OnPropertyChanged(() => PowerConditionFactor);
            OnPropertyChanged(() => PumpingCalculated);
            OnPropertyChanged(() => PumpingCommercial);
            OnPropertyChanged(() => FuelGasConsumptionCalculated);
            OnPropertyChanged(() => UnitEfficiencyFactor);
            OnPropertyChanged(() => TurbineEfficiencyFactor);
            OnPropertyChanged(() => SuperchargerEfficiencyFactor);
            OnPropertyChanged(() => UnitAvailablePower);
            OnPropertyChanged(() => UnitEffectivePower);
            OnPropertyChanged(() => SuperchargerPower);
            OnPropertyChanged(() => Usage);
            OnPropertyChanged(() => SurgeDistance);


            ResultList = new List<ResultRow>
            {
                new ResultRow("Q объемная ЦБН", "м³/мин", _unitModel.Results.PumpingCalculated, "n3"),
                new ResultRow("Q коммерческая ЦБН", "млн.м³/сут.", _unitModel.Results.PumpingCommercial, "n6"),
                new ResultRow("Эффективная мощность ГПА", "кВт", _unitModel.Results.PowerEffective, "n0"),
                new ResultRow("Внутренняя мощность ЦБН", "кВт", _unitModel.Results.PowerIn, "n0"),
                new ResultRow("Политропный КПД ЦБН", "", _unitModel.Results.PolytropicEfficiency, "n3"),
                new ResultRow("КПД ГПА", "", _unitModel.Results.NGgpa, "n3"),
                new ResultRow("Коэф. тех. сост. ЦБН", "", _unitModel.Results.CoefTechState, "n3"),
                new ResultRow("Удаленность от границ помпажа", "%", _unitModel.Results.Surging, "n0")
            };

            if (IsTurbine)
                ResultList.AddRange(new List<ResultRow>
                {
                    new ResultRow("Q топливного газа", "тыс.м³/ч", _unitModel.Results.FuelGasConsumptionCalculated, "n3"),
                    new ResultRow("Располагаемая мощность ГТУ", "кВт", _unitModel.Results.PowerMax, "n0"),
                    new ResultRow("Эффективный КПД ГТУ", "", _unitModel.Results.Efficiency, "n3"),
                    new ResultRow("Коэф. тех. сост. ГТУ по мощности", "", _unitModel.Results.CoefTechStateByPower, "n3"),
                    new ResultRow("Коэф. тех. сост. ГТУ по топл. газу", "", _unitModel.Results.CoefTechStateByFuelGas,
                        "n3"),
                    new ResultRow("Коэф. загрузки", "", _unitModel.Results.CoefficientCharge, "n3"),
                });

            OnPropertyChanged(() => ResultList);


            OnPropertyChanged(() => CanCalculate);

            CreateMainChart();

            if (res)
            {
                CreateCompRatioChart();
                CreateKpdChart();
                CreateCompUnitPowerChart();

                if (IsTurbine)
                    CreatePowerFuelGasConsimptionChart();
            }
            else
            {
                CompRatioChartControl.Series.Clear();
                KpdChartControl.Series.Clear();
                CompUnitPowerChartControl.Series.Clear();
                PowerFuelGasConsimptionChartControl.Series.Clear();
            }
        }

        #region Create Charts

        private void CreateMainChart()
        {
            var lineList = ChartHelper.CreateMainChartSource(_unitModel);

            MainChartControl.Series.Clear();
            MainChartControl.Series.AddRange(lineList.Select(l => l.Series));

            #region Отцентровка графика

            var minX = lineList.SelectMany(line => line.Points).Min(p => p.Pumping);
            var maxX = lineList.SelectMany(line => line.Points).Max(p => p.Pumping);
            if (minX.HasValue && maxX.HasValue)
            {
                var tmp = (maxX - minX)/5;

                MainMinimumX = Math.Round((double) (minX - tmp), 1) > 0 ? Math.Round((double) (minX - tmp), 1) : 0;
                MainMaximumX = Math.Round((double) (maxX + tmp), 1);
            }

            var minY = lineList.SelectMany(line => line.Points).Min(p => p.CompressionRatio);
            var maxY = lineList.SelectMany(line => line.Points).Max(p => p.CompressionRatio);
            if (minY.HasValue && maxY.HasValue)
            {
                var tmp = (maxY - minY)/10;

                MainMinimumY = Math.Round((double) (minY - tmp), 1) > 1 ? Math.Round((double) (minY - tmp), 1) : 1;
                MainMaximumY = Math.Round((double) (maxY + tmp), 1);
            }

            #endregion
        }


        private void CreateCompRatioChart()
        {
            var lineList = ChartHelper.CreateCompRatioChartSource(_unitModel);
            CompRatioChartControl.Series.Clear();

            if (lineList[1].Points[0].Pumping.HasValue && lineList[1].Points[0].CompressionRatio.HasValue)
            {
                CompRatioChartControl.Series.AddRange(lineList.Select(l => l.Series));

                #region Отцентровка графика

                var minX = lineList.SelectMany(line => line.Points).Min(p => p.Pumping);
                var maxX = lineList.SelectMany(line => line.Points).Max(p => p.Pumping);
                //     ChartHelper.SmoothRange(ref minX, ref maxX);
                if (minX.HasValue && maxX.HasValue)
                {
                    var tmp = (maxX - minX)/5;

                    CompRatioMinimumX = Math.Round((double) (minX - tmp), 1) > 0
                        ? Math.Round((double) (minX - tmp), 1)
                        : 0;
                    CompRatioMaximumX = Math.Round((double) (maxX + tmp), 1);
                }

                var minY = lineList.SelectMany(line => line.Points).Min(p => p.CompressionRatio);
                var maxY = lineList.SelectMany(line => line.Points).Max(p => p.CompressionRatio);
                //        ChartHelper.SmoothRange(ref minY, ref maxY);
                if (minY.HasValue && maxY.HasValue)
                {
                    var tmp = (maxY - minY)/10;

                    CompRatioMinimumY = Math.Round((double) (minY - tmp), 1) > 1 &&
                                        Math.Round((double) (minY - tmp), 1) < minY
                        ? Math.Round((double) (minY - tmp), 1)
                        : 1;
                    CompRatioMaximumY = Math.Round((double) (maxY + tmp), 1);
                }

                #endregion
            }
        }

        private void CreateKpdChart()
        {
            var lineList = ChartHelper.CreateEfficiencyChartSource(_unitModel);
            KpdChartControl.Series.Clear();

            if (lineList[1].Points[0].Pumping.HasValue && lineList[1].Points[0].Efficiency.HasValue)
            {
                KpdChartControl.Series.AddRange(lineList.Select(l => l.Series));

                #region Отцентровка графика

                var minX = lineList.SelectMany(line => line.Points).Min(p => p.Pumping);
                var maxX = lineList.SelectMany(line => line.Points).Max(p => p.Pumping);
                // ChartHelper.SmoothRange(ref minX,ref  maxX);
                if (minX.HasValue && maxX.HasValue)
                {
                    var tmp = (maxX - minX)/5;

                    KpdMinimumX = Math.Round((double) (minX - tmp), 1) > 0 ? Math.Round((double) (minX - tmp), 1) : 0;
                    KpdMaximumX = Math.Round((double) (maxX + tmp), 1);
                }

                var minY = lineList.SelectMany(line => line.Points).Min(p => p.Efficiency);
                var maxY = lineList.SelectMany(line => line.Points).Max(p => p.Efficiency);
                //  ChartHelper.SmoothRange(ref minY, ref  maxY);
                if (minY.HasValue && maxY.HasValue)
                {
                    var tmp = (maxY - minY)/10;

                    KpdMinimumY = Math.Round((double) (minY - tmp), 1) > 0 &&
                                  Math.Round((double) (minY - tmp), 1) < minY
                        ? Math.Round((double) (minY - tmp), 1)
                        : 0;
                    KpdMaximumY = Math.Round((double) (maxY + tmp), 1);
                }

                #endregion
            }
        }

        private void CreateCompUnitPowerChart()
        {
            var lineList = ChartHelper.CreateCompUnitPowerChartSource(_unitModel);
            CompUnitPowerChartControl.Series.Clear();

            if (lineList.Any())
                if (lineList[1].Points.Any() && lineList[1].Points[0].Pumping.HasValue &&
                    lineList[1].Points[0].Power.HasValue)
                {
                    if (lineList[1].Points[0].Pumping.HasValue)
                        CompUnitPowerChartControl.Series.AddRange(lineList.Select(l => l.Series));

                    #region Отцентровка графика

                    var minX = lineList.SelectMany(line => line.Points).Min(p => p.Pumping);
                    var maxX = lineList.SelectMany(line => line.Points).Max(p => p.Pumping);
                    //     ChartHelper.SmoothRange(ref minX,ref  maxX);
                    if (minX.HasValue && maxX.HasValue)
                    {
                        var tmp = (maxX - minX)/5;

                        CompUnitPowerMinimumX = Math.Round((double) (minX - tmp), 1) > 0
                            ? Math.Round((double) (minX - tmp), 1)
                            : 0;
                        CompUnitPowerMaximumX = Math.Round((double) (maxX + tmp), 1);
                    }

                    var minY = lineList.SelectMany(line => line.Points).Min(p => p.Power);
                    var maxY = lineList.SelectMany(line => line.Points).Max(p => p.Power);
                    //     ChartHelper.SmoothRange(ref minY, ref maxY);
                    if (minY.HasValue && maxY.HasValue)
                    {
                        var tmp = (maxY - minY)/10;

                        CompUnitPowerMinimumY = Math.Round((double) (minY - tmp), 1) > 0
                            ? Math.Round((double) (minY - tmp), 1)
                            : 0;
                        CompUnitPowerMaximumY = Math.Round((double) (maxY + tmp), 1);
                    }

                    #endregion
                }
        }

        private void CreatePowerFuelGasConsimptionChart()
        {
            var lineList = ChartHelper.CreatePowerFuelGasConsimptionChartSource(_unitModel);
            PowerFuelGasConsimptionChartControl.Series.Clear();
            if (lineList.Any())
                if (lineList[1].Points[0].Power.HasValue && lineList[1].Points[0].Pumping.HasValue)
                {
                    PowerFuelGasConsimptionChartControl.Series.AddRange(lineList.Select(l => l.Series));

                    #region Отцентровка графика

                    var minX = lineList.SelectMany(line => line.Points).Min(p => p.Power);
                    var maxX = lineList.SelectMany(line => line.Points).Max(p => p.Power);
                    //   ChartHelper.SmoothRange(ref minX, ref  maxX);
                    if (minX.HasValue && maxX.HasValue)
                    {
                        var tmp = (maxX - minX)/5;

                        PowerFuelGasConsimptionMinimumX = Math.Round((double) (minX - tmp), 1) > 0
                            ? Math.Round((double) (minX - tmp), 1)
                            : 0;
                        PowerFuelGasConsimptionMaximumX = Math.Round((double) (maxX + tmp), 1);
                    }

                    var minY = lineList.SelectMany(line => line.Points).Min(p => p.Pumping);
                    var maxY = lineList.SelectMany(line => line.Points).Max(p => p.Pumping);
                    //    ChartHelper.SmoothRange(ref minY, ref maxY);
                    if (minY.HasValue && maxY.HasValue)
                    {
                        var tmp = (maxY - minY)/10;

                        PowerFuelGasConsimptionMinimumY = Math.Round((double) (minY - tmp), 1) > 0
                            ? Math.Round((double) (minY - tmp), 1)
                            : 0;
                        PowerFuelGasConsimptionMaximumY = Math.Round((double) (maxY + tmp), 1);
                    }

                    #endregion
                }
        }

        #endregion

        #endregion

        #region Properties

        private CommonEntityDTO _selectedEntity;
        private CompUnitDTO _compUnitDto;
        private CompUnitModel _unitModel;

        public CommonEntityDTO SelectedEntity
        {
            get { return _selectedEntity; }
            set
            {
                if (SetProperty(ref _selectedEntity, value))
                    RefreshModel();
                //    RefreshMeasurings();
            }
        }
        
        
        private DateTime _date;
        public DateTime Date
        {
            get { return _date; }
            set
            {
                if (SetProperty(ref _date, value))
                    RefreshMeasurings();
            }
        }

        

        /// <summary>
        /// Выбран ли ГПА
        /// </summary>
        public bool IsUnitSelected
        {
            get { return SelectedEntity != null; }
        }

        /// <summary>
        /// ГПА с газотурбинным приводом?
        /// </summary>
        public bool IsTurbine
        {
            get
            {
                if (CompUnitType == null) return false;
                return CompUnitType.EngineClassId == EngineClass.Turbine;
            }
        }

        /// <summary>
        /// ГПА 
        /// </summary>
        public bool IsMotorisierte
        {
            get
            {
                if (CompUnitType == null) return false;
                return CompUnitType.EngineClassId == EngineClass.Motorisierte;
            }
        }

        public List<EntityType> AllowedType
        {
            get { return new List<EntityType> {EntityType.CompUnit}; }
        }

        #region Charts

        public RadCartesianChart MainChartControl { get; set; }
        public RadCartesianChart CompRatioChartControl { get; set; }
        public RadCartesianChart KpdChartControl { get; set; }
        public RadCartesianChart CompUnitPowerChartControl { get; set; }
        public RadCartesianChart PowerFuelGasConsimptionChartControl { get; set; }

        private List<ChartLine> _mainChartLines = new List<ChartLine>();

        public List<ChartLine> MainChartLines
        {
            get { return _mainChartLines; }
            set
            {
                _mainChartLines = value;
                OnPropertyChanged(() => MainChartLines);
            }
        }

        #region Max and min Charts

        private double _mainMinimumX;

        public double MainMinimumX
        {
            get { return _mainMinimumX; }
            set
            {
                if (_mainMinimumX == value) return;
                _mainMinimumX = value;
                OnPropertyChanged(() => MainMinimumX);
            }
        }

        private double _compRatioMinimumX;

        public double CompRatioMinimumX
        {
            get { return _compRatioMinimumX; }
            set
            {
                if (_compRatioMinimumX == value) return;
                _compRatioMinimumX = value;
                OnPropertyChanged(() => CompRatioMinimumX);
            }
        }

        private double _kpdMinimumX;

        public double KpdMinimumX
        {
            get { return _kpdMinimumX; }
            set
            {
                if (_kpdMinimumX == value) return;
                _kpdMinimumX = value;
                OnPropertyChanged(() => KpdMinimumX);
            }
        }

        private double _compUnitPowerMinimumX;

        public double CompUnitPowerMinimumX
        {
            get { return _compUnitPowerMinimumX; }
            set
            {
                if (_compUnitPowerMinimumX == value) return;
                _compUnitPowerMinimumX = value;
                OnPropertyChanged(() => CompUnitPowerMinimumX);
            }
        }

        private double _powerFuelGasConsimptionMinimumX;

        public double PowerFuelGasConsimptionMinimumX
        {
            get { return _powerFuelGasConsimptionMinimumX; }
            set
            {
                if (_powerFuelGasConsimptionMinimumX == value) return;
                _powerFuelGasConsimptionMinimumX = value;
                OnPropertyChanged(() => PowerFuelGasConsimptionMinimumX);
            }
        }


        private double _mainMaximumX;

        public double MainMaximumX
        {
            get { return _mainMaximumX; }
            set
            {
                if (_mainMaximumX == value) return;
                _mainMaximumX = value;
                OnPropertyChanged(() => MainMaximumX);
            }
        }

        private double _compRatioMaximumX;

        public double CompRatioMaximumX
        {
            get { return _compRatioMaximumX; }
            set
            {
                if (_compRatioMaximumX == value) return;
                _compRatioMaximumX = value;
                OnPropertyChanged(() => CompRatioMaximumX);
            }
        }

        private double _kpdMaximumX;

        public double KpdMaximumX
        {
            get { return _kpdMaximumX; }
            set
            {
                if (_kpdMaximumX == value) return;
                _kpdMaximumX = value;
                OnPropertyChanged(() => KpdMaximumX);
            }
        }

        private double _compUnitPowerMaximumX;

        public double CompUnitPowerMaximumX
        {
            get { return _compUnitPowerMaximumX; }
            set
            {
                if (_compUnitPowerMaximumX == value) return;
                _compUnitPowerMaximumX = value;
                OnPropertyChanged(() => CompUnitPowerMaximumX);
            }
        }

        private double _powerFuelGasConsimptionMaximumX;

        public double PowerFuelGasConsimptionMaximumX
        {
            get { return _powerFuelGasConsimptionMaximumX; }
            set
            {
                if (_powerFuelGasConsimptionMaximumX == value) return;
                _powerFuelGasConsimptionMaximumX = value;
                OnPropertyChanged(() => PowerFuelGasConsimptionMaximumX);
            }
        }


        private double _mainMinimumY;

        public double MainMinimumY
        {
            get { return _mainMinimumY; }
            set
            {
                if (_mainMinimumY == value) return;
                _mainMinimumY = value;
                OnPropertyChanged(() => MainMinimumY);
            }
        }

        private double _compRatioMinimumY;

        public double CompRatioMinimumY
        {
            get { return _compRatioMinimumY; }
            set
            {
                if (_compRatioMinimumY == value) return;
                _compRatioMinimumY = value;
                OnPropertyChanged(() => CompRatioMinimumY);
            }
        }

        private double _kpdMinimumY;

        public double KpdMinimumY
        {
            get { return _kpdMinimumY; }
            set
            {
                if (_kpdMinimumY == value) return;
                _kpdMinimumY = value;
                OnPropertyChanged(() => KpdMinimumY);
            }
        }

        private double _compUnitPowerMinimumY;

        public double CompUnitPowerMinimumY
        {
            get { return _compUnitPowerMinimumY; }
            set
            {
                if (_compUnitPowerMinimumY == value) return;
                _compUnitPowerMinimumY = value;
                OnPropertyChanged(() => CompUnitPowerMinimumY);
            }
        }

        private double _powerFuelGasConsimptionMinimumY;

        public double PowerFuelGasConsimptionMinimumY
        {
            get { return _powerFuelGasConsimptionMinimumY; }
            set
            {
                if (_powerFuelGasConsimptionMinimumY == value) return;
                _powerFuelGasConsimptionMinimumY = value;
                OnPropertyChanged(() => PowerFuelGasConsimptionMinimumY);
            }
        }


        private double _mainMaximumY;

        public double MainMaximumY
        {
            get { return _mainMaximumY; }
            set
            {
                if (_mainMaximumY == value) return;
                _mainMaximumY = value;
                OnPropertyChanged(() => MainMaximumY);
            }
        }

        private double _compRatioMaximumY;

        public double CompRatioMaximumY
        {
            get { return _compRatioMaximumY; }
            set
            {
                if (_compRatioMaximumY == value) return;
                _compRatioMaximumY = value;
                OnPropertyChanged(() => CompRatioMaximumY);
            }
        }

        private double _kpdMaximumY;

        public double KpdMaximumY
        {
            get { return _kpdMaximumY; }
            set
            {
                if (_kpdMaximumY == value) return;
                _kpdMaximumY = value;
                OnPropertyChanged(() => KpdMaximumY);
            }
        }

        private double _compUnitPowerMaximumY;

        public double CompUnitPowerMaximumY
        {
            get { return _compUnitPowerMaximumY; }
            set
            {
                if (_compUnitPowerMaximumY == value) return;
                _compUnitPowerMaximumY = value;
                OnPropertyChanged(() => CompUnitPowerMaximumY);
            }
        }

        private double _powerFuelGasConsimptionMaximumY;

        public double PowerFuelGasConsimptionMaximumY
        {
            get { return _powerFuelGasConsimptionMaximumY; }
            set
            {
                if (_powerFuelGasConsimptionMaximumY == value) return;
                _powerFuelGasConsimptionMaximumY = value;
                OnPropertyChanged(() => PowerFuelGasConsimptionMaximumY);
            }
        }

        #endregion

        #endregion

        #endregion

        /// <summary>
        /// Возможно ли выполнить расчет
        /// </summary>
        public bool CanCalculate
        {
            get
            {
                if (SuperchargerType == null || CompUnitType == null)
                    return false;

                bool canCalculate = CompUnitTest == null
                    ? !HasErrors
                      && SuperchargerType.ChartPoints.Any(p => p.LineType == 1)
                      && SuperchargerType.ChartPoints.Any(p => p.LineType == 2)
                      && SuperchargerType.RpmRated.HasValue
                      && SuperchargerType.KaRated.HasValue
                    : !HasErrors
                      && CompUnitTest.ChartPoints.Any(p => p.LineType == 1)
                      && CompUnitTest.ChartPoints.Any(p => p.LineType == 2)
                      && SuperchargerType.RpmRated.HasValue
                      && SuperchargerType.KaRated.HasValue;


                if (IsTurbine)
                {
                    canCalculate = canCalculate && CompUnitType.RatedPower > 0 &&
                                          CompUnitType.KTechStateFuel.HasValue &&
                                          CompUnitType.RatedEfficiency > 0;
                }

                return canCalculate;
            }
        }

        public string CompUnitTestDate
        {
            get { return IsCompUnitTest ? CompUnitTest.CompUnitTestDate.ToString("dd.MM.yyyy") : ""; }
        }


        public SuperchargerTypeDTO SuperchargerType
        {
            get
            {
                if (_compUnitDto == null) return null;
                return
                    ClientCache.DictionaryRepository.SuperchargerTypes.Single(
                        st => st.Id == _compUnitDto.SuperchargerTypeId);
            }
        }

        public CompUnitTypeDTO CompUnitType
        {
            get
            {
                if (_compUnitDto == null) return null;
                return
                    ClientCache.DictionaryRepository.CompUnitTypes.Single(
                        u => u.Id == _compUnitDto.CompUnitTypeId);
            }
        }

        public CompUnitTestDTO CompUnitTest { get; set; }

        public bool IsCompUnitTest { get { return CompUnitTest != null; } }

        /// <summary>
        /// Давление на входе нагнетателя, кг/см2
        /// </summary>
        public Pressure PressureSuperchargerInlet
        {
            get { return _unitModel.Measurings.PIn; }
            set
            {
                _unitModel.Measurings.PIn = value;
                OnPropertyChanged(() => PressureSuperchargerInlet);
                OnPropertyChanged(() => PressureSuperchargerOutlet);
                OnPropertyChanged(() => CompressionRatio);
                Calculate();
            }
        }


        /// <summary>
        /// Давление на выходе нагнетателя, кг/см2
        /// </summary>
        public Pressure PressureSuperchargerOutlet
        {
            get { return _unitModel.Measurings.POut; }
            set
            {
                _unitModel.Measurings.POut = value;
                OnPropertyChanged(() => PressureSuperchargerInlet);
                OnPropertyChanged(() => PressureSuperchargerOutlet);
                OnPropertyChanged(() => CompressionRatio);
                Calculate();
            }
        }

        public double CompressionRatio
        {
            get
            {
                var pin = PressureSuperchargerInlet.Mpa +
                          PhysicalQuantityConversions.mmHg2Mpa(PressureAir);
                var pout = PressureSuperchargerOutlet.Mpa +
                           PhysicalQuantityConversions.mmHg2Mpa(PressureAir);
                return pin > 0 ? pout/pin : 1;
            }
        }


        /// <summary>
        /// Температура на входе нагнетателя, Гр.С
        /// </summary>
        public Temperature TemperatureSuperchargerInlet
        {
            get { return _unitModel.Measurings.TIn; }
            set
            {
                _unitModel.Measurings.TIn = value;
                OnPropertyChanged(() => TemperatureSuperchargerInlet);
                OnPropertyChanged(() => TemperatureSuperchargerOutlet);
                Calculate();
            }
        }


        /// <summary>
        /// Температура на выходе нагнетателя, Гр.С
        /// </summary>
        public Temperature TemperatureSuperchargerOutlet
        {
            get { return _unitModel.Measurings.TOut; }
            set
            {
                _unitModel.Measurings.TOut = value;
                OnPropertyChanged(() => TemperatureSuperchargerOutlet);
                OnPropertyChanged(() => TemperatureSuperchargerInlet);
                Calculate();
            }
        }


        /// <summary>
        /// Объемная производительность ЦБН, тыс.м3
        /// </summary>
        public double Pumping
        {
            get { return _unitModel.Measurings.PumpingMeasured; }
            set
            {
                _unitModel.Measurings.PumpingMeasured = value;
                OnPropertyChanged(() => Pumping);
                OnPropertyChanged(() => IsPumpingKnown);
                Calculate();
            }
        }

        public bool IsPumpingKnown
        {
            get { return Pumping > 0; }
        }


        /// <summary>
        /// Расход топливного газа, тыс.м3
        /// </summary>
        public double FuelGasConsumption
        {
            get { return _unitModel.Measurings.FuelGasConsumptionMeasured; }
            set
            {
                _unitModel.Measurings.FuelGasConsumptionMeasured = value;
                OnPropertyChanged(() => FuelGasConsumption);
                OnPropertyChanged(() => IsFuelGasConsumptionKnown);
                Calculate();
            }
        }

        public bool IsFuelGasConsumptionKnown
        {
            get { return FuelGasConsumption > 0; }
        }


        /// <summary>
        /// Обороты нагнетателя, об/мин
        /// </summary>
        public double RpmSupercharger
        {
            get { return _unitModel.Measurings.Rpm; }
            set
            {
                _unitModel.Measurings.Rpm = value;
                OnPropertyChanged(() => RpmSupercharger);
                Calculate();
            }
        }


        /// <summary>
        /// Плотность газа, кг/м3
        /// </summary>
        public double Density
        {
            get { return _unitModel.Measurings.Density.KilogramsPerCubicMeter; }
            set
            {
                _unitModel.Measurings.Density = Utils.Units.Density.FromKilogramsPerCubicMeter(value);
                OnPropertyChanged(() => Density);
                Calculate();
            }
        }

        /// <summary>
        /// Низшая теплота сгорания, ккал/м3
        /// </summary>
        public CombustionHeat CombHeatLow
        {
            get { return _unitModel.Measurings.CombustionHeatLow; }
            set
            {
                _unitModel.Measurings.CombustionHeatLow = value;
                OnPropertyChanged(() => CombHeatLow);
                Calculate();
            }
        }


        /// <summary>
        /// Давление атмосферное, мм рт.ст.
        /// </summary>
        public double PressureAir
        {
            get { return _unitModel.Measurings.PressureAir.MmHg; }
            set
            {
                _unitModel.Measurings.PressureAir = Pressure.FromMmHg(value);
                OnPropertyChanged(() => PressureAir);
                OnPropertyChanged(() => CompressionRatio);
                Calculate();
            }
        }


        /// <summary>
        /// Температура воздуха, Гр.С
        /// </summary>
        public Temperature TemperatureAir
        {
            get { return _unitModel.Measurings.TemperatureAir; }
            set
            {
                _unitModel.Measurings.TemperatureAir = value;
                OnPropertyChanged(() => TemperatureAir);
                Calculate();
            }
        }


        private void SetValidationRules()
        {
            AddValidationFor(() => PressureSuperchargerInlet)
                .When(
                    () =>
                        ValueRangeHelper.PressureRange.IsOutsideRange(PressureSuperchargerInlet))
                .Show(ValueRangeHelper.PressureRange.ViolationMessage);

            AddValidationFor(() => PressureSuperchargerOutlet)
                .When(
                    () =>
                        ValueRangeHelper.PressureRange.IsOutsideRange(PressureSuperchargerOutlet))
                .Show(ValueRangeHelper.PressureRange.ViolationMessage);

            AddValidationFor(() => TemperatureSuperchargerInlet)
                .When(
                    () => ValueRangeHelper.TemperatureRange.IsOutsideRange(TemperatureSuperchargerInlet)
                        )
                .Show(ValueRangeHelper.TemperatureRange.ViolationMessage);

            AddValidationFor(() => TemperatureSuperchargerOutlet)
                .When(
                    () => ValueRangeHelper.TemperatureRange.IsOutsideRange(TemperatureSuperchargerOutlet))
                    
                .Show(ValueRangeHelper.TemperatureRange.ViolationMessage);


            AddValidationFor(() => FuelGasConsumption)
                .When(
                    () =>
                        FuelGasConsumption < 0)
                .Show("Недопустимое значение. Должно быть больше 0.");

            AddValidationFor(() => Pumping)
                .When(
                    () =>
                        Pumping < 0)
                .Show("Недопустимое значение. Должно быть больше 0.");


            AddValidationFor(() => RpmSupercharger)
                .When(
                    () =>
                       ValueRangeHelper.RpmRange.IsOutsideRange(RpmSupercharger))
                .Show(ValueRangeHelper.RpmRange.ViolationMessage);


            AddValidationFor(() => Density)
                .When(
                    () =>
                        Density < ValueRangeHelper.DensityRange.Min ||
                        Density > ValueRangeHelper.DensityRange.Max)
                .Show(ValueRangeHelper.DensityRange.ViolationMessage);

            AddValidationFor(() => CombHeatLow)
                .When(
                    () =>
                        ValueRangeHelper.CombHeatRange.IsOutsideRange(CombHeatLow))
                .Show(ValueRangeHelper.CombHeatRange.ViolationMessage);

            AddValidationFor(() => PressureAir)
                .When(
                    () =>
                        PressureAir < ValueRangeHelper.PressureAirRange.Min ||
                        PressureAir > ValueRangeHelper.PressureAirRange.Max)
                .Show(ValueRangeHelper.PressureAirRange.ViolationMessage);

            AddValidationFor(() => TemperatureAir)
                .When(
                    () => ValueRangeHelper.TemperatureAirRange.IsOutsideRange(TemperatureAir))
                       .Show(ValueRangeHelper.TemperatureAirRange.ViolationMessage);


            AddValidationFor(() => PressureSuperchargerInlet)
                .When(
                    () =>
                        PressureSuperchargerInlet > PressureSuperchargerOutlet)
                .Show("Давление перед ЦБН\nдолжно быть меньше\nдавления за ЦБН");

            AddValidationFor(() => PressureSuperchargerOutlet)
                .When(
                    () =>
                        PressureSuperchargerInlet > PressureSuperchargerOutlet)
                .Show("Давление за ЦБН\nдолжно быть больше\nдавления перед ЦБН");

            AddValidationFor(() => TemperatureSuperchargerInlet)
                .When(
                    () =>
                        TemperatureSuperchargerInlet > TemperatureSuperchargerOutlet)
                .Show("Температура перед ЦБН\nдолжна быть меньше\nтемпературы за ЦБН");

            AddValidationFor(() => TemperatureSuperchargerOutlet)
                .When(
                    () =>
                        TemperatureSuperchargerInlet > TemperatureSuperchargerOutlet)
                .Show("Температура за ЦБН\nдолжна быть больше\nтемпературы перед ЦБН");
        }


        /// <summary>
        /// Коэф. техн. состояния ЦБН
        /// </summary>
        public double? SuperchargerConditionFactor
        {
            get { return _unitModel.Results.CoefTechState; }
        }


        /// <summary>
        /// Коэф. техн. состояния ГТУ по топливному газу
        /// </summary>
        public double? FuelConditionFactor
        {
            get { return _unitModel.Results.CoefTechStateByFuelGas; }
        }


        /// <summary>
        /// Коэф. техн. состояния ГТУ по мощности
        /// </summary>
        public double? PowerConditionFactor
        {
            get { return _unitModel.Results.CoefTechStateByPower; }
        }


        /// <summary>
        /// Объемная производительность ЦБН (расчетная), м3/мин
        /// </summary>
        public double? PumpingCalculated
        {
            get { return _unitModel.Results.PumpingCalculated; }
        }


        /// <summary>
        /// Коммерческая производительность ЦБН (расчетная), млн.м3/сут
        /// </summary>
        public double? PumpingCommercial
        {
            get { return _unitModel.Results.PumpingCommercial; }
        }


        /// <summary>
        /// Расход топливного газа (расчетный), тыс.м3
        /// </summary>
        public double? FuelGasConsumptionCalculated
        {
            get { return _unitModel.Results.FuelGasConsumptionCalculated; }
        }


        /// <summary>
        /// КПД ГПА
        /// </summary>
        public double? UnitEfficiencyFactor
        {
            get { return _unitModel.Results.NGgpa; }
        }


        /// <summary>
        /// Эффективный КПД ГТУ
        /// </summary>
        public double? TurbineEfficiencyFactor
        {
            get { return _unitModel.Results.Efficiency; }
        }


        /// <summary>
        /// Политропный КПД ЦБН
        /// </summary>
        public double? SuperchargerEfficiencyFactor
        {
            get { return _unitModel.Results.PolytropicEfficiency; }
        }


        /// <summary>
        /// Располагаемая мощность ГПА, кВт
        /// </summary>
        public double? UnitAvailablePower
        {
            get { return _unitModel.Results.PowerMax; }
        }


        /// <summary>
        /// Эффективная мощность ГПА, кВт
        /// </summary>
        public double? UnitEffectivePower
        {
            get { return _unitModel.Results.PowerEffective; }
        }


        /// <summary>
        /// Внутреняя мощность ЦБН, кВт
        /// </summary>
        public double? SuperchargerPower
        {
            get { return _unitModel.Results.PowerIn; }
        }


        /// <summary>
        /// Коэффициент загруженности
        /// </summary>
        public double? Usage
        {
            get { return _unitModel.Results.CoefficientCharge; }
        }


        /// <summary>
        /// Удаленность от зоны помпажа, %
        /// </summary>
        public double? SurgeDistance
        {
            get { return _unitModel.Results.Surging; }
        }


        public List<ResultRow> ResultList { get; set; }
    }

    public class ResultRow
    {
        public ResultRow(string paramName, string eu, double? value, string format)
        {
            ParamName = paramName;
            EU = eu;
            Value = value.HasValue ? value.Value.ToString(format) : "";
        }

        public string ParamName { get; set; }
        public string EU { get; set; }
        public string Value { get; set; }
    }

    
}