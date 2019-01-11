using System;
using System.Windows.Browser;
using DataProviders;

using GazRouter.DTO.Dictionaries.PeriodTypes;
namespace GazRouter.DataProviders
{
    /// <summary> 
    /// 
    /// 
    /// 
    ///    UriBuilder -> HttpHandler -> navigate
    /// 
    /// 
    /// 
    ///    <handlers>
    ///      <add verb = "GET" path="*.blob" name="BlobHandler"        type="GazRouter.DataServices.HttpHandlers.BlobHandler" />
    ///      <add verb = "GET" path="*.exml" name="ExchangeXmlHandler" type="GazRouter.DataServices.HttpHandlers.ExchangeXmlHandler" />
    ///      <add verb = "GET" path="*.astra" name="AstraHandler"      type="GazRouter.DataServices.HttpHandlers.AstraHandler" />s
    ///   +  <add verb = "GET" path="*.sapbo" name="SapBoHandler"      type="GazRouter.DataServices.HttpHandlers.SapBoHandler" />
    ///      <add verb = "GET" path="*.asdu" name="AsduBoHandler"      type="GazRouter.DataServices.HttpHandlers.AsduHandler" />
    ///    </handlers>
    /// </summary>
    public static class UriBuilder
    {
#region handlers_webConfig2
//            BlobHandler
//            ExchangeXmlHandler
//            AstraHandler
            public static string GetSapBoUri2 { get; set; }
//            AsduBoHandler
#endregion
#region handlers_webConfig
        public static Uri GetBlobHandlerUri(Guid blobId)
        {
            var result = new System.UriBuilder(BaseUrl)
                         {
                             Query =
                                 $"id={blobId.ToString().Replace("-", string.Empty).ToUpper()}"
                         };
            result.Path += $"/{DateTime.Now.Ticks}.blob";
            return result.Uri;
        }

        public static Uri GetSapBoUri()
        {
            var result = new System.UriBuilder(BaseUrl);
            result.Path += $"/sapbo.sapbo";
            return result.Uri;
        }
#endregion

        public static Uri GetServiceUri(string serviceUri)
        {
          return  new Uri(DataProvideSettings.ServerUri.OriginalString.TrimEnd('/') + "/" + serviceUri.TrimStart('/'));
        }
        public static Uri GetSpecificExchangeHandlerUri(int cfgId, DateTime dt, PeriodType  periodType, bool xmlOnly = false)
        {
            var result = new System.UriBuilder(BaseUrl)
                         {
                             Query = $"id={cfgId}&dt={dt.Ticks}&periodTypeId={(int)periodType}&isSpecific={true}{(xmlOnly ? "&xmlOnly=1" : "")}"
                         };
            result.Path += $"/{DateTime.Now.Ticks}.exml";
            return result.Uri;
        }
        public static Uri GetAsduHandlerUri(PeriodType periodType, DateTime timeStamp)
        {
            var result = new System.UriBuilder(BaseUrl)
            {
                Query = $"periodType={(int)periodType}&timeStamp={timeStamp.Ticks}"
            };
            result.Path += $"/{DateTime.Now.Ticks}.asdu";
            return result.Uri;
        }
        public static Uri GetTypicalExchangeHandlerUri(Guid entId, DateTime dt, PeriodType periodTypeId = PeriodType.Twohours, bool isCryptable = false)
        {
            var result = new System.UriBuilder(BaseUrl)
                         {
                             Query =
                                 $"id={entId.ToString().Replace("-", string.Empty).ToUpper()}&dt={dt.Ticks}&periodTypeId={(int)periodTypeId}&isSpecific={false}&isCryptable={isCryptable}"
                         };
            result.Path += $"/{DateTime.Now.Ticks}.exml";
            return result.Uri;
        }

        private static string BaseUrl => HtmlPage.Document.DocumentUri.OriginalString.ToLower()
            .Replace(@"/default.aspx", string.Empty).TrimEnd('/');
        public static Uri GetAstraUri(DateTime dt, PeriodType periodType)
        {
            var result = new System.UriBuilder(BaseUrl)
            {
                Query = $"dt={dt.Ticks}&periodtype={(int)periodType}"
            };
            result.Path += $"/astra.astra";
            return result.Uri;
        }

    }
}
