using System.Globalization;
using System.Runtime.Serialization;
using System.Text;

namespace GazRouter.DTO.DataLoadMonitoring
{
    
    
    
    [DataContract]
    public class ChangeModeValue<T>
    {
        [DataMember]
        public T Value { get; set; }

        [DataMember]
        public T PreviousValue { get; set; }

        [DataMember]
        public string Color { get; set; } 
    }

    [DataContract]
    public class ChangeModeValueDouble : ChangeModeValue<double?>
    {
        public ChangeModeValueDouble()
        {
            Color = "#FF000000";
            
        }

        private string _dif;
        public string Difference
        {
            get { return _dif; }
        }

        public void CalcDiff()
        {
            _dif = CalcDifference();
        }

        private string CalcDifference()
        {
            if (!Value.HasValue || !PreviousValue.HasValue) return string.Empty;

            var dif = Value.Value - PreviousValue.Value;
            if (dif > 0)
            {
                return "+" + dif.ToString("0.0");
            }

            return dif == 0 ? "" : dif.ToString("0.0");
        }
    }
}