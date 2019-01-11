using System;
using System.Linq;
using GazRouter.DAL.Balances.Routes;
using GazRouter.DAL.Dictionaries.GasTransportSystem;
using GazRouter.DAL.Dictionaries.Regions;
using GazRouter.DAL.ObjectModel.CompShops;
using GazRouter.DAL.ObjectModel.CompStations;
using GazRouter.DAL.ObjectModel.DistrStations;
using GazRouter.DAL.ObjectModel.MeasStations;
using GazRouter.DAL.ObjectModel.Sites;
using GazRouter.DTO.Balances.Routes;
using GazRouter.DTO.Dictionaries.BalanceSigns;
using GazRouter.DTO.Dictionaries.EngineClasses;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PipelineTypes;
using GazRouter.DTO.ObjectModel.CompShops;
using GazRouter.DTO.ObjectModel.CompStations;
using GazRouter.DTO.ObjectModel.DistrStations;
using GazRouter.DTO.ObjectModel.MeasStations;
using GazRouter.DTO.ObjectModel.Sites;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DALTest.Balances
{
	[TestClass]
    public class RoutesTest : DalTestBase
	{
		[TestMethod, TestCategory(Stable)]
        public void FullRoutesTest()
		{
            const string measStationName = "newStation";
            var newGuidEnt = GetEnterpriseId();
         //   var newGuidSite = CreateSite(newGuidEnt);

            var gasTransport = new GetGasTransportSystemListQuery(Context).Execute().First();
		    var newGuidSite = new AddSiteCommand(Context).Execute(new AddSiteParameterSet
		                                                {
		                                                    Name = "TestSite" + Guid.NewGuid(),
		                                                    ParentId = newGuidEnt,
		                                                    GasTransportSystemId = gasTransport.Id
		                                                });
            //var transportSystemId = (new GetSiteListQuery(Context).Execute(new GetSiteListParameterSet {SiteId = newGuidSite}));
            //var c = transportSystemId.Count();

            var measStationId =
                new AddMeasStationCommand(Context).Execute(new AddMeasStationParameterSet
                {
                    ParentId = newGuidSite,
                    Name = measStationName,
                    BalanceSignId = Sign.In
                });

            newGuidEnt = GetEnterpriseId();
            newGuidSite = CreateSite(newGuidEnt);
		    var distrStationId =
		        new AddDistrStationCommand(Context).Execute(new AddDistrStationParameterSet
		                                                        {
		                                                            Name = "Тест ГРС",
		                                                            ParentId = newGuidSite
		                                                        });

		    var routeId = new SetRouteCommand(Context).Execute(new SetRouteParameterSet
		                                                           {
                                                                       InletId = measStationId,
                                                                       OutletId = distrStationId,
                                                                       Length = 10
		                                                           });
            Assert.IsNotNull(routeId);
            
            var routeslist = new GetRouteListQuery(Context).Execute(new GetRouteListParameterSet {InletId = measStationId});

            Assert.IsTrue(routeslist.Any());

		    var pipelineId = CreatePipeline(PipelineType.Branch);

             var siteId = new AddSiteCommand(Context).Execute(new AddSiteParameterSet
            {
                Name = "TestSite1" + Guid.NewGuid(),
                ParentId = newGuidEnt,
                GasTransportSystemId = gasTransport.Id
            });

         //   var siteId = CreateSite();
            var region = new GetRegionListQuery(Context).Execute().First();
            var compStationId =
                new AddCompStationCommand(Context).Execute(new AddCompStationParameterSet
                {
                    ParentId = siteId,
                    Name = "TestCompStation",
                    RegionId = region.Id
                });
           

            routeId = new SetRouteCommand(Context).Execute(new SetRouteParameterSet
            {
                InletId = measStationId,
                OutletId = compStationId,
                Length = 10
            });
            Assert.IsNotNull(routeId);

            routeslist = new GetRouteListQuery(Context).Execute(new GetRouteListParameterSet());

            Assert.IsTrue(routeslist.Any());
            

            new DeleteRouteCommand(Context).Execute(routeId);
		}
	}
}
