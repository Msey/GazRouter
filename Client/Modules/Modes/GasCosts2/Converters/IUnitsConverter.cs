using System;
namespace GazRouter.Modes.GasCosts2.Converters
{
    public interface IUnitsConverter
    {
        Action<int> UnitsChanged { get; set; }
    }
}
