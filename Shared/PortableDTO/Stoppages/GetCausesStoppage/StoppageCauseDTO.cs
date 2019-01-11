
using System.Runtime.Serialization;

namespace GazRouter.DTO.Stoppages.GetCausesStoppage
{
    [DataContract]
    public class StoppageCauseDTO
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }
    }
}
