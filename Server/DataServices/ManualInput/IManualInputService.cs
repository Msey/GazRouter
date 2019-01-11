using System;
using System.Collections.Generic;
using System.ServiceModel;
using GazRouter.DataServices.Infrastructure.Attributes;
using GazRouter.DTO.Attachments;
using GazRouter.DTO.Calculations;
using GazRouter.DTO.ManualInput;
using GazRouter.DTO.ManualInput.ChemicalTests;
using GazRouter.DTO.ManualInput.CompUnitStates;
using GazRouter.DTO.ManualInput.CompUnitTests;
using GazRouter.DTO.ManualInput.DependantSites;
using GazRouter.DTO.ManualInput.InputStates;
using GazRouter.DTO.ManualInput.InputStory;
using GazRouter.DTO.ManualInput.ValveSwitches;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.SeriesData.EntityValidationStatus;
using GazRouter.DTO.ManualInput.PipelineLimits;
using GazRouter.DTO.ManualInput.ContractPressures;

namespace GazRouter.DataServices.ManualInput
{
    [Service("Ручной ввод")]
    [ServiceContract]
    public interface IManualInputService
    {
        [ServiceAction("Получение статуса ввода")]
        [OperationContract]
        List<ManualInputStateDTO> GetInputStateList(GetManualInputStateListParameterSet parameters);

        [ServiceAction("Изменение статуса ввода")]
        [OperationContract]
        void SetInputState(SetManualInputStateParameterSet parameters);

        [ServiceAction("Получение истории изменения объекта")]
        [OperationContract]
        List<ManualInputStoryDTO> GetInputStory(GetManualInputStoryParameterSet parameters);


        [ServiceAction("Получение получения статусов проверки на достоверность и полноту по объектам")]
        [OperationContract]
        List<EntityValidationStatusDTO> GetEntityValidationStatusList(GetEntityValidationStatusListParameterSet parameters);



        [ServiceAction("Получение списка переключений кранов")]
        [OperationContract]
        List<ValveSwitchDTO> GetValveSwitchList(GetValveSwitchListParameterSet parameters);

        [ServiceAction("Добавление переключения крана")]
        [OperationContract]
        void AddValveSwitch(AddValveSwitchParameterSet parameters);

        [ServiceAction("Удаление переключения крана")]
        [OperationContract]
        void DeleteValveSwitch(DeleteValveSwitchParameterSet parameters);




        [ServiceAction("Получение списка состояний ГПА")]
        [OperationContract]
        List<CompUnitStateDTO> GetCompUnitStateList(GetCompUnitStateListParameterSet parameters);

        [ServiceAction("Добавление нового состояния ГПА")]
        [OperationContract]
        int AddCompUnitState(AddCompUnitStateParameterSet parameters);

        [ServiceAction("Изменение данных по текущему состоянию ГПА")]
        [OperationContract]
        void EditCompUnitState(EditCompUnitStateParameterSet parameters);

        [ServiceAction("Добавление пуска ГПА, связанного с остановом")]
        [OperationContract]
        void AddFailureRelatedCompUnitStart(AddFailureRelatedUnitStartParameterSet parameters);

        [ServiceAction("Удаление пуска ГПА, связанного с остановом")]
        [OperationContract]
        void DeleteFailureRelatedCompUnitStart(AddFailureRelatedUnitStartParameterSet parameters);

        [ServiceAction("Прикрепить документ к останову")]
        [OperationContract]
        int AddFailureAttachment(AddAttachmentParameterSet<int> parameters);

        [ServiceAction("Удалить прикрепленный к останову файл")]
        [OperationContract]
        void DeleteFailureAttachment(int parameters);

        [ServiceAction("Удаление текущего состояния ГПА")]
        [OperationContract]
        void DeleteCompUnitState(int parameters);

        [ServiceAction("Получение списка аварийных и вынужденных остановов ГПА")]
        [OperationContract]
        List<CompUnitStateDTO> GetCompUnitFailureList(GetFailureListParameterSet parameters);




        [ServiceAction("Получение списка результатов химического анализа")]
        [OperationContract]
        List<ChemicalTestDTO> GetChemicalTestList(GetChemicalTestListParameterSet parameters);

        [ServiceAction("Добавление результата химического анализа")]
        [OperationContract]
        int AddChemicalTest(AddChemicalTestParameterSet parameters);

        [ServiceAction("Редактирование результата химического анализа")]
        [OperationContract]
        void EditChemicalTest(EditChemicalTestParameterSet parameters);

        [ServiceAction("Удаление результата химического анализа")]
        [OperationContract]
        void DeleteChemicalTest(int parameters);




        [ServiceAction("Получение списка испытаний ГПА")]
        [OperationContract]
        List<CompUnitTestDTO> GetCompUnitTestList(GetCompUnitTestListParameterSet parameters);

        [ServiceAction("Добавление испытания ГПА")]
        [OperationContract]
        int AddCompUnitTest(AddCompUnitTestParameterSet parameters);

        [ServiceAction("Изменение испытания ГПА")]
        [OperationContract]
        void EditCompUnitTest(EditCompUnitTestParameterSet parameters);

        [ServiceAction("Удаление испытания ГПА")]
        [OperationContract]
        void DeleteCompUnitTest(int parameters);

        [ServiceAction("Добавление вложения по испытанию")]
        [OperationContract]
        int AddCompUnitTestAttachment(AddAttachmentParameterSet<int> parameters);

        [ServiceAction("Удаление вложения по испытанию")]
        [OperationContract]
        void RemoveCompUnitTestAttachment(int parameters);

        [ServiceAction("Добавление точки на ГДХ")]
        [OperationContract]
        void AddCompUnitTestPoint(AddCompUnitTestPointParameterSet parameters);

        [ServiceAction("Удаление точек ГДХ одного типа кривой")]
        [OperationContract]
        void RemoveCompUnitTestPoint(DeleteCompUnitTestPointParameterSet parameters);




        [ServiceAction("Получение списка ограничений газопровода")]
        [OperationContract]
        List<PipelineLimitDTO> GetPipelineLimitList(GetPipelineLimitListParameterSet parameters);

        [ServiceAction("Добавление ограничения газопровода")]
        [OperationContract]
        int AddPipelineLimit(AddPipelineLimitParameterSet parameters);

        [ServiceAction("Изменение ограничения газопровода")]
        [OperationContract]
        void EditPipelineLimit(EditPipelineLimitParameterSet parameters);

        [ServiceAction("Удаление ограничения газопровода")]
        [OperationContract]
        void DeletePipelineLimit(int parameters);

        [ServiceAction("Добавление вложения по ограничению")]
        [OperationContract]
        int AddPipelineLimitAttachment(AddAttachmentParameterSet<int> parameters);

        [ServiceAction("Изменение вложения по ограничению")]
        [OperationContract]
        void EditPipelineLimitAttachment(AddAttachmentParameterSet<int> parameters);

        [ServiceAction("Удаление вложения по ограничению")]
        [OperationContract]
        void RemovePipelineLimitAttachment(int parameters);

        [ServiceAction("Добавление истории по ограничению")]
        [OperationContract]
        void AddPipelineLimitStory(AddPipelineLimitStoryParameterSet parameters);

        [ServiceAction("Изменение истории по ограничению")]
        [OperationContract]
        void EditPipelineLimitStory(AddPipelineLimitStoryParameterSet parameters);



        [ServiceAction("Добавление подчиненного ЛПУ")]
        [OperationContract]
        void AddDependantSite(AddRemoveDependantSiteParameterSet parameters);


        [ServiceAction("Удаление подчиненного ЛПУ")]
        [OperationContract]
        void RemoveDependantSite(AddRemoveDependantSiteParameterSet parameters);



        [ServiceAction("Получение списка догворных давлений выходов ГРС")]
        [OperationContract]
        List<ContractPressureDTO> GetContractPressureList(GetContractPressureListQueryParameterSet parameters);

        [ServiceAction("Добавление/редактирование значений догворных давлений выходов ГРС")]
        [OperationContract]
        void AddEditContractPressures(List<AddEditContractPressureParameterSet> parameters);

        [ServiceAction("Получение истории изменений догворного давления выхода ГРС")]
        [OperationContract]
        List<ContractPressureHistoryDTO> GetContractPressureHistoryList(Guid parameters);
    }
}

