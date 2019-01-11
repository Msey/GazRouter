using System;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using GazRouter.DTO.DataExchange.ExchangeTask;

namespace GazRouter.Service.Exchange.Lib.Transport
{
    public class LocalTransport : BaseTransport
    {
        public LocalTransport(ExchangeTaskDTO task)
            : base(task)
        {
        }

        protected override void _Execute()
        {
            foreach (var fullName in FileInfos.Select(fi => fi.FullName))
                try
                {
                    var fileName = Path.GetFileName(fullName);
                    var dest = Path.Combine(Task.TransportAddress ?? string.Empty, fileName);
                    File.Copy(fullName, dest, true);
                    Logger.Info($"SMB: Отправлен файл {fullName} в {dest}");
                    new FileIOPermission(FileIOPermissionAccess.Read, new[] {fullName}).Demand();
                }
                catch (Exception e)
                {
                    Logger.WriteException(e, "SMB: ошибка транспорта по SAMBA");
                }
        }
    }
}