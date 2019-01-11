using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.DAL.Repairs.Complexes;
using GazRouter.DAL.Repairs.Plan;
using GazRouter.DAL.Repairs.Repair;
using GazRouter.DAL.Repairs.RepairWorks;
using GazRouter.DAL.Repairs.Reports;
using GazRouter.DAL.Repairs.Agreement;
using GazRouter.DataServices.Infrastructure.Attributes.Behaviors;
using GazRouter.DataServices.Infrastructure.Services;
using GazRouter.DTO.Repairs;
using GazRouter.DTO.Repairs.Complexes;
using GazRouter.DTO.Repairs.Plan;
using GazRouter.DTO.Repairs.RepairReport;
using GazRouter.DTO.Repairs.RepairWorks;
using GazRouter.DTO.Repairs.Agreed;
using GazRouter.DTO.Repairs.Workflow;
using GazRouter.DAL.Repairs.Workflow;
using Utils.Extensions;
using GazRouter.DTO.Dictionaries.PlanTypes;
using GazRouter.DTO.Repairs.PrintForms;
using GazRouter.DAL.Repairs.PrintForms;
using GazRouter.DTO.Dictionaries.RepairTypes;
using GazRouter.DAL.Dictionaries.RepairTypes;
using GazRouter.DAL.ObjectModel.Segment.Regions;
using GazRouter.DAL.ObjectModel.Segment.Site;

namespace GazRouter.DataServices.Repairs
{
    [ErrorHandlerLogger("mainLogger")]
    [Authorization]
    public class RepairsService : ServiceBase, IRepairsService
    {
        #region Plan

        public RepairPlanDataDTO GetRepairPlan(GetRepairPlanParameterSet parameters)
        {
            using (var context = OpenDbContext())
            {
                var result = new RepairPlanDataDTO {RepairList = new List<RepairPlanBaseDTO>()};

                var pipelineworks = new GetPlanRepairListForPipelinesQuery(context).Execute(parameters.SiteId.HasValue ?
                    new GetRepairPlanParameterSet()
                    {
                        SystemId = parameters.SystemId,
                        Year = parameters.Year
                    } : parameters);
                result.RepairList.AddRange(pipelineworks);

                result.RepairList.AddRange(new GetPlanRepairListForCompShopsQuery(context).Execute(parameters));
                result.RepairList.AddRange(new GetPlanRepairListForDistrStationQuery(context).Execute(parameters));

                var workList = new GetPlanRepairWorkListQuery(context).Execute(parameters.Year);

                foreach (var r in result.RepairList)
                {
                    var works = workList.GetOrDefault(r.Id);
                    if (works != null)
                    {
                        r.Works.AddRange(works);
                    }
                }


                foreach (var pw in pipelineworks)
                {
                    double? kmStart = pw.Works.Min(o => o.KilometerStart);
                    double? kmEnd = pw.Works.Max(o => o.KilometerEnd);
                    string LpuList = "";
                    if (kmStart.HasValue && kmEnd.HasValue)
                    {                        
                        var segms = new GetSiteSegmentListQuery(context).Execute(pw.EntityId);
                        
                        foreach (var segm in segms)
                        {
                            /// вся работа в одном ЛПУ
                            if (segm.KilometerOfStartPoint <= kmStart && segm.KilometerOfEndPoint >= kmEnd)
                            {
                                pw.SiteName = segm.SiteName;
                                if (parameters.SiteId.HasValue && segm.SiteId == parameters.SiteId.Value)
                                {
                                    pw.SiteId = segm.SiteId;
                                }
                                break;
                            }
                            /// работа начинается в одном ЛПУ, и продолжается в другом(их)
                            else if (kmStart >= segm.KilometerOfStartPoint && kmEnd >= segm.KilometerOfEndPoint && kmStart < segm.KilometerOfEndPoint)
                            {
                                LpuList = segm.SiteName;
                                if (parameters.SiteId.HasValue && segm.SiteId == parameters.SiteId.Value)
                                {
                                    pw.SiteId = segm.SiteId;
                                }
                            }
                            /// границы работы за пределами ЛПУ снаружи
                            else if (kmStart < segm.KilometerOfStartPoint && kmEnd > segm.KilometerOfEndPoint)
                            {
                                LpuList += ", " + segm.SiteName;
                                if (parameters.SiteId.HasValue && segm.SiteId == parameters.SiteId.Value)
                                {
                                    pw.SiteId = segm.SiteId;
                                }
                            }
                            /// работа в ЛПУ входит, но не выходит
                            else if (kmStart < segm.KilometerOfStartPoint && kmEnd <= segm.KilometerOfEndPoint && kmEnd > segm.KilometerOfStartPoint)
                            {
                                LpuList += ", " + segm.SiteName;
                                if (parameters.SiteId.HasValue && segm.SiteId == parameters.SiteId.Value)
                                {
                                    pw.SiteId = segm.SiteId;
                                }
                            }
                        }
                    }
                   
                    if (LpuList != "")
                        pw.SiteName = LpuList;
                }

                if (parameters.SiteId.HasValue)
                {
                    List<RepairPlanBaseDTO> idtodel = new List<RepairPlanBaseDTO>();
                    idtodel = result.RepairList.Where(o => o.SiteId != parameters.SiteId.Value).ToList();
                    foreach (var id in idtodel)
                    {
                        result.RepairList.Remove(id);
                    }
                }

                result.ComplexList = new GetComplexListQuery(context).Execute(parameters).ToList();

                result.PlanningStage = new GetPlanningStageQuery(context).Execute(parameters);

                return result;
            }
        }

        public RepairPlanDataDTO GetWorkflowList(GetRepairWorkflowsParameterSet parameters)
        {
            using (var context = OpenDbContext())
            {
                var result = new RepairPlanDataDTO { RepairList = new List<RepairPlanBaseDTO>() };

                var pipelineworks = new GetWorkflowRepairListForPipelinesQuery(context).Execute(parameters.SiteId.HasValue ?
                    new GetRepairWorkflowsParameterSet()
                    {
                        SystemId = parameters.SystemId,
                        Year = parameters.Year,
                        Stage = parameters.Stage,
                        UserId = parameters.UserId
                    } : parameters);
                result.RepairList.AddRange(pipelineworks);

                result.RepairList.AddRange(new GetWorkflowRepairListForCompShopsQuery(context).Execute(parameters));
                result.RepairList.AddRange(new GetWorkflowRepairListForDistrStationQuery(context).Execute(parameters));

                var workList = new GetWorkflowRepairWorkListQuery(context).Execute(parameters.Year);

                foreach (var r in result.RepairList)
                {
                    var works = workList.GetOrDefault(r.Id);
                    if (works != null)
                    {
                        r.Works.AddRange(works);
                    }
                }

                foreach (var pw in pipelineworks)
                {
                    double? kmStart = pw.Works.Min(o => o.KilometerStart);
                    double? kmEnd = pw.Works.Max(o => o.KilometerEnd);
                    string LpuList = "";
                    if (kmStart.HasValue && kmEnd.HasValue)
                    {
                        var segms = new GetSiteSegmentListQuery(context).Execute(pw.EntityId);

                        foreach (var segm in segms)
                        {
                            /// вся работа в одном ЛПУ
                            if (segm.KilometerOfStartPoint <= kmStart && segm.KilometerOfEndPoint >= kmEnd)
                            {
                                pw.SiteName = segm.SiteName;
                                if (parameters.SiteId.HasValue && segm.SiteId == parameters.SiteId.Value)
                                {
                                    pw.SiteId = segm.SiteId;
                                }
                                break;
                            }
                            /// работа начинается в одном ЛПУ, и продолжается в другом(их)
                            else if (kmStart >= segm.KilometerOfStartPoint && kmEnd >= segm.KilometerOfEndPoint && kmStart < segm.KilometerOfEndPoint)
                            {
                                LpuList = segm.SiteName;
                                if (parameters.SiteId.HasValue && segm.SiteId == parameters.SiteId.Value)
                                {
                                    pw.SiteId = segm.SiteId;
                                }
                            }
                            /// границы работы за пределами ЛПУ снаружи
                            else if (kmStart < segm.KilometerOfStartPoint && kmEnd > segm.KilometerOfEndPoint)
                            {
                                LpuList += ", " + segm.SiteName;
                                if (parameters.SiteId.HasValue && segm.SiteId == parameters.SiteId.Value)
                                {
                                    pw.SiteId = segm.SiteId;
                                }
                            }
                            /// работа в ЛПУ входит, но не выходит
                            else if (kmStart < segm.KilometerOfStartPoint && kmEnd <= segm.KilometerOfEndPoint && kmEnd > segm.KilometerOfStartPoint)
                            {
                                LpuList += ", " + segm.SiteName;
                                if (parameters.SiteId.HasValue && segm.SiteId == parameters.SiteId.Value)
                                {
                                    pw.SiteId = segm.SiteId;
                                }
                            }
                        }
                    }

                    if (LpuList != "")
                        pw.SiteName = LpuList;
                }

                if (parameters.SiteId.HasValue && !parameters.UserId.HasValue)
                {
                    List<RepairPlanBaseDTO> idtodel = new List<RepairPlanBaseDTO>();
                    idtodel = result.RepairList.Where(o => o.SiteId != parameters.SiteId.Value).ToList();
                    foreach (var id in idtodel)
                    {
                        result.RepairList.Remove(id);
                    }
                }

                return result;
            }
        }
        

        public void SetPlanningStage(SetPlanningStageParameterSet parameters)
        {
            ExecuteNonQuery<SetPlanningStageCommand, SetPlanningStageParameterSet>(parameters);
        }

        #endregion

        #region Complexes

        public List<ComplexDTO> GetComplexList(GetRepairPlanParameterSet parameters)
        {
            return ExecuteRead<GetComplexListQuery, List<ComplexDTO>, GetRepairPlanParameterSet>(parameters);
        }

        public void DeleteComplex(int parameters)
        {
            ExecuteNonQuery<DeleteComplexCommand, int>(parameters);
        }

        public int AddComplex(AddComplexParameterSet parameters)
        {
            return ExecuteRead<AddComplexCommand, int, AddComplexParameterSet>(parameters);
        }

        public void EditComplex(EditComplexParameterSet parameters)
        {
            ExecuteNonQuery<EditComplexCommand, EditComplexParameterSet>(parameters);
        }

        public void AddRepairToComplex(AddRepairToComplexParameterSet parameters)
        {
            ExecuteNonQuery<AddRepairToComplexCommand, AddRepairToComplexParameterSet>(parameters);
        }

        public void MoveComplex(EditComplexParameterSet changes)
        {
            using (var context = OpenDbContext())
            {
                var complex = new GetComplexByIdQuery(context).Execute(changes.Id);

                var isShifted = complex.EndDate - complex.StartDate == changes.EndDate - changes.StartDate;
                var isShorten = complex.EndDate - complex.StartDate > changes.EndDate - changes.StartDate;

                var repairList = new GetPlanRepairListByComplexIdQuery(context).Execute(changes.Id);
                foreach (var repair in repairList)
                {
                    if (repair.StartDate >= complex.StartDate && repair.EndDate <= complex.EndDate)
                    {
                        // Если комплекс просто передвинули, двигаем на такой же интервал работы внутри комплекса
                        if (isShifted)
                        {
                            new EditRepairDatesCommand(context).Execute(
                                new EditRepairDatesParameterSet
                                {
                                    RepairId = repair.Id,
                                    DateType = DateTypes.Plan,
                                    DateStart = repair.StartDate + (changes.StartDate - complex.StartDate),
                                    DateEnd = repair.EndDate + (changes.StartDate - complex.StartDate)
                                });
                        }

                        // Если комплекс сократили по срокам, то сокращаем работы, которые не умещаются в комплекс
                        if (isShorten)
                        {
                            // Если в результате изменения дат комплекса, продолжительность комплекса
                            // стала меньше продолжительности ремонта, то выставляем даты начала и окончания ремонта
                            // такие же как у комплекса
                            var startDate = changes.StartDate;
                            var endDate = changes.EndDate;
                            
                            // Если работа все еще меньше
                            if (changes.EndDate - changes.StartDate > repair.EndDate - repair.StartDate)
                            {
                                // То начинаем шаманить: 
                                // сдвигаем дату начала ремонта к началу комплекса + исходный сдвиг внутри комплекса
                                startDate = changes.StartDate + (repair.StartDate - complex.StartDate);
                                // Выставляем дату окончания ремонта так, чтобы ремонт имел исходную продолжительность
                                endDate = startDate + (repair.EndDate - repair.StartDate);
                                // Теперь проверяем, чтобы дата окончания ремонта не выходила за границы комплекса
                                if (endDate > changes.EndDate)
                                {
                                    // если выходит, сдвигаем работу внутри комплекса так, чтобы она не выходила 
                                    // (как раз на столько, на сколько выходит)
                                    startDate = startDate - (endDate - changes.EndDate);
                                    // таким образом окончание работы как совпадет с окончание комплекса
                                    endDate = changes.EndDate;
                                }

                            }

                            new EditRepairDatesCommand(context).Execute(
                                new EditRepairDatesParameterSet
                                {
                                    RepairId = repair.Id,
                                    DateType = DateTypes.Plan,
                                    DateStart = startDate,
                                    DateEnd = endDate
                                });

                        }
                    }
                }

                new EditComplexCommand(context).Execute(changes);
            }
        }

        #endregion

        #region Repairs

        public void DeleteRepair(int parameters)
        {
            ExecuteNonQuery<DeleteRepairCommand, int>(parameters);
        }

        public int AddRepair(AddRepairParameterSet parameters)
        {
            using (var context = OpenDbContext())
            {
                var repairId = new AddRepairCommand(context).Execute(parameters);
                foreach (var repairWork in parameters.RepairWorks)
                {
                    repairWork.RepairId = repairId;
                    new AddRepairWorkCommand(context).Execute(repairWork);
                }
                return repairId;
            }
        }

        public void EditRepair(EditRepairParameterSet parameters)
        {
            using (var context = OpenDbContext())
            {
                new EditRepairCommand(context).Execute(parameters);
                new DeleteRepairWorksCommand(context).Execute(parameters.Id);

                foreach (var repairWork in parameters.RepairWorks)
                {
                    repairWork.RepairId = parameters.Id;
                    new AddRepairWorkCommand(context).Execute(repairWork);
                }
            }
        }

        public void EditRepairDates(EditRepairDatesParameterSet parameters)
        {
            ExecuteNonQuery<EditRepairDatesCommand, EditRepairDatesParameterSet>(parameters);
        }
        
        public List<RepairUpdateDTO> GetRepairUpdateHistory(int parameters)
        {
            return ExecuteRead<GetRepairUpdateHistoryQuery, List<RepairUpdateDTO>, int>(parameters);
        }

        public int AddRepairReport(RepairReportParametersSet parameters)
        {
            return ExecuteRead<AddReportCommand, int, RepairReportParametersSet>(parameters);
        }

        public void EditRepairReport(RepairReportParametersSet parameters)
        {
            ExecuteNonQuery<EditReportCommand, RepairReportParametersSet>(parameters);
        }

        public void DeleteRepairReport(int parameters)
        {
            ExecuteNonQuery<DeleteReportCommand, int>(parameters);
        }

        public List<RepairReportDTO> GetRepairReportsByRepair(int parameters)
        {
            return ExecuteRead<GetReportByWorkQuery, List<RepairReportDTO>, int>(parameters);
        }

        public int AddRepairReportAttachment(RepairReportAttachmentParamentersSet parameters)
        {
            return ExecuteRead<AddReportAttachmentCommand, int, RepairReportAttachmentParamentersSet>(parameters);
        }

        public void DeleteRepairReportAttachment(int parameters)
        {
            ExecuteNonQuery<DeleteReportAttachmentCommand, int>(parameters);
        }

        public List<RepairReportAttachmentDTO> GetRepairReportAttachments(int parameters)
        {
            return ExecuteRead<GetReportAttachementQuery, List<RepairReportAttachmentDTO>, int>(parameters);
        }

        //

        public int AddRepairWorkAttachment(AddRepairWorkAttachmentParameterSet parameters)
        {
            return ExecuteRead<AddRepairWorkAttachmentCommand, int, AddRepairWorkAttachmentParameterSet>(parameters);
        }

        public void EditRepairWorkAttachment(EditRepairWorkAttachmentParameterSet parameters)
        {
            ExecuteNonQuery<EditRepairWorkAttachmentCommand, EditRepairWorkAttachmentParameterSet>(parameters);
        }

        public void RemoveRepairWorkAttachment(int parameters)
        {
            ExecuteNonQuery<RemoveRepairWorkAttachmentCommand, int>(parameters);
        }

        public void RemoveAllRepairWorkAttachments(int parameters)
        {
            ExecuteNonQuery<RemoveAllRepairWorkAttachmentsCommand, int>(parameters);
        }

        public List<RepairWorkAttachmentDTO> GetRepairAttachementsList(int parameters)
        {
            return ExecuteRead<GetRepairWorkAttachmentListQuery, List<RepairWorkAttachmentDTO>, int>(parameters);
        }

        //

        public List<AgreedRepairRecordDTO> GetAgreedRepairRecordList(int parameters)
        {
            return ExecuteRead<GetAgreementListQuery, List<AgreedRepairRecordDTO>, int>(parameters);
        }

        public int AddAgreedRepairRecord(AddEditAgreedRepairRecordParameterSet parameters)
        {
            return ExecuteRead<AddAgreementCommand, int, AddEditAgreedRepairRecordParameterSet>(parameters);
        }

        public void EditAgreedRepairRecord(AddEditAgreedRepairRecordParameterSet parameters)
        {
            ExecuteNonQuery<EditAgreementCommand, AddEditAgreedRepairRecordParameterSet>(parameters);
        }

        public void DeleteAgreedRepairRecord(int parameters)
        {
            ExecuteNonQuery<DeleteAgreementCommand, int>(parameters);
        }

        public List<AgreedRepairRecordDTO> GetRepairAgreementsList(int parameters)
        {
            return ExecuteRead<GetAgreementListQuery, List<AgreedRepairRecordDTO>, int>(parameters);
        }

        public void ChangeWorkflowState(ChangeRepairWfParametrSet parameters)
        {
            ExecuteNonQuery<ChangeStateCommand, ChangeRepairWfParametrSet>(parameters);
            if (parameters.WState == WorkStateDTO.WorkStates.Current || parameters.WState == WorkStateDTO.WorkStates.Completed)
            {
                EditRepairParameterSet updateSet = new EditRepairParameterSet
                {
                    BleedAmount = parameters.repair.BleedAmount,
                    SavingAmount = parameters.repair.SavingAmount,
                    CapacityWinter = parameters.repair.CapacityWinter,
                    CapacitySummer = parameters.repair.CapacitySummer,
                    CapacityTransition = parameters.repair.CapacityTransition,
                    MaxTransferWinter = parameters.repair.MaxTransferWinter,
                    MaxTransferSummer = parameters.repair.MaxTransferSummer,
                    MaxTransferTransition = parameters.repair.MaxTransferTransition,
                    CalculatedTransfer = parameters.repair.CalculatedTransfer,
                    StartDate = parameters.repair.StartDate,
                    EndDate = parameters.repair.EndDate,
                    Description = parameters.repair.Description,
                    DescriptionGtp = parameters.repair.DescriptionGtp,
                    EntityId = parameters.repair.EntityId,
                    Id = parameters.repair.Id,
                    IsCritical = parameters.repair.IsCritical,
                    PlanType = parameters.repair.PlanType == 1 ? PlanType.Planned : (parameters.repair.PlanType == 2 ? PlanType.Unplanned : PlanType.Emergency),
                    RepairType = parameters.repair.RepairTypeId,
                    ExecutionMeans = parameters.repair.ExecutionMeans,
                    IsExternalCondition = false,
                    PartsDeliveryDate = parameters.repair.PartsDeliveryDate,
                    RepairWorks = parameters.repair.Works
                                .Where(w => w.IsUseWork)
                                .Select(w =>
                                    new RepairWorkParameterSet
                                    {
                                        WorkTypeId = w.Id,
                                        KilometerStart = w.KilometerStart,
                                        KilometerEnd = w.KilometerEnd,
                                    }).ToList(),


                    WorkflowState = (int)parameters.WFState,
                    RepairState = (int)parameters.WState,

                    DateEndFact = parameters.repair.DateEndFact,
                    DateStartFact = parameters.repair.DateStartFact,

                    DateEndShed = parameters.repair.DateEndSched,
                    DateStartShed = parameters.repair.DateStartSched,

                    FireworkType = (int)parameters.repair.Firework,

                    ResolutionDate = parameters.repair.ResolutionDate,
                    ResolutionDateCpdd = parameters.repair.ResolutionDateCpdd,
                    ResolutionNum = parameters.repair.ResolutionNum,
                    ResolutionNumCpdd = parameters.repair.ResolutionNumCpdd,
                };

                if (parameters.WState == WorkStateDTO.WorkStates.Current && !parameters.repair.DateStartFact.HasValue)
                {
                    updateSet.DateStartFact = DateTime.Now;
                    EditRepair(updateSet);
                }

                if (parameters.WState == WorkStateDTO.WorkStates.Completed && !parameters.repair.DateEndFact.HasValue)
                {
                    updateSet.DateEndFact = DateTime.Now;
                    EditRepair(updateSet);
                }
            }
        }

        public List<WorkflowHistoryDTO> GetWorkflowHistory(int parameters)
        {
            return ExecuteRead<GetWorkflowHistoryQuery, List<WorkflowHistoryDTO>, int>(parameters);
        }

        public List<PlanTypeDTO> GetRepairPlanTypes()
        {
            return ExecuteRead<GetPlanTypesQuery, List<PlanTypeDTO>>();
        }

        public List<RepairTypeDTO> GetRepairTypes()
        {
            return ExecuteRead<GetRepairTypeListQuery, List<RepairTypeDTO>>();
        }

        public SignersDTO GetSighers(GetSignersSet parameters)
        {
            SignersDTO result = new SignersDTO();

            if (parameters.IsCpdd)
            {
                result.To = ExecuteRead<GetCpddToQuery, List<TargetingUserDTO>, int>(parameters.EntityTypeId);
                result.From = ExecuteRead<GetSiteFromQuery, List<TargetingUserDTO>, GetSignersSet>(parameters);
            }
            else
            {
                result.To = ExecuteRead<GetSiteToQuery, List<TargetingUserDTO>, GetSignersSet>(parameters);
                result.From = ExecuteRead<GetSiteFromQuery, List<TargetingUserDTO>, GetSignersSet>(new GetSignersSet() { EntityTypeId = parameters.EntityTypeId, ToId = parameters.FromId.Value });
            }

            return result;
        }

        public byte[] GetPrintDocumentPattern(string DocType)
        {
            string FilePath = System.Web.Hosting.HostingEnvironment.MapPath($"~/App_Data/PrintDocumentPatterns/{DocType.ToUpper()}/blank.docx");
            if (System.IO.File.Exists(FilePath))
            {
                return System.IO.File.ReadAllBytes(FilePath);
            }
            else
            {
                return new byte[] { };
            }
        }

        #endregion
    }
}