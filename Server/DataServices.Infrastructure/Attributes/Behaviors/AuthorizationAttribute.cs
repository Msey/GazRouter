using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using GazRouter.DataServices.Infrastructure.Authorization;

namespace GazRouter.DataServices.Infrastructure.Attributes.Behaviors
{
    public class AuthorizationAttribute : Attribute, IServiceBehavior
    {
        private readonly bool _checkRights;
        public AuthorizationAttribute(bool checkRights = true)
        {
            _checkRights = checkRights;
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            var authorizationBehavior = serviceHostBase.Description.Behaviors.Find<ServiceAuthorizationBehavior>();

            authorizationBehavior.ServiceAuthorizationManager = _checkRights ? new AuthorizationManager() : new AuthorizationManagerVersion();
        }
    }
}