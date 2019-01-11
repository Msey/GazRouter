using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GazRouter.DTO.DataExchange.ExchangeTask;
using GazRouter.Log;

namespace GazRouter.Service.Exchange.Lib.Run
{
    public class ExchangeTasksRegistry
    {
        private readonly Func<List<ExchangeTaskDTO>> _refreshTasks;
        //private static readonly Lazy<ExchangeTasksRegistry> Instance = new Lazy<ExchangeTasksRegistry>(() => new ExchangeTasksRegistry());
        private List<ExchangeTaskDTO> _tasks;

        public ExchangeTasksRegistry(Func<List<ExchangeTaskDTO>> refreshTasks)
        {
            _refreshTasks = refreshTasks;

            LazyInitializer.EnsureInitialized(ref _tasks, () => _refreshTasks());
        }

        public ExchangeTaskDTO Run(Func<ExchangeTaskDTO, bool> runner, bool updateTasks = true)
        {
            ExchangeTaskDTO result = null;
            if ((result = _tasks.FirstOrDefault(runner)) == null && updateTasks)
            {
                //Если есть неподгруженные таски, то подгружаем их
                _tasks = _refreshTasks();
                result = _tasks.FirstOrDefault(runner);
            }
            return result;
        }

        public void RunAll(Action<ExchangeTaskDTO> action)
        {
            _tasks.ForEach(action);
        }

    }
}