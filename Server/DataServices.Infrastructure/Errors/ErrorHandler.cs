using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using GazRouter.DAL.Core;
using GazRouter.DataServices.Infrastructure.Authorization;
using GazRouter.DTO.Infrastructure.Faults;
using GazRouter.Log;
using NLog;

namespace GazRouter.DataServices.Infrastructure.Errors
{
    public class ErrorHandler : IErrorHandler
    {
        private Guid _logRecordId;
        private LogLevel _level;
        private string _actionPath;
        private string _login;
        private readonly MyLogger _logger;

        public ErrorHandler(MyLogger logger)
        {
            _logger = logger;
        }
        
        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            _logRecordId = Guid.NewGuid();
            _actionPath = OperationContext.Current.GetActionPath();
            _login = OperationContext.Current.GetLogin();

            var ex = error as FaultException;
            FaultDetail detail;
            if ((ex != null) && (ex.Message == "Access is denied."))
            {
                var reason = OperationContext.Current.GetAccessDeniedReason();
                switch (reason)
                {
                    case AccessDeniedReason.Rights:
                        {
                            var action = ServiceActions.AllActions.First(item => item.ActionPath == _actionPath);
                            detail = new FaultDetail
                                {
                                    FaultType = FaultType.AccessDenied,
                                    LogRecordId = _logRecordId,
                                    Message = action.ActionDescription
                                };
                            _level = LogLevel.Warn;
                        }
                        break;
                    case AccessDeniedReason.UserNotRegistered:
                        detail = new FaultDetail
                        {
                            FaultType = FaultType.UserNotRegistered,
                            LogRecordId = _logRecordId,
                            Message = _login
                        };
                        _level = LogLevel.Warn;
                        break;
                    case AccessDeniedReason.Version:
                        detail = new FaultDetail
                        {
                            FaultType = FaultType.VersionIncompatible,
                            LogRecordId = _logRecordId,
                        };
                        _level = LogLevel.Warn;
                        break;
                    default:
                        detail = new FaultDetail { FaultType = FaultType.Error, LogRecordId = _logRecordId };
                        _level = LogLevel.Error;
                        break;
                }
            }
            else
            {
                if (error is IntegrityConstraintException)
                {
                    detail = new FaultDetail { FaultType = FaultType.IntegrityConstraint, LogRecordId = _logRecordId, Message = error.Message};
                    _level = LogLevel.Info;
                }
                else
                {
                    detail = new FaultDetail { FaultType = FaultType.Error, LogRecordId = _logRecordId };
                    _level = LogLevel.Error;
                }
            }



            var faultException = new FaultException<FaultDetail>(detail, detail.FaultType.ToString());
            var messageFault = faultException.CreateMessageFault();

            fault = Message.CreateMessage(version, messageFault, faultException.Action);
            fault.Properties[HttpResponseMessageProperty.Name] = new HttpResponseMessageProperty { StatusCode = HttpStatusCode.OK };
        }

        public bool HandleError(Exception error)
        {
            //в некоторых случаях HandleError вызывается без вызова ProvideFault
            if (_logRecordId == Guid.Empty)
                _logRecordId = Guid.NewGuid();
            if (_level == null)
                _level = LogLevel.Error;
#if DEBUG
            if (Debugger.IsAttached && !(error is IntegrityConstraintException))
            {
                Debugger.Break();
            }
#endif
            _logger.WriteServiceException(_logRecordId, _actionPath, error, _level);

           return true;
        }
    }
}