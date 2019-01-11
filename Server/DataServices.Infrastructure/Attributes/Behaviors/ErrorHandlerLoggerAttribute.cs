using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using GazRouter.DataServices.Infrastructure.Errors;
using GazRouter.Log;

namespace GazRouter.DataServices.Infrastructure.Attributes.Behaviors
{
    public class ErrorHandlerLoggerAttribute : Attribute, IServiceBehavior
    {
        public MyLogger Logger { get; private set; }
    

        public ErrorHandlerLoggerAttribute(string loggerName)
        {
            Logger = new MyLogger(loggerName);
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (var channelDispatcherBase in serviceHostBase.ChannelDispatchers)
            {
                var channelDispatcher = (ChannelDispatcher) channelDispatcherBase;
                channelDispatcher.ErrorHandlers.Add(new ErrorHandler(Logger));

                foreach (var endpoint in channelDispatcher.Endpoints)
                {
                    endpoint.DispatchRuntime.MessageInspectors.Add(new CallInspector(Logger));
                }
            }
        }

        private class CallInspector : IDispatchMessageInspector
        {
            private Guid _logRecordId;
            private readonly MyLogger _logger;

            public CallInspector(MyLogger logger)
            {
                _logger = logger;
            }

            #region IDispatchMessageInspector

            public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
            {
                _logRecordId = Guid.NewGuid();
               var req = request;
               _logger.LogCall(_logRecordId, req.ToString);
                return null;
            }

            public void BeforeSendReply(ref Message reply, object correlationState)
            {
                var rep = reply;
                _logger.LogCall(_logRecordId, rep.ToString);
            }

            #endregion
        }
    }

}