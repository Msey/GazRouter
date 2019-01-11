using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace GazRouter.DTO.DataExchange.Integro
{
    [DataContract]
    public class SummatyLoadResult
    {
        [DataMember]
        public Guid SummaryId { get; set; }

        private List<AddEditSummaryPParameterSet> notFoundEntity = new List<AddEditSummaryPParameterSet>();
        [DataMember]
        public List<AddEditSummaryPParameterSet> NotFoundEntity
        {
            get { return notFoundEntity; }
            set { notFoundEntity = value; }
        }

        private List<AddEditSummaryPParameterSet> dublicateEntity = new List<AddEditSummaryPParameterSet>();
        [DataMember]
        public List<AddEditSummaryPParameterSet> DublicateEntity
        {
            get { return dublicateEntity; }
            set { dublicateEntity = value; }
        }

        private List<AddEditSummaryPParameterSet> loadedEntity = new List<AddEditSummaryPParameterSet>();
        [DataMember]
        public List<AddEditSummaryPParameterSet> LoadedEntity
        {
            get { return loadedEntity; }
            set { loadedEntity = value; }
        }
    }
}
