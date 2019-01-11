using System;
using System.Linq;
using GazRouter.DAL.Core;
using GazRouter.DAL.Dictionaries.BalanceSigns;
using GazRouter.DAL.Dictionaries.ConsumerTypes;
using GazRouter.DAL.Dictionaries.GasTransportSystem;
using GazRouter.DAL.Dictionaries.Regions;
using GazRouter.DAL.ObjectModel.CompShops;
using GazRouter.DAL.ObjectModel.CompStations;
using GazRouter.DAL.ObjectModel.Consumers;
using GazRouter.DAL.ObjectModel.DistrStationOutlets;
using GazRouter.DAL.ObjectModel.DistrStations;
using GazRouter.DAL.ObjectModel.MeasLine;
using GazRouter.DAL.ObjectModel.MeasStations;
using GazRouter.DAL.ObjectModel.Pipelines;
using GazRouter.DAL.ObjectModel.ReducingStations;
using GazRouter.DAL.ObjectModel.Segment.Site;
using GazRouter.DAL.ObjectModel.Sites;
using GazRouter.DAL.ObjectModel.Valves;
using GazRouter.DTO.Dictionaries.BalanceSigns;
using GazRouter.DTO.Dictionaries.EngineClasses;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PipelineTypes;
using GazRouter.DTO.Dictionaries.Regions;
using GazRouter.DTO.Dictionaries.ValvePurposes;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.CompShops;
using GazRouter.DTO.ObjectModel.CompStations;
using GazRouter.DTO.ObjectModel.Consumers;
using GazRouter.DTO.ObjectModel.DistrStationOutlets;
using GazRouter.DTO.ObjectModel.DistrStations;
using GazRouter.DTO.ObjectModel.MeasLine;
using GazRouter.DTO.ObjectModel.MeasStations;
using GazRouter.DTO.ObjectModel.Pipelines;
using GazRouter.DTO.ObjectModel.ReducingStations;
using GazRouter.DTO.ObjectModel.Segment;
using GazRouter.DTO.ObjectModel.Valves;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DALTest.ObjectModel
{
    [TestClass]
    public class IntegrityConstraintsTest : DalTestBase
    {
        [TestMethod, ExpectedException(typeof(IntegrityConstraintException)), TestCategory(Stable)]
        public void TestSiteIntegrityConstraintsMethod1()
        {
            Guid siteId = CreateSite();
            RegionDTO region = new GetRegionListQuery(Context).Execute().First();
            new AddCompStationCommand(Context).Execute(new AddCompStationParameterSet
                {
                    ParentId = siteId,
                    Name = "TestCompStation",
                    RegionId = region.Id
                });
            new DeleteSiteCommand(Context).Execute(new DeleteEntityParameterSet
                {
                    EntityType = EntityType.Site,
                    Id = siteId
                });
        }

        [TestMethod, ExpectedException(typeof(IntegrityConstraintException)), TestCategory(Stable)]
        public void TestCompStationIntegrityConstraintsMethod1()
        {
            Guid siteId = CreateSite();
            RegionDTO region = new GetRegionListQuery(Context).Execute().First();
            Guid compStationId =
                new AddCompStationCommand(Context).Execute(new AddCompStationParameterSet
                    {
                        ParentId = siteId,
                        Name = "TestCompStation",
                        RegionId = region.Id
                    });
            Guid pipelineId = CreatePipeline(PipelineType.Branch);

            new AddCompShopCommand(Context).Execute(new AddCompShopParameterSet
                {
                    ParentId = compStationId,
                    Name = "TestCompShop",
                    KmOfConn = 2.5,
                    PipelineId = pipelineId,
                    EngineClassId = EngineClass.Motorisierte
                });
            new DeleteCompStationCommand(Context).Execute(new DeleteEntityParameterSet
                {
                    EntityType = EntityType.CompStation,
                    Id = compStationId
                });
        }

        [TestMethod, ExpectedException(typeof(IntegrityConstraintException)), TestCategory(Stable)]
        public void TestCompShopIntegrityConstraintsMethod1()
        {
            Guid pipelineId = CreatePipeline(PipelineType.Branch);
            Guid siteId = CreateSite();
            RegionDTO region = new GetRegionListQuery(Context).Execute().First();
            Guid compStationId =
                new AddCompStationCommand(Context).Execute(new AddCompStationParameterSet
                    {
                        ParentId = siteId,
                        Name = "TestCompStation",
                        RegionId = region.Id
                    });
            Guid compShopId =
                new AddCompShopCommand(Context).Execute(new AddCompShopParameterSet
                    {
                        ParentId = compStationId,
                        Name = "TestCompShop",
                        KmOfConn = 2.5,
                        PipelineId = pipelineId,
                        EngineClassId = EngineClass.Motorisierte
                    });

            new AddValveCommand(Context).Execute(new AddValveParameterSet
                {
                    Name = "TestValv",
                    ValvePurposeId = ValvePurpose.OutletCompShop,
                    ValveTypeId = 10,
                    CompShopId = compShopId,
                    PipelineId = pipelineId
                });

            new DeleteCompShopCommand(Context).Execute(new DeleteEntityParameterSet
                {
                    EntityType = EntityType.CompShop,
                    Id = compShopId
                });
        }

        [TestMethod, ExpectedException(typeof(IntegrityConstraintException)), TestCategory(Stable)]
        public void TestCompShopIntegrityConstraintsMethod()
        {
			var gastransport = new GetGasTransportSystemListQuery(Context).Execute().First();
            Guid siteId =
                CreateSite();
            RegionDTO region = new GetRegionListQuery(Context).Execute().First();
            Guid compStationId =
                new AddCompStationCommand(Context).Execute(new AddCompStationParameterSet
                    {
                        ParentId = siteId,
                        Name = "TestCompStation",
                        RegionId = region.Id
                    });
            const double kilometerOfStartAfterCreation = 0.23;
            Guid pipelineId =
                new AddPipelineCommand(Context).Execute(new AddPipelineParameterSet
                    {
                        Name = "Pipeline1",
                        PipelineTypeId = PipelineType.Bridge,
                        Hidden = true,
                        SortOrder = 1,
                        KilometerOfStart = kilometerOfStartAfterCreation,
                        KilometerOfEnd = 0.43,
                        GasTransportSystemId = gastransport.Id
                    });
            Guid compShopId =
                new AddCompShopCommand(Context).Execute(new AddCompShopParameterSet
                    {
                        ParentId = compStationId,
                        Name = "TestCompShop",
                        KmOfConn = 2.5,
                        PipelineId = pipelineId,
                        EngineClassId = EngineClass.Motorisierte
                    });

            new DeletePipelineCommand(Context).Execute(new DeleteEntityParameterSet
                {
                    EntityType = EntityType.CompShop,
                    Id = pipelineId
                });
        }

        //в базе отсутствует констрайнт
        //[TestMethod, ExpectedException(typeof(IntegrityConstraintException))]
        //public void TestCompUnitIntegrityConstraintsMethod1()
        //{
        //    var enterpriseId =GetEnterpriseId(context);
        //    var siteId =
        //        CreateSite();
        //    var region = new GetRegionListCommand(Context).Execute().First();
        //    var compStationId =
        //        new AddCompStationCommand(Context).Execute(new AddCompStationParameterSet { ParentId = siteId, Name = "TestCompStation", RegionId = region.Id });
        //    var pipeline = new GetPipelineListQuery(Context).Execute().First();
        //    var compShopId = new AddCompShopCommand(Context).Execute(new AddCompShopParameterSet { ParentId = compStationId, Name = "TestCompShop", KmOfConn = 2.5, PipelineId = pipeline.Id, EngineClassId = (int)EngineClass.Motorisierte });
        //    var SuperchargerTypeId = new GetSuperchargerTypesCommand(Context).Execute().FirstOrDefault();
        //    var CompUnitTypeId = new GetCompUnitTypeListCommand(Context).Execute().FirstOrDefault();
        //    var compunitId =
        //        new AddCompUnitCommand(Context).Execute(new AddCompUnitParameterSet()
        //                                                    {
        //                                                        Name = "Test",
        //                                                        ParentId = compShopId,
        //                                                        SuperchargerTypeId = SuperchargerTypeId.Id,
        //                                                        CompUnitTypeId = CompUnitTypeId.Id,
        //                                                        IsHidden = false,
        //                                                        IsVirtual = false,
        //                                                        SortOrder = 1
        //                                                    });

        //    new DeleteCompShopCommand(Context).Execute(new DeleteEntityParameterSet { EntityType = EntityTypeEnum.CompressorStationName, Id = compShopId });
        //}

        [TestMethod, ExpectedException(typeof(IntegrityConstraintException)), TestCategory(Stable)]
        public void TestPipeLinesValveConstrainst()
        {
            const double kilometerOfStartAfterCreation = 0.23;
            const double kilometerOfEndAfterCreation = 0.43;
			var gastransport = new GetGasTransportSystemListQuery(Context).Execute().First();
            Guid pipelineId =
                new AddPipelineCommand(Context).Execute(new AddPipelineParameterSet
                    {
                        Name = "Pipeline1",
                        PipelineTypeId = PipelineType.Branch,
                        Hidden = true,
                        SortOrder = 1,
                        KilometerOfStart = kilometerOfStartAfterCreation,
                        KilometerOfEnd = kilometerOfEndAfterCreation,
                        GasTransportSystemId = gastransport.Id
                    });

            Assert.AreNotEqual(default(int), pipelineId);

            Guid valveId =
                new AddValveCommand(Context).Execute(new AddValveParameterSet
                    {
                        Name = "NewValve",
                        Kilometr = 2,
                        ValveTypeId = 10,
                        PipelineId = pipelineId,
                        ValvePurposeId = ValvePurpose.Linear
                    });
            Assert.AreNotEqual(default(Guid), valveId);
            new DeletePipelineCommand(Context).Execute(new DeleteEntityParameterSet
                {
                    Id = pipelineId,
                    EntityType = EntityType.Pipeline
                });
        }

        //[TestMethod, ExpectedException(typeof(IntegrityConstraintException)), TestCategory(Stable)]
        //public void TestPipeLinesConnConstrainst()
        //{
        //    const double kilometerOfStartAfterCreation = 0.23;
        //    const double lenghtAfterCreation = 0.2;
        //    var gastransport = new GetGasTransportSystemListCommand(Context).Execute().First();
        //    var pipelineId = new AddPipelineCommand(Context).Execute(
        //        new AddPipelineParameterSet
        //        {
        //            Name = "Pipeline1",
        //            PipelineTypeId = PipelineType.Looping,
        //            Hidden = true,
        //            SortOrder = 1,
        //            KilometerOfStart = kilometerOfStartAfterCreation,
        //            Lenght = lenghtAfterCreation,SystemId = gastransport.Id
        //        });

        //    Assert.IsNotNull(pipelineId);
        //    var destPipelineId = new AddPipelineCommand(Context).Execute(
        //        new AddPipelineParameterSet
        //        {
        //            Name = "Pipeline2",
        //            PipelineTypeId = PipelineType.Main,
        //            Hidden = true,
        //            SortOrder = 1,
        //            SystemId = gastransport.Id
        //        });

        //    var pipelineConnsId = new AddPipelineConnCommand(Context).Execute(
        //        new AddPipelineConnParameterSet
        //        {
        //            EndTypeId = PipelineEndType.StartType,
        //            DestEntityId = destPipelineId,
        //            PipelineId = pipelineId,
        //            Kilometr = 2
        //        });

        //    Assert.AreNotEqual(default(int), pipelineConnsId);
        //    new DeletePipelineCommand(Context).Execute(
        //        new DeleteEntityParameterSet
        //        {
        //            Id = pipelineId,
        //            EntityType = EntityType.Pipeline
        //        });
        //}

        [TestMethod, ExpectedException(typeof(IntegrityConstraintException)), TestCategory(Stable)]
        public void TestMeasuringLineIntegrityConstraintsMethod1()
        {
            Guid siteId =
                CreateSite();
            Guid measStation =
                new AddMeasStationCommand(Context).Execute(new AddMeasStationParameterSet
                    {
                        IsVirtual = false,
                        IsHidden = false,
                        Name = "Test123123",
                        ParentId = siteId,
                        BalanceSignId = Sign.In
                    });
            BalanceSignDTO balansSign = new GetBalanceSignsListQuery(Context).Execute().First();
            const double kilometerOfStartAfterCreation = 0.23;
            const double lenghtAfterCreation = 0.2;
			var gastransport = new GetGasTransportSystemListQuery(Context).Execute().First();
            Guid pipelineId = CreatePipeline(PipelineType.Bridge);/*
                new AddPipelineCommand(Context).Execute(new AddPipelineParameterSet
                    {
                        Name = "Pipeline1",
                        PipelineTypeId = PipelineType.Bridge,
                        Hidden = true,
                        SortOrder = 1,
                        KilometerOfStart = kilometerOfStartAfterCreation,
                        Lenght = lenghtAfterCreation,SystemId = gastransport.Id
                    });*/
            new AddMeasLineCommand(Context).Execute(new AddMeasLineParameterSet
                {
                    IsVirtual = false,
                    IsHidden = false,
                    KmOfConn = 10,
                    Name = "TestLine",
                    ParentId = measStation,
                    PipelineId = pipelineId,
                    Status = true
                });
            new DeleteMeasStationCommand(Context).Execute(new DeleteEntityParameterSet
                {
                    EntityType = EntityType.MeasStation,
                    Id = measStation
                });
        }

        [TestMethod, ExpectedException(typeof(IntegrityConstraintException)), TestCategory(Stable)]
        public void TestMeasuringLineIntegrityConstraintsMethod()
        {
            Guid siteId = CreateSite();
            Guid measStation =
                new AddMeasStationCommand(Context).Execute(new AddMeasStationParameterSet
                    {
                        IsVirtual = false,
                        IsHidden = false,
                        Name = "Test123123",
                        ParentId = siteId,
                        BalanceSignId = Sign.In
                    });
            var  balansSigns = new GetBalanceSignsListQuery(Context).Execute();

            var balansSign = balansSigns.First();
            

            const double kilometerOfStartAfterCreation = 0.23;
            const double lenghtAfterCreation = 0.2;
			var gastransport = new GetGasTransportSystemListQuery(Context).Execute().First();
            Guid pipelineId = CreatePipeline(PipelineType.Bridge);
/*
                new AddPipelineCommand(Context).Execute(new AddPipelineParameterSet
                    {
                        Name = "Pipeline1",
                        PipelineTypeId = PipelineType.Bridge,
                        Hidden = true,
                        SortOrder = 1,
                        KilometerOfStart = kilometerOfStartAfterCreation,
                        Lenght = lenghtAfterCreation,SystemId = gastransport.Id
                    });
*/
            new AddMeasLineCommand(Context).Execute(new AddMeasLineParameterSet
                {
                    IsVirtual = false,
                    IsHidden = false,
                    KmOfConn = 10,
                    Name = "TestLine",
                    ParentId = measStation,
                    PipelineId = pipelineId,
                    Status = true
                });
            new DeletePipelineCommand(Context).Execute(new DeleteEntityParameterSet
                {
                    EntityType = EntityType.Pipeline,
                    Id = pipelineId
                });
        }

        [TestMethod, ExpectedException(typeof(IntegrityConstraintException)), TestCategory(Stable)]
        public void TestMeasuringstationIntegrityConstraintsMethod1()
        {
            Guid siteId = CreateSite();
            new AddMeasStationCommand(Context).Execute(new AddMeasStationParameterSet
                {
                    IsVirtual = false,
                    IsHidden = false,
                    Name = "Test123123",
                    ParentId = siteId,
                    BalanceSignId = Sign.In
                });
            new DeleteSiteCommand(Context).Execute(new DeleteEntityParameterSet
                {
                    EntityType = EntityType.Site,
                    Id = siteId
                });
        }

        [TestMethod, ExpectedException(typeof(IntegrityConstraintException)), TestCategory(Stable)]
        public void TestDistrStationIntegrityConstraintsMethod()
        {
            Guid siteId = CreateSite();
            Guid distrId =
                new AddDistrStationCommand(Context).Execute(new AddDistrStationParameterSet
                    {
                        IsVirtual = false,
                        IsHidden = false,
                        Name = "TestDistrStation",
                        ParentId = siteId,
                        CapacityRated = 2,
                        PressureRated = 5
                    });
            new AddDistrStationOutletCommand(Context).Execute(new AddDistrStationOutletParameterSet
                {
                    CapacityRated = 5.5,
                    IsVirtual = false,
                    IsHidden = false,
                    ParentId = distrId,
                    Name = "TestOut",
                    PressureRated = 2
                });

            new DeleteDistrStationCommand(Context).Execute(new DeleteEntityParameterSet
                {
                    EntityType = EntityType.Site,
                    Id = distrId
                });
        }

        [TestMethod, ExpectedException(typeof(IntegrityConstraintException)), TestCategory(Stable)]
        public void TestDistrConsumerIntegrityConstraintsMethod()
        {
            var regionId = new GetRegionListQuery(Context).Execute().First().Id;
            Guid siteId = CreateSite();
            Guid distrId =
                new AddDistrStationCommand(Context).Execute(new AddDistrStationParameterSet
                    {
                        IsVirtual = false,
                        IsHidden = false,
                        Name = "TestDistrStation",
                        ParentId = siteId,
                        CapacityRated = 2,
                        PressureRated = 5
                    });
            var constypes = new GetConsumerTypesListQuery(Context).Execute();
            var constype = constypes.First();

            Guid consId =
                new AddConsumerCommand(Context).Execute(new AddConsumerParameterSet
                    {
                        IsVirtual = false,
                        IsHidden = false,
                        Name = "TestCons1",
                        ParentId = distrId,
                        ConsumerType = constype.Id,
                        RegionId = regionId
                    });

            new DeleteDistrStationCommand(Context).Execute(new DeleteEntityParameterSet
                {
                    EntityType = EntityType.Site,
                    Id = distrId
                });
        }

        [TestMethod, ExpectedException(typeof(IntegrityConstraintException)), TestCategory(Stable)]
        public void TestDistrReducingStationIntegrityConstraintsMethod()
        {
            Guid siteId =
                CreateSite();
            var gastransport = new GetGasTransportSystemListQuery(Context).Execute().First();
            Guid pipelineId =
                new AddPipelineCommand(Context).Execute(new AddPipelineParameterSet
                    {
                        Name = "Pipeline1",
                        PipelineTypeId = PipelineType.Bridge,
                        Hidden = true,
                        SortOrder = 1,
                        KilometerOfStart = 0.23,
                        KilometerOfEnd = 0.43,
                        GasTransportSystemId = gastransport.Id
                    });
            const double kilometer = 20.0;
            Guid reducStationId =
                new AddReducingStationCommand(Context).Execute(new AddReducingStationParameterSet
                    {
                        IsVirtual = false,
                        Hidden = false,
                        Name = "TestRS",
                        SortOrder = 1,
                        Kilometer = kilometer,
                        MainPipelineId = pipelineId,
                        SiteId = siteId,
                        Status = true
                    });
            new DeletePipelineCommand(Context).Execute(new DeleteEntityParameterSet
                {
                    EntityType = EntityType.Pipeline,
                    Id = pipelineId
                });
        }

        [TestMethod, ExpectedException(typeof(IntegrityConstraintException)), TestCategory(Stable)]
        public void TestDistrReducingStationIntegrityConstraintsMethod1()
        {
            
            Guid siteId =
                CreateSite();
           
            Guid pipelineId = CreatePipeline(PipelineType.Bridge);
               
            const double kilometer = 20.0;
            Guid reducStationId =
                new AddReducingStationCommand(Context).Execute(new AddReducingStationParameterSet
                    {
                        IsVirtual = false,
                        Hidden = false,
                        Name = "TestRS2",
                        SortOrder = 1,
                        Kilometer = kilometer,
                        MainPipelineId = pipelineId,
                        SiteId = siteId,
                        Status = true
                    });
            new DeleteSiteCommand(Context).Execute(new DeleteEntityParameterSet
                {
                    EntityType = EntityType.Site,
                    Id = siteId
                });
        }

        [TestMethod, ExpectedException(typeof(IntegrityConstraintException)), TestCategory(Stable)]
        public void TestDistrSegmentIntegrityConstraintsMethod()
        {
            Guid siteId =
                CreateSite();
            const double kilometerOfStartAfterCreation = 0.23;
            const double lenghtAfterCreation = 0.2;
			var gastransport = new GetGasTransportSystemListQuery(Context).Execute().First();
            Guid pipelineId = CreatePipeline(PipelineType.Bridge)
                ;/*                new AddPipelineCommand(Context).Execute(new AddPipelineParameterSet
                    {
                        Name = "Pipeline1",
                        PipelineTypeId = PipelineType.Bridge,
                        Hidden = true,
                        SortOrder = 1,
                        KilometerOfStart = kilometerOfStartAfterCreation,
                        Lenght = lenghtAfterCreation,SystemId = gastransport.Id
                    });*/
            const double kilometerOfEnd = 20.0;
            int segmentId =
                new AddSiteSegmentCommand(Context).Execute(new AddSiteSegmentParameterSet
                    {
                        PipelineId = pipelineId,
                        SiteId = siteId,
                        KilometerOfStartPoint = kilometerOfStartAfterCreation,
                        KilometerOfEndPoint = kilometerOfEnd
                    });
            new DeletePipelineCommand(Context).Execute(new DeleteEntityParameterSet
                {
                    EntityType = EntityType.Pipeline,
                    Id = pipelineId
                });
        }

        [TestMethod, ExpectedException(typeof(IntegrityConstraintException)), TestCategory(Stable)]
        public void TestDistrSegmentIntegrityConstraintsMethod1()
        {
            Guid siteId =
                CreateSite();
            const double kilometerOfStartAfterCreation = 0.23;
            const double lenghtAfterCreation = 0.2;
			var gastransport = new GetGasTransportSystemListQuery(Context).Execute().First();
            Guid pipelineId = CreatePipeline(PipelineType.Bridge);/*
                new AddPipelineCommand(Context).Execute(new AddPipelineParameterSet
                    {
                        Name = "Pipeline1",
                        PipelineTypeId = PipelineType.Bridge,
                        Hidden = true,
                        SortOrder = 1,
                        KilometerOfStart = kilometerOfStartAfterCreation,
                        Lenght = lenghtAfterCreation,SystemId = gastransport.Id
                    });*/
            const double kilometerOfEnd = 20.0;
            new AddSiteSegmentCommand(Context).Execute(new AddSiteSegmentParameterSet
                {
                    PipelineId = pipelineId,
                    SiteId = siteId,
                    KilometerOfStartPoint = kilometerOfStartAfterCreation,
                    KilometerOfEndPoint = kilometerOfEnd
                });
            new DeleteSiteCommand(Context).Execute(new DeleteEntityParameterSet
                {
                    EntityType = EntityType.Site,
                    Id = siteId
                });
        }

        [TestMethod, ExpectedException(typeof(IntegrityConstraintException)), TestCategory(Stable)]
        public void TestDistrStationSiteConstraintsMethod()
        {
            Guid newGuidSite = CreateSite();

            Guid newDistrStationSite =
                new AddDistrStationCommand(Context).Execute(new AddDistrStationParameterSet
                    {
                        CapacityRated = 12,
                        PressureRated = 12,
                        IsVirtual = true,
                        ParentId = newGuidSite,
                        Name = "TestDistrStationSite"
                    });

            new DeleteSiteCommand(Context).Execute(new DeleteEntityParameterSet
                {
                    EntityType = EntityType.Site,
                    Id = newGuidSite
                });
        }
        
    }
}