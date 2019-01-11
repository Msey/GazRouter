using System;
using System.IO;
using System.Xml.Serialization;
using GazRouter.DAL.Core;
using GazRouter.DTO.DataExchange.ExchangeLog;
using GazRouter.DTO.DataExchange.ExchangeTask;
using GazRouter.Service.Exchange.Lib.Cryptography;

namespace GazRouter.Service.Exchange.Lib.Import
{
    public class TypicalExchangeImporter : ExchangeImporter
    {
        public override bool IsValid(ExchangeTaskDTO task, string fullPath)
        {
            return task.EnterpriseCode != null && (Path.GetFileNameWithoutExtension(fullPath)?.ToLower()?.Contains(task.EnterpriseCode.ToLower()) ?? false);
        }

        public override void Run(ExecutionContext context, ExchangeTaskDTO task, string fullPath)
        {
            using (var fs = FileTools.OpenOrCreate(fullPath))
            {
                var buffer = new byte[fs.Length];
                fs.Read(buffer, 0, (int)fs.Length);
                buffer = Cryptoghraphy.Decrypt(buffer);
                var eo = XmlHelper.Get<ExchangeObject<TypicalExchangeData>>(buffer);
                eo?.Sync(context, task);
            }
        }
    }
}