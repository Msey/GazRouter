using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using GazRouter.DataServices.Infrastructure;
using GazRouter.DTO.DataExchange.ExchangeTask;
using GazRouter.DTO.Dictionaries.ExchangeTypes;
using GazRouter.Service.Exchange.Lib.Import;
using MailKit.Net.Pop3;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.IO;

namespace GazRouter.Service.Exchange.Lib.Transport
{
    public class MailKitEmailTransport : BaseTransport
    {
        private static readonly string SmtpHost = AppSettingsManager.EmailSmtpHost;
        private static readonly string Pop3Host = AppSettingsManager.EmailPop3Host;
        private static readonly string Login = AppSettingsManager.EmailLogin;
        private static readonly string Password = AppSettingsManager.EmailPassword;
        private static readonly string SystemAddress = AppSettingsManager.EmailSystemAddress;

        public MailKitEmailTransport(ExchangeTaskDTO task) : base(task)
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
                var msgs = GetMessages().ToList();
                if (!msgs.Any()) return;
                var attachments =
                    msgs.Where(m => m.Attachments.Any()).SelectMany(m => m.Attachments, (m, a) => a).ToList();
                attachments
                    .ForEach(a =>
                    {
                        var fullName = Path.Combine(folder, a.ContentType.Name);
                        using (var stream = FileTools.OpenOrCreate(fullName))
                        {
                            if (a is MessagePart)
                            {
                                var part = (MessagePart) a;

                                part.Message.WriteTo(stream);
                            }
                            else
                            {
                                var part = (MimePart) a;
                                part.ContentObject.DecodeTo(stream);
                            }

                            //FileTools.WriteFile(fullName, contentStream);
                            Logger.Info(
                                $"EMAIL: Обработан аттачмент {a.ContentId}. Расположение файла: {fullName}. Размер: {a.ContentDisposition.Size}");
                        }
                    });
                msgs.Where(m => !m.Attachments.Any())
                    .Select(m => new {Name = Regex.Match(m.Subject, "#(.*)#").Groups[1].Value, m.TextBody})
                    .ToList()
                    .ForEach(m =>
                    {
                        var fullName = Path.Combine(folder, m.Name);
                        File.WriteAllText(fullName, m.TextBody);
                        Logger.Info(
                            $"EMAIL: Обработано тело письма {m.Name}. Расположение файла: {fullName}. Размер: {m.TextBody?.Length ?? 0}");
                    });
            }
            catch (Exception e)
            {
                throw new Exception($@"EMAIL: Ошибка при получении письма", e);
            }
        }

        private void Send()
        {
            SmtpClient client = null;
            var host = SmtpHost;
            var login = Login;
            var password = Password;
            using (client = new SmtpClient())
            {
                try
                {
                    // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    client.Connect(host, 0, false);

                    // Note: since we don't have an OAuth2 token, disable
                    // the XOAUTH2 authentication mechanism.
                    client.AuthenticationMechanisms.Remove("XOAUTH2");

                    // Note: only needed if the SMTP server requires authentication
                    try
                    {
                        client.Authenticate(login, password);
                    }
                    catch (Exception e)
                    {
                        Logger.WriteException(e,
                            "EMAIL: Smtp аутентификация не поддерживается или неправильные логин/пароль");
                    }

                    var @from = new List<InternetAddress> {InternetAddress.Parse(SystemAddress)};
                    var to = Task.TransportAddress?.Split(';').Select(InternetAddress.Parse).ToList();
                    foreach (var fileInfo in FileInfos)
                    {
                        string subject = $"{ExchangeHelper.DefaultEmailSubject}.#{fileInfo.Name}#";
                        var message = CreateMessage(@from, to, subject, fileInfo);
                        client.Send(message);
                        Logger.Info($"EMAIL: Письмо с темой {subject} отправлено");
                    }
                }
                catch (Exception e)
                {
                    throw new Exception($@"EMAIL: Ошибка при получении письма", e);
                }
                finally { client.Disconnect(true); }
            }
        }

        private MimeMessage CreateMessage(List<InternetAddress> @from, List<InternetAddress> to, string subject,
            FileInfo fi)
        {
            // create our message text, just like before (except don't set it as the message.Body)


            var attachment = GetAttachment(fi);

            var body = new Multipart("mixed");
            body.Add(attachment);

            var mm = new MimeMessage(@from, to, subject, attachment);

            return mm;
        }

        private static MimePart GetAttachment(FileInfo f)
        {
            var mbs = new MemoryBlockStream();
            using (var stream = File.OpenRead(f.FullName))
            {
                stream.CopyTo(mbs);
            }
            return new MimePart("text", "xml")
            {
                ContentObject = new ContentObject(mbs),
                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                FileName = f.Name
            };
        }

        public IEnumerable<MimeMessage> GetMessages()
        {
            var messages = new List<MimeMessage>();
            try
            {
                var host = Pop3Host;
                var login = Login;
                var pwd = Password;

                using (var pop3 = new Pop3Client())
                {
                    pop3.Disconnected += Pop3_Disconnected;
                    try
                    {
                        try
                        {
                            Logger.Info($"EMAIL: Подключение к почтовому серверу");
                            pop3.ServerCertificateValidationCallback = (s, c, h, e) => true;
                            pop3.ConnectWithRetries(host);
                            pop3.AuthenticationMechanisms.Remove("XOAUTH2");
                            pop3.Authenticate(login, pwd);
                        }
                        catch (Exception e)
                        {
                            Logger.WriteException(e, $"EMAIL: Ошибка аутентификации почты: {e.Message}");
                        }
                        int count;
                        Logger.Info($"EMAIL: На сервере {count = pop3.Count} писем");
                        if (count > 0)
                        {
                            for (var i = 0; i < count; i++)
                            {
                                var m = pop3.GetMessage(i);
                                pop3.DeleteMessage(i);
                                if (!m.Subject.Contains(ExchangeHelper.DefaultEmailSubject)) continue;
                                messages.Add(m);
                            }
                            Logger.Info($"EMAIL: Получено {messages.Count} писем");
                            Logger.Info($"EMAIL: Удалено {count} писем");
                        }
                    }
                  
                    catch (Exception e)
                    {
                        Logger.WriteException(e, $"EMAIL: {e.Message}");
                        return messages;
                    }
                    finally
                    {
                        pop3.Disconnect(true);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.WriteException(e, e.Message);
                return messages;
            }
            return messages;
        }

        private void Pop3_Disconnected(object sender, EventArgs e)
        {
            (sender as Pop3Client)?.Connect(Pop3Host);
        }
    }

    public static class Pop3ClientExtensions
    {
        const int NumberOfRetries = 5;

        public static  void ConnectWithRetries(this Pop3Client pop3, string host)
        {
                var retryCount = NumberOfRetries;
                var success = false;
                while (!success && retryCount > 0)
                {
                    try
                    {
                        pop3.Connect(host);
                        success = true;
                    }
                    catch (Exception e)

                {
                    retryCount--;

                        if (retryCount == 0)
                        {
                            throw; //or handle error and break/return
                        }
                    }
                }

        }
    }
}