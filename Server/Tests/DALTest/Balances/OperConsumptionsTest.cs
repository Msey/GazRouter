using System;
using System.Linq;
using GazRouter.DAL.Balances.Contracts;
using GazRouter.DAL.Dictionaries.ConsumerTypes;
using GazRouter.DAL.Dictionaries.GasTransportSystem;
using GazRouter.DAL.Dictionaries.Regions;
using GazRouter.DAL.ObjectModel.DistrStations;
using GazRouter.DAL.ObjectModel.OperConsumers;
using GazRouter.DTO.Balances.Contracts;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.ObjectModel.DistrStations;
using GazRouter.DTO.ObjectModel.OperConsumers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DALTest.Balances
{
    [TestClass]
    public class OperConsumptionsTest : DalTestBase
    {
        [TestMethod, TestCategory(Stable)]
        public void FullOperConsumptionsTest()
        {
            var regionId = new GetRegionListQuery(Context).Execute().First().Id;
            int consumerTypeId = new GetConsumerTypesListQuery(Context).Execute().First().Id;
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
            var consumerId =
                new AddOperConsumerCommand(Context).Execute(new AddEditOperConsumerParameterSet
                                                             {
                                                                 ConsumerType = consumerTypeId,
                                                                 ConsumerName = "qwerty",
                                                                 IsDirectConnection = false,
                                                                 SiteId = newGuidSite,RegionId = regionId
                                                             });
            var date = new DateTime(2013, 1, 1);
            var systemId = new GetGasTransportSystemListQuery(Context).Execute().First().Id;
            var contractId =
                new AddContractCommand(Context).Execute(new AddContractParameterSet
                                                            {
                                                                ContractDate = date,
                                                                PeriodTypeId = PeriodType.Day,
                                                                TargetId = Target.Project,
                                                                GasTransportSystemId = systemId
                                                            });
            Assert.AreNotEqual(contractId, default(int));
            //var ownersId =
            //    new AddGasOwnerCommand(Context).Execute(new AddGasOwnerParameterSet { Name = "testOwnerName", Description = "Desc" });

            //var consumId =
            //    new AddOperConsumptionCommand(Context).Execute(
            //        new SetBalanceValueParameterSet
            //        {
            //            ContractId = contractId,
            //            //PointId = consumerId,
            //            GasOwnerId = ownersId,
            //            Value = 1,
            //            EndDate = DateTime.Now.AddDays(1),
            //            StartDate = date,
            //            ValueType = BalanceValueType.Commercial
            //        });



            //var list = new GetOperConsumptionQuery(Context).Execute(contractId);

        }

    }
}