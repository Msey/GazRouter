using System.Runtime.Serialization;

namespace GazRouter.DTO.Dictionaries
{
	[DataContract]
	public class BaseDictionaryDTO : BaseDto<int>
	{
	    [DataMember]
		public string Name { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
                return false;
            
            return ((BaseDictionaryDTO) obj).Id == Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
	}
}
