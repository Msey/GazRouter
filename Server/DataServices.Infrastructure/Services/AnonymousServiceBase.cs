using System;
using System.Linq;
using GazRouter.DAL.Core;
using GazRouter.DataServices.Infrastructure.Attributes.Behaviors;
using GazRouter.DataServices.Infrastructure.Sessions;
using GazRouter.Log;

namespace GazRouter.DataServices.Infrastructure.Services
{
	public abstract class AnonymousServiceBase
	{
        protected readonly MyLogger _logger;
        
        protected AnonymousServiceBase()
        {
            var attr = GetType().GetCustomAttributes(typeof (ErrorHandlerLoggerAttribute), true).FirstOrDefault();
            if (attr != null)
            {
                _logger = ((ErrorHandlerLoggerAttribute) attr).Logger;
            }
            if (_logger == null)
                throw new Exception("Логгер не задан.");
        }

        protected TResult ExecuteRead<TCommand, TResult>()
            where TCommand : QueryBase<TResult>
        {
            using (var context = OpenDbContext())
            {
                var command = (TCommand)Activator.CreateInstance(typeof(TCommand), context);
                return command.Execute();
            }
        }

        protected TResult ExecuteRead<TCommand, TResult, TParams>(TParams parameters)
            where TCommand : QueryBase<TParams, TResult>
        {
            using (var context = OpenDbContext())
            {
                var command = (TCommand)Activator.CreateInstance(typeof(TCommand), context);
                return command.Execute(parameters);
            }
        }


        protected void ExecuteNonQuery<TCommand, TParams>(TParams parameters)
            where TCommand : CommandNonQuery<TParams>
        {
            using (var context = OpenDbContext())
            {
                var command = (TCommand)Activator.CreateInstance(typeof(TCommand), context);
                command.Execute(parameters);
            }
        }

        protected virtual ExecutionContextReal OpenDbContext()
        {
            return DbContextHelper.OpenDbContext(SessionManager.SystemUserLogin, _logger);
        }
    }
}