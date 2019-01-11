using System.Runtime.Serialization;

namespace GazRouter.DTO.Dictionaries.RepairExecutionMeans
{
    [DataContract]
    public class RepairExecutionMeansDTO : BaseDictionaryDTO
    {
        public ExecutionMeans ExecutionMeans
        {
            get { return (ExecutionMeans)Id; }
        }
        
    }
}
