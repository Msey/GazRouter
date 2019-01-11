using System;
using System.IO;
using System.ServiceModel;
using System.Windows;
using GazRouter.Common;
using GazRouter.DTO.Infrastructure.Faults;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;

namespace GazRouter.Controls.Dialogs
{
    public partial class ErrorWindow
    {
        public ErrorWindow(Exception ex, Uri serverUri = null)
        {
            InitializeComponent();

            var serverException = ex as ServerException;
            if (serverException != null)
            {
                TxtMessage.Text = "Ошибка на сервере приложений";

                var serverExceptionType = serverException.InnerException.GetType();
                if (serverExceptionType == typeof(TimeoutException))
                {
                    TxtErrorDetail.Text = "Истекло время ожидания ответа сервера.";
                }
                else if (serverExceptionType == typeof(FaultException<FaultDetail>))
                {
                    var detail = ((FaultException<FaultDetail>) serverException.InnerException).Detail;
                    switch (detail.FaultType)
                    {
                        case FaultType.UserNotRegistered:
                        {
                            TxtErrorDetail.Text =
                                string.Format("Пользователь {1} не зарегистрирован в системе. Запись в логе № {0}",
                                    detail.LogRecordId, detail.Message);
                            break;
                        }
                        case FaultType.AccessDenied:
                        {
                            TxtErrorDetail.Text =
                                string.Format(
                                    "У пользователя нет прав на выполнение операции {1}. Запись в логе № {0}",
                                    detail.LogRecordId, detail.Message);
                            break;
                        }
                        case FaultType.VersionIncompatible:
                        {
                            TxtErrorDetail.Text =
                                $"Версия клиента не совпадает с версией сервера. Обновите клиент(Ctrl+F5). Запись в логе № {detail.LogRecordId}";
                            break;
                        }
                        default:
                        {
                            TxtErrorDetail.Text = $"Непредвиденная ошибка. Запись в логе № {detail.LogRecordId}";
                            break;
                        }
                    }
                }
                else
                {
                    TxtErrorDetail.Text =
                        $"Ошибка при обращении к северу.\n\rUri:{serverUri?.OriginalString ?? string.Empty}\n\r{GetExceptionDetails(ex)}";
                }
            }
            else
            {
                TxtMessage.Text = "Ошибка на машине пользователя";
                TxtErrorDetail.Text = GetExceptionDetails(ex);
            }
        }

        private static string GetExceptionDetails(Exception e)
        {
            using (var textWriter = new StringWriter())
            {
                var formatter = new TextExceptionFormatter(textWriter, e);
                formatter.Format();
                return textWriter.ToString();
            }
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CopyButtonClick(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(TxtErrorDetail.Text);
            MessageBox.Show("Подробности скопированы в буфер обмена.");
        }
    }
}