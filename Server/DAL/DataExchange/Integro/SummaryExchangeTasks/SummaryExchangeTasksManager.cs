using GazRouter.DAL.Core;
using GazRouter.DAL.DataExchange.ExchangeTask;
using GazRouter.DTO.DataExchange.ExchangeTask;
using GazRouter.DTO.DataExchange.Integro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazRouter.DAL.DataExchange.Integro.SummaryExchangeTasks
{
    public class SummaryExchangeTasksManager
    {
        private ExecutionContext context;
        public SummaryExchangeTasksManager(ExecutionContext context)
        {
            this.context = context;
        }
        public Guid SaveSummaryExchangeTask(SummaryExchTaskParamSet param)
        {
            if (param.ExchTaskParam.Id > 0)
            {
                new EditExchangeTaskCommand(context).Execute(param.ExchTaskParam);
            }
            else
            {
                var taskId = new AddExchangeTaskCommand(context).Execute(param.ExchTaskParam);
                param.SummatyParam.ExchangeTaskId = taskId;
            }
            new AddEditSummaryCommand(context).Execute(param.SummatyParam);
            return param.SummatyParam.Id; // Пока создается на клиенте, переделать на int и создавать в базе
        }
    }
}
