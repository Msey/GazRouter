
using System;

namespace GazRouter.DTO.Authorization.User
{
    public class EditUserParameterSet
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Description { get; set; }
        public string Phone { get; set; }
        public Guid? SiteId { get; set; }
    }
}
