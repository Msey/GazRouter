using System;

namespace GazRouter.Flobus.FloScheme
{
    /// <summary>
    /// Слои схемы
    /// </summary>
    [Flags]
    public enum SchemeLayers
    {
        Base = 1,
        Measurings = 2,
        Events = 4,
        Thumbnail = 8,
        All = Base | Measurings | Events 
    }
}
