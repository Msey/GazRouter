using System;
using System.Web;
using GazRouter.DataServices.Infrastructure;

namespace GazRouter.DataServices.HttpHandlers
{
    public class SapBoHandler : IHttpHandler
    {
        void IHttpHandler.ProcessRequest(HttpContext context)
        {
            try
            {
                context.Response.Redirect(AppSettingsManager.SapBoUrl);
            }
            catch(Exception e)
            {
            }

        }

        bool IHttpHandler.IsReusable
        {
            get { return true; }
        }

    }
}