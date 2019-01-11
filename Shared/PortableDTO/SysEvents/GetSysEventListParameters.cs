using System;

namespace GazRouter.DTO.SysEvents
{
    public class GetSysEventListParameters
    {
        public SysEventType? EventTypeId { get; set; }
        public SysEventStatus? EventStatusId { get; set; }
        public DateTime? CreateDate { get; set; }
    }

    public enum SysEventStatus
    {
        Created = 1,
        Waiting = 2, 
        Finished = 3
    }

    public enum SysEventType
    {
        END_LOAD_DATA_SITE = 1, //Завершение загрузки режимно-технологических данных от ЛПУ
        END_LOAD_DATA_ALL_SITES = 2, //Завершение загрузки режимно-технологических данных от всех ЛПУ
        END_CALCULATION_AFTER_LOAD_DATA = 3, //Расчеты после сбора данных со всех ЛПУ завершены
        END_EXPORT_TASK = 4, //Экспорт файла завершен
        END_LOAD_NEIGHBOR = 5, //Загрузка данных от соседнего ГТО завершена
        END_LOAD_ALL_NEIGHBOR = 6, //Сбор режимно- технологических данных от всех соседних предприятий завершен
        END_EXPORT_TASK_ASTRA = 7, //Экспорт файлов в АСТРу завершен
        END_LOAD_ASTRA = 8, //Загрузка расчетных данных из АСТРы завершена
        DATA_SERIE_COMPLETE = 9, //Все данные расчитаны и загружены
        DATA_CORRECTED = 10, //Режимно-технологические данные по ЛПУ скорректированы и сохранены на уровне ПДС
        SERIE_UNBLOCKED = 11, //Серия данных снята с блокировки на уровне ПДС
        END_EXPORT_MASDUESG = 12, //Экспорт режимно-технологических данных в М АСДУ ЕСГ завершен
    }

    public enum SysEventResult
    {
        Success = 1,
        Error = 2,
    }
}