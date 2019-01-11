using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GazRouter.DTO.Authorization.TargetingList
{
    public class AddEditTargetListUserParameterSet
    {
        public int? Id { get; set; }
        public int EntityTypeId { get; set; }
        public int AgreedUserId { get; set; }
        public int SortNum { get; set; }
        public Guid? SiteId { get; set; }
        public string FIO { get; set; }
        public string Department { get; set; }
        public string Fax { get; set; }
        public bool IsCpdd { get; set; } = false;
    }
}
