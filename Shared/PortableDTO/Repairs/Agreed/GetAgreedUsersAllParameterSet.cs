using System;
namespace GazRouter.DTO.Repairs.Agreed
{

    public class GetAgreedUsersAllParameterSet
    {
        public DateTime? TargetDate { get; set; }
        public Guid? SiteId { get; set; }
    }


}