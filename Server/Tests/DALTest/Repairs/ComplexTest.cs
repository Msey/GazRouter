using System;
using System.Linq;
using GazRouter.DAL.Dictionaries.EntityTypes;
using GazRouter.DAL.Dictionaries.GasTransportSystem;
using GazRouter.DAL.Dictionaries.RepairTypes;
using GazRouter.DAL.Repairs.Complexes;
using GazRouter.DAL.Repairs.Plan;
using GazRouter.DAL.Repairs.Repair;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PipelineTypes;
using GazRouter.DTO.Dictionaries.PlanTypes;
using GazRouter.DTO.Dictionaries.RepairExecutionMeans;
using GazRouter.DTO.Repairs.Complexes;
using GazRouter.DTO.Repairs.Plan;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;

namespace DALTest.Repairs
{
    [TestClass]
    public class ComplexTest : DalTestBase
    {
        [TestMethod ,TestCategory(Stable)]
        public void TestModifyComplex()
        {
            #region (Add)

            var system = new GetGasTransportSystemListQuery(Context).Execute().First();
            const int year = 2000;

            var complexId = new AddComplexCommand(Context).Execute(
                new AddComplexParameterSet
                    {
                        ComplexName = "cmplx",
                        StartDate = new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Local),
                        EndDate = new DateTime(year, 1, 3, 0, 0, 0, DateTimeKind.Local),
                        IsLocal = false,
                        SystemId = system.Id
                    });
            Assert.IsTrue(complexId > 0);
           
            var complexList = new GetComplexListQuery(Context).Execute(
                new GetRepairPlanParameterSet
                {
                    Year = year,
                    SystemId = system.Id
                });
            AssertHelper.IsNotEmpty(complexList);

            var complex = complexList.Single(c => c.Id == complexId);


            Assert.IsNotNull(complex);
            Assert.AreEqual(complex.ComplexName, "cmplx");
            Assert.AreEqual(complex.IsLocal, false);
            Assert.AreEqual(complex.StartDate, new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Local));
            Assert.AreEqual(complex.EndDate, new DateTime(year, 1, 3, 0, 0, 0, DateTimeKind.Local));

            
            // Добаваление ремонта
            var repairTypes = new GetRepairTypeListQuery(Context).Execute();
            var entityType = new GetEntityTypeListQuery(Context).Execute().Single(et => et.EntityType == EntityType.Pipeline);
            var entityId = CreatePipeline(PipelineType.Bridge);
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


            new AddRepairToComplexCommand(Context).Execute(
                new AddRepairToComplexParameterSet
                {
                    ComplexId = complexId,
                    RepairId = repairId
                });


            var repairList = new GetPlanRepairListByComplexIdQuery(Context).Execute(complexId);
            Assert.IsTrue(repairList.Any(r => r.Id == repairId));
            

            #endregion

            #region (Edit)

            new EditComplexCommand(Context).Execute(
                new EditComplexParameterSet
                    {
                        Id = complexId,
                        ComplexName = "cmplx2",
                        StartDate = new DateTime(year, 3, 1, 0, 0, 0, DateTimeKind.Local),
                        EndDate = new DateTime(year, 3, 2, 0, 0, 0, DateTimeKind.Local),
                        SystemId = system.Id
                    });

            complex = new GetComplexByIdQuery(Context).Execute(complexId);
            Assert.IsNotNull(complex);

            Assert.AreEqual(complex.ComplexName, "cmplx2");
            Assert.AreEqual(complex.StartDate, new DateTime(year, 3, 1, 0, 0, 0, DateTimeKind.Local));
            Assert.AreEqual(complex.EndDate, new DateTime(year, 3, 2, 0, 0, 0, DateTimeKind.Local));

            #endregion

            #region (Delete)

            
            new DeleteComplexCommand(Context).Execute(complexId);

            complexList = new GetComplexListQuery(Context).Execute(
                new GetRepairPlanParameterSet
                {
                    Year = year,
                    SystemId = system.Id
                });
            
            Assert.IsFalse(complexList.Any(c => c.Id == complexId));

            #endregion
        }
    }
}
