using GazRouter.Common.ViewModel;
using System;
using System.Windows.Media;

namespace GazRouter.ManualInput.Hourly
{
    public class CellColorTemplate : ViewModelBase
    {
        private const int FromSeconds = 3; // - seconds required for setting alpha
        private bool _pressureUpdated;
        public bool PressureUpdated
        {
            get { return _pressureUpdated; }
            set
            {
                if (value)
                    UpdateColor();
                else AlphaChannel = 0;
                _pressureUpdated = value;
            }
        }

        private bool _temperatureAireUpdated;
        public bool TemperatureAirUpdated
        {
            get { return _temperatureAireUpdated; }
            set
            {
                if (value)
                    UpdateColor();
                else AlphaChannel = 0;

                _temperatureAireUpdated = value;
            }
        }

        private bool _temperatureEarthUpdated;
        public bool TemperatureEarthUpdated
        {
            get { return _temperatureEarthUpdated; }
            set
            {
                if (value)
                    UpdateColor();
                else AlphaChannel = 0;
                _temperatureEarthUpdated = value;
            }
        }


        private bool _GroupCountUpdated;
        public bool GroupCountUpdated
        {
            get { return _GroupCountUpdated; }
            set
            {
                if (value)
                    UpdateColor();
                else AlphaChannel = 0;
                _GroupCountUpdated = value;
            }
        }

        private bool _CompressionStageCountUpdated;
        public bool CompressionStageCountUpdated
        {
            get { return _CompressionStageCountUpdated; }
            set
            {
                if (value)
                    UpdateColor();
                else AlphaChannel = 0;
                _CompressionStageCountUpdated = value;
            }
        }
        private bool _PressureInletUpdated;
        public bool PressureInletUpdated
        {
            get { return _PressureInletUpdated; }
            set
            {
                if (value)
                    UpdateColor();
                else AlphaChannel = 0;
                _PressureInletUpdated = value;
            }
        }


        private bool _PressureOutletUpdated;
        public bool PressureOutletUpdated
        {
            get { return _PressureOutletUpdated; }
            set
            {
                if (value)
                    UpdateColor();
                else AlphaChannel = 0;
                _PressureOutletUpdated = value;
            }
        }


        private bool _TemperatureInletUpdated;
        public bool TemperatureInletUpdated
        {
            get { return _TemperatureInletUpdated; }
            set
            {
                if (value)
                    UpdateColor();
                else AlphaChannel = 0;
                _TemperatureInletUpdated = value;
            }
        }

        private bool _TemperatureOutletUpdated;
        public bool TemperatureOutletUpdated
        {
            get { return _TemperatureOutletUpdated; }
            set
            {
                if (value)
                    UpdateColor();
                else AlphaChannel = 0;
                _TemperatureOutletUpdated = value;
            }
        }

        private bool _TemperatureCoolingUpdated;
        public bool TemperatureCoolingUpdated
        {
            get { return _TemperatureCoolingUpdated; }
            set
            {
                if (value)
                    UpdateColor();
                else AlphaChannel = 0;
                _TemperatureCoolingUpdated = value;
            }
        }

        private bool _FuelGasConsumptionUpdated;
        public bool FuelGasConsumptionUpdated
        {
            get { return _FuelGasConsumptionUpdated; }
            set
            {
                if (value)
                    UpdateColor();
                else AlphaChannel = 0;
                _FuelGasConsumptionUpdated = value;
            }
        }

        private bool _PumpingUpdated;
        public bool PumpingUpdated
        {
            get { return _PumpingUpdated; }
            set
            {
                if (value)
                    UpdateColor();
                else AlphaChannel = 0;
                _PumpingUpdated = value;
            }
        }

        private bool _CoolingUnitsInUseUpdated;
        public bool CoolingUnitsInUseUpdated
        {
            get { return _CoolingUnitsInUseUpdated; }
            set
            {
                if (value)
                    UpdateColor();
                else AlphaChannel = 0;
                _CoolingUnitsInUseUpdated = value;
            }
        }

        private bool _CoolingUnitsInReserveUpdated;
        public bool CoolingUnitsInReserveUpdated
        {
            get { return _CoolingUnitsInReserveUpdated; }
            set
            {
                if (value)
                    UpdateColor();
                else AlphaChannel = 0;
                _CoolingUnitsInReserveUpdated = value;
            }
        }

        private bool _CoolingUnitsUnderRepairUpdated;
        public bool CoolingUnitsUnderRepairUpdated
        {
            get { return _CoolingUnitsUnderRepairUpdated; }
            set
            {
                if (value)
                    UpdateColor();
                else AlphaChannel = 0;
                _CoolingUnitsUnderRepairUpdated = value;
            }
        }
        private bool _DustCatchersInUseUpdated;
        public bool DustCatchersInUseUpdated
        {
            get { return _DustCatchersInUseUpdated; }
            set
            {
                if (value)
                    UpdateColor();
                else AlphaChannel = 0;
                _DustCatchersInUseUpdated = value;
            }
        }
        private bool _DustCatchersInReserveUpdated;
        public bool DustCatchersInReserveUpdated
        {
            get { return _DustCatchersInReserveUpdated; }
            set
            {
                if (value)
                    UpdateColor();
                else AlphaChannel = 0;
                _DustCatchersInReserveUpdated = value;
            }
        }
        private bool _DustCatchersUnderRepairUpdated;
        public bool DustCatchersUnderRepairUpdated
        {
            get { return _DustCatchersUnderRepairUpdated; }
            set
            {
                if (value)
                    UpdateColor();
                else AlphaChannel = 0;
                _DustCatchersUnderRepairUpdated = value;
            }
        }

        private bool _PressureSuperchargerInletUpdated;
        public bool PressureSuperchargerInletUpdated
        {
            get { return _PressureSuperchargerInletUpdated; }
            set
            {
                if (value)
                    UpdateColor();
                else AlphaChannel = 0;
                _PressureSuperchargerInletUpdated = value;
            }
        }

        private bool _PressureSuperchargerOutletUpdated;
        public bool PressureSuperchargerOutletUpdated
        {
            get { return _PressureSuperchargerOutletUpdated; }
            set
            {
                if (value)
                    UpdateColor();
                else AlphaChannel = 0;
                _PressureSuperchargerOutletUpdated = value;
            }
        }

        private bool _TemperatureSuperchargerInletUpdated;
        public bool TemperatureSuperchargerInletUpdated
        {
            get { return _TemperatureSuperchargerInletUpdated; }
            set
            {
                if (value)
                    UpdateColor();
                else AlphaChannel = 0;
                _TemperatureSuperchargerInletUpdated = value;
            }
        }

        private bool _TemperatureSuperchargerOutletUpdated;
        public bool TemperatureSuperchargerOutletUpdated
        {
            get { return _TemperatureSuperchargerOutletUpdated; }
            set
            {
                if (value)
                    UpdateColor();
                else AlphaChannel = 0;
                _TemperatureSuperchargerOutletUpdated = value;
            }
        }

        private bool _RpmSuperchargerUpdated;
        public bool RpmSuperchargerUpdated
        {
            get { return _RpmSuperchargerUpdated; }
            set
            {
                if (value)
                    UpdateColor();
                else AlphaChannel = 0;
                _RpmSuperchargerUpdated = value;
            }
        }

        private bool _FlowUpdated;
        public bool FlowUpdated
        {
            get { return _FlowUpdated; }
            set
            {
                if (value)
                    UpdateColor();
                else AlphaChannel = 0;
                _FlowUpdated = value;
            }
        }


        private bool _OpeningPercentageUpdated;
        public bool OpeningPercentageUpdated
        {
            get { return _OpeningPercentageUpdated; }
            set
            {
                if (value)
                    UpdateColor();
                else AlphaChannel = 0;
                _OpeningPercentageUpdated = value;
            }
        }

        private bool _DewPointUpdated;
        public bool DewPointUpdated
        {
            get { return _DewPointUpdated; }
            set
            {
                if (value)
                    UpdateColor();
                else AlphaChannel = 0;
                _DewPointUpdated = value;
            }
        }

        private bool _DewPointHydrocarbonUpdated;
        public bool DewPointHydrocarbonUpdated
        {
            get { return _DewPointHydrocarbonUpdated; }
            set
            {
                if (value)
                    UpdateColor();
                else AlphaChannel = 0;
                _DewPointHydrocarbonUpdated = value;
            }
        }

        private bool _DensityUpdated;
        public bool DensityUpdated
        {
            get { return _DensityUpdated; }
            set
            {
                if (value)
                    UpdateColor();
                else AlphaChannel = 0;
                _DensityUpdated = value;
            }
        }

        private bool _DensityStandardUpdated;
        public bool DensityStandardUpdated
        {
            get { return _DensityStandardUpdated; }
            set
            {
                if (value)
                    UpdateColor();
                else AlphaChannel = 0;
                _DensityStandardUpdated = value;
            }
        }


        private bool _CombustionHeatLowUpdated;
        public bool CombustionHeatLowUpdated
        {
            get { return _DensityStandardUpdated; }
            set
            {
                if (value)
                    UpdateColor();
                else AlphaChannel = 0;
                _CombustionHeatLowUpdated = value;
            }
        }

        private bool _CombustionHeatHighUpdated;
        public bool CombustionHeatHighUpdated
        {
            get { return _CombustionHeatHighUpdated; }
            set
            {
                if (value)
                    UpdateColor();
                else AlphaChannel = 0;
                _CombustionHeatHighUpdated = value;
            }
        }

        private bool _WobbeUpdated;
        public bool WobbeUpdated
        {
            get { return _WobbeUpdated; }
            set
            {
                if (value)
                    UpdateColor();
                else AlphaChannel = 0;
                _WobbeUpdated = value;
            }
        }

        private bool _ContentNitrogenUpdated;
        public bool ContentNitrogenUpdated
        {
            get { return _ContentNitrogenUpdated; }
            set
            {
                if (value)
                    UpdateColor();
                else AlphaChannel = 0;
                _ContentNitrogenUpdated = value;
            }
        }

        private bool _ContentCarbonDioxidUpdated;
        public bool ContentCarbonDioxidUpdated
        {
            get { return _ContentCarbonDioxidUpdated; }
            set
            {
                if (value)
                    UpdateColor();
                else AlphaChannel = 0;
                _ContentCarbonDioxidUpdated = value;
            }
        }

        private bool _ContentMethaneUpdated;
        public bool ContentMethaneUpdated
        {
            get { return _ContentMethaneUpdated; }
            set
            {
                if (value)
                    UpdateColor();
                else AlphaChannel = 0;
                _ContentMethaneUpdated = value;
            }
        }

        private bool _ContentEthaneUpdated;
        public bool ContentEthaneUpdated
        {
            get { return _ContentEthaneUpdated; }
            set
            {
                if (value)
                    UpdateColor();
                else AlphaChannel = 0;
                _ContentEthaneUpdated = value;
            }
        }

        private bool _ContentPropaneUpdated;
        public bool ContentPropaneUpdated
        {
            get { return _ContentPropaneUpdated; }
            set
            {
                if (value)
                    UpdateColor();
                else AlphaChannel = 0;
                _ContentPropaneUpdated = value;
            }
        }

        private bool _ContentButaneUpdated;
        public bool ContentButaneUpdated
        {
            get { return _ContentButaneUpdated; }
            set
            {
                if (value)
                    UpdateColor();
                else AlphaChannel = 0;
                _ContentButaneUpdated = value;
            }
        }

        private bool _ContentIsobutaneUpdated;
        public bool ContentIsobutaneUpdated
        {
            get { return _ContentIsobutaneUpdated; }
            set
            {
                if (value)
                    UpdateColor();
                else AlphaChannel = 0;
                _ContentIsobutaneUpdated = value;
            }
        }

        private bool _ContentPentaneUpdated;
        public bool ContentPentaneUpdated
        {
            get { return _ContentPentaneUpdated; }
            set
            {
                if (value)
                    UpdateColor();
                else AlphaChannel = 0;
                _ContentPentaneUpdated = value;
            }
        }

        private bool _ContentIsopentaneUpdated;
        public bool ContentIsopentaneUpdated
        {
            get { return _ContentIsopentaneUpdated; }
            set
            {
                if (value)
                    UpdateColor();
                else AlphaChannel = 0;
                _ContentIsopentaneUpdated = value;
            }
        }


        private bool _ContentNeopentaneUpdated;
        public bool ContentNeopentaneUpdated
        {
            get { return _ContentNeopentaneUpdated; }
            set
            {
                if (value)
                    UpdateColor();
                else AlphaChannel = 0;
                _ContentNeopentaneUpdated = value;
            }
        }

        private bool _ContentHexaneUpdated;
        public bool ContentHexaneUpdated
        {
            get { return _ContentHexaneUpdated; }
            set
            {
                if (value)
                    UpdateColor();
                else AlphaChannel = 0;
                _ContentHexaneUpdated = value;
            }
        }

        private bool _ContentHydrogenUpdated;
        public bool ContentHydrogenUpdated
        {
            get { return _ContentHydrogenUpdated; }
            set
            {
                if (value)
                    UpdateColor();
                else AlphaChannel = 0;
                _ContentHydrogenUpdated = value;
            }
        }

        private bool _ContentHeliumUpdated;
        public bool ContentHeliumUpdated
        {
            get { return _ContentHeliumUpdated; }
            set
            {
                if (value)
                    UpdateColor();
                else AlphaChannel = 0;
                _ContentHeliumUpdated = value;
            }
        }

        private bool _ConcentrationSourSulfurUpdated;
        public bool ConcentrationSourSulfurUpdated
        {
            get { return _ConcentrationSourSulfurUpdated; }
            set
            {
                if (value)
                    UpdateColor();
                else AlphaChannel = 0;
                _ConcentrationSourSulfurUpdated = value;
            }
        }

        private bool _ConcentrationHydrogenSulfideUpdated;
        public bool ConcentrationHydrogenSulfideUpdated
        {
            get { return _ConcentrationHydrogenSulfideUpdated; }
            set
            {
                if (value)
                    UpdateColor();
                else AlphaChannel = 0;
                _ConcentrationHydrogenSulfideUpdated = value;
            }
        }

        private bool _ConcentrationOxygenUpdated;
        public bool ConcentrationOxygenUpdated
        {
            get { return _ConcentrationOxygenUpdated; }
            set
            {
                if (value)
                    UpdateColor();
                else AlphaChannel = 0;
                _ConcentrationOxygenUpdated = value;
            }
        }

        private bool _DrynessUpdated;
        public bool DrynessUpdated
        {
            get { return _DrynessUpdated; }
            set
            {
                if (value)
                    UpdateColor();
                else AlphaChannel = 0;
                _DrynessUpdated = value;
            }
        }

        private bool _ContentImpuritiesUpdated;
        public bool ContentImpuritiesUpdated
        {
            get { return _ContentImpuritiesUpdated; }
            set
            {
                if (value)
                    UpdateColor();
                else AlphaChannel = 0;
                _ContentImpuritiesUpdated = value;
            }
        }


        private byte _alpaChannel;
        private byte AlphaChannel
        {
            get { return _alpaChannel; }
            set
            {
                _alpaChannel = value;
                OnPropertyChanged(() => PressureColor);
                OnPropertyChanged(() => TemperatureAirColor);
                OnPropertyChanged(() => TemperatureEarthColor);
                OnPropertyChanged(() => GroupCountColor);
                OnPropertyChanged(() => CompressionStageCountColor);
                OnPropertyChanged(() => PressureInLetColor);
                OnPropertyChanged(() => PressureOutLetColor);
                OnPropertyChanged(() => TemperatureInletColor);
                OnPropertyChanged(() => TemperatureOutletColor);
                OnPropertyChanged(() => TemperatureCoolingColor);
                OnPropertyChanged(() => FuelGasConsumptionColor);
                OnPropertyChanged(() => PumpingColor);
                OnPropertyChanged(() => CoolingUnitsInUseColor);
                OnPropertyChanged(() => CoolingUnitsInReserveColor);
                OnPropertyChanged(() => CoolingUnitsUnderRepairColor);
                OnPropertyChanged(() => DustCatchersInUseColor);
                OnPropertyChanged(() => DustCatchersInReserveColor);
                OnPropertyChanged(() => DustCatchersUnderRepairColor);
                OnPropertyChanged(() => PressureSuperchargerInletColor);
                OnPropertyChanged(() => PressureSuperchargerOutletColor);
                OnPropertyChanged(() => TemperatureSuperchargerInletColor);
                OnPropertyChanged(() => TemperatureSuperchargerOutletColor);
                OnPropertyChanged(() => RpmSuperchargerColor);
                OnPropertyChanged(() => FlowColor);
                OnPropertyChanged(() => OpeningPercentageColor);
                OnPropertyChanged(() => ContentImpuritiesColor);
                OnPropertyChanged(() => DrynessColor);
                OnPropertyChanged(() => ConcentrationOxygenColor);
                OnPropertyChanged(() => ConcentrationHydrogenSulfideColor);
                OnPropertyChanged(() => ConcentrationSourSulfurColor);
                OnPropertyChanged(() => ContentHeliumColor);
                OnPropertyChanged(() => ContentHydrogenColor);
                OnPropertyChanged(() => ContentHexaneColor);
                OnPropertyChanged(() => ContentNeopentaneColor);
                OnPropertyChanged(() => ContentIsopentaneColor);
                OnPropertyChanged(() => ContentPentaneColor);
                OnPropertyChanged(() => ContentIsoButaneColor);
                OnPropertyChanged(() => ContentButaneColor);
                OnPropertyChanged(() => ContentPropaneColor);
                OnPropertyChanged(() => ContentEthaneColor);
                OnPropertyChanged(() => ContentMethaneColor);
                OnPropertyChanged(() => ContentCarbonDioxidColor);
                OnPropertyChanged(() => ContentNitrogenColor);
                OnPropertyChanged(() => WobbeColor);
                OnPropertyChanged(() => CombustionHeatHighColor);
                OnPropertyChanged(() => CombustionHeatLowColor);
                OnPropertyChanged(() => DensityStandardColor);
                OnPropertyChanged(() => DensityColor);
                OnPropertyChanged(() => DewPointHydrocarbonColor);
                OnPropertyChanged(() => DewPointColor);
            }
        }

        public SolidColorBrush ContentImpuritiesColor => GetBrush(ContentImpuritiesUpdated);
        public SolidColorBrush DrynessColor => GetBrush(DrynessUpdated);
        public SolidColorBrush ConcentrationOxygenColor => GetBrush(ConcentrationOxygenUpdated);
        public SolidColorBrush ConcentrationHydrogenSulfideColor => GetBrush(ConcentrationHydrogenSulfideUpdated);
        public SolidColorBrush ConcentrationSourSulfurColor => GetBrush(ConcentrationSourSulfurUpdated);
        public SolidColorBrush ContentHeliumColor => GetBrush(ContentHeliumUpdated);
        public SolidColorBrush ContentHydrogenColor => GetBrush(ContentHydrogenUpdated);
        public SolidColorBrush ContentHexaneColor => GetBrush(ContentHexaneUpdated);
        public SolidColorBrush ContentNeopentaneColor => GetBrush(ContentNeopentaneUpdated);
        public SolidColorBrush ContentIsopentaneColor => GetBrush(ContentIsopentaneUpdated);
        public SolidColorBrush ContentPentaneColor => GetBrush(ContentPentaneUpdated);
        public SolidColorBrush ContentIsoButaneColor => GetBrush(ContentIsobutaneUpdated);
        public SolidColorBrush ContentButaneColor => GetBrush(ContentButaneUpdated);
        public SolidColorBrush ContentPropaneColor => GetBrush(ContentPropaneUpdated);
        public SolidColorBrush ContentCarbonDioxidColor => GetBrush(ContentCarbonDioxidUpdated);
        public SolidColorBrush ContentEthaneColor => GetBrush(ContentEthaneUpdated);
        public SolidColorBrush ContentMethaneColor => GetBrush(ContentMethaneUpdated);
        public SolidColorBrush ContentNitrogenColor => GetBrush(ContentNitrogenUpdated);
        public SolidColorBrush WobbeColor => GetBrush(WobbeUpdated);
        public SolidColorBrush CombustionHeatHighColor => GetBrush(CombustionHeatHighUpdated);
        public SolidColorBrush CombustionHeatLowColor => GetBrush(CombustionHeatLowUpdated);
        public SolidColorBrush DensityStandardColor => GetBrush(DensityStandardUpdated);
        public SolidColorBrush DensityColor => GetBrush(DensityUpdated);
        public SolidColorBrush DewPointHydrocarbonColor => GetBrush(DewPointHydrocarbonUpdated);
        public SolidColorBrush DewPointColor => GetBrush(DewPointUpdated);
        public SolidColorBrush PressureColor => GetBrush(PressureUpdated);
        public SolidColorBrush TemperatureAirColor => GetBrush(TemperatureAirUpdated);
        public SolidColorBrush TemperatureEarthColor => GetBrush(TemperatureEarthUpdated);
        public SolidColorBrush GroupCountColor => GetBrush(GroupCountUpdated);
        public SolidColorBrush CompressionStageCountColor => GetBrush(CompressionStageCountUpdated);
        public SolidColorBrush PressureInLetColor => GetBrush(PressureInletUpdated);
        public SolidColorBrush PressureOutLetColor => GetBrush(PressureOutletUpdated);
        public SolidColorBrush TemperatureInletColor => GetBrush(TemperatureInletUpdated);
        public SolidColorBrush TemperatureOutletColor => GetBrush(TemperatureOutletUpdated);
        public SolidColorBrush TemperatureCoolingColor => GetBrush(TemperatureCoolingUpdated);
        public SolidColorBrush FuelGasConsumptionColor => GetBrush(FuelGasConsumptionUpdated);
        public SolidColorBrush PumpingColor => GetBrush(PumpingUpdated);
        public SolidColorBrush CoolingUnitsInUseColor => GetBrush(CoolingUnitsInUseUpdated);
        public SolidColorBrush CoolingUnitsInReserveColor => GetBrush(CoolingUnitsInReserveUpdated);
        public SolidColorBrush CoolingUnitsUnderRepairColor => GetBrush(CoolingUnitsUnderRepairUpdated);
        public SolidColorBrush DustCatchersInUseColor => GetBrush(DustCatchersInUseUpdated);
        public SolidColorBrush DustCatchersInReserveColor => GetBrush(DustCatchersInReserveUpdated);
        public SolidColorBrush DustCatchersUnderRepairColor => GetBrush(DustCatchersUnderRepairUpdated);
        public SolidColorBrush PressureSuperchargerInletColor => GetBrush(PressureSuperchargerInletUpdated);
        public SolidColorBrush PressureSuperchargerOutletColor => GetBrush(PressureSuperchargerOutletUpdated);
        public SolidColorBrush TemperatureSuperchargerInletColor => GetBrush(TemperatureSuperchargerInletUpdated);
        public SolidColorBrush TemperatureSuperchargerOutletColor => GetBrush(TemperatureSuperchargerOutletUpdated);
        public SolidColorBrush RpmSuperchargerColor => GetBrush(RpmSuperchargerUpdated);
        public SolidColorBrush FlowColor => GetBrush(FlowUpdated);
        public SolidColorBrush OpeningPercentageColor => GetBrush(OpeningPercentageUpdated);

        private SolidColorBrush GetBrush(bool isUpdated)
        {
            return new SolidColorBrush(Color.FromArgb(isUpdated ? AlphaChannel : (byte)0, 220, 255, 115));
        }

        DataRefreshWatcher drw = new DataRefreshWatcher();

        private bool TimerInitialized;
        private void UpdateColor()
        {
            if (!TimerInitialized)
            {
                TimerInitialized = true;
                drw.TimeToRefresh += ChangeAlphaChannel;
            }
            AlphaChannel = byte.MinValue;
            TimerSpeed = 1 / (byte.MaxValue / FromSeconds);
            drw.Run(TimerSpeed);
        }
        double TimerSpeed;
        private void ChangeAlphaChannel(object sender, EventArgs e)
        {
            if (AlphaChannel < 200)
            {
                AlphaChannel++;
            }
            else drw.Stop();
        }
    }
}