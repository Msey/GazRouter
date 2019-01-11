using System;
using System.Linq;
using GazRouter.DAL.Dictionaries.EntityTypes;
using GazRouter.DAL.Dictionaries.GasTransportSystem;
using GazRouter.DAL.DispatcherTasks;
using GazRouter.DAL.DispatcherTasks.TaskRecords;
using GazRouter.DAL.DispatcherTasks.Tasks;
using GazRouter.DAL.DispatcherTasks.TaskStatuses;
using GazRouter.DAL.ObjectModel.Pipelines;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PipelineTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.DispatcherTasks.TaskRecords;
using GazRouter.DTO.DispatcherTasks.Tasks;
using GazRouter.DTO.ObjectModel.Pipelines;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DALTest.DispatcherTask
{
	[TestClass]
    public class TaskRecordsTest : DalTestBase
	{
        [TestMethod ,TestCategory(Stable)]
		public void TestTaskRecords()
		{

            {
				var gastransport = new GetGasTransportSystemListQuery(Context).Execute().First();
				var pipelineId =
                    new AddPipelineCommand(Context).Execute(new AddPipelineParameterSet { Name = "Pipeline1",GasTransportSystemId = gastransport.Id, PipelineTypeId = PipelineType.Bridge, Hidden = true, SortOrder = 1, 
                        KilometerOfStart = 10, KilometerOfEnd = 11 });

                var taskid = new AddTaskCommand(Context).Execute(new AddTaskParameterSet
				{
					Description = "TestDescription1",
					Subject = "TestSubject1"
				});
                var taskList = new GetTaskListQuery(Context).Execute(new GetTaskListParameterSet { IsEnterprise = true });
                var task = taskList.FirstOrDefault(p => p.Id == taskid);
                Assert.IsTrue(task !=null);
                var taskVersionId = task.LastVersionId;

                
                
				Guid newGuidSite = CreateSite();
                var recordId = new AddTaskRecordPDSCommand(Context).Execute(new AddTaskRecordPdsParameterSet
                                                        {
															TaskId = taskid,
															Description = "TestDescription",
															CompletionDate = DateTime.Now,
                                                            OrderNo = 1,
                                                            PeriodTypeId = PeriodType.Twohours,
                                                            PropertyTypeId = PropertyType.PressureInlet,
															TargetValue = "TestValue",
                                                            SiteId = newGuidSite,
															EntityId = pipelineId
                                                        });

                new EditTaskRecordPDSCommand(Context).Execute(new EditTaskRecordPdsParameterSet
                                                                  {
                                                                      RowId = recordId,
                                                                      EntityId = pipelineId,
                                                                      PeriodTypeId = PeriodType.Twohours,
                                                                      PropertyTypeId = PropertyType.PressureInlet,
                                                                      TargetValue = "NextValue",
                                                                      CompletionDate = DateTime.Now,
                                                                      OrderNo = 1,
                                                                      Description = "TestDescription",
                                                                      SiteId = newGuidSite
                                                                  });

                var list = new GetTaskRecordsLpuListQuery(Context).Execute(new GetTaskRecordsPdsParameterSet { SiteId = newGuidSite });

                new SetToControlTaskRecordCommand(Context).Execute(recordId);
                new ResetToControlTaskRecordCommand(Context).Execute(recordId);

                var taskRecordsList = new GetTaskRecordsListQuery(Context).Execute(new GetTaskRecordsCpddParameterSet { IsCpdd = false, TaskVersionId = taskVersionId });
	            Assert.IsTrue(taskRecordsList.FirstOrDefault(p => p.Id == recordId) != null);

				

				new SetToControlTaskRecordCommand(Context).Execute(recordId);


                new TaskApprovedPdsCommand(Context).Execute(taskid);

				new TaskRecordsExecutedCommand(Context).Execute(recordId);
				taskRecordsList = new GetTaskRecordsListQuery(Context).Execute(new GetTaskRecordsCpddParameterSet { IsCpdd = false, TaskVersionId = taskVersionId });
				var t1 = taskRecordsList.FirstOrDefault(p => p.Id == recordId);
				Assert.IsTrue(t1 != null);
				Assert.IsTrue(t1.ExecutedDate.HasValue);
				new TaskRecordsResetExecutedCommand(Context).Execute(recordId);
				taskRecordsList = new GetTaskRecordsListQuery(Context).Execute(new GetTaskRecordsCpddParameterSet { IsCpdd = false, TaskVersionId = taskVersionId });
				t1 = taskRecordsList.FirstOrDefault(p => p.Id == recordId);
				Assert.IsTrue(t1 != null);
				Assert.IsFalse(t1.ExecutedDate.HasValue);
                //var taskRecordsList1 = new GetTaskRecordsLpuListQuery(context).Execute(new GetTaskRecordsPDSParameterSet { SiteId = newGuidSite,IsList = true});
                //Assert.IsTrue(taskRecordsList1.FirstOrDefault(p => p.Id == recordId) != null);
                //new TaskRecordsExecutedCommand(context).Execute(recordId);
				//taskRecordsList1 = new GetTaskRecordsLpuListQuery(context).Execute(new GetTaskRecordsPDSParameterSet { SiteId = newGuidSite, IsList = true });
                //Assert.IsTrue(taskRecordsList1.FirstOrDefault(p => p.Id == recordId) == null);
				//taskRecordsList1 = new GetTaskRecordsLpuListQuery(context).Execute(new GetTaskRecordsPDSParameterSet { SiteId = newGuidSite, IsList = false });
                //Assert.IsTrue(taskRecordsList1.FirstOrDefault(p => p.Id == recordId) != null);

                new RemoveTaskRecordCommand(Context).Execute(recordId);
            }
		}
	}
}
