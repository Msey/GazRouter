using System;
using GazRouter.DTO.ObjectModel.Entities;

namespace GazRouter.DTO.ObjectModel.MeasStations
{
    public class GetMeasStationListParameterSet : GetEntityListParameterSetBase
    {
        public int? SystemId { get; set; }
        public Guid? SiteId { get; set; }

        /// <summary>
        /// Для того чтобы получить ГИСы только своего предприятия, нужно установить этот параметр в TRUE
        /// </summary>
        public bool ThisEnterprise { get; set; }

        public Guid? EnterpriseId { get; set; }
    }
}