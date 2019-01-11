using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace GazRouter.DTO.ObjectModel
{
    [DataContract]
    public abstract class EntityDTO : GtsEntityDTO
    {
        [DataMember]
        public Guid? ParentId { get; set; }
    }
}
