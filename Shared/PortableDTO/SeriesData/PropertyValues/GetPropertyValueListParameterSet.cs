using System;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;


namespace GazRouter.DTO.SeriesData.PropertyValues
{
	public class GetPropertyValueListParameterSet
    {
        public Guid EntityId { get; set; }
		public PropertyType PropertyTypeId { get; set; }
        public PeriodType PeriodTypeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        /// <summary>
        /// ����� ���������� � tru�, 
        /// ���� �� ������� ���������� ���������� �� ������ ��������,
        /// �� ������ ������ ��� ������ �� �������� ��������
        /// ����� ������ �������, ��� ��������� ��������� ����������� ������
        /// ������������ ������.
        /// </summary>
        public bool LoadMessages { get; set; }
    }
}
