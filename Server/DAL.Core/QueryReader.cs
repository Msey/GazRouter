using System.Collections.Generic;
using System.Linq;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Core
{
    public abstract class QueryReader<TParameters, TResult> : QueryBase<TParameters, TResult>
    {
        protected QueryReader(ExecutionContext context)
            : base(context)
        {

        }

        public override TResult Execute(TParameters parameters)
        {
            string query = GetCommandTextAndLog(parameters);
            return SafeExecute(parameters, query, (comm, parameters1) =>
                                                      {
                                                          BindParameters(comm, parameters1);
                                                          using (var reader = comm.ExecuteReader())
                                                          {
                                                              return GetResult(reader, parameters1);
                                                          }
                                                      });
        }

        protected abstract TResult GetResult(OracleDataReader reader, TParameters parameters);

        /// <summary>
        /// Создает строку типа (:p0, :p1, :p2,...) для использования в выражения с IN 
        /// </summary>
        /// <param name="count">Кол-во элементов</param>
        /// <param name="prefix">Префикс в наименовании параметров</param>
        /// <returns></returns>
        protected static string CreateInClause(int count, string prefix = "p")
        {
            return "(" + string.Join(",", Enumerable.Range(0, count).Select(g => $":{prefix}{g}")) + ")";
        }
    }

    public abstract class QueryReader<TResult> : QueryBase<TResult>
    {
        protected QueryReader(ExecutionContext context)
            : base(context)
        {

        }

        public override TResult Execute()
        {
            string query = GetCommandTextAndLog();
            return SafeExecute(query, comm =>
                                          {
                                              BindParameters(comm);
                                              using (var reader = comm.ExecuteReader())
                                              {
                                                  return GetResult(reader);
                                              }
                                          });
        }

        protected abstract TResult GetResult(OracleDataReader reader);
    }
}