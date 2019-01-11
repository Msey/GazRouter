namespace GazRouter.DataServices.Infrastructure.Authorization
{
    public struct ServiceAction
    {
        public bool IsAllowedByDefault { get; private set; }
        public string ActionPath { get; private set; }
        public string ActionDescription { get; private set; } 
        public string ServiceDescription { get; private set; }

        public ServiceAction(string actionPath, bool isAllowedByDefault, string actionDescription = "", string serviceDescription = "")
            : this()
        {
            IsAllowedByDefault = isAllowedByDefault;
            ActionPath = actionPath;
            ActionDescription = actionDescription;
            ServiceDescription = serviceDescription;
        }

        public bool Equals(ServiceAction other)
        {
            return Equals(other.ActionPath, ActionPath);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(ServiceAction)) return false;
            return Equals((ServiceAction)obj);
        }

        public override int GetHashCode()
        {
            return (ActionPath != null ? ActionPath.GetHashCode() : 0);
        }
    }
}