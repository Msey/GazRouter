using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.DAL.ManualInput.ChemicalTests;
using GazRouter.DAL.ManualInput.CompUnitStates;
using GazRouter.DAL.ManualInput.CompUnitStates.Failures;
using GazRouter.DAL.ManualInput.CompUnitStates.Failures.Attachment;
using GazRouter.DAL.ManualInput.CompUnitStates.Failures.RelatedUnitStart;
using GazRouter.DAL.ManualInput.CompUnitTests;
using GazRouter.DAL.ManualInput.CompUnitTests.Attachment;
using GazRouter.DAL.ManualInput.InputStates;
using GazRouter.DAL.ManualInput.InputStory;
using GazRouter.DAL.ManualInput.ValveSwitches;
using GazRouter.DAL.ManualInput.ContractPressures;
using GazRouter.DAL.SeriesData.ValueMessages;
using GazRouter.DataServices.Infrastructure;
using GazRouter.DataServices.Infrastructure.Attributes.Behaviors;
using GazRouter.DataServices.Infrastructure.Services;
using GazRouter.DAL.ManualInput.DependantSites;
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
using GazRouter.DTO.ManualInput.ContractPressures;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.SeriesData.EntityValidationStatus;
using GazRouter.DTO.ManualInput.PipelineLimits;
using GazRouter.DAL.ManualInput.PipelineLimits;
using GazRouter.DAL.ManualInput.PipelineLimits.Attachment;

namespace GazRouter.DataServices.ManualInput
{
    [Authorization]
    [ErrorHandlerLogger("mainLogger")]
    public class ManualInputService : ServiceBase, IManualInputService
    {
        public List<ManualInputStateDTO> GetInputStateList(GetManualInputStateListParameterSet parameters)
        {
            return ExecuteRead<GetManualInputStateListQuery, List<ManualInputStateDTO>, GetManualInputStateListParameterSet>(parameters);
        }

        public void SetInputState(SetManualInputStateParameterSet parameters)
        {
            ExecuteNonQuery<SetManualInputStateCommand, SetManualInputStateParameterSet>(parameters);
        }

        public List<ManualInputStoryDTO> GetInputStory(GetManualInputStoryParameterSet parameters)
        {
            if (!parameters.SiteId.HasValue && !parameters.EntityId.HasValue)
                parameters.EntityId = AppSettingsManager.CurrentEnterpriseId;
            return ExecuteRead<GetManualInputStoryQuery, List<ManualInputStoryDTO>, GetManualInputStoryParameterSet>(parameters);
        }



        public List<EntityValidationStatusDTO> GetEntityValidationStatusList(GetEntityValidationStatusListParameterSet parameters)
        {
            return
                ExecuteRead
                    <GetEntityValidationStatusListQuery, List<EntityValidationStatusDTO>,
                        GetEntityValidationStatusListParameterSet>(parameters);
        }


        public SerializableTuple<TreeDataDTO, Dictionary<Guid, EntityValidationStatus>> GetSiteTree(GetSiteTreeParameterSet parameters)
        {
            using (var context = OpenDbContext())
            {
                // Посмотреть выполнялся ли автоматический расчет для всей серии. 
                // Для этого нужно запросить InputStory для текущего предприятия,
                // т.к. алгоритм проверки всей серии фиксирует факт 
                // выполнения в объект типа предприятие
                var isSerieChecked = new GetManualInputStoryQuery(context).Execute(
                    new GetManualInputStoryParameterSet
                    {
                        SerieId = parameters.SerieId,
                        EntityId = AppSettingsManager.CurrentEnterpriseId
                    }).Any();

            }

            return new SerializableTuple<TreeDataDTO, Dictionary<Guid, EntityValidationStatus>>();
        }




        #region COMP UNIT STATES

        public List<CompUnitStateDTO> GetCompUnitStateList(GetCompUnitStateListParameterSet parameters)
        {
            using (var context = OpenDbContext())
            {
                var states = new GetCompUnitStateListQuery(context).Execute(parameters);

                var failures = states.Where(s => s.FailureDetails != null).ToList();

                if (failures.Any())
                {
                    // подгружаем список связанных пусков
                    var relatedChanges =
                        new GetFailureRelatedUnitStartListQuery(context).Execute(failures.Select(s => s.Id).ToList());
                    failures.ForEach(
                        s =>
                            s.FailureDetails.UnitStartList =
                                relatedChanges.Where(rc => rc.FailureId == s.FailureDetails.FailureId).ToList());

                    // подгружаем список прикрепленных файлов
                    var attachments =
                        new GetFailureAttachmentListQuery(context).Execute(failures.Select(s => s.Id).ToList());
                    failures.ForEach(
                        s =>
                            s.FailureDetails.AttachmentList =
                                attachments.Where(rc => rc.ExternalId == s.FailureDetails.FailureId).ToList());
                }

                return states;
            }
        }


        public int AddCompUnitState(AddCompUnitStateParameterSet parameters)
        {
            return ExecuteRead<AddCompUnitStateCommand, int, AddCompUnitStateParameterSet>(parameters);
        }

        public void EditCompUnitState(EditCompUnitStateParameterSet parameters)
        {
            ExecuteNonQuery<EditCompUnitStateCommand, EditCompUnitStateParameterSet>(parameters);
        }

        public void AddFailureRelatedCompUnitStart(AddFailureRelatedUnitStartParameterSet parameters)
        {
            ExecuteNonQuery<AddFailureRelatedUnitStartCommand, AddFailureRelatedUnitStartParameterSet>(parameters);
        }

        public void DeleteFailureRelatedCompUnitStart(AddFailureRelatedUnitStartParameterSet parameters)
        {
            ExecuteNonQuery<RemoveFailureRelatedUnitStartCommand, AddFailureRelatedUnitStartParameterSet>(parameters);
        }

        public int AddFailureAttachment(AddAttachmentParameterSet<int> parameters)
        {
            return ExecuteRead<AddFailureAttachmentCommand, int, AddAttachmentParameterSet<int>>(parameters);
        }

        public void DeleteFailureAttachment(int parameters)
        {
            ExecuteNonQuery<DeleteFailureAttachmentCommand, int>(parameters);
        }

        public void DeleteCompUnitState(int parameters)
        {
            ExecuteNonQuery<DeleteCompUnitStateCommand, int>(parameters);
        }

        public List<CompUnitStateDTO> GetCompUnitFailureList(GetFailureListParameterSet parameters)
        {
            using (var context = OpenDbContext())
            {
                var failures = new GetFailureListQuery(context).Execute(parameters);
                if (failures.Any())
                {
                    // подгружаем список связанных пусков
                    var relatedChanges =
                        new GetFailureRelatedUnitStartListQuery(context).Execute(failures.Select(s => s.Id).ToList());
                    failures.ForEach(
                        s =>
                            s.FailureDetails.UnitStartList =
                                relatedChanges.Where(rc => rc.FailureId == s.FailureDetails.FailureId).ToList());

                    // подгружаем список прикрепленных файлов
                    var attachments =
                        new GetFailureAttachmentListQuery(context).Execute(failures.Select(s => s.Id).ToList());
                    failures.ForEach(
                        s =>
                            s.FailureDetails.AttachmentList =
                                attachments.Where(rc => rc.ExternalId == s.FailureDetails.FailureId).ToList());
                }

                return failures;
            }
        }


        #endregion


        #region VALVE SWITCHES

        public List<ValveSwitchDTO> GetValveSwitchList(GetValveSwitchListParameterSet parameters)
        {
            return ExecuteRead<GetValveSwitchListQuery, List<ValveSwitchDTO>, GetValveSwitchListParameterSet>(parameters);
        }


        public void AddValveSwitch(AddValveSwitchParameterSet parameters)
        {
            ExecuteNonQuery<AddValveSwitchCommand, AddValveSwitchParameterSet>(parameters);
        }

        public void DeleteValveSwitch(DeleteValveSwitchParameterSet parameters)
        {
            ExecuteNonQuery<DeleteValveSwitchCommand, DeleteValveSwitchParameterSet>(parameters);
        }

        #endregion



        #region CHEMICAL TESTS

        public List<ChemicalTestDTO> GetChemicalTestList(GetChemicalTestListParameterSet parameters)
        {
            return ExecuteRead<GetChemicalTestListQuery, List<ChemicalTestDTO>, GetChemicalTestListParameterSet>(parameters);
        }


        public int AddChemicalTest(AddChemicalTestParameterSet parameters)
        {
            return ExecuteRead<AddChemicalTestCommand, int, AddChemicalTestParameterSet>(parameters);
        }

        public void EditChemicalTest(EditChemicalTestParameterSet parameters)
        {
            ExecuteNonQuery<EditChemicalTestCommand, EditChemicalTestParameterSet>(parameters);
        }

        public void DeleteChemicalTest(int parameters)
        {
            ExecuteNonQuery<DeleteChemicalTestCommand, int>(parameters);
        }

        #endregion



        #region COMP UNIT TESTS

        public List<CompUnitTestDTO> GetCompUnitTestList(GetCompUnitTestListParameterSet parameters)
        {
            using (var context = OpenDbContext())
            {
                var tests = new GetCompUnitTestListQuery(context).Execute(parameters);
                if (tests.Any())
                {
                    var attachments =
                        new GetCompUnitTestAttachmentListQuery(context).Execute(tests.Select(t => t.Id).ToList());
                    tests.ForEach(t => t.AttachmentList = attachments.Where(a => a.ExternalId == t.Id).ToList());

                    var points = new GetCompUnitTestPointsQuery(context).Execute();
                    tests.ForEach(t => t.ChartPoints = points.Where(p => p.ParentId == t.Id).ToList());
                }

                return tests;
            }
        }

        public int AddCompUnitTest(AddCompUnitTestParameterSet parameters)
        {
            return ExecuteRead<AddCompUnitTestCommand, int, AddCompUnitTestParameterSet>(parameters);
        }

        public void EditCompUnitTest(EditCompUnitTestParameterSet parameters)
        {
            ExecuteNonQuery<EditCompUnitTestCommand, EditCompUnitTestParameterSet>(parameters);
        }

        public void DeleteCompUnitTest(int parameters)
        {
            ExecuteNonQuery<DeleteCompUnitTestCommand, int>(parameters);
        }


        public int AddCompUnitTestAttachment(AddAttachmentParameterSet<int> parameters)
        {
            return ExecuteRead<AddCompUnitTestAttachmentCommand, int, AddAttachmentParameterSet<int>>(parameters);
        }

        public void RemoveCompUnitTestAttachment(int parameters)
        {
            ExecuteNonQuery<DeleteCompUnitTestAttachmentCommand, int>(parameters);
        }

        public void EditCompUnitTestAttachment(AddAttachmentParameterSet<int> parameters)
        {
            ExecuteNonQuery<EditCompUnitTestAttachmentCommand, AddAttachmentParameterSet<int>>(parameters);
        }

        public void AddCompUnitTestPoint(AddCompUnitTestPointParameterSet parameters)
        {
            ExecuteNonQuery<AddCompUnitTestPointCommand, AddCompUnitTestPointParameterSet>(parameters);
        }

        public void RemoveCompUnitTestPoint(DeleteCompUnitTestPointParameterSet parameters)
        {
            ExecuteNonQuery<DeleteCompUnitTestPointCommand, DeleteCompUnitTestPointParameterSet>(parameters);
        }

        #endregion

        #region PIPELINE LIMITS

        public List<PipelineLimitDTO> GetPipelineLimitList(GetPipelineLimitListParameterSet parameters)
        {
            using (var context = OpenDbContext())
            {
                var limits = new GetPipelineLimitsListQuery(context).Execute(parameters);
                if (limits.Any())
                {
                    var attachments =
                        new GetPipelineLimitAttachmentListQuery(context).Execute(limits.Select(t => t.Id).ToList());
                    var change_story = new GetPipelineLimitStoryQuery(context).Execute(parameters.LimitId);
                    limits.ForEach(t =>
                    {
                        var current_story = change_story.Where(a => a.EntityId == t.Id).OrderByDescending(f => f.ChangeDate).FirstOrDefault();
                        if (current_story != null)
                        {
                            t.User = current_story.User;
                            t.UserName = current_story.UserName;
                            t.UserDescription = current_story.UserDescription;
                            t.UserSite = current_story.UserSite;
                            t.ChangeDate = current_story.ChangeDate;
                        }
                    });
                    limits.ForEach(t => t.AttachmentList = attachments.Where(a => a.ExternalId == t.Id).ToList());

                }

                return limits;
            }
        }


        public int AddPipelineLimit(AddPipelineLimitParameterSet parameters)
        {
            return ExecuteRead<AddPipelineLimitCommand, int, AddPipelineLimitParameterSet>(parameters);
        }

        public void EditPipelineLimit(EditPipelineLimitParameterSet parameters)
        {
            ExecuteNonQuery<EditPipelineLimitCommand, EditPipelineLimitParameterSet>(parameters);
        }

        public void DeletePipelineLimit(int parameters)
        {
            ExecuteNonQuery<DeletePipelineLimitCommand, int>(parameters);
        }

        public int AddPipelineLimitAttachment(AddAttachmentParameterSet<int> parameters)
        {
            return ExecuteRead<AddPipelineLimitAttachmentCommand, int, AddAttachmentParameterSet<int>>(parameters);
        }

        public void RemovePipelineLimitAttachment(int parameters)
        {
            ExecuteNonQuery<DeletePipelineLimitAttachmentCommand, int>(parameters);
        }

        public void EditPipelineLimitAttachment(AddAttachmentParameterSet<int> parameters)
        {
            ExecuteNonQuery<EditPipelineLimitAttachmentCommand, AddAttachmentParameterSet<int>>(parameters);
        }
        public void AddPipelineLimitStory(AddPipelineLimitStoryParameterSet parameters)
        {
            ExecuteNonQuery<AddPipelineLimitStoryCommand, AddPipelineLimitStoryParameterSet>(parameters);
        }
        public void EditPipelineLimitStory(AddPipelineLimitStoryParameterSet parameters)
        {
            ExecuteNonQuery<EditPipelineLimitStoryCommand, AddPipelineLimitStoryParameterSet>(parameters);
        }


        #endregion



        public void AddDependantSite(AddRemoveDependantSiteParameterSet parameters)
        {
            ExecuteNonQuery<AddDependantSiteCommand, AddRemoveDependantSiteParameterSet>(parameters);
        }

        public void RemoveDependantSite(AddRemoveDependantSiteParameterSet parameters)
        {
            ExecuteNonQuery<RemoveDependantSiteCommand, AddRemoveDependantSiteParameterSet>(parameters);
        }

        public List<ContractPressureDTO> GetContractPressureList(GetContractPressureListQueryParameterSet parameters)
        {
            return ExecuteRead<GetContractPressureListQuery, List<ContractPressureDTO>, GetContractPressureListQueryParameterSet>(parameters);
        }

        public void AddEditContractPressures(List<AddEditContractPressureParameterSet> parameters)
        {
            using (var context = OpenDbContext())
            {
                foreach (var ContractPressure in parameters)
                    new AddEditContractPressureCommand(context).Execute(ContractPressure);
            }
        }

        public List<ContractPressureHistoryDTO> GetContractPressureHistoryList(Guid parameters)
        {
            return ExecuteRead<GetContractPressureHistoryListQuery, List<ContractPressureHistoryDTO>, Guid>(parameters);
        }
    }
}