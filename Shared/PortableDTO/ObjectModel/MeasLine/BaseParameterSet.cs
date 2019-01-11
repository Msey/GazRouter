using System;

namespace GazRouter.DTO.ObjectModel.MeasLine
{
    public abstract class BaseParameterSet
    {
        public Guid? MeasLineId { get; set; }
        public string MeasLineName { get; set; }
        public bool Status { get; set; }
        public int? SortOrder { get; set; }
        public bool Hidden { get; set; }
        public bool IsVirtual { get; set; }
        public Guid? MeasStationId { get; set; }
        public int? BalanceSignId { get; set; }
    }
}