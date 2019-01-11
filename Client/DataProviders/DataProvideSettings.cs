using System;
using System.Linq;
using System.Reflection;
using System.Windows.Browser;

namespace DataProviders
{
    public static class DataProvideSettings
    {
        internal static readonly string ClientVersion = Assembly.GetExecutingAssembly().GetCustomAttributes(false).OfType<AssemblyFileVersionAttribute>().Single().Version;

        public static Uri ServerUri = GetServerUri(); //HtmlPage.Document.DocumentUri;

        private static Uri GetServerUri()
        {
            var absoluteUri = System.Windows.Application.Current.Host.Source.AbsoluteUri;
        var uri = new Uri(    absoluteUri.Remove(absoluteUri.IndexOf("ClientBin", StringComparison.Ordinal)));
            return uri;
        }
    }
}