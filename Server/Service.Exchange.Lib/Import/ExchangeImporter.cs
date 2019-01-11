using GazRouter.DAL.Core;
using GazRouter.DTO.DataExchange.ExchangeTask;

namespace GazRouter.Service.Exchange.Lib.Import
{

    public abstract class ExchangeImporter 
    {
        public abstract void Run(ExecutionContext context, ExchangeTaskDTO task, string fullPath);
        public abstract bool IsValid(ExchangeTaskDTO task, string fullPath);
    }
}