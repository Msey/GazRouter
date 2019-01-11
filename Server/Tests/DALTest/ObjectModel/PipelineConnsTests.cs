using System.Linq;
using GazRouter.DAL.Dictionaries.GasTransportSystem;
using GazRouter.DAL.Dictionaries.Regions;
using GazRouter.DAL.ObjectModel.CompShops;
using GazRouter.DAL.ObjectModel.CompStations;
using GazRouter.DAL.ObjectModel.PipelineConns;
using GazRouter.DAL.ObjectModel.Pipelines;
using GazRouter.DTO.Dictionaries.EngineClasses;
using GazRouter.DTO.Dictionaries.PipelineEndType;
using GazRouter.DTO.Dictionaries.PipelineTypes;
using GazRouter.DTO.ObjectModel.CompShops;
using GazRouter.DTO.ObjectModel.CompStations;
using GazRouter.DTO.ObjectModel.PipelineConns;
using GazRouter.DTO.ObjectModel.Pipelines;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oracle.ManagedDataAccess.Client;

namespace DALTest.ObjectModel
{
	[TestClass]
    public class PipelineConnsTests : DalTestBase
	{
		[TestMethod ,TestCategory(Stable)]
		public void FullTest()
		{
			var gastransport = new GetGasTransportSystemListQuery(Context).Execute().First();
		    var pipelineId =
		        new AddPipelineCommand(Context).Execute(new AddPipelineParameterSet
		            {
		                Name = "Pipeline1",
		                PipelineTypeId = PipelineType.Branch,
		                Hidden = true,
		                SortOrder = 1,GasTransportSystemId = gastransport.Id
		            });

		    var destPipelineId =
                new AddPipelineCommand(Context).Execute(new AddPipelineParameterSet
		            {
		                Name = "Pipeline2",
		                PipelineTypeId = PipelineType.Inlet,
		                Hidden = true,
						SortOrder = 1,
						GasTransportSystemId = gastransport.Id
		            });

		    var siteId = CreateSite();
            var region = new GetRegionListQuery(Context).Execute().First();
		    var compStationId =
                new AddCompStationCommand(Context).Execute(new AddCompStationParameterSet
		            {
		                ParentId = siteId,
		                Name = "TestCompStation",
		                RegionId = region.Id
		            });
            var compShopId = new AddCompShopCommand(Context).Execute(new AddCompShopParameterSet
		        {
		            ParentId = compStationId,
		            Name = "TestCompShop",
		            KmOfConn = 2.5,
		            PipelineId = pipelineId,
		            EngineClassId = EngineClass.Motorisierte
		        });

		    var pipelineConnsId =
                new AddPipelineConnCommand(Context).Execute(new AddPipelineConnParameterSet
		            {
		                EndTypeId = PipelineEndType.StartType,
		                DestEntityId = destPipelineId,
		                PipelineId = pipelineId,
		                Kilometr = 2
		            });

		    Assert.AreNotEqual(default(int), pipelineConnsId);

		    var editedPipelineConns =
                new GetPipelineConnListQuery(Context).Execute(new GetPipelineConnListParameterSet {PipelineId =  pipelineId}).First(p => p.Id == pipelineConnsId);
		    Assert.AreEqual(2, editedPipelineConns.Kilometr);

            new DeletePipelineConnCommand(Context).Execute(pipelineConnsId);

		    var delitedPipelineConns =
                new GetPipelineConnListQuery(Context).Execute(new GetPipelineConnListParameterSet { PipelineId = pipelineId }).FirstOrDefault(p => p.Id == pipelineConnsId);
		    Assert.IsNull(delitedPipelineConns);
		}

	    [TestMethod ,TestCategory(Stable)]
		[ExpectedException(typeof(OracleException))]
		public void TestMoreAtTwoConnections()
	    {
			var gastransport = new GetGasTransportSystemListQuery(Context).Execute().First();
	        var pipelineId =
                new AddPipelineCommand(Context).Execute(new AddPipelineParameterSet
	                {
	                    Name = "Pipeline1",
	                    PipelineTypeId = PipelineType.Branch,
	                    Hidden = true,
	                    SortOrder = 1,GasTransportSystemId = gastransport.Id
	                });

	        var destPipelineId =
                new AddPipelineCommand(Context).Execute(new AddPipelineParameterSet
	                {
	                    Name = "Pipeline1",
	                    PipelineTypeId = PipelineType.Distribution,
	                    Hidden = true,
	                    SortOrder = 1,GasTransportSystemId = gastransport.Id
	                });


	        var pipelineConnsId =
                new AddPipelineConnCommand(Context).Execute(new AddPipelineConnParameterSet
	                {
	                    EndTypeId = PipelineEndType.StartType,
	                    DestEntityId = destPipelineId,
	                    PipelineId = pipelineId,
	                    Kilometr = 2
	                });

	        Assert.AreNotEqual(default(int), pipelineConnsId);
	        var pipelineConnsId2 =
                new AddPipelineConnCommand(Context).Execute(new AddPipelineConnParameterSet
	                {
	                    EndTypeId = PipelineEndType.StartType,
                        DestEntityId = destPipelineId,
	                    PipelineId = pipelineId,
	                    Kilometr = 2
	                });
	        var pipelineConnsId3 =
                new AddPipelineConnCommand(Context).Execute(new AddPipelineConnParameterSet
	                {
	                    EndTypeId = PipelineEndType.StartType,
                        DestEntityId = destPipelineId,
	                    PipelineId = pipelineId,
	                    Kilometr = 2
	                });
	    }
	}
}