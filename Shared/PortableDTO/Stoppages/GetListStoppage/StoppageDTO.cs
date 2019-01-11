using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.CompUnitFailureCauses;
using GazRouter.DTO.Dictionaries.CompUnitFailureFeatures;
using GazRouter.DTO.Dictionaries.CompUnitStopTypes;
using GazRouter.DTO.Stoppages.Attachments;
using GazRouter.DTO.Stoppages.GetListReserveUnit;

namespace GazRouter.DTO.Stoppages.GetListStoppage
{
    [DataContract]
    public class StoppageDTO
    {
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// Дата отказа
        /// </summary>
        [DataMember]
        public DateTime FailureDate { get; set; }



        /// <summary>
        /// Дата и время запуска резервного ГПА
        /// </summary>
        [DataMember]
        public DateTime? ReserveUnitStartDate { get; set; }


        /// <summary>
        /// Дата и время загрузки или вывода в резерв
        /// </summary>
        [DataMember]
        public DateTime? LoadDate { get; set; }



        /// <summary>
        /// Дата устранения отказа
        /// </summary>
        [DataMember]
        public DateTime? FixDate { get; set; }

   

        /// <summary>
        /// флаг: отправлено сообщение в ЦПДД об отказе
        /// </summary>
        [DataMember]
        public bool IsTransferAffected { get; set; }

        /// <summary>
        /// Внешнее проявление отказа
        /// </summary>
        [DataMember]
        public string StoppageExternalView { get; set; }

        /// <summary>
        /// Описание причины отказа
        /// </summary>
        [DataMember]
        public string StoppageCauseDescription { get; set; }

        /// <summary>
        /// Выполненные работы по устранению причин отказа
        /// </summary>
        [DataMember]
        public string WorkPerformed { get; set; }

        /// <summary>
        /// Вид отказа ГПА
        /// </summary>
        [DataMember]
        public CompUnitStopType CompUnitStopType { get; set; }

        /// <summary>
        /// Признак отказа ГПА (R1-R7)
        /// </summary>
        [DataMember]
        public CompUnitFailureFeature CompUnitFailureFeature { get; set; }

        /// <summary>
        /// Классификация причины отказа
        /// </summary>
        [DataMember]
        public CompUnitFailureCause CompUnitFailureCause { get; set; }

        /// <summary>
        /// Идентификатор ЛПУ
        /// </summary>
        [DataMember]
        public Guid SiteId { get; set; }

        /// <summary>
        /// Наименование ЛПУ
        /// </summary>
        [DataMember]
        public string SiteName { get; set; }


        /// <summary>
        /// Идентификатор КС
        /// </summary>
        [DataMember]
        public Guid CompressorStationId { get; set; }

        /// <summary>
        /// Наименование КС
        /// </summary>
        [DataMember]
        public string CompressorStationName { get; set; }


        /// <summary>
        /// Идентификатор КЦ
        /// </summary>
        [DataMember]
        public Guid CompressorShopId { get; set; }

        /// <summary>
        /// Наименование КЦ
        /// </summary>
        [DataMember]
        public string CompressorShopName { get; set; }


        /// <summary>
        /// Идентификатор ГПА
        /// </summary>
        [DataMember]
        public Guid CompressorUnitId { get; set; }

        /// <summary>
        /// Имя ГПА
        /// </summary>
        [DataMember]
        public string CompressorUnitName { get; set; }

        /// <summary>
        /// Тип ГПА
        /// </summary>
        [DataMember]
        public int CompressorUnitTypeId { get; set; }


        [DataMember]
        public List<StoppageAttachmentDTO> Attachments { get; set; }


        [DataMember]
        public List<ReserveUnitDTO> ReservedUnits { get; set; }

        

        
    }
}
