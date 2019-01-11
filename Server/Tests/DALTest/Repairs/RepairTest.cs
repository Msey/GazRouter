using System;
using System.Linq;
using GazRouter.DAL.Dictionaries.EntityTypes;
using GazRouter.DAL.Dictionaries.GasTransportSystem;
using GazRouter.DAL.Dictionaries.RepairTypes;
using GazRouter.DAL.Repairs.Plan;
using GazRouter.DAL.Repairs.Repair;
using GazRouter.DAL.Repairs.RepairWorks;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PipelineTypes;
using GazRouter.DTO.Dictionaries.PlanTypes;
using GazRouter.DTO.Dictionaries.RepairExecutionMeans;
using GazRouter.DTO.Repairs.Plan;
using GazRouter.DTO.Repairs.RepairWorks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;

namespace DALTest.Repairs
{
	[TestClass]
    public class RepairTest : DalTestBase
	{
        [TestMethod, TestCategory(Stable)]
        public void TestRepairs()
        {
            var entityType = new GetEntityTypeListQuery(Context).Execute().Single(et => et.EntityType == EntityType.Pipeline);
            var system = new GetGasTransportSystemListQuery(Context).Execute().First();

            const int year = 2000;

            // Добаваление ремонта
            var entityId = CreatePipeline(PipelineType.Bridge);
            var repairTypes = new GetRepairTypeListQuery(Context).Execute();
            var repairId = new AddRepairCommand(Context).Execute(
                new AddRepairParameterSet
                {
                    RepairType = repairTypes.First().Id,
                    Description = "A",
                    BleedAmount = 10,
                    CapacitySummer = 11,
                    CapacityWinter = 12,
                    CapacityTransition = 13,
                    ComplexId = null,
                    StartDate = new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Local),
                    EndDate = new DateTime(year, 2, 1, 0, 0, 0, DateTimeKind.Local),
                    DescriptionCPDD = string.Empty,
                    DescriptionGtp = "X",
                    IsCritical = false,
                    MaxTransferSummer = 14,
                    MaxTransferWinter = 15,
                    MaxTransferTransition = 16,
                    CalculatedTransfer = 17,
                    EntityId = entityId,
                    PlanType = PlanType.Planned,
                    ExecutionMeans = ExecutionMeans.Internal,
                    IsExternalCondition = false,
                    PartsDeliveryDate = new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Local),
                    SavingAmount = 50

                });
            Assert.IsNotNull(repairId);
            

            // Проверка получение списка ремонтов (для газопроводов), проверка добавился ли ремонт
            var repairList = new GetPlanRepairListForPipelinesQuery(Context).Execute(
                new GetRepairPlanParameterSet { SystemId = system.Id, Year = year});
            Assert.IsTrue(repairList.Any(r => r.Id == repairId));


            var repair = repairList.Single(r => r.Id == repairId);
            Assert.AreEqual(repair.Description, "A");
            Assert.AreEqual(repair.BleedAmount, 10);
            Assert.AreEqual(repair.CapacitySummer, 11);
            Assert.AreEqual(repair.CapacityWinter, 12);
            Assert.AreEqual(repair.CapacityTransition, 13);
            Assert.AreEqual(repair.StartDate, new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Local));
            Assert.AreEqual(repair.EndDate, new DateTime(year, 2, 1, 0, 0, 0, DateTimeKind.Local));
            Assert.AreEqual(repair.DescriptionGtp, "X");
            Assert.AreEqual(repair.IsCritical, false);
            Assert.AreEqual(repair.MaxTransferSummer, 14);
            Assert.AreEqual(repair.MaxTransferWinter, 15);
            Assert.AreEqual(repair.MaxTransferTransition, 16);
            Assert.AreEqual(repair.CalculatedTransfer, 17);
            Assert.AreEqual(repair.ExecutionMeans, ExecutionMeans.Internal);
            Assert.AreEqual(repair.IsExternalCondition, false);
            Assert.AreEqual(repair.PartsDeliveryDate, new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Local));
            Assert.AreEqual(repair.SavingAmount, 50);


            // Проверка изменения ремонта
            new EditRepairCommand(Context).Execute(
                new EditRepairParameterSet
                {
                    Id = repairId,
                    EntityId = entityId,
                    RepairType = repairTypes.First().Id,
                    Description = "B",
                    BleedAmount = 2,
                    CapacitySummer = 20,
                    CapacityWinter = 21,
                    CapacityTransition = 22,
                    ComplexId = null,
                    StartDate = new DateTime(year, 3, 1, 0, 0, 0, DateTimeKind.Local),
                    EndDate = new DateTime(year, 4, 1, 0, 0, 0, DateTimeKind.Local),
                    DescriptionGtp = "Y",
                    IsCritical = true,
                    MaxTransferSummer = 30,
                    MaxTransferWinter = 31,
                    MaxTransferTransition = 32,
                    CalculatedTransfer = 40,
                    ExecutionMeans = ExecutionMeans.Contract,
                    IsExternalCondition = true,
                    PartsDeliveryDate = new DateTime(year, 6, 6, 0, 0, 0, DateTimeKind.Local),
                    SavingAmount = 55
                });

            // Проверка получение списка ремонтов (для газопроводов), проверка изменился ли ремонт
            repairList = new GetPlanRepairListForPipelinesQuery(Context).Execute(
                new GetRepairPlanParameterSet { SystemId = system.Id, Year = year });
            Assert.IsTrue(repairList.Any(r => r.Id == repairId));

            repair = repairList.Single(r => r.Id == repairId);
            Assert.AreEqual(repair.Description, "B");
            Assert.AreEqual(repair.BleedAmount, 2);
            Assert.AreEqual(repair.CapacitySummer, 20);
            Assert.AreEqual(repair.CapacityWinter, 21);
            Assert.AreEqual(repair.CapacityTransition, 22);
            Assert.AreEqual(repair.StartDate, new DateTime(year, 3, 1, 0, 0, 0, DateTimeKind.Local));
            Assert.AreEqual(repair.EndDate, new DateTime(year, 4, 1, 0, 0, 0, DateTimeKind.Local));
            Assert.AreEqual(repair.DescriptionGtp, "Y");
            Assert.AreEqual(repair.IsCritical, true);
            Assert.AreEqual(repair.MaxTransferSummer, 30);
            Assert.AreEqual(repair.MaxTransferWinter, 31);
            Assert.AreEqual(repair.MaxTransferTransition, 32);
            Assert.AreEqual(repair.CalculatedTransfer, 40);
            Assert.AreEqual(repair.ExecutionMeans, ExecutionMeans.Contract);
            Assert.AreEqual(repair.IsExternalCondition, true);
            Assert.AreEqual(repair.PartsDeliveryDate, new DateTime(year, 6, 6, 0, 0, 0, DateTimeKind.Local));
            Assert.AreEqual(repair.SavingAmount, 55);


            // Проверка функции изменения дат ремонта
            new EditRepairDatesCommand(Context).Execute(
                new EditRepairDatesParameterSet
                {
                    RepairId = repairId,
                    DateStart = new DateTime(year, 5, 1, 0, 0, 0, DateTimeKind.Local),
                    DateEnd = new DateTime(year, 6, 1, 0, 0, 0, DateTimeKind.Local),
                    DateType = DateTypes.Plan
                });

            // Проверка получение списка ремонтов (для газопроводов), проверка изменились ли даты ремонта
            repairList = new GetPlanRepairListForPipelinesQuery(Context).Execute(
                new GetRepairPlanParameterSet { SystemId = system.Id, Year = year });
            Assert.IsTrue(repairList.Any(r => r.Id == repairId));
            repair = repairList.Single(r => r.Id == repairId);
            Assert.AreEqual(repair.StartDate, new DateTime(year, 5, 1, 0, 0, 0, DateTimeKind.Local));
            Assert.AreEqual(repair.EndDate, new DateTime(year, 6, 1, 0, 0, 0, DateTimeKind.Local));


            // Получение списка километров
            var kmList = new KilommetrListGetQuery(Context).Execute(entityId);
            Assert.IsTrue(kmList.Count >= 2);

            // Добавление работы по ремонту
            new AddRepairWorkCommand(Context).Execute(
                new RepairWorkParameterSet
                {
                    RepairId = repairId,
                    WorkTypeId = repairTypes.First().RepairWorkTypes.First().Id,
                    KilometerStart = kmList.First(),
                    KilometerEnd = kmList.Last()
                });

            // Получение списка работ по ремонту
            var workList = new GetPlanRepairWorkListQuery(Context).Execute(year);
            AssertHelper.IsNotEmpty(workList);

            // Удалить все работы по выбранному ремонту
            new DeleteRepairWorksCommand(Context).Execute(repairId);

            // Получение списка работ по ремонту
            workList = new GetPlanRepairWorkListQuery(Context).Execute(year);
            AssertHelper.IsEmpty(workList);





            // Проверка получения истории изменений ремонта
            var hist = new GetRepairUpdateHistoryQuery(Context).Execute(repairId);
            AssertHelper.IsNotEmpty(hist);


            // Удаление ремонта
            new DeleteRepairCommand(Context).Execute(repairId);

            // Проверка получение списка ремонтов (для газопроводов), проверка удалился ли ремонт
            repairList = new GetPlanRepairListForPipelinesQuery(Context).Execute(
                new GetRepairPlanParameterSet { SystemId = system.Id, Year = year });
            AssertHelper.IsEmpty(repairList.Where(r => r.Id == repairId).ToList());

            // Проверка получение списка ремонтов (для КЦ)
            new GetPlanRepairListForCompShopsQuery(Context).Execute(
                new GetRepairPlanParameterSet
                {
                    SystemId = system.Id,
                    Year = year
                });

            // Проверка получение списка ремонтов (для ГРС)
            new GetPlanRepairListForDistrStationQuery(Context).Execute(
                new GetRepairPlanParameterSet
                {
                    SystemId = system.Id,
                    Year = year
                });



            // Проверка установки этапа планирования
            new SetPlanningStageCommand(Context).Execute(
                new SetPlanningStageParameterSet
                {
                    Year = year,
                    SystemId = system.Id,
                    Stage = PlanningStage.Approved
                });

            var stage = new GetPlanningStageQuery(Context).Execute(
                new GetRepairPlanParameterSet
                {
                    Year = year,
                    SystemId = system.Id
                });
            Assert.AreEqual(stage.Stage, PlanningStage.Approved);

        }


        
        

	}
}
