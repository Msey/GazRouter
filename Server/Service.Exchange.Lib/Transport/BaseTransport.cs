using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GazRouter.DTO.DataExchange.ExchangeTask;
using GazRouter.DTO.Dictionaries.ExchangeTypes;
using GazRouter.DTO.Dictionaries.TransportTypes;
using GazRouter.Log;
using GazRouter.Service.Exchange.Lib.Import;

namespace GazRouter.Service.Exchange.Lib.Transport
{
    public abstract class BaseTransport
    {
        protected readonly MyLogger Logger = new MyLogger("exchangeLogger");
        protected List<FileInfo> FileInfos = new List<FileInfo>();

        protected string Folder;

        protected BaseTransport(ExchangeTaskDTO task)
        {
            Task = task;
            Folder = ExchangeHelper.GetFolder(task);
        }

        protected ExchangeTaskDTO Task { get; set; }


        public void Execute()
        {
            if (Task?.ExchangeTypeId == ExchangeType.Export)
            {
                FileInfos = new DirectoryInfo(Folder).EnumerateFiles().ToList();
                if (!FileInfos.Any()) return;
            }

            _Execute();

            FileTools.EnsureDelete(FileInfos.Select(fi => fi.FullName).ToList());
            FileInfos.Clear();
        }

        protected virtual void _Execute()
        {
        }

        public static BaseTransport Create(ExchangeTaskDTO cfg, bool throwException = false)
        {
            TransportType? type;
            if ((type = cfg.TransportTypeId) == null)
                throw new ArgumentOutOfRangeException("TransportTypeId", @"ExchangeTask: Не задан TransportTypeId");
            if (cfg.ExchangeTypeId == ExchangeType.Export && string.IsNullOrEmpty(cfg.TransportAddress))
                throw new ArgumentException("TranspotAddress", @"ExchangeTask: Не задан TransportAddress");
            switch (type)
            {
                case TransportType.Folder:
                    return new LocalTransport(cfg);
                case TransportType.Ftp:
                    return new WinScpFtpTransport(cfg);
                case TransportType.Email:
                    return new MailKitEmailTransport(cfg);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}