namespace GazRouter.DTO.Bindings.Sources
{
    public class AddEditSourceParameterSet
    {
        public int? SourceId { get; set; }

        public string SourceName { get; set; }

        public string SystemName { get; set; }

        public string Description { get; set; }

        public bool IsHidden { get; set; }

        public bool IsReadonly { get; set; }


    }
}