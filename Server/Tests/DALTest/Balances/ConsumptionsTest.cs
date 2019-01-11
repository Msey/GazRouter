using System;
using System.Linq;
using GazRouter.DAL.Balances.Contracts;
using GazRouter.DAL.Balances.GasOwners;
using GazRouter.DAL.Dictionaries.ConsumerTypes;
using GazRouter.DAL.Dictionaries.GasTransportSystem;
using GazRouter.DAL.Dictionaries.Regions;
using GazRouter.DAL.ObjectModel.Consumers;
using GazRouter.DAL.ObjectModel.DistrStations;
using GazRouter.DTO.Balances.Contracts;
using GazRouter.DTO.Balances.GasOwners;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.ObjectModel.Consumers;
using GazRouter.DTO.ObjectModel.DistrStations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DALTest.Balances
{
	[TestClass]
    public class ConsumptionsTest : DalTestBase
	{
		[TestMethod ,TestCategory(Stable)]
		public void FullConsumptionsTest()
		{
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
            var systemId = new GetGasTransportSystemListQuery(Context).Execute().First().Id;
            var date = new DateTime(2013, 1, 1);
            var contractId =
                new AddContractCommand(Context).Execute(new AddContractParameterSet
                    {
                        ContractDate = date,
                        PeriodTypeId = PeriodType.Year,
                        TargetId = Target.Project,
                        GasTransportSystemId = systemId
                    });
            Assert.AreNotEqual(contractId, default(int));
            var ownersId =
                new AddGasOwnerCommand(Context).Execute(new AddGasOwnerParameterSet { Name = "testOwnerName", Description = "Desc" });
            
		}
	}
}
