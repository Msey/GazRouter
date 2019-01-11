using System.Runtime.Serialization;

namespace GazRouter.DTO.SystemVariables
{
    [DataContract]
    public class IusVariableDTO : BaseDto<string>
    {
        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string Value { get; set; }

        [DataMember]
        public string Name
        {
            get { return Id; }
            set { Id = value; }
        }
    }

}