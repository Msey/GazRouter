using System;

namespace GazRouter.DataServices.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ServiceActionAttribute : Attribute
    {
        private readonly string _actionDescription;
        private readonly bool _allowedByDefault;

        public bool AllowedByDefault
        {
            get { return _allowedByDefault; }
        }

        public string ActionDescription
        {
            get { return _actionDescription; }
        }

        public ServiceActionAttribute(string actionDescription, bool allowedByDefault = false)
        {
            _actionDescription = actionDescription;
            _allowedByDefault = allowedByDefault;
        }
    }
}