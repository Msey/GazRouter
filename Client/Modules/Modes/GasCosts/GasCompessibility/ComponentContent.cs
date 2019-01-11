using GazRouter.DTO.Dictionaries.PropertyTypes;

namespace GazRouter.Modes.GasCosts.GasCompessibility
{
    public class ComponentContent
    {
        /// <summary>
        /// ��������� ����, ������������
        /// </summary>
        public PropertyType Component { get; set; }

        /// <summary>
        /// ���������� ����������, ���. ����
        /// </summary>
        public double Concentration { get; set; }
    }
}