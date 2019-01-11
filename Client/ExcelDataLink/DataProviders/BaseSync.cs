using System;
using System.Threading.Tasks;

namespace DataProviders
{
    public class BaseSync
    {
        public static TOut ExecuteSync<TOut, TIn1, TIn2>(Func<TIn1, TIn2, Task<TOut>> func, TIn1 param1, TIn2 param2)
        {
            var task = Task.Run(async () => await func(param1, param2));
            return task.Result;
        }

        protected static TOut ExecuteSync<TOut, TIn>(Func<TIn, Task<TOut>> func, TIn param)
        {
            var task = Task.Run(async () => await func(param));
            //var task = Task.Run(() => func(param));
            //task.ConfigureAwait(false);
            //task.Wait();
            return task.Result;
        }
        protected static TOut ExecuteSync<TOut>(Func<Task<TOut>> func)
        {
            var task = Task.Run(async () => await func());
            //var task = Task.Run(func);
            //task.ConfigureAwait(false);
            //task.Wait();
            return task.Result;
        }
        public static void ExecuteSync<TIn>(Func<TIn, Task> func, TIn param)
        {
            var task = Task.Run(async () => await func(param));
            //var task = Task.Run(() => func(param));
            //task.ConfigureAwait(false);
            task.Wait();
        }
    }
}