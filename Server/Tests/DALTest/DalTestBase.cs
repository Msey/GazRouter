using System;
using System.Configuration;
using System.Linq;
using GazRouter.DAL.Dictionaries.Enterprises;
using GazRouter.DAL.Dictionaries.GasTransportSystem;
using GazRouter.DAL.Dictionaries.Regions;
using GazRouter.DAL.ObjectModel.CompShops;
using GazRouter.DAL.ObjectModel.CompStations;
using GazRouter.DAL.ObjectModel.Pipelines;
using GazRouter.DAL.ObjectModel.Sites;
using GazRouter.DTO.Dictionaries.EngineClasses;
using GazRouter.DTO.Dictionaries.PipelineTypes;
using GazRouter.DTO.ObjectModel.CompShops;
using GazRouter.DTO.ObjectModel.CompStations;
using GazRouter.DTO.ObjectModel.Pipelines;
using GazRouter.DTO.ObjectModel.Sites;
using TestBase.Infra;

namespace DALTest
{
    public class DalTestBase : TransactionTestsBase
    {

        protected Guid GetEnterpriseId()
        {
            return new GetEnterpriseListQuery(Context).Execute(null).First().Id;
        }

        protected Guid CreateSite(Guid? enterpriseId = null)
        {
            if (!enterpriseId.HasValue)
                enterpriseId = GetEnterpriseId();
            var gastransport = new GetGasTransportSystemListQuery(Context).Execute().First();
            return new AddSiteCommand(Context).Execute(new AddSiteParameterSet { Name = "TestSite" + Guid.NewGuid(), ParentId = enterpriseId.Value, GasTransportSystemId = gastransport.Id });
        }

        protected Guid CreatePipeline(PipelineType pipelineType)
        {
            var gastransport = new GetGasTransportSystemListQuery(Context).Execute().First();
            Guid pipelineId =
                new AddPipelineCommand(Context).Execute(new AddPipelineParameterSet
                {
                    Name = "TstPipeline" + Guid.NewGuid(),
                    PipelineTypeId = pipelineType,
                    GasTransportSystemId = gastransport.Id,
                    KilometerOfStart = 0,
                    KilometerOfEnd = 100
                });
            return pipelineId;
        }

        protected Guid GetCompShop(ExecutionContextTest context, Guid? siteId = null)
        {
            if (siteId == null)
                siteId = CreateSite();

            var region = new GetRegionListQuery(context).Execute().First();
            var compStationId =
                new AddCompStationCommand(context).Execute(new AddCompStationParameterSet
                {
                    ParentId = siteId,
                    Name = "TestCompStation" + Guid.NewGuid(),
                    RegionId = region.Id
                });
            var gastransport = new GetGasTransportSystemListQuery(Context).Execute().First();
            var pipelineId =
                new AddPipelineCommand(context).Execute(new AddPipelineParameterSet
                {
                    Name = "Pipeline1" + Guid.NewGuid(),
                    PipelineTypeId = PipelineType.Branch,
                    GasTransportSystemId = gastransport.Id
                });

            var compShopId =
                new AddCompShopCommand(context).Execute(new AddCompShopParameterSet
                {
                    ParentId = compStationId,
                    Name = "TestCompShop" + Guid.NewGuid(),
                    KmOfConn = 2.5,
                    PipelineId = pipelineId,
                    EngineClassId = EngineClass.Motorisierte
                });
            return compShopId;
        }
    }
}
