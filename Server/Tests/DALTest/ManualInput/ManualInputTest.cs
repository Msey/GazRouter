using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.DAL.Dictionaries.CompUnitTypes;
using GazRouter.DAL.Dictionaries.SuperchargerTypes;
using GazRouter.DAL.ManualInput.ChemicalTests;
using GazRouter.DAL.ManualInput.CompUnitStates;
using GazRouter.DAL.ManualInput.CompUnitStates.Failures.Attachment;
using GazRouter.DAL.ManualInput.CompUnitTests;
using GazRouter.DAL.ManualInput.CompUnitTests.Attachment;
using GazRouter.DAL.ManualInput.InputStates;
using GazRouter.DAL.ManualInput.InputStory;
using GazRouter.DAL.ManualInput.ValveSwitches;
using GazRouter.DAL.ObjectModel.CompShops;
using GazRouter.DAL.ObjectModel.CompUnits;
using GazRouter.DAL.ObjectModel.MeasPoint;
using GazRouter.DAL.ObjectModel.Valves;
using GazRouter.DAL.SeriesData.Series;
using GazRouter.DTO.Attachments;
using GazRouter.DTO.Dictionaries.CompUnitFailureCauses;
using GazRouter.DTO.Dictionaries.CompUnitFailureFeatures;
using GazRouter.DTO.Dictionaries.CompUnitRepairTypes;
using GazRouter.DTO.Dictionaries.CompUnitSealingTypes;
using GazRouter.DTO.Dictionaries.CompUnitStopTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.StatesModel;
using GazRouter.DTO.Dictionaries.ValvePurposes;
using GazRouter.DTO.ManualInput.ChemicalTests;
using GazRouter.DTO.ManualInput.CompUnitStates;
using GazRouter.DTO.ManualInput.CompUnitTests;
using GazRouter.DTO.ManualInput.InputStates;
using GazRouter.DTO.ManualInput.InputStory;
using GazRouter.DTO.ManualInput.ValveSwitches;
using GazRouter.DTO.ObjectModel.CompUnits;
using GazRouter.DTO.ObjectModel.MeasPoint;
using GazRouter.DTO.ObjectModel.Valves;
using GazRouter.DTO.SeriesData.Series;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DALTest.ManualInput
{
    [TestClass]
    public class ManualInputTest : DalTestBase
    {
        [TestMethod, TestCategory(Stable)]
        public void Hourly()
        {
            var date = DateTime.Today;
            var siteId = CreateSite();
            var serieId = new AddSeriesCommand(Context).Execute(
                new AddSeriesParameterSet
                {
                    KeyDate = date,
                    PeriodTypeId = PeriodType.Twohours
                });

            Assert.IsNotNull(serieId);

            new SetManualInputStateCommand(Context).Execute(
                new SetManualInputStateParameterSet
                {
                    SerieId = serieId,
                    SiteId = siteId,
                    State = ManualInputState.Approved
                });

            var state = new GetManualInputStateListQuery(Context).Execute(
                new GetManualInputStateListParameterSet
                {
                    SerieId = serieId,
                    SiteId = siteId
                }).First();

            Assert.IsNotNull(state);
            Assert.AreEqual(state.State, ManualInputState.Approved);

            
            var story = new GetManualInputStoryQuery(Context).Execute(
                new GetManualInputStoryParameterSet
                {
                    EntityId = siteId,
                    SerieId = serieId
                });
            Assert.IsNotNull(story);

        }


        [TestMethod, TestCategory(Stable)]
        public void CompUnitStates()
        {
            var shopId = GetCompShop(Context);
            var unitTypeId = new GetCompUnitTypeListQuery(Context).Execute().First().Id;
            var superchargerTypeId = new GetSuperchargerTypesQuery(Context).Execute().First().Id;
            var firstUnitId = new AddCompUnitCommand(Context).Execute(
                new AddCompUnitParameterSet
                {
                    Name = "TestUnit1",
                    ParentId = shopId,
                    CompUnitTypeId = unitTypeId,
                    SuperchargerTypeId = superchargerTypeId,
                    SealingType = CompUnitSealingType.Dry
                });

            var secondUnitId = new AddCompUnitCommand(Context).Execute(
                new AddCompUnitParameterSet
                {
                    Name = "TestUnit2",
                    ParentId = shopId,
                    CompUnitTypeId = unitTypeId,
                    SuperchargerTypeId = superchargerTypeId,
                    SealingType = CompUnitSealingType.Dry
                });

            var theDate = DateTime.Now.AddMinutes(-5);


            // Добавляем аварийны останов и проверяем чтобы он добавился
            var failureId = new AddCompUnitStateCommand(Context).Execute(
                new AddCompUnitStateParameterSet
                {
                    CompUnitId = firstUnitId,
                    State = CompUnitState.Repair,
                    StateChangeDate = theDate,
                    StopType = CompUnitStopType.Emergency,
                    FailureCause = CompUnitFailureCause.Operational,
                    FailureFeature = CompUnitFailureFeature.R21,
                    RepairType = CompUnitRepairType.Current,
                    RepairCompletionDate = DateTime.Now.AddMonths(1)
                });
            Assert.IsNotNull(failureId);
            var changeStateList = new GetCompUnitStateListQuery(Context).Execute(new GetCompUnitStateListParameterSet());
            Assert.IsTrue(changeStateList.Any(s => s.Id == failureId));
            var state = changeStateList.Single(s => s.Id == failureId);
            Assert.AreEqual(state.State, CompUnitState.Repair);
            Assert.IsNotNull(state.FailureDetails);


            // Редактируем состояние
            new EditCompUnitStateCommand(Context).Execute(
                new EditCompUnitStateParameterSet
                {
                    StateId = failureId,
                    RepairType = CompUnitRepairType.Demounted,
                    StopType = CompUnitStopType.Forced,
                    FailureCause = CompUnitFailureCause.Factory,
                    FailureFeature = CompUnitFailureFeature.R1
                });
            changeStateList = new GetCompUnitStateListQuery(Context).Execute(new GetCompUnitStateListParameterSet());
            state = changeStateList.Single(s => s.Id == failureId);
            Assert.AreEqual(state.StopType, CompUnitStopType.Forced);
            Assert.AreEqual(state.FailureDetails.FailureCause, CompUnitFailureCause.Factory);
            Assert.AreEqual(state.FailureDetails.FailureFeature, CompUnitFailureFeature.R1);
            Assert.AreEqual(state.RepairType, CompUnitRepairType.Demounted);

            
            
            // Добавляем пуск агрегата
            var workId = new AddCompUnitStateCommand(Context).Execute(
                new AddCompUnitStateParameterSet
                {
                    CompUnitId = secondUnitId,
                    State = CompUnitState.Work,
                    StateChangeDate = theDate.AddMinutes(5)
                });
            Assert.IsNotNull(workId);
            changeStateList = new GetCompUnitStateListQuery(Context).Execute(new GetCompUnitStateListParameterSet());
            Assert.IsTrue(changeStateList.Any(s => s.Id == workId));



            //// Связываем пуск агрегата с аварийным остановом
            //new AddFailureRelatedUnitStartCommand(Context).Execute(
            //    new AddFailureRelatedUnitStartParameterSet
            //    {
            //        FailureDetailId = state.FailureDetails.FailureId,
            //        StateChangeId = workId
            //    });
            //var relatedUnits = new GetFailureRelatedUnitStartListQuery(Context).Execute(new List<int>{failureId});
            //Assert.IsNotNull(relatedUnits);
            //Assert.IsTrue(relatedUnits.Any(s => s.StateChangeId == workId));



            //// Удаляем связку
            //new RemoveFailureRelatedUnitStartCommand(Context).Execute(
            //    new AddFailureRelatedUnitStartParameterSet
            //    {
            //        FailureDetailId = failureId,
            //        StateChangeId = workId
            //    });
            //relatedUnits = new GetFailureRelatedUnitStartListQuery(Context).Execute(new List<int> { failureId });
            //Assert.IsTrue(relatedUnits.All(s => s.StateChangeId != workId));



            // Добавляем вложение
            var attachmentId = new AddFailureAttachmentCommand(Context).Execute(
                new AddAttachmentParameterSet<int>
                {
                    ExternalId = failureId,
                    Description = "unzip",
                    FileName = "unzip.zip"
                });
            var attachments = new GetFailureAttachmentListQuery(Context).Execute(new List<int> {failureId});
            Assert.IsNotNull(attachments);
            Assert.IsTrue(attachments.Any(a => a.ExternalId == failureId));


            // Удаляем вложение
            new DeleteFailureAttachmentCommand(Context).Execute(attachmentId);
            attachments = new GetFailureAttachmentListQuery(Context).Execute(new List<int> { failureId });
            Assert.IsTrue(attachments.All(a => a.ExternalId != failureId));


            // Удаляем состояние
            new DeleteCompUnitStateCommand(Context).Execute(failureId);
            changeStateList = new GetCompUnitStateListQuery(Context).Execute(new GetCompUnitStateListParameterSet());
            Assert.IsTrue(changeStateList.All(s => s.Id != failureId));
        }
        

        [TestMethod, TestCategory(Stable)]
        public void ValveSwitch()
        {
            var compShopId = GetCompShop(Context);
            var compShop = new GetCompShopByIdQuery(Context).Execute(compShopId);

            var valveId =
                new AddValveCommand(Context).Execute(new AddValveParameterSet
                {
                    Name = "NewValve" + Guid.NewGuid(),
                    Kilometr = 2,
                    ValveTypeId = 10,
                    PipelineId = compShop.PipelineId,
                    CompShopId = compShop.Id,
                    ValvePurposeId = ValvePurpose.TransversalCompShop
                });

            var date = DateTime.Now;

            new AddValveSwitchCommand(Context).Execute(
                new AddValveSwitchParameterSet
                {
                    ValveId = valveId,
                    SwitchingDate = date,
                    ValveSwitchType = ValveSwitchType.Valve,
                    State = ValveState.Opened
                });

            var list = 
            new GetValveSwitchListQuery(Context).Execute(
                new GetValveSwitchListParameterSet
                {
                    BeginDate = date.AddDays(-1),
                    EndDate = date.AddDays(1)
                });

            Assert.IsNotNull(list);
            Assert.IsTrue(list.Any(sw => sw.Id == valveId));


            new DeleteValveSwitchCommand(Context).Execute(
                new DeleteValveSwitchParameterSet
                {
                    ValveId = valveId,
                    SwitchingDate = date,
                    ValveSwitchType = ValveSwitchType.Valve
                });

            list =
            new GetValveSwitchListQuery(Context).Execute(
                new GetValveSwitchListParameterSet
                {
                    BeginDate = date.AddDays(-1),
                    EndDate = date.AddDays(1)
                });

            Assert.IsNotNull(list);
            Assert.IsTrue(list.All(sw => sw.Id != valveId));

        }


        [TestMethod, TestCategory(Stable)]
        public void ChemicalTest()
        {
            var compShopId = GetCompShop(Context);
            
            var measPointId = new AddMeasPointCommand(Context).Execute(
                new AddMeasPointParameterSet
                {
                    CompShopId = compShopId,
                    ChromatographConsumptionRate = 1,
                    ChromatographTestTime = 0,
                    Name = "Тест"
                });

            var date = DateTime.Today;

            var testId = 
            new AddChemicalTestCommand(Context).Execute(
                new AddChemicalTestParameterSet
                {
                    MeasPointId = measPointId,
                    TestDate = date,
                    DewPoint = -10,
                    DewPointHydrocarbon = - 11,
                    ContentNitrogen = 2,
                    ConcentrSourSulfur = 0.001,
                    ConcentrHydrogenSulfide = 0.002,
                    ContentCarbonDioxid = 3,
                    Density = 0.686,
                    CombHeatLow = 7100
                });

            Assert.IsNotNull(testId);

            var testList = new GetChemicalTestListQuery(Context).Execute(null);
            Assert.IsTrue(testList.Any(t => t.ChemicalTestId == testId));
            var test = testList.Single(t => t.ChemicalTestId == testId);
            Assert.AreEqual(test.MeasPointId, measPointId);
            Assert.AreEqual(test.TestDate, date);
            Assert.AreEqual(test.DewPoint, -10);
            Assert.AreEqual(test.DewPointHydrocarbon,  -11);
            Assert.AreEqual(test.ContentNitrogen, 2);
            Assert.AreEqual(test.ConcentrSourSulfur, 0.001);
            Assert.AreEqual(test.ConcentrHydrogenSulfide, 0.002);
            Assert.AreEqual(test.ContentCarbonDioxid, 3);
            Assert.AreEqual(test.Density, 0.686);
            Assert.AreEqual(test.CombHeatLow, 7100);


            date = DateTime.Today.AddHours(-1);
            new EditChemicalTestCommand(Context).Execute(
                new EditChemicalTestParameterSet
                {
                    ChemicalTestId = testId,
                    TestDate = date,
                    DewPoint = -12,
                    DewPointHydrocarbon = - 13,
                    ContentNitrogen = 3,
                    ConcentrSourSulfur = 0.333,
                    ConcentrHydrogenSulfide = 0.222,
                    ContentCarbonDioxid = 4,
                    Density = 0.777,
                    CombHeatLow = 8888
                });

            testList = new GetChemicalTestListQuery(Context).Execute(new GetChemicalTestListParameterSet());
            Assert.IsTrue(testList.Any(t => t.ChemicalTestId == testId));
            test = testList.Single(t => t.ChemicalTestId == testId);
            Assert.AreEqual(test.TestDate, date);
            Assert.AreEqual(-12, test.DewPoint);
            Assert.AreEqual(-13,test.DewPointHydrocarbon);
            Assert.AreEqual(test.ContentNitrogen, 3);
            Assert.AreEqual(test.ConcentrSourSulfur, 0.333);
            Assert.AreEqual(test.ConcentrHydrogenSulfide, 0.222);
            Assert.AreEqual(test.ContentCarbonDioxid, 4);
            Assert.AreEqual(test.Density, 0.777);
            Assert.AreEqual(test.CombHeatLow, 8888);

            new DeleteChemicalTestCommand(Context).Execute(testId);

            testList = new GetChemicalTestListQuery(Context).Execute(new GetChemicalTestListParameterSet());
            Assert.IsFalse(testList.Any(t => t.ChemicalTestId == testId));
            

        }


        [TestMethod]
        public void CompUnitsTests()
        {
            var shopId = GetCompShop(Context);
            var unitTypeId = new GetCompUnitTypeListQuery(Context).Execute().First().Id;
            var superchargerTypeId = new GetSuperchargerTypesQuery(Context).Execute().First().Id;
            var unitId = new AddCompUnitCommand(Context).Execute(
                new AddCompUnitParameterSet
                {
                    Name = "TestUnit",
                    ParentId = shopId,
                    CompUnitTypeId = unitTypeId,
                    SuperchargerTypeId = superchargerTypeId,
                    SealingType = CompUnitSealingType.Dry
                });

            // Добавление испытания
            var testId = new AddCompUnitTestCommand(Context).Execute(new AddCompUnitTestParameterSet
            {
                CompUnitId = unitId,
                CompUnitTestDate = new DateTime(2000, 10, 01),
                Description = "Test",
                Density = 0.77,
                Qmax = 500,
                Qmin = 100,
                TemperatureIn = 290
            });


            // Проверка получения списка испытания, проверка добавилось ли испытание
            var listTests = new GetCompUnitTestListQuery(Context).Execute(
                new GetCompUnitTestListParameterSet { CompUnitId = unitId });
            Assert.IsTrue(listTests.Any(t => t.Id == testId));

            new AddCompUnitTestPointCommand(Context).Execute(new AddCompUnitTestPointParameterSet
                                                                     {
                                                                         CompUnitTestId
                                                                             = testId,
                                                                         LineType = 1,
                                                                         X = 1,
                                                                         Y = 1
                                                                     });
            new AddCompUnitTestPointCommand(Context).Execute(new AddCompUnitTestPointParameterSet
                                                                 {
                                                                     CompUnitTestId
                                                                         = testId,
                                                                     LineType = 1,
                                                                     X = 1.2,
                                                                     Y = 1.3
                                                                 });
            new AddCompUnitTestPointCommand(Context).Execute(new AddCompUnitTestPointParameterSet
                                                                 {
                                                                     CompUnitTestId
                                                                         = testId,
                                                                     LineType = 2,
                                                                     X = 2,
                                                                     Y = 2
                                                                 });

            var points = new GetCompUnitTestPointsQuery(Context).Execute();
            Assert.IsTrue(points.Any(p => p.ParentId == testId));

            new DeleteCompUnitTestPointCommand(Context).Execute(new DeleteCompUnitTestPointParameterSet
                                                                    {
                                                                        CompUnitTestId
                                                                            = testId,
                                                                        LineType = 1
                                                                    });

            points = new GetCompUnitTestPointsQuery(Context).Execute();
            Assert.IsTrue(points.Any());

            // Проверка изменения испытания
            new EditCompUnitTestCommand(Context).Execute(new EditCompUnitTestParameterSet
            {
                CompUnitTestId = testId,
                CompUnitTestDate = new DateTime(2000, 10, 01),
                Description = "Test1",
                CompUnitId = unitId
            });

            listTests = new GetCompUnitTestListQuery(Context).Execute(
                new GetCompUnitTestListParameterSet { CompUnitId = unitId });
            Assert.IsTrue(listTests.Any(t => t.Description == "Test1"));



            var attId = new AddCompUnitTestAttachmentCommand(Context).Execute(
                new AddAttachmentParameterSet<int>
                {
                    Description = "Att1",
                    FileName = "File1",
                    ExternalId = testId
                });

            // Проверка получения списка испытания, проверка добавилось ли испытание
            var attachments = new GetCompUnitTestAttachmentListQuery(Context).Execute(listTests.Select(t => t.Id).ToList());
            Assert.IsTrue(attachments.Any(a => a.Id == attId));

            new DeleteCompUnitTestAttachmentCommand(Context).Execute(attId);

            attachments = new GetCompUnitTestAttachmentListQuery(Context).Execute(listTests.Select(t => t.Id).ToList());
            Assert.IsTrue(attachments.All(a => a.Id != attId));


            //Удаление испытания, проверка удалось ли удалить испытание
            new DeleteCompUnitTestCommand(Context).Execute(testId);
            listTests = new GetCompUnitTestListQuery(Context).Execute(
                new GetCompUnitTestListParameterSet { CompUnitId = unitId });
            Assert.IsTrue(listTests.All(t => t.Id != testId));

        }
    }
}