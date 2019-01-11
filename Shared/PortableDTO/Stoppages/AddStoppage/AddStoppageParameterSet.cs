using System;
using GazRouter.DTO.Dictionaries.CompUnitFailureCauses;
using GazRouter.DTO.Dictionaries.CompUnitFailureFeatures;
using GazRouter.DTO.Dictionaries.CompUnitStopTypes;

namespace GazRouter.DTO.Stoppages.AddStoppage
{
    public class AddStoppageParameterSet
    {
        /// <summary>
        /// ID вида отказа
        /// </summary>
        public CompUnitStopType CompUnitStopType { get; set; }

        /// <summary>
        /// ID признака отказа
        /// </summary>
        public CompUnitFailureFeature CompUnitFailureFeature { get; set; }

        /// <summary>
        /// ID причины отказа
        /// </summary>
        public CompUnitFailureCause CompUnitFailureCause { get; set; }

        /// <summary>
        /// дата отказа
        /// </summary>
        public DateTime FailureDate { get; set; }

        /// <summary>
        /// дата запуска зеревного ГПА
        /// </summary>
        public DateTime? ReserveUnitStartDate { get; set; }

        /// <summary>
        /// Дата загрузки или вывода в резерв ремонтируемого ГПА
        /// </summary>
        public DateTime? LoadDate { get; set; }

        /// <summary>
        /// Дата устранения отказа
        /// </summary>
        public DateTime? FixDate { get; set; }

        /// <summary>
        /// Внешнее проявление отказа
        /// </summary>
        public string StoppageExternalView { get; set; }

        /// <summary>
        /// Описание причины отказа
        /// </summary>
        public string StoppageCauseDescription { get; set; }


        /// <summary>
        /// Ремонтные работы
        /// </summary>
        public string WorkPerformed { get; set; }

        /// <summary>
        /// Отправлено письмо в ЦПДД
        /// </summary>
        public bool? IsTransferAffected { get; set; }

        /// <summary>
        /// IDы резервнюых ГПА
        /// </summary>
        public Guid FailedUnitId { get; set; }

        /// <summary>
        /// ID резервнюых ГПА
        /// </summary>
        public Guid[] ReserveEntityId { get; set; }
    }
}
