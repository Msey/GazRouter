using GazRouter.Controls.Measurings;

namespace GazRouter.Flobus.VM.Model
{
    public interface ICompressorShopMeasuring
    {
        DoubleMeasuring CompressionRatio { get; }
        DoubleMeasuring FuelGasConsumption { get; }
        StringMeasuring Pattern { get; }
        DoubleMeasuring PressureInlet { get; }
        DoubleMeasuring PressureOutlet { get; }
        DoubleMeasuring Pumping { get; }
        DoubleMeasuring TemperatureCooling { get; }
        DoubleMeasuring TemperatureInlet { get; }
        DoubleMeasuring TemperatureOutlet { get; }
    }
}