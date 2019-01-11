using System;
using System.IO;
using System.Linq;
using System.Net;
using GazRouter.DataServices.Infrastructure;
using GazRouter.DataServices.Infrastructure.Sessions;
using GazRouter.DTO.DataExchange.ExchangeTask;
using GazRouter.Log;
using java.util.logging;

namespace GazRouter.Service.Exchange.Lib.Transport
{
    public class FtpTransport : BaseTransport
    {
        public FtpTransport(ExchangeTaskDTO config)
            : base(config)
        {
        }

        protected override void _Execute()
        {
            foreach (var filePath in FileInfos.Select(fi => fi.FullName))
            {
                UploadFile(filePath);
            }
        }

        private bool UploadFile(string filePath)
        {
            try
            {
                using (FileStream stream = File.OpenRead(filePath))
                {

                    string uri = $"{Task.TransportAddress}/{Path.GetFileName(filePath)}";
                    var request = (FtpWebRequest) WebRequest.Create(uri);
                    
                    request.Credentials = new NetworkCredential(Task.TransportLogin, Task.TransportPassword);
                    request.Method = WebRequestMethods.Ftp.UploadFile;
                    request.KeepAlive = false;
                    request.UseBinary = true;
                    request.UsePassive = true;
                    request.ContentLength = stream.Length;
                    request.EnableSsl = request.RequestUri.Scheme == "ftps"; 

                    const int buffLen = 2048;
                    var buff = new byte[buffLen];
                    ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, errors) => true; //hack

                    try
                    {
                        Stream ftpStream = request.GetRequestStream();
                        int readBytes;
                        do
                        {
                            readBytes = stream.Read(buff, 0, buffLen);
                            ftpStream.Write(buff, 0, readBytes);
                        } while (readBytes != 0);
                        ftpStream.Flush();
                        ftpStream.Close();
                    }
                    catch (Exception e)
                    {
                        Logger.WriteException(e, e.Message);
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.WriteException(e, e.Message);
                return false;
            }

            return true;
        }
    }
}