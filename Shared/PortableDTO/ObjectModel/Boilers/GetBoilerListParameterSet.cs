using System;
namespace GazRouter.DTO.ObjectModel.Boilers
{
    public class GetBoilerListParameterSet
    {
        public int? SystemId { get; set; }
        public Guid? SiteId { get; set; }
    }
}
