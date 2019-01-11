using System;
using System.Linq;
using GazRouter.DAL.Balances.GasOwners;
using GazRouter.DAL.Dictionaries.ConsumerTypes;
using GazRouter.DAL.Dictionaries.GasTransportSystem;
using GazRouter.DAL.Dictionaries.Regions;
using GazRouter.DAL.ObjectModel.Consumers;
using GazRouter.DAL.ObjectModel.DistrStations;
using GazRouter.DTO.Balances.GasOwners;
using GazRouter.DTO.ObjectModel.Consumers;
using GazRouter.DTO.ObjectModel.DistrStations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DALTest.Balances
{
	[TestClass]
	public class SystemGasOwnersTest : DalTestBase
	{
		[TestMethod ,TestCategory(Stable)]
		public void FullSystemGasOwnersTest()
		{
		    var rnd = new Random();
		    var sourceId =
		        new AddGasOwnerCommand(Context).Execute(new AddGasOwnerParameterSet
		            {
		                Name = "testName" + rnd.Next(0, 1000),
		                Description = "Desc"
		            });
            var owner = new GetGasOwnerListQuery(Context).Execute(null).FirstOrDefault(p => p.Id == sourceId);
            Assert.IsTrue(owner != null);
            Assert.IsTrue(owner.SystemList.Count == 0);
		    Assert.IsTrue(new GetGasTransportSystemListQuery(Context).Execute().Count >0);
			var system1 = new GetGasTransportSystemListQuery(Context).Execute().FirstOrDefault();
		    if (system1 == null) return;
		    new SetGasOwnerSystemCommand(Context).Execute(
                new SetGasOwnerSystemParameterSet
		        {
		            GasOwnerId = owner.Id,
		            SystemId = system1.Id,
                    IsActive = true
		        });

            owner = new GetGasOwnerListQuery(Context).Execute(system1.Id).FirstOrDefault(p => p.Id == sourceId);
            Assert.IsTrue(owner != null);
            Assert.IsTrue(owner.SystemList.Count == 1);
            Assert.IsTrue(owner.SystemList[0] == system1.Id);
            new SetGasOwnerSystemCommand(Context).Execute(
                new SetGasOwnerSystemParameterSet
                {
                    GasOwnerId = owner.Id,
                    SystemId = system1.Id,
                    IsActive = false
                });
            new DeleteGasOwnerCommand(Context).Execute(sourceId);
		}

		[TestMethod, TestCategory(Stable)]
		public void FullGasOwners2ConsumerTest()
		{
			var rnd = new Random();
			var regionId = new GetRegionListQuery(Context).Execute().First().Id;
			const string measStationName = "newStation";
			int consumerId = new GetConsumerTypesListQuery(Context).Execute().First().Id;
			Guid newGuidEnt = GetEnterpriseId();
			Guid newGuidSite = CreateSite(newGuidEnt);
			Guid newDistrStation =
				new AddDistrStationCommand(Context).Execute(new AddDistrStationParameterSet
				{
					CapacityRated = 1,
					Name = "test1",
					ParentId = newGuidSite,
					PressureRated = 1
				});
			Guid measStationId =
				new AddConsumerCommand(Context).Execute(new AddConsumerParameterSet
				{
					ParentId = newDistrStation,
					Name = measStationName,
					ConsumerType = consumerId,
					RegionId = regionId
				});
			var sourceId =
				new AddGasOwnerCommand(Context).Execute(new AddGasOwnerParameterSet
				{
					Name = "testName" + rnd.Next(0, 1000),
					Description = "Desc"
				});
			var owner = new GetGasOwnerListQuery(Context).Execute(null).FirstOrDefault(p => p.Id == sourceId);
			Assert.IsTrue(owner != null);
			//Assert.IsTrue(owner.ConsumerList.Count == 0);
			//new AddGasOwners2ConsumerCommand(Context).Execute(new BaseGasOwner2ConsumerParameterSet
			//	{
			//		GasOwnerId = owner.Id,
			//		ConsumerId = measStationId
			//	});
			owner = new GetGasOwnerListQuery(Context).Execute(null).FirstOrDefault(p => p.Id == sourceId);
			Assert.IsTrue(owner != null);
			//Assert.IsTrue(owner.ConsumerList.Count == 1);
			//Assert.IsTrue(owner.ConsumerList[0] == measStationId);
			//new DeleteGasOwners2ConsumerCommand(Context).Execute(new BaseGasOwner2ConsumerParameterSet { GasOwnerId = owner.Id, ConsumerId = measStationId });
			new DeleteGasOwnerCommand(Context).Execute(sourceId);
		}
	}
}
