using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using NLog;

namespace GazRouter.Log
{
    public class MyLogger
    {
		private static readonly ConcurrentDictionary<string, Logger> _loggers = new ConcurrentDictionary<string, Logger>();
        private readonly Logger _logger;

        public MyLogger(string loggerName)
        {
            _logger = _loggers.GetOrAdd(loggerName, LogManager.GetLogger);
        }

        #region ServicesLogging

        public void WriteServiceException(Guid id, string actionPath, Exception exception, LogLevel level)
        {
            _logger?.Log(level,  exception, id + Environment.NewLine + actionPath);
        }

        public void WriteException(Exception exception, string message)
        {
            _logger?.Log(LogLevel.Error,  exception,message);
        }

        public void WriteFullException(Exception exception, string message)
        {
            _logger?.Log(LogLevel.Error, exception, $"Исключение: {message + Environment.NewLine}Стeк исключения: {Environment.NewLine + exception.StackTrace}" );
        }
        public void Error(string message)
        {
            _logger?.Log(LogLevel.Error,  message);
        }

        public void WriteIntegrationServiceException(Guid id, Exception exception, string data)
        {
            _logger?.Log(LogLevel.Error, exception, id + Environment.NewLine + data + Environment.NewLine);
        }

        public void LogCall(Guid id, Func<string> getCallString)
        {
            if (_logger != null)
            {
                //проверка для оптимизации чтобы реквест не сериализовался если трассировка отключена
                if (_logger.IsTraceEnabled)
                {
                    _logger.Log(LogLevel.Trace, FormatCall(id, getCallString()));
                }
            }
        }

        private static string FormatCall(Guid id, string call)
        {
            var sb = new StringBuilder();
            sb.AppendLine(id.ToString());
            sb.AppendLine(call);
            return sb.ToString();
        }

        #endregion

        #region DALLogging

        //private static List<string> queries = new List<string>();
        public void WriteQuery(string query, object parameters)
        {
            _logger?.Log(LogLevel.Debug, GetQueryStrWithParameters(query, parameters));

            //Вывод всех строк запросов к БД
            //if (!queries.Contains(query))
            //{
            //    File.AppendAllText(@"c:\logs\a.txt", query + Environment.NewLine);
            //    queries.Add(query);
            //}
        }

        public void WriteContextAction(Guid contextId, string action)
        {
            _logger?.Log(LogLevel.Debug, $"Context {contextId} {action}.");
        }

        private static string GetQueryStrWithParameters(string query, object parameters)
        {
            var sb = new StringBuilder();
            sb.AppendLine(query);
            sb.AppendLine(SerializeParameters(parameters));
            var mess = sb.ToString();
            return mess;
        }

        private static string SerializeParameters(object parameters)
        {
            if (parameters == null) return string.Empty;
            var type = parameters.GetType();
            if (type.Namespace != null && type.Namespace.StartsWith("System"))
            {
                return $"{type.Name} prarameters = {parameters}";
            }

            var sb = new StringBuilder();
            foreach (var property in type.GetProperties().Where(property => property.CanRead))
            {
                sb.AppendLine($"{property.PropertyType} {property.Name} = {property.GetValue(parameters, null)}");
            }
            return sb.ToString();
        }

        #endregion

        public void Trace(string message, params object[] arguments)
        {
            _logger?.Log(LogLevel.Trace, message, arguments);
        }

        public void Info(string message, params object[] arguments)
        {
            _logger?.Log(LogLevel.Info, message, arguments);
        }

        public void Debug(string message)
        {
            _logger?.Log(LogLevel.Debug, message);
        }
    }
}
