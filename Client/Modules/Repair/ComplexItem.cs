using System;
using GazRouter.DTO.Repairs.Complexes;

namespace GazRouter.Repair
{
    public class ComplexItem
    {
        private readonly ComplexDTO _dto;

        public ComplexItem(ComplexDTO dto, RepairMainViewModel repairMainViewModel)
        {
            RepairMainViewModel = repairMainViewModel;
            _dto = dto;
        }

        public RepairMainViewModel RepairMainViewModel { get; private set; }

        /// <summary>
        /// Идентификатор комплекса
        /// </summary>
        public int Id => _dto.Id;

        public ComplexDTO Dto => _dto;

        /// <summary>
        /// Наименование комплекса
        /// </summary>
        public string Name => _dto.ComplexName;

        /// <summary>
        /// Дата начала комплекса
        /// </summary>
        public DateTime StartDate => _dto.StartDate;

        /// <summary>
        /// Дата окончания комплекса
        /// </summary>
        public DateTime EndDate => _dto.EndDate;

        /// <summary>
        /// Признак локального комплекса
        /// </summary>
        public bool IsLocal => _dto.IsLocal;

        /// <summary>
        /// Идентификатор газотранспортной системы
        /// </summary>
        public int SystemId => _dto.SystemId;

        /// <summary>
        /// Ошибки по работам включенным в комплекс
        /// </summary>
        public bool HasErrors { get; set; }

    }
}