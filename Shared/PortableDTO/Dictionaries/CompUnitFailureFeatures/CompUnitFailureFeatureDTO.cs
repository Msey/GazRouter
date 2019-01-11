namespace GazRouter.DTO.Dictionaries.CompUnitFailureFeatures
{
    public class CompUnitFailureFeatureDTO : BaseDictionaryDTO
    {
        public CompUnitFailureFeature CompUnitFailureFeature
        {
            get { return (CompUnitFailureFeature) Id; }
        }

        public string Description { get; set; }
    }
}