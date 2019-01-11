using System.Runtime.Serialization;

namespace GazRouter.DTO
{
    [DataContract]
    public abstract class NamedDto<T> : BaseDto<T>
    {
        private string _name = string.Empty;

        [DataMember]
        public string Name
        {
            get { return _name ?? string.Empty; }
            set { _name = value; }
        }
    }
}