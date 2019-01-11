using System;
using System.IO;
using System.Linq;
using GazRouter.DataServices.Infrastructure;
using GazRouter.DataServices.Infrastructure.Sessions;
using GazRouter.DTO.DataExchange.ExchangeTask;
using GazRouter.DTO.Dictionaries.ExchangeTypes;
using GazRouter.Log;
using GazRouter.Service.Exchange.Lib.Import;
using WinSCP;

namespace GazRouter.Service.Exchange.Lib.Transport
{
    public sealed class WinScpFtpTransport : BaseTransport
    {

        public WinScpFtpTransport(ExchangeTaskDTO config) : base(config)
        {
        }

        protected override void _Execute()
        {
            var uri = new Uri(Task.TransportAddress);
            try
            {
                var sessionOptions = GetSessionOptions();
                using (var session = new Session())
                {
                    session.Open(sessionOptions);
                    if (Task.ExchangeTypeId == ExchangeType.Export)
                    {
                        SynchronizeDirs(session, SynchronizationMode.Remote, Folder, uri.LocalPath, "*");
                        Logger.Info(
                            $"SFTP: Отправлены файлы из локальной папки {Folder} в удаленную папку {uri.LocalPath}");
                    }
                    if (Task.ExchangeTypeId == ExchangeType.Import)
                    {
                        SynchronizeDirs(session, SynchronizationMode.Local, Folder, uri.LocalPath, "*");
                        Logger.Info(
                            $"SFTP: Доставлены файлы из удаленной папки {uri.LocalPath} в локальную папку {Folder}");
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception($"SFTP: ошибка при передачи по ссылке: {uri}", e);
            }
        }

        private void SynchronizeDirs(Session session, SynchronizationMode mode, string localPath,
            string remotePath, string fileMask)
        {
            var transferOptions = new TransferOptions
            {
                FileMask = fileMask
            };
            var result = session.SynchronizeDirectories(mode, localPath, remotePath, false, options: transferOptions);
            var ops = mode == SynchronizationMode.Remote ? result.Uploads : result.Downloads;
            foreach (FileOperationEventArgs args in ops)
            {
                if (args.Error != null)
                {
                    Logger.WriteException(args.Error, $@"SFTP: Ошибка операции с файлом: {args.FileName}");
                }

                if (mode == SynchronizationMode.Remote)
                {
                    try
                    {
                        FileTools.EnsureDelete(args.FileName);
                    }
                    catch (Exception e)
                    {
                        Logger.WriteException(e, e.Message);
                    }
                }
                else
                {
                    var removeResult = session.RemoveFiles(session.EscapeFileMask(args.FileName));
                    if (!removeResult.IsSuccess)
                    {
                        Logger.WriteException(removeResult.Failures.FirstOrDefault(),
                            $@"SFTP: Ошибка операции с файлом: {args.FileName}");
                    }
                }
            }
        }

        private SessionOptions GetSessionOptions()
        {


            SessionOptions sessionOptions = null;
            try
            {
                var uri = new Uri(Task.TransportAddress);

                if (Task.ExchangeTypeId == ExchangeType.Import)
                {

                    sessionOptions = new SessionOptions();
                    var baseUrl = uri.GetLeftPart(UriPartial.Authority); 
                    sessionOptions.ParseUrl(baseUrl);
                }
                else
                {
                    var protocol = uri.Scheme == "ftp" ? Protocol.Ftp : uri.Scheme == "sftp" ? Protocol.Sftp : Protocol.Ftp;
                    var userName = Task.TransportLogin;
                    var password = Task.TransportPassword;
                    var hostKey = Task.HostKey;
                    sessionOptions = new SessionOptions
                    {
                        Protocol = protocol,
                        PortNumber = uri.IsDefaultPort ? protocol == Protocol.Ftp ? 21 : 22 : uri.Port,
                        HostName = uri.Host,
                        UserName = userName,
                        Password = password,
                        SshHostKeyFingerprint = hostKey
                    };

                    if (protocol == Protocol.Ftp)
                    {
                        sessionOptions.FtpSecure = uri.Scheme == "ftps" ? FtpSecure.ExplicitSsl : FtpSecure.None;
                    }

                }
               return sessionOptions;
            }
            catch (Exception e)
            {
                Logger.WriteException(e, $"FTP: Адрес: {Task.TransportAddress}");
                throw;
            }

        }
    }
}