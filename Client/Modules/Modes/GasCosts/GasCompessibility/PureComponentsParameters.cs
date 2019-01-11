using GazRouter.DTO.Dictionaries.PropertyTypes;

namespace GazRouter.Modes.GasCosts.GasCompessibility
{
    public class PureComponentsParameters
    {
        public PropertyType Component { get; set; }

        /// <summary>
        /// �������� �����,��/�����
        /// </summary>
        public double Mi { get; set; }
         
        /// <summary>
        /// ����������� ����������� ��� ����������� ��������, -
        /// </summary>
        public double zci { get; set;}

        /// <summary>
        /// �������������� ��������, -
        /// </summary>
        public double Ei { get; set; }

        /// <summary>
        /// �������� �������, (�3/�����)^(1/3)
        /// </summary>
        public double Ki { get; set; }

        /// <summary>
        /// �������������� ��������, -
        /// </summary>
        public double Gi { get; set; }

        /// <summary>
        /// ������������� ��������, -
        /// </summary>
        public double Qi { get; set; }

        /// <summary>
        /// ������������������� ��������, -
        /// </summary>
        public double Fi { get; set; }

        /// <summary>
        /// ��������� ��������, -
        /// </summary>
        public double Si { get; set; }

        /// <summary>
        /// �������� ����������, -
        /// </summary>
        public double Wi { get; set; }
    }
}