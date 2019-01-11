using System;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PipelineTypes;

namespace GazRouter.DTO.ObjectModel.Pipelines
{
    [DataContract]
    public class PipelineDTO : GtsEntityDTO
    {
        [DataMember]
        public PipelineType Type { get; set; }

        [DataMember]
        public string TypeName { get; set; }

        [DataMember]
        public Guid? BeginEntityId { get; set; }

        /// <summary>
        ///     Километр подклюючения начала данного газопровода, к другому газопровод, иначе null
        /// </summary>
        [DataMember]
        public double? KilometerOfBeginConn { get; set; }

        [DataMember]
        public Guid? EndEntityId { get; set; }

        /// <summary>
        ///     Километр подклюючения конца данного газопровода, к другому газопроводу, иначе null
        /// </summary>
        [DataMember]
        public double? KilometerOfEndConn { get; set; }

        /// <summary>
        ///     Километр начала данного газпровода т.к. может быть не 0
        /// </summary>
        [DataMember]
        public double KilometerOfStartPoint { get; set; }

        /// <summary>
        ///     Километр конца
        /// </summary>
        [DataMember]
        public double KilometerOfEndPoint { get; set; }

        public string FullName =>
            $"{Name} (км. н-ла = {KilometerOfStartPoint}; длина = {KilometerOfEndPoint - KilometerOfStartPoint} км)";

        public override EntityType EntityType => EntityType.Pipeline;

        public double Length => KilometerOfEndPoint - KilometerOfStartPoint;
    }
}