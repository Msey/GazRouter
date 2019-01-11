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

namespace GazRouter.DataServices.DataExchange
{
    [Service("Информационное взаимодействие")]
    [ServiceContract]
    public interface IDataExchangeService
    {
        [ServiceAction("Получение списка источников данных")]
        [OperationContract]
        List<DataSourceDTO> GetDataSourceList(GetDataSourceListParameterSet parameter);

        [ServiceAction("Добавление источника данных")]
        [OperationContract]
        int AddDataSource(AddDataSourceParameterSet parameter);

        [ServiceAction("Редактирование источника данных")]
        [OperationContract]
        void EditDataSource(EditDataSourceParameterSet parameter);

        [ServiceAction("Удаление источника данных")]
        [OperationContract]
        void DeleteDataSource(int parameter);



        //[ServiceAction("Получение списка сущностей типового обмена")]
        //[OperationContract]
        //List<TypicalExchangeEntityDTO> GetTypicalExchangeEntityList(GetTypicalExchangeEntityListParameterSet parameter);

        //[ServiceAction("Добавление сущности типового обмена")]
        //[OperationContract]
        //void AddTypicalExchangeEntity(AddTypicalExchangeEntityParameterSet parameter);

        //[ServiceAction("Удаление сущности типового обмена")]
        //[OperationContract]
        //void DeleteTypicalExchangeEntity(DeleteTypicalExchangeEntityParameterSet parameter);


        [ServiceAction("Редактирование таймера")]
        [OperationContract]
        void EditTimer(TimerSettingsDTO parameters);

        [ServiceAction("Изменение статуса таймера")]
        [OperationContract]
        void ChangeTimerStatus(TimerSettingsDTO parameters);

        [ServiceAction("Получение списка таймеров")]
        [OperationContract]
        List<TimerSettingsDTO> GetTimers();


        [ServiceAction("Получение списка заданий")]
        [OperationContract]
        List<ExchangeTaskDTO> GetExchangeTaskList(GetExchangeTaskListParameterSet parameter);
        
        [ServiceAction("Добавление задания")]
        [OperationContract]
        int AddExchangeTask(AddExchangeTaskParameterSet parameter);

        [ServiceAction("Редактирование задания")]
        [OperationContract]
        void EditExchangeTask(EditExchangeTaskParameterSet parameter);

        [ServiceAction("Удаление задания")]
        [OperationContract]
        void DeleteExchangeTask(int parameter);

        [ServiceAction("Контрольный запуск задания")]
        [OperationContract]
        void RunExchangeTask(RunExchangeTaskParameterSet parameters);


        [ServiceAction("Контрольный запуск Астры")]
        [OperationContract]
        void RunAstra(RunAstaParameterSet parameters);


        [ServiceAction("Получение списка объектов, учавствующих в обмене")]
        [OperationContract]
        List<ExchangeEntityDTO> GetExchangeEntityList(GetExchangeEntityListParameterSet parameter);

        [ServiceAction("Добавление объекта для обмена")]
        [OperationContract]
        void AddExchangeEntity(AddEditExchangeEntityParameterSet parameter);

        [ServiceAction("Редактирование объекта для обмена")]
        [OperationContract]
        void EditExchangeEntity(AddEditExchangeEntityParameterSet parameter);

        [ServiceAction("Удаление объекта для обмена")]
        [OperationContract]
        void DeleteExchangeEntity(AddEditExchangeEntityParameterSet parameter);

        [ServiceAction("Добавление/Редактирование/Удаление (в зависимости от значения  ExtId) объекта для обмена")]
        [OperationContract]
        void SetExchangeEntity(AddEditExchangeEntityParameterSet parameter);



        [ServiceAction("Получение списка свойств объекта, учавствующих в обмене")]
        [OperationContract]
        List<ExchangePropertyDTO> GetExchangePropertyList(GetExchangeEntityListParameterSet parameter);

        [ServiceAction("Изменение свойства объекта для обмена")]
        [OperationContract]
        void SetExchangeProperty(SetExchangePropertyParameterSet parameter);



        [ServiceAction("Тестирование трансформации")]
        [OperationContract]
        string TransformFile(ImportParams parameter);


        [ServiceAction("Получение лога")]
        [OperationContract]
        List<ExchangeLogDTO> GetExchangeLog(GetExchangeLogParameterSet parameter);



        [ServiceAction("Получение соседей по обмену предприятия")]
        [OperationContract]
        List<EnterpriseDTO> GetEnterpriseExchangeNeighbourList();

        [ServiceAction("Получение соседей по обмену предприятия с идентификаторами тасков ")]
        [OperationContract]
        List<NeighbourEnterpriseExchangeTask> GetTypicalExchangeEnterpriseList();
       


        [ServiceAction("Тестирование импорта")]
        [OperationContract]
        void ImportTypicalExchange(string parameters);

        [ServiceAction("Запуск импорта данных из АСУ ТП (локальных ИУС)")]
        [OperationContract]
        void AsutpImport(ASUTPImportParameterSet parameters);


        [ServiceAction("Получение свойств АСДУ ЕСГ")]
        [OperationContract]
        List<AsduPropertyDTO> GetAsduPropertyList(GetAsduEntityListParameterSet parameter);

        [ServiceAction("Редактирование привязки свойств АСДУ ЕСГ")]
        [OperationContract]
        void SetAsduProperty(SetAsduPropertyParameterSet parameter);

        [ServiceAction("Редактирование привязки сущности АСДУ ЕСГ")]
        [OperationContract]
        void SetAsduEntity(SetAsduPropertyParameterSet parameter);

    }
}
