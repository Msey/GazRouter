using System;

namespace GazRouter.DataServices.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class ServiceAttribute : Attribute
    {
        private readonly string _serviceDescription;

        public ServiceAttribute(string serviceDescription)
        {
            _serviceDescription = serviceDescription;
        }

        public string ServiceDescription
        {
            get { return _serviceDescription; }
        }
    }
}