using System;
using System.Collections.Generic;
using System.ServiceModel;
using GazRouter.DataServices.Infrastructure.Attributes;
using GazRouter.DTO.Repairs;
using GazRouter.DTO.Repairs.Complexes;
using GazRouter.DTO.Repairs.Plan;
using GazRouter.DTO.Repairs.RepairReport;
using GazRouter.DTO.Repairs.RepairWorks;
using GazRouter.DTO.Repairs.Agreed;
using GazRouter.DTO.Repairs.Workflow;
using GazRouter.DTO.Dictionaries.PlanTypes;
using GazRouter.DTO.Repairs.PrintForms;
using GazRouter.DTO.Dictionaries.RepairTypes;

namespace GazRouter.DataServices.Repairs
{
    [Service("Ремонты")]
    [ServiceContract]
    public interface IRepairsService
    {
        #region Plan

        [ServiceAction("Получение данных планов ремонтов")]
        [OperationContract]
        [ServiceKnownType(typeof(RepairPlanCompShopDTO))]
        RepairPlanDataDTO GetRepairPlan(GetRepairPlanParameterSet parameters);

        [ServiceAction("Получение данных планов ремонтов")]
        [OperationContract]
        [ServiceKnownType(typeof(RepairPlanCompShopDTO))]
        RepairPlanDataDTO GetWorkflowList(GetRepairWorkflowsParameterSet parameters);

        [ServiceAction("Изменить этап планирования")]
        [OperationContract]
        void SetPlanningStage(SetPlanningStageParameterSet parameters);

        #endregion

        #region Complexes

        [ServiceAction("Получение списка комплексов")]
        [OperationContract]
        List<ComplexDTO> GetComplexList(GetRepairPlanParameterSet parameters);

        [ServiceAction("Добавление комплекса")]
        [OperationContract]
        int AddComplex(AddComplexParameterSet parameters);

        [ServiceAction("Редактирование комплекса")]
        [OperationContract]
        void EditComplex(EditComplexParameterSet parameters);

        [ServiceAction("Удаление комплекса")]
        [OperationContract]
        void DeleteComplex(int parameters);

        [ServiceAction("Добавление ремонта в комплекс")]
        [OperationContract]
        void AddRepairToComplex(AddRepairToComplexParameterSet parameters);

        [ServiceAction("Добавление ремонта в комплекс")]
        [OperationContract]
        void MoveComplex(EditComplexParameterSet parameters);

        #endregion

        #region Repair

        [ServiceAction("Добавление ремонта")]
        [OperationContract]
        int AddRepair(AddRepairParameterSet parameters);

        [ServiceAction("Редактирование ремонта")]
        [OperationContract]
        void EditRepair(EditRepairParameterSet parameters);

        [ServiceAction("Изменение дат ремонта")]
        [OperationContract]
        void EditRepairDates(EditRepairDatesParameterSet parameters);

        [ServiceAction("Удаление ремонта")]
        [OperationContract]
        void DeleteRepair(int parameters);
        
        [ServiceAction("Получение истории изменения ремонта")]
        [OperationContract]
        List<RepairUpdateDTO> GetRepairUpdateHistory(int parameters);

        [ServiceAction("Добавление отчета по ремонту")]
        [OperationContract]
        int AddRepairReport(RepairReportParametersSet parameters);

        [ServiceAction("Редактирование отчета по ремонту")]
        [OperationContract]
        void EditRepairReport(RepairReportParametersSet parameters);

        [ServiceAction("Удаление отчета по ремонту")]
        [OperationContract]
        void DeleteRepairReport(int parameters);

        [ServiceAction("Получение отчетов по ремонту")]
        [OperationContract]
        List<RepairReportDTO> GetRepairReportsByRepair(int parameters);

        [ServiceAction("Добавление вложения к отчету по ремонту")]
        [OperationContract]
        int AddRepairReportAttachment(RepairReportAttachmentParamentersSet parameters);

        [ServiceAction("Удаление вложения к отчету по ремонту")]
        [OperationContract]
        void DeleteRepairReportAttachment(int parameters);


        [ServiceAction("Получение вложений к отчету по ремонту")]
        [OperationContract]
        List<RepairReportAttachmentDTO> GetRepairReportAttachments(int parameters);

        //

        [ServiceAction("Добавление вложения")]
        [OperationContract]
        int AddRepairWorkAttachment(AddRepairWorkAttachmentParameterSet parameters);

        [ServiceAction("Редактирование вложения")]
        [OperationContract]
        void EditRepairWorkAttachment(EditRepairWorkAttachmentParameterSet parameters);

        [ServiceAction("Удаление вложения")]
        [OperationContract]
        void RemoveRepairWorkAttachment(int parameters);

        [ServiceAction("Удаление всех вложений ремонта")]
        [OperationContract]
        void RemoveAllRepairWorkAttachments(int parameters);

        [ServiceAction("Получение списка вложений ремонта")]
        [OperationContract]
        List<RepairWorkAttachmentDTO> GetRepairAttachementsList(int parameters);

        //

        [ServiceAction("Получение списка согласования")]
        [OperationContract]
        List<AgreedRepairRecordDTO> GetAgreedRepairRecordList(int parameters);

        [ServiceAction("Добавление записи в список согласования")]
        [OperationContract]
        int AddAgreedRepairRecord(AddEditAgreedRepairRecordParameterSet parameters);

        [ServiceAction("Редактирование записи в списке согласования")]
        [OperationContract]
        void EditAgreedRepairRecord(AddEditAgreedRepairRecordParameterSet parameters);

        [ServiceAction("Удаление записи из списка согласования")]
        [OperationContract]
        void DeleteAgreedRepairRecord(int parameters);

        [ServiceAction("Загрузка шаблона для печати")]
        [OperationContract]
        byte[] GetPrintDocumentPattern(string DocType);

        #endregion

        #region Согласование

        [ServiceAction("Получение листа согласований")]
        [OperationContract]
        List<AgreedRepairRecordDTO> GetRepairAgreementsList(int parameters);

        [ServiceAction("Изменение состояния работ")]
        [OperationContract]
        void ChangeWorkflowState(ChangeRepairWfParametrSet parameters);

        [ServiceAction("История изменения состояния работ")]
        [OperationContract]
        List<WorkflowHistoryDTO> GetWorkflowHistory(int parameters);

        #endregion


        [ServiceAction("История изменения состояния работ")]
        [OperationContract]
        SignersDTO GetSighers(GetSignersSet parameters); 

        #region Справочники

        [ServiceAction("Типы рем работ")]
        [OperationContract]
        List<PlanTypeDTO> GetRepairPlanTypes();


        [ServiceAction("Типы работ")]
        [OperationContract]
        List<RepairTypeDTO> GetRepairTypes();

        #endregion
    }
}