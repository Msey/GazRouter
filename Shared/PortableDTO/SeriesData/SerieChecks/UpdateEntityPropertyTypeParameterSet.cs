

namespace GazRouter.DTO.SeriesData.SerieChecks
{
    public class UpdateEntityPropertyTypeParameterSet
    {
        public int EntityTypeId { get; set; }

        public int PropertyTypeId { get; set; }

        public bool? IsMandatory { get; set; }

        public bool? IsInput { get; set; }
    }
}
