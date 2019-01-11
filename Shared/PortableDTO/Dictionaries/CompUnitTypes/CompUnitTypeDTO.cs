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
        /// �������������� ����� ������� ��, �� �.�./���-�
        /// </summary>
        [DataMember]
        public double GasConsumptionRate { get; set; }

        /// <summary>
        /// ����������� ��������, ���
        /// </summary>
        [DataMember]
        public double RatedPower { get; set; }

        /// <summary>
        /// ����������� ��� �����������
        /// </summary>
        [DataMember]
        public double RatedEfficiency { get; set; }

        /// <summary>
        /// �������������� ����� ������� ��������������
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
        /// ����������� ����������� ������������ ��������� �� ��������
        /// </summary>
        [DataMember]
        public double? KTechStatePow { get; set; }

        /// <summary>
        /// ����������� ����������� ������������ �������� ��� �� ���������� ���� 
        /// </summary>
        [DataMember]
        public double? KTechStateFuel { get; set; }

        /// <summary>
        /// ������ � ��� ��������������, ����������� ������� ����������� �������
        /// </summary>
        [DataMember]
        public List<CompUnitKtAirDTO> CompUnitKtAirs { get; set; }

        /// <summary>
        /// ��� ����������������
        /// </summary>
        [DataMember]
        public double? MotorisierteEfficiencyFactor { get; set; }

        /// <summary>
        /// ��� ���������
        /// </summary>
        [DataMember]
        public double? ReducerEfficiencyFactor { get; set; }

	}
}