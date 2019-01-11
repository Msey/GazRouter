using System.Linq;
using System.Reflection;

namespace DataProviders
{
    internal static class DataProvideSettings
    {
        internal static readonly string ClientVersion = Assembly.GetExecutingAssembly().GetCustomAttributes(false).OfType<AssemblyFileVersionAttribute>().Single().Version;
    }
}