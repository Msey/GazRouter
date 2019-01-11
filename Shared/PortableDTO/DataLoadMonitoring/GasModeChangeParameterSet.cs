using System;

namespace GazRouter.DTO.DataLoadMonitoring
{
    public class GasModeChangeParameterSet
    {
        //метка текущего режима 
        public DateTime KeyDate1 { get; set; }
        //метка времени режима для сравнения
        public DateTime KeyDate2 { get; set; }
        //уставка по давлению 
        public Double PLimit { get; set; }
        //уставка по температуре 
        public Double  TLimit { get; set; }
        //уставка по расходу газа (проценты)
        public Double QLimit { get; set; }
    }
}
