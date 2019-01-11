using System;
using System.Linq;
using GazRouter.DAL.Dictionaries.Regions;
using GazRouter.DAL.ObjectModel.CompShops;
using GazRouter.DAL.ObjectModel.CompStations;
using GazRouter.DAL.ObjectModel.Entities;
using GazRouter.DAL.ObjectModel.Valves;
using GazRouter.DTO.Dictionaries.EngineClasses;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PipelineTypes;
using GazRouter.DTO.Dictionaries.ValvePurposes;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.CompShops;
using GazRouter.DTO.ObjectModel.CompStations;
using GazRouter.DTO.ObjectModel.Valves;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DALTest.ObjectModel
{
    [TestClass]
    public class ValveTests : DalTestBase
    {
        [TestMethod, TestCategory(Stable)]
        public void FullTest()
        {
            var pipelineId = CreatePipeline(PipelineType.Branch);
            var siteId = CreateSite();
            var region = new GetRegionListQuery(Context).Execute().First();
            var compStationId =
                new AddCompStationCommand(Context).Execute(new AddCompStationParameterSet
                    {
                        ParentId = siteId,
                        Name = "TestCompStation",
                        RegionId = region.Id
                    });
            var compShopId =
                new AddCompShopCommand(Context).Execute(new AddCompShopParameterSet
                    {
                        ParentId = compStationId,
                        Name = "TestCompShop",
                        KmOfConn = 2.5,
                        PipelineId = pipelineId,
                        EngineClassId = EngineClass.Motorisierte
                    });

            var valveId =
                new AddValveCommand(Context).Execute(new AddValveParameterSet
                    {
                        Name = "NewValve",
                        Kilometr = 2,
                        ValveTypeId = 10,
                        PipelineId = pipelineId,
                        CompShopId = compShopId,
                        ValvePurposeId = ValvePurpose.TransversalCompShop
                    });

            Assert.AreNotEqual(default(Guid), valveId);
            var valves = new GetValveListQuery(Context).Execute(null);
            Assert.IsTrue(valves.Any());

            var valveDto = new GetValveByIdQuery(Context).Execute((valves.First()).Id);
            Assert.IsTrue(valveDto != null);

            const string bulshitName = "asdfasdfasfdasfdf";
            new EditValveCommand(Context).Execute(new EditValveParameterSet
                {
                    Id = valveId,
                    Name = bulshitName,
                    ValveTypeId = 10,
                    PipelineId = pipelineId,
                    CompShopId = compShopId,
                    ValvePurposeId = ValvePurpose.Linear
                });
			new SetSortOrderCommand(Context).Execute(new SetSortOrderParameterSet { Id = valveId, SortOrder = 10 });
            var editedValve =
                new GetValveListQuery(Context).Execute(new GetValveListParameterSet {PipelineId = pipelineId})
                    .First(p => p.Id == valveId);
			Assert.AreEqual(10, editedValve.SortOrder);
            Assert.AreEqual(bulshitName, editedValve.Name);

            new DeleteValveCommand(Context).Execute(new DeleteEntityParameterSet
                {
                    Id = valveId,
                    EntityType = EntityType.Valve
                });

            var delitedValve =
                new GetValveListQuery(Context).Execute(new GetValveListParameterSet {PipelineId = pipelineId})
                    .FirstOrDefault(p => p.Id == valveId);

            Assert.IsNull(delitedValve);
        }
    }
}
