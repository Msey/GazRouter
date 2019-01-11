using System.Runtime.Serialization;

namespace GazRouter.DTO.Calculations
{
    [DataContract]
    public struct SerializableTuple<T1, T2>
    {
        public SerializableTuple(T1 item1, T2 item2) : this()
        {
            Item1 = item1;
            Item2 = item2;
        }

        [DataMember]
        public T1 Item1 { get; set; }
        [DataMember]
        public T2 Item2 { get; set; }
    }
}
