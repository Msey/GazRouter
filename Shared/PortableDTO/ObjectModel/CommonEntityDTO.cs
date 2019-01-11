using System;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.EntityTypes;

namespace GazRouter.DTO.ObjectModel
{
    [DataContract]
    public class CommonEntityDTO : NamedDto<Guid>
    {
        private string _shortPath;
        private string _path;

        [DataMember]
        public bool IsVirtual { get; set; }

        [DataMember]
        public virtual EntityType EntityType { get; set; }

        [DataMember]
        public string Path
        {
            get { return _path ?? string.Empty; }
            set { _path = value; }
        }

        [DataMember]
        public string ShortPath
        {
            get { return _shortPath ?? string.Empty; }
            set { _shortPath = value; }
        }


        [DataMember]
		public int SortOrder { get; set; }

        [DataMember]
        public string Description { get; set; }

        public string DisplayShortPath => !string.IsNullOrEmpty(ShortPath) ? ShortPath : Name;

        [DataMember]
        public string BalanceName { get; set; }

        [DataMember]
        public int? BalanceGroupId { get; set; }

        [DataMember]
        public string BalanceGroupName { get; set; }

        [DataMember]
        public int? OwnBalanceGroupId { get; set; }

        [DataMember]
        public string OwnBalanceGroupName { get; set; }
    }
}
