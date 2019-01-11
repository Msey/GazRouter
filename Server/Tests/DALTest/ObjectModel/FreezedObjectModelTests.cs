using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using GazRouter.DAL.Dictionaries.CompUnitTypes;
using GazRouter.DAL.Dictionaries.Enterprises;
using GazRouter.DAL.Dictionaries.GasTransportSystem;
using GazRouter.DAL.Dictionaries.Regions;
using GazRouter.DAL.Dictionaries.SuperchargerTypes;
using GazRouter.DAL.ObjectModel.CompShops;
using GazRouter.DAL.ObjectModel.CompStationCoolingRecomended;
using GazRouter.DAL.ObjectModel.CompStations;
using GazRouter.DAL.ObjectModel.CompUnits;
using GazRouter.DAL.ObjectModel.DistrStationOutlets;
using GazRouter.DAL.ObjectModel.DistrStations;
using GazRouter.DAL.ObjectModel.MeasStations;
using GazRouter.DAL.ObjectModel.Sites;
using GazRouter.DTO.Dictionaries.BalanceSigns;
using GazRouter.DTO.Dictionaries.CompUnitSealingTypes;
using GazRouter.DTO.Dictionaries.CompUnitTypes;
using GazRouter.DTO.Dictionaries.EngineClasses;
using GazRouter.DTO.Dictionaries.Enterprises;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PipelineTypes;
using GazRouter.DTO.Dictionaries.Regions;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.CompShops;
using GazRouter.DTO.ObjectModel.CompStationCoolingRecomended;
using GazRouter.DTO.ObjectModel.CompStations;
using GazRouter.DTO.ObjectModel.CompUnits;
using GazRouter.DTO.ObjectModel.DistrStationOutlets;
using GazRouter.DTO.ObjectModel.DistrStations;
using GazRouter.DTO.ObjectModel.MeasStations;
using GazRouter.DTO.ObjectModel.Sites;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;

namespace DALTest.ObjectModel
{
    [TestClass]
    public class FreezedObjectModelTests : DalTestBase
    {
        [TestMethod ,TestCategory(Stable)]
        public void GetEnterpriseListCommandTest()
        {
            List<EnterpriseDTO> list = new GetEnterpriseListQuery(Context).Execute(null);

            AssertHelper.IsNotEmpty(list);
        }

        [TestMethod ,TestCategory(Stable)]
        public void GetSiteListCommandTest()
        {
            CreateSite();
            List<SiteDTO> list = new GetSiteListQuery(Context).Execute(null);
            AssertHelper.IsNotEmpty(list);
        }

        [TestMethod ,TestCategory(Stable)]
        public void GetCompStationListCommandTest()
        {
            var siteId =
                CreateSite();
            var region = new GetRegionListQuery(Context).Execute().First();
            var compStationId =
                new AddCompStationCommand(Context).Execute(new AddCompStationParameterSet
                    {
                        ParentId = siteId,
                        Name = "TestCompStation",
                        RegionId = region.Id
                    });
            List<CompStationDTO> list = new GetCompStationListQuery(Context).Execute(null);
            AssertHelper.IsNotEmpty(list);
            var compDto = new GetCompStationByIdQuery(Context).Execute((list.First()).Id);
            list = new GetCompStationListQuery(Context).Execute(new GetCompStationListParameterSet {SiteId = siteId});
            Assert.IsTrue(compDto != null);
            Assert.IsTrue(list.Any());
        }


        [TestMethod ,TestCategory(Stable)]
        public void GetDistrStationListCommandTest()
        {
            var siteId = CreateSite();
            var distrId =
                new AddDistrStationCommand(Context).Execute(new AddDistrStationParameterSet
                    {
                        Name = "TestDistrStation",
                        ParentId = siteId,
                        CapacityRated = 2,
                        PressureRated = 5
                    });
            List<DistrStationDTO> list;
            {
                list = new GetDistrStationListQuery(Context).Execute(null);
            }
            Assert.IsTrue(list != null && list.FirstOrDefault(p => p.Name == "TestDistrStation") != null);
            var distrDto = new GetDistrStationByIdQuery(Context).Execute(list.First().Id);
            Assert.IsTrue(distrDto != null);
        }

        [TestMethod ,TestCategory(Stable)]
        public void AddEditDeleteSiteTest()
        {
            Guid newEntGuid =
                GetEnterpriseId();
            Guid newGuid = CreateSite(newEntGuid);
            Assert.IsTrue(newGuid != Guid.Empty);
			var gastransport = new GetGasTransportSystemListQuery(Context).Execute().First();
            new EditSiteCommand(Context).Execute(new EditSiteParameterSet
                {
                    Id = newGuid,
                    Name = "NNNNN",
					ParentId = newEntGuid,
					GasTransportSystemId = gastransport.Id
                });
            List<SiteDTO> list = new GetSiteListQuery(Context).Execute(null);
            SiteDTO edited = list.FirstOrDefault(l => l.Id == newGuid);
            Assert.IsTrue(edited != null && edited.Name == "NNNNN");

            new DeleteSiteCommand(Context).Execute(new DeleteEntityParameterSet
                {
                    Id = newGuid,
                    EntityType = EntityType.Site
                });
            list = new GetSiteListQuery(Context).Execute(null);
            Assert.IsFalse(list.Any(l => l.Id == newGuid));
        }


        [TestMethod ,TestCategory(Stable)]
        public void AddEditDeleteDistrStationTest()
        {
            {
                Guid newGuidSite =
                                  CreateSite();
                Guid newGuid =
                    new AddDistrStationCommand(Context).Execute(new AddDistrStationParameterSet
                                                                    {
                                                                        Name = "Тест ГРС",
                                                                        ParentId = newGuidSite
                                                                    });
                Assert.IsTrue(newGuid != Guid.Empty);
                new EditDistrStationCommand(Context).Execute(new EditDistrStationParameterSet { Id = newGuid, Name = "NNNNN", ParentId = newGuidSite });
                List<DistrStationDTO> list = new GetDistrStationListQuery(Context).Execute(null);
                DistrStationDTO edited = list.FirstOrDefault(l => l.Id == newGuid);
                Assert.IsTrue(edited != null && edited.Name == "NNNNN");
                new DeleteDistrStationCommand(Context).Execute(new DeleteEntityParameterSet
                                                                   {
                                                                       Id = newGuid,
                                                                       EntityType = EntityType.DistrStation
                                                                   });
                list = new GetDistrStationListQuery(Context).Execute(null);
                Assert.IsFalse(list.Any(l => l.Id == newGuid));
            }
        }

        [TestMethod ,TestCategory(Stable)]
        public void AddEditDeleteDistrStationOutletTest()
        {
            Guid newGuidSite = CreateSite();
            Guid distrStationId =
                new AddDistrStationCommand(Context).Execute(new AddDistrStationParameterSet
                    {
                        Name = "TestSite",
                        ParentId = newGuidSite
                    });
            Guid dsoId =
                new AddDistrStationOutletCommand(Context).Execute(new AddDistrStationOutletParameterSet
                    {
                        Name = "Outlet1",
                        ParentId = distrStationId,
                        CapacityRated = 5.5
                    });
            new EditDistrStationOutletCommand(Context).Execute(new EditDistrStationOutletParameterSet
                {
                    Id = dsoId,
                    Name = "Outlet2",
                    ParentId = distrStationId,
                    CapacityRated = 6
                });
            DistrStationOutletDTO distrStationOutlet =
                new GetDistrStationOutletListQuery(Context).Execute(new GetDistrStationOutletListParameterSet { DistrStationId = distrStationId }).Single(dso => dso.Id == dsoId);
            new DeleteDistrStationOutletCommand(Context).Execute(new DeleteEntityParameterSet
                {
                    Id = dsoId,
                    EntityType =
                        EntityType.DistrStationOutlet
                });
            Assert.IsTrue(
                new GetDistrStationOutletListQuery(Context).Execute(new GetDistrStationOutletListParameterSet { DistrStationId = distrStationId }).All(dso => dso.Id != dsoId));
        }

        [TestMethod ,TestCategory(Stable)]
        public void AddEditDeleteMeasStationTest()
        {
            Guid newGuidSite = CreateSite();
            Guid newGuid =
                new AddMeasStationCommand(Context).Execute(new AddMeasStationParameterSet
                    {
                        Name = "TestSite",
                        ParentId = newGuidSite,
                        BalanceSignId = Sign.In
                    });
            Assert.IsTrue(newGuid != Guid.Empty);
            new EditMeasStationCommand(Context).Execute(new EditMeasStationParameterSet
                {
                    Id = newGuid,
                    Name = "NNNNN",
                    ParentId = newGuidSite,
                    BalanceSignId = Sign.In
                });
            List<MeasStationDTO> list = new GetMeasStationListQuery(Context).Execute(null);
            MeasStationDTO edited = list.FirstOrDefault(l => l.Id == newGuid);
            Assert.IsTrue(edited != null && edited.Name == "NNNNN");
            var measDto = new GetMeasStationByIdQuery(Context).Execute(list.First().Id);
            Assert.IsTrue(measDto != null);
            new DeleteMeasStationCommand(Context).Execute(new DeleteEntityParameterSet
                {
                    Id = newGuid,
                    EntityType = EntityType.MeasStation
                });
            list = new GetMeasStationListQuery(Context).Execute(null);
            Assert.IsFalse(list.Any(l => l.Id == newGuid));
        }

        [TestMethod ,TestCategory(Stable)]
        public void GetRegionListCommandTest()
        {
            List<RegionDTO> list = new GetRegionListQuery(Context).Execute();
            AssertHelper.IsNotEmpty(list);
        }

        [TestMethod ,TestCategory(Stable)]
        public void AddEditDeleteCompStationTest()
        {
            Guid newGuidSite = CreateSite();
            const int chitaRegionId = 76;
            Guid newGuid =
                new AddCompStationCommand(Context).Execute(new AddCompStationParameterSet
                    {
                        Name = "TestCompStation",
                        ParentId = newGuidSite,
                        RegionId = chitaRegionId
                    });
            Assert.IsTrue(newGuid != null);
            new EditCompStationCommand(Context).Execute(new EditCompStationParameterSet
                {
                    Id = newGuid,
                    Name = "NNNNN",
                    ParentId = newGuidSite,
                    RegionId = chitaRegionId
                });
            List<CompStationDTO> list = new GetCompStationListQuery(Context).Execute(null);
            CompStationDTO edited = list.FirstOrDefault(l => l.Id == newGuid);
            Assert.IsTrue(edited != null && edited.Name == "NNNNN" && edited.RegionId == chitaRegionId);

            new DeleteCompStationCommand(Context).Execute(new DeleteEntityParameterSet
                {
                    Id = newGuid,
                    EntityType = EntityType.CompStation
                });
            list = new GetCompStationListQuery(Context).Execute(null);
            Assert.IsFalse(list.Any(l => l.Id == newGuid));
        }


        [TestMethod ,TestCategory(Stable)]
        public void AddEditDeleteCompShopTest()
        {
            Guid newGuidSite = CreateSite();
            Guid stationId =
                new AddCompStationCommand(Context).Execute(new AddCompStationParameterSet
                    {
                        Name = "TestCompStation",
                        ParentId = newGuidSite,
                        RegionId = 76
                    });
            
            var pipelineId = CreatePipeline(PipelineType.CompressorShopBridge);
         
            Guid shopId =
                new AddCompShopCommand(Context).Execute(new AddCompShopParameterSet
                    {
                        Name = "TestShop",
                        ParentId = stationId,
                        PipelineId = pipelineId,
                        EngineClassId = EngineClass.Motorisierte
                    });
            new EditCompShopCommand(Context).Execute(new EditCompShopParameterSet
                {
                    Id = shopId,
                    Name = "NNNNN",
                    ParentId = stationId,
                    PipelineId = pipelineId,
                    EngineClassId = EngineClass.Turbine
                });
            List<CompShopDTO> list = new GetCompShopListQuery(Context).Execute(null);

            var edited = list.FirstOrDefault(l => l.Id == shopId);
            Assert.IsTrue(edited != null && edited.Name == "NNNNN");
            new DeleteCompShopCommand(Context).Execute(new DeleteEntityParameterSet
                {
                    Id = shopId,
                    EntityType = EntityType.CompShop
                });
            list = new GetCompShopListQuery(Context).Execute(null);
            Assert.IsFalse(list.Any(l => l.Id == shopId));
            var compShopDto = new GetCompShopByIdQuery(Context).Execute((list.First()).Id);
            Assert.IsTrue(compShopDto != null);
        }

        [TestMethod ,TestCategory(Stable)]
        public void AddEditDeleteCompUnitTest()
        {
            Guid newGuidSite =
                CreateSite();

            Guid stationId =
                new AddCompStationCommand(Context).Execute(new AddCompStationParameterSet
                    {
                        Name = "TestCompStation",
                        ParentId = newGuidSite,
                        RegionId = 76
                    });
            
            var pipelineId = CreatePipeline(PipelineType.Branch);
              
            Guid shopId =
                new AddCompShopCommand(Context).Execute(new AddCompShopParameterSet
                    {
                        Name = "TestShop",
                        ParentId = stationId,
                        PipelineId = pipelineId,
                        EngineClassId = EngineClass.Motorisierte
                    });
            var unitTypeIds = new GetCompUnitTypeListQuery(Context).Execute();
            int unitTypeId1 = unitTypeIds[0].Id;
            int unitTypeId2 = unitTypeIds[1].Id;
            int superchargerTypeId = new GetSuperchargerTypesQuery(Context).Execute().First().Id;
            var unitId =
                new AddCompUnitCommand(Context).Execute(new AddCompUnitParameterSet
                    {
                        Name = "TestUnit",
                        ParentId = shopId,
                        CompUnitTypeId = unitTypeId1,
                        SuperchargerTypeId = superchargerTypeId,
                        SealingType = CompUnitSealingType.Dry
                    });
            new EditCompUnitCommand(Context).Execute(new EditCompUnitParameterSet
                {
                    Id = unitId,
                    Name = "TestUnit1",
                    ParentId = shopId,
                    CompUnitTypeId = unitTypeId2,
                    SuperchargerTypeId = superchargerTypeId,
                    SealingType = CompUnitSealingType.Dry
                });
            var unit = new GetCompUnitListQuery(Context).Execute(null).First(cu => cu.Id == unitId);
            Assert.IsTrue(unit.Name == "TestUnit1");
            List<CompUnitDTO> list = new GetCompUnitListQuery(Context).Execute(null);
            var compUnitDto = new GetCompUnitByIdQuery(Context).Execute((list.First()).Id);
            Assert.IsTrue(compUnitDto != null);

            var compUnitListByStationId =
                new GetCompUnitListQuery(Context).Execute(new GetCompUnitListParameterSet {StationId = stationId});
            Assert.IsTrue(compUnitListByStationId.Any());

            var compUnitListByShopId =
                new GetCompUnitListQuery(Context).Execute(new GetCompUnitListParameterSet {ShopId = shopId});
            Assert.IsTrue(compUnitListByShopId.Any());

            new DeleteCompUnitCommand(Context).Execute(new DeleteEntityParameterSet
                {
                    Id = unitId,
                    EntityType = EntityType.CompUnit
                });
            list = new GetCompUnitListQuery(Context).Execute(null);
            Assert.IsFalse(list.Any(l => l.Id == unitId));
        }

		[TestMethod, TestCategory(Stable)]
		public void GetCompStationCoolingRecomendedListCommandTest()
		{
			var siteId =
				CreateSite();
			var region = new GetRegionListQuery(Context).Execute().First();
			var compStationId =
				new AddCompStationCommand(Context).Execute(new AddCompStationParameterSet
				{
					ParentId = siteId,
					Name = "TestCompStation",
					RegionId = region.Id
				});
		    
            new SetCompStationCoolingRecomendedCommand(Context).Execute(new SetCompStationCoolingRecomendedParameterSet
		    {
		        CompStationId = compStationId,
		        Month = 3,
		        Temperature = 6.0
		    });

		    var list = new GetCompStationCoolingRecomendedListQuery(Context).Execute(compStationId);
            Assert.IsNotNull(list);
            Assert.IsTrue(list.Any());
            Assert.AreEqual(list.Single(t => t.Month == 3 && t.CompStationId == compStationId).Temperature, 6.0);


            new SetCompStationCoolingRecomendedCommand(Context).Execute(new SetCompStationCoolingRecomendedParameterSet
            {
                CompStationId = compStationId,
                Month = 3,
                Temperature = 11.0
            });

            list = new GetCompStationCoolingRecomendedListQuery(Context).Execute(compStationId);
            Assert.IsNotNull(list);
            Assert.IsTrue(list.Any());
            Assert.AreEqual(list.Single(t => t.Month == 3 && t.CompStationId == compStationId).Temperature, 11.0);

            new SetCompStationCoolingRecomendedCommand(Context).Execute(new SetCompStationCoolingRecomendedParameterSet
            {
                CompStationId = compStationId,
                Month = 3
            });

            list = new GetCompStationCoolingRecomendedListQuery(Context).Execute(compStationId);
            Assert.IsNotNull(list);
            Assert.IsFalse(list.Any());
		}
        
    }
}