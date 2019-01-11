using System;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.EntityTypes;

namespace GazRouter.DTO.GasLeaks
{
    [DataContract]
    public class LeakDTO
    {
        [DataMember]
        public int Id { get; set; }

        
        [DataMember]
        public double Kilometer { get; set; }

        [DataMember]
        public string Place { get; set; }

        [DataMember]
        public string Reason { get; set; }

        [DataMember]
        public double VolumeDay { get; set; }
        

        [DataMember]
        public string RepairActivity { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string ContactName { get; set; }

        [DataMember]
        public DateTime CreateDate { get; set; }

        [DataMember]
        public DateTime DiscoverDate { get; set; }
        

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public Guid EntityId { get; set; }

        [DataMember]
        public string EntityName { get; set; }

        [DataMember]
        public string EntityShortPath { get; set; }

        [DataMember]
        public EntityType EntityType { get; set; }

        [DataMember]
        public string SiteName { get; set; }

        [DataMember]
        public DateTime? RepairPlanDate { get; set; }

        [DataMember]
        public DateTime? RepairPlanFact { get; set; }
    }
}
