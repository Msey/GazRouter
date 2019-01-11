using System.Collections.Generic;
using System.ServiceModel;
using GazRouter.DataServices.Infrastructure.Attributes;
using GazRouter.DTO.Bindings.ExchangeEntities;
using GazRouter.DTO.DataExchange.Asdu;
using GazRouter.DTO.DataExchange.ASUTPImport;
using GazRouter.DTO.DataExchange.DataSource;
using GazRouter.DTO.DataExchange.ExchangeEntity;
using GazRouter.DTO.DataExchange.ExchangeLog;
using GazRouter.DTO.DataExchange.ExchangeProperty;
using GazRouter.DTO.DataExchange.ExchangeTask;
using GazRouter.DTO.DataExchange.Transformation;
using GazRouter.DTO.Dictionaries.Enterprises;
using GazRouter.DTO.Exchange.ExchangeSettings;
using GazRouter.DTO.SeriesData.PropertyValues;
using System;
using GazRouter.DTO.DataExchange.Integro;

namespace GazRouter.DataServices.Integro
{
    [Service("Интеграция")]
    [ServiceContract]
    public interface IIntegroService
    {
        [ServiceAction("Выгрузка файла")]
        [OperationContract]
        ExportResult ExportSummary(ExportSummaryParams parameters);

        [ServiceAction("Сохранение сводки")]
        [OperationContract]
        Guid SaveSummaryExchTask(SummaryExchTaskParamSet param);

        [ServiceAction("Получение списка сводок")]
        [OperationContract]
        List<SummaryDTO> GetSummariesList();

        [ServiceAction("Получение списка сводок по параметрам")]
        [OperationContract]
        List<SummaryDTO> GetSummariesListByParams(GetSummaryParameterSet parameters);

        [ServiceAction("Добавление/редактирование сводки")]
        [OperationContract]
        void AddEditSummary(AddEditSummaryParameterSet parameters);

        [ServiceAction("Удаление сводки")]
        [OperationContract]
        void DeleteSummary(Guid parameters);

        [ServiceAction("Добавление/редактирование параметров сводки")]
        [OperationContract]
        void AddEditSummaryParam(AddEditSummaryPParameterSet parameters);

        [ServiceAction("Добавление параметров сводки")]
        [OperationContract]
        SummatyLoadResult AddSummaryParamList(List<AddEditSummaryPParameterSet> parameters); 

        [ServiceAction("Добавление/редактирование деталек параметров сводки")]
        [OperationContract]
        void AddEditSummaryParamContent(AddEditSummaryPContentParameterSet parameters);

        [ServiceAction("Получение списка параметров по сводкам")]
        [OperationContract]
        List<SummaryParamDTO> GetSummariesParamList(Guid summaryId);

        [ServiceAction("Получение списка параметров по сводкам")]
        [OperationContract]
        List<SummaryParamContentDTO> GetSummariesParamContentList(Guid parameter);

        [ServiceAction("Удаление параметров сводки")]
        [OperationContract]
        void DeleteSummaryParam(Guid parameters);
    }
}
