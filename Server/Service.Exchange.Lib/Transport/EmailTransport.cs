using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Security.Authentication;
using System.Text.RegularExpressions;
using GazRouter.DataServices.Infrastructure;
using GazRouter.DTO.DataExchange.ExchangeTask;
using GazRouter.DTO.Dictionaries.ExchangeTypes;
using GazRouter.DTO.Exchange;
using GazRouter.Service.Exchange.Lib.Import;
using OpaqueMail.Net;
using MailMessage = OpaqueMail.Net.MailMessage;
using SmtpClient = OpaqueMail.Net.SmtpClient;

namespace GazRouter.Service.Exchange.Lib.Transport
{
    public class EmailTransport : BaseTransport
    {
        private static readonly string SmtpHost = AppSettingsManager.EmailSmtpHost;
        private static readonly string Pop3Host = AppSettingsManager.EmailPop3Host;
        private static readonly string Login = AppSettingsManager.EmailLogin;
        private static readonly string Password = AppSettingsManager.EmailPassword;
        private static readonly string SystemAddress = AppSettingsManager.EmailSystemAddress;

        public EmailTransport(ExchangeTaskDTO task, bool throwException = false) : base(task, throwException)
        {
        }

        protected override void _Execute()
        {
            if (Task.ExchangeTypeId == ExchangeType.Import)
            {
                Receive();
            }
            else
            {
                Send();
            }
        }

        private void Receive()
        {
            try
            {
                var folder = ExchangeHelper.GetFolder(Task);
                var msgs = GetMessages();
                var attachments =
                    msgs.Where(m => m.Attachments.Any()).SelectMany(m => m.Attachments, (m, a) => a).ToList();
                attachments
                    .ForEach(a =>
                    {
                        using (a)
                        {
                            var fullName = Path.Combine(folder, a.Name);
                            var contentStream = (MemoryStream) a.ContentStream;
                            FileTools.WriteFile(fullName, contentStream);
                            Logger.Info(
                                $"EMAIL: Обработан аттачмент {a.Name}. Расположение файла: {fullName}. Размер: {contentStream.Length}");
                        }
                    });
                msgs.Where(m => !m.Attachments.Any())
                    .Select(m => new {Name = Regex.Match(m.Subject, "#(.*)#").Groups[1].Value, m.RawBody})
                    .ToList()
                    .ForEach(m =>
                    {
                        var fullName = Path.Combine(folder, m.Name);
                        File.WriteAllText(fullName, m.RawBody);
                        Logger.Info(
                            $"EMAIL: Обработано тело письма {m.Name}. Расположение файла: {fullName}. Размер: {m.RawBody?.Length ?? 0}");
                    });
            }
            catch (Exception e)
            {
                Logger.WriteException(e, e.Message);
            }
        }

        private void Send()
        {
            try
            {
                var host = SmtpHost;
                var login = Login;
                var password = Password;
                using (var smtp = new SmtpClient { Host = host, Credentials = new NetworkCredential(login, password), EnableSsl = false})
                {
                    var @from = SystemAddress;
                    var to = Task.TransportAddress;
                    using (var mm = new MailMessage(@from, to))
                    {
                        mm.Subject = string.Format(ExchangeHelper.DefaultEmailSubject);
                        FileInfos.ForEach(f =>
                        {
                            var fileNameSubject = string.Format(ExchangeHelper.FileNameEmailSubject, f.Name);
                            mm.Subject = $"{ExchangeHelper.DefaultEmailSubject}.{fileNameSubject}";
                            mm.Attachments.Add(new Attachment(f.FullName, new ContentType { MediaType = MediaTypeNames.Text.Xml }));
                            smtp.Send(mm);
                            Logger.Info($"EMAIL: Отправлено письмо {mm.Subject}");

                        });
                    }
                }
            }
                        catch (Exception e)
            {
                Logger.WriteException(e, e.Message);
            }
        }

        public List<ReadOnlyMailMessage> GetMessages()
        {
            var result = new List<ReadOnlyMailMessage>();
            try
            {
                var host = Pop3Host;
                var login = Login;
                var pwd = Password;
                var port = 110;

                using (var pop3 = new Pop3Client(host, port, login, pwd, false))
                {
                    try
                    {
                        try
                        {
                            Logger.Info($"EMAIL: Подключение к почтовому серверу");

                            pop3.Connect();
                            pop3.Authenticate();
                        }
                        catch (Exception e)
                        {
                            throw new AuthenticationException("EMAIL: Ошибка аутентификации почты", e);
                        }
                        pop3.ProcessingFlags  = ReadOnlyMailMessageProcessingFlags.IncludeWinMailData;
                        int count;
                        Logger.Info($"EMAIL: На сервере {count = pop3.GetMessageCount()} писем", pop3.LastCommandIssued);
                        if (count > 0)
                        {
                            result = pop3.GetMessages().Where(m => m.Subject.Contains(ExchangeHelper.DefaultEmailSubject)).ToList();
                            Logger.Info($"EMAIL: Получено {result.Count} писем", pop3.LastCommandIssued);
                            pop3.DeleteMessages(result.Select(m => m.Index).ToArray());
                            Logger.Info($"EMAIL: Удалено {result.Count} писем", pop3.LastCommandIssued);
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.WriteException(e, $"EMAIL: {pop3.LastErrorMessage}" );
                    }
                }
                return result;
            }
            catch (Exception e)
            {
                Logger.WriteException(e, e.Message);
                return result;
            }
        }
    }
}