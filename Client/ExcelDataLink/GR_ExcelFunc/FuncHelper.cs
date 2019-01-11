using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace GR_ExcelFunc
{
    public static class FuncHelper
    {
        public static TOut ExecuteSync<TOut, TIn1, TIn2>(Func<TIn1, TIn2, Task<TOut>> func, TIn1 param1, TIn2 param2)
        {
            Task<TOut> @out = func(param1, param2);
            @out.Wait();
            return @out.Result;
        }
        public static TOut ExecuteSync<TOut, TIn>(Func<TIn, Task<TOut>> func, TIn param)
        {
            Task<TOut> @out = func(param);
            @out.Wait();
            return @out.Result;
        }
        public static void ExecuteSync<TIn>(Func<TIn, Task> action, TIn param)
        {
            Task @out = action(param);
            @out.Wait();
        }
    }
}
