namespace GazRouter.DTO.ObjectModel.Consumers
{
    public class EditConsumerParameterSet : EditEntityParameterSet
    {
        public int ConsumerType { get; set; }
        public int RegionId { get; set; }

        public int? DistrNetworkId { get; set; }
    }
}
