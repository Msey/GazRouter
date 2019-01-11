using System;
using System.Linq;
using GazRouter.DAL.Balances.Contracts;
using GazRouter.DAL.Balances.GasOwners;
using GazRouter.DAL.Dictionaries.GasTransportSystem;
using GazRouter.DAL.ObjectModel.MeasStations;
using GazRouter.DTO.Balances.Contracts;
using GazRouter.DTO.Balances.GasOwners;
using GazRouter.DTO.Dictionaries.BalanceSigns;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.ObjectModel.MeasStations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DALTest.Balances
{
	[TestClass]
    public class AuxConsumptionsTest : DalTestBase
	{
		[TestMethod ,TestCategory(Stable)]
		public void FullAuxConsumptionsTest()
		{
            const string measStationName = "newStation";
            Guid newGuidEnt = GetEnterpriseId();
            Guid newGuidSite = CreateSite(newGuidEnt);
			var measStationId = new AddMeasStationCommand(Context).Execute(new AddMeasStationParameterSet
			{
				ParentId = newGuidSite,
				Name = measStationName,
				BalanceSignId = Sign.In
			});
			Assert.AreNotEqual(measStationId, Guid.Empty);
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
