using System.Collections.Generic;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.EngineClasses;
using GazRouter.DTO.ObjectModel.CompUnitKtAirs;

namespace GazRouter.DTO.Dictionaries.CompUnitTypes
{
	public class CompUnitTypeDTO : BaseDictionaryDTO
	{
        public CompUnitTypeDTO()
        {
            CompUnitKtAirs = new List<CompUnitKtAirDTO>();
        }

        /// <summary>
        /// Индивидуальная норма расхода ТГ, кг у.т./кВт-ч
        /// </summary>
        [DataMember]
        public double GasConsumptionRate { get; set; }

        /// <summary>
        /// Номинальная мощность, кВт
        /// </summary>
        [DataMember]
        public double RatedPower { get; set; }

        /// <summary>
        /// Номинальный КПД нагнетателя
        /// </summary>
        [DataMember]
        public double RatedEfficiency { get; set; }

        /// <summary>
        /// Индивидуальная норма расхода электроэнергии
        /// </summary>
        [DataMember]
        public double ElectricityConsumptionRate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string EngineClassName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public EngineClass EngineClassId { get; set; }

        /// <summary>
        /// Номинальный коэффициент технического состояния по мощности
        /// </summary>
        [DataMember]
        public double? KTechStatePow { get; set; }

        /// <summary>
        /// Номинальный коэффициент технического состония ГТУ по топливному газу 
        /// </summary>
        [DataMember]
        public double? KTechStateFuel { get; set; }

        /// <summary>
        /// Список с ном коэффициентами, учитывающий влияние температуры воздуха
        /// </summary>
        [DataMember]
        public List<CompUnitKtAirDTO> CompUnitKtAirs { get; set; }

        /// <summary>
        /// КПД электродвигателя
        /// </summary>
        [DataMember]
        public double? MotorisierteEfficiencyFactor { get; set; }

        /// <summary>
        /// КПД редуктора
        /// </summary>
        [DataMember]
        public double? ReducerEfficiencyFactor { get; set; }

	}
}