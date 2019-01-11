using GazRouter.DTO.Stoppages.AddStoppage;

namespace GazRouter.DTO.Stoppages.EditStoppage
{
    public class EditStoppageParameterSet : AddStoppageParameterSet
    {
        ///// <summary>
        ///// ID вида отказа
        ///// </summary>
        //public CompUnitStopType CompUnitStopType { get; set; } 
      
        ///// <summary>
        ///// ID признака отказа
        ///// </summary>
        //public CompUnitFailureFeature CompUnitFailureFeature { get; set; }

        ///// <summary>
        ///// причина отказа
        ///// </summary>
        //public CompUnitFailureCause CompUnitFailureCause { get; set; }

        ///// <summary>
        ///// дата отказа
        ///// </summary>
        //public DateTime? FailureDate { get; set; }

        ///// <summary>
        ///// дата запуска резервного ГПА
        ///// </summary>
        //public DateTime? ReserveUnitStartDate { get; set; }

        ///// <summary>
        ///// Дата загрузки или вывода в резерв ремонтируемого ГПА
        ///// </summary>
        //public DateTime? LoadDate { get; set; }

        ///// <summary>
        ///// Внешнее проявление отказа
        ///// </summary>
        //public string StoppageExternalView { get; set; }
        
        ///// <summary>
        ///// Описание причины отказа
        ///// </summary>
        //public string StoppageCauseDescription { get; set; }

        ///// <summary>
        ///// Ремонтные работы
        ///// </summary>
        //public string WorkPerformed { get; set; }

        ///// <summary>
        ///// Отправлено письмо в ЦПДД
        ///// </summary>
        //public bool? IsTransferAffected { get; set; }

        ///// <summary>
        ///// ID сломанного ГПА
        ///// </summary>
        //public Guid FailedUnitId { get; set; }

        ///// <summary>
        ///// ID резервных ГПА
        ///// </summary>
        //public Guid[] ReserveEntityId { get; set; }

        /// <summary>
        /// ID StoppageDTO
        /// </summary>
        public int StoppageId { get; set; }
    }
}
