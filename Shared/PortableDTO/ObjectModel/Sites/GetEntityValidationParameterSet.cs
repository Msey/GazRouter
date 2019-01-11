using System;

namespace GazRouter.DTO.ObjectModel.Sites
{
    public class GetEntityValidationParameterSet
    {
        public DateTime? KeyDate { get; set; }
        public int? SerieId { get; set; }
        public Guid? EntityId { get; set; }
    }
}