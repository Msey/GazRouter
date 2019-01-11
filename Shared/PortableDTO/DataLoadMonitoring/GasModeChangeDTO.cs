using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.StatesModel ;
namespace GazRouter.DTO.DataLoadMonitoring
{
    [DataContract]
    public class GasModeChangeData
    {
       //объекты и параметры, по которым поменялись значения 
       [DataMember]
        public List<CompressorShopValuesChangeDTO> CompShopChangedValues { get; set; }
       [DataMember]
       public List<MeasureLineGasFlowChangeDTO> MeasureLineChangedValues { get; set; }
       [DataMember]
       public List<ConsumerGasFlowChangeDTO> ConsumerChangedValues { get; set; }
       [DataMember]
       public DateTime KeyDate1 { get; set; }
       [DataMember]
       public DateTime KeyDate2 { get; set; }
    }

    [DataContract]
    public class Valve
    {
        [DataMember]
        public string ValveName { get; set; }

        [DataMember]
        public Guid? ValveId { get; set; }

        [DataMember]
        public ChangeModeValueDouble Valve20PressureInlet { get; set; }

        [DataMember]
        public ChangeModeValueDouble Valve20PressureOutlet { get; set; }

        [DataMember]
        public ChangeModeValueDouble Valve20TemperatureInlet { get; set; }

        [DataMember]
        public ChangeModeValueDouble Valve20TemperatureOutlet { get; set; }

        [DataMember]
        public ChangeModeValue<int?> ValveState { get; set; }

        [DataMember]
        public ChangeModeValue<int?> StateByPass1 { get; set; }

        [DataMember]
        public ChangeModeValue<int?> StateByPass2 { get; set; }

        [DataMember]
        public ChangeModeValue<int?> StateByPass3 { get; set; }

        [DataMember]
        public string Color { get; set; }

        [DataMember]
        public string TooltipText { get; set; }

        public void SetValveColor()
        {
            if (ValveState.Value.HasValue == false)
            {
                Color = "#FFA29D9D";
                TooltipText = "Нет данных о состоянии";
            }
            else
            {
                switch ((ValveState) ValveState.Value.Value)
                {
                    case Dictionaries.StatesModel.ValveState.Opened:
                        Color = "#FF2AC72A"; // светло-зеленый
                        TooltipText = "Открыт";
                        break;
                    case Dictionaries.StatesModel.ValveState.Closed:
                        Color = "#FFF60505"; // красный;
                        TooltipText = "Закрыт";
                        if (StateByPass1.Value.HasValue && (ValveState) StateByPass1.Value.Value == Dictionaries.StatesModel.ValveState.Opened)
                            {
                                Color = "#FFE3EE01"; //желтый
                                TooltipText = "Кран закрыт, байпас  открыт";
                            }
                        else if (StateByPass2.Value.HasValue && (ValveState) StateByPass2.Value.Value == Dictionaries.StatesModel.ValveState.Opened)
                            {
                                Color = "#FFE3EE01"; //желтый
                                TooltipText = "Кран закрыт, байпас  открыт";
                            }
                        else if (StateByPass3.Value.HasValue && (ValveState) StateByPass3.Value.Value == Dictionaries.StatesModel.ValveState.Opened)
                            {
                                Color = "#FFE3EE01"; //желтый
                                TooltipText = "Кран закрыт, байпас  открыт";
                            }
                        break;
                    case Dictionaries.StatesModel.ValveState.TransitionState:
                        Color = "#FFE3EE01"; //синий
                        TooltipText = "Переходное состояние";

                        break;
                    default:
                        Color = "#FF887979"; //серый
                        TooltipText = "---";
                        break;
                }
            }
        }
    }

    [DataContract]
    public class CompressorShopValuesChangeDTO
    {
        [DataMember]
        public Guid SiteId { get; set; }
        [DataMember]
        public string SiteName { get; set; }
        [DataMember]
        public Guid CompStationId { get; set; }
        [DataMember]
        public string CompStationName { get; set; }
        [DataMember]
        public string CompShopName { get; set; }
        [DataMember]
        public Guid CompShopId { get; set; }
        [DataMember]
        public Valve Valve20 { get; set; }
        [DataMember]
        public ChangeModeValueDouble CompShopPressureInlet { get; set; }
        [DataMember]
        public ChangeModeValueDouble CompShopPressureOutlet { get; set; }
        [DataMember]
        public ChangeModeValueDouble CompShopTemperatureInlet { get; set; }
        [DataMember]
        public ChangeModeValueDouble CompShopTemperatureOutlet { get; set; }
        [DataMember]
        public ChangeModeValueDouble CompShopTemperatureCooling { get; set; }
        [DataMember]
        public ChangeModeValue<string> CompShopScheme { get; set; } 
    }


    [DataContract]
    public abstract class BaseGasFlowChangeDTO
    {
        [DataMember]
        public Guid SiteId { get; set; }
        [DataMember]
        public string SiteName { get; set; }
        [DataMember]
        public ChangeModeValueDouble Q { get; set; }
        [DataMember]
        public ChangeModeValueDouble ParentQ { get; set; }
    }

    [DataContract]
    public class ConsumerGasFlowChangeDTO : BaseGasFlowChangeDTO
    {
        [DataMember]
        public Guid DistrStationId  { get; set; }
        [DataMember]
        public string DistrStationName { get; set; }
        [DataMember]
        public Guid GasConsumerId  { get; set; }
        [DataMember]
        public string GasConsumerName { get; set; }    
        [DataMember]
        public string ConsumerTypeName { get; set; }  
        [DataMember]
        public string RegionName { get; set; }  
    }

    [DataContract]
    public class MeasureLineGasFlowChangeDTO : BaseGasFlowChangeDTO
    {
        [DataMember]
        public Guid MeasStationId  { get; set; }
        [DataMember]
        public string MeasStationName { get; set; }
        [DataMember]
        public Guid MeasLineId  { get; set; }
        [DataMember]
        public string MeasLineName { get; set; }    
    }

}
