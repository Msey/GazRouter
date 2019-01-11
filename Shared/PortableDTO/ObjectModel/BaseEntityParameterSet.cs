using System;

namespace GazRouter.DTO.ObjectModel
{
    public class BaseEntityParameterSet
    {
        public string Name { get; set; }
        public Guid? ParentId { get; set; }
        public bool IsHidden { get; set; }
        public bool IsVirtual { get; set; }

        public bool UseInBalance { get; set; }

        public string BalanceName { get; set; }

        public int? BalanceGroupId { get; set; }
    }
}