using System;
using System.IO;
using System.Web;
using GazRouter.DAL.DataStorage;
using GazRouter.DataServices.Infrastructure;
using GazRouter.DataServices.Infrastructure.Sessions;
using GazRouter.DTO.DataStorage;
using GazRouter.Log;

namespace GazRouter.DataServices.HttpHandlers
{
    public class BlobHandler : IHttpHandler
    {
        void IHttpHandler.ProcessRequest(HttpContext context)
        {
            var request = context.Request;
            var response = context.Response;


            Guid guid;
            if (!Guid.TryParse(request.QueryString["id"], out guid))
            {
                response.Write("Неверный id");
                return;
            }

            BlobDTO blob;
            using (var dbContext = DbContextHelper.OpenDbContext(SessionManager.SystemUserLogin, new MyLogger("mainLogger")))
            {
                blob = new GetBlobByIdQuery(dbContext).Execute(guid);
            }

            if (blob == null)
            {
                response.Write("Ошибка при чтении из БД.");
                return;
            }

            var extension = Path.GetExtension(blob.FileName);
            bool attach = false;
            if (extension != null)
                switch (extension.ToLower())
                {
                    case ".pdf":
                        response.ContentType = "application/pdf";
                        break;
                    case ".bmp":
                        response.ContentType = "image/x-ms-bmp";
                        break;
                    case ".jpg":
                        response.ContentType = "image/jpeg";
                        break;
                    case ".png":
                        response.ContentType = "image/png";
                        break;
                    case ".gif":
                        response.ContentType = "image/gif";
                        break;
                    default:
                        attach = true;
                        response.ContentType = "application/octet-stream";
			            
                        break;
                }
            response.AddHeader("Content-Disposition", string.Format("{0}; filename=\"{1}\";",  attach ? "attachment": "inline", Uri.EscapeDataString(blob.FileName)));

            response.BinaryWrite(blob.Data);
        }

        bool IHttpHandler.IsReusable
        {
            get { return true; }
        }
    }
}