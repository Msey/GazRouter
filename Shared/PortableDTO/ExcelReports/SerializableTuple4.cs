using System.Runtime.Serialization;

namespace GazRouter.DTO.ExcelReports
{
    [DataContract]
    public struct SerializableTuple4<T1, T2, T3, T4>
    {
        public SerializableTuple4(T1 item1, T2 item2, T3 item3, T4 item4) : this()
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
            Item4 = item4;
        }

        [DataMember]
        public T1 Item1 { get; set; }
        [DataMember]
        public T2 Item2 { get; set; }
        [DataMember]
        public T3 Item3 { get; set; }
        [DataMember]
        public T4 Item4 { get; set; }
    }
}