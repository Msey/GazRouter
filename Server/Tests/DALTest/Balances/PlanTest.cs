using System;
using System.Linq;
using GazRouter.DAL.Balances.Contracts;
using GazRouter.DAL.Balances.GasOwners;
using GazRouter.DAL.Balances.Plan;
using GazRouter.DAL.Dictionaries.ConsumerTypes;
using GazRouter.DAL.Dictionaries.GasTransportSystem;
using GazRouter.DAL.Dictionaries.Regions;
using GazRouter.DAL.ObjectModel.DistrStations;
using GazRouter.DAL.ObjectModel.MeasStations;
using GazRouter.DTO.Balances.Contracts;
using GazRouter.DTO.Balances.GasOwners;
using GazRouter.DTO.Balances.Values;
using GazRouter.DTO.Dictionaries.BalanceSigns;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.ObjectModel.DistrStations;
using GazRouter.DTO.ObjectModel.MeasStations;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace DALTest.Balances
{
	[TestClass]
    public class PlanTest : DalTestBase
	{
		//[TestMethod ,TestCategory(Stable)]
		//public void GetIntakeTransitItemsTest()
		//{
  //          var regionId = new GetRegionListQuery(Context).Execute().First().Id;
  //          const string measStationName = "newStation";
  //          int consumerId = new GetConsumerTypesListQuery(Context).Execute().First().Id;
  //          Guid newGuidEnt = GetEnterpriseId();
  //          Guid newGuidSite = CreateSite(newGuidEnt);
  //          Guid newDistrStation =
  //              new AddDistrStationCommand(Context).Execute(new AddDistrStationParameterSet
  //              {
  //                  CapacityRated = 1,
  //                  Name = "test1",
  //                  ParentId = newGuidSite,
  //                  PressureRated = 1
  //              });
  //          Guid measStationId =
  //              new AddConsumerCommand(Context).Execute(new AddConsumerParameterSet
  //              {
  //                  ParentId = newDistrStation,
  //                  Name = measStationName,
  //                  ConsumerType = consumerId,
  //                  RegionId = regionId
  //              });
  //          var date = new DateTime(2013, 1, 1);
  //          var systemId = new GetGasTransportSystemListQuery(Context).Execute().First().Id;
  //          var contractId =
  //              new AddContractCommand(Context).Execute(new AddContractParameterSet
  //              {
  //                  ContractDate = date,
  //                  PeriodTypeId = PeriodType.Year,
  //                  TargetId = Target.Project,
  //                  SystemId = systemId
  //              });
  //          Assert.AreNotEqual(contractId, default(int));
  //          var ownersId =
  //              new AddGasOwnerCommand(Context).Execute(new AddGasOwnerParameterSet { Name = "testOwnerName1", Description = "Desc" });

  //          new SetConsumptionCommand(Context).Execute(
  //              new SetConsumptionParameterSet
  //              {
  //                  ContractId = contractId,
  //                  GasOwnerId = ownersId,
  //                  GasConsumerId = measStationId,
  //                  Value = 1,
  //                  EndDate = DateTime.Now.AddDays(1),
  //                  ValueType = BalanceValueType.Base,
  //                  StartDate = DateTime.Now
  //              });

  //          new SetConsumptionCommand(Context).Execute(
  //              new SetConsumptionParameterSet
  //              {
  //                  ContractId = contractId,
  //                  GasOwnerId = ownersId,
  //                  GasConsumerId = measStationId,
  //                  Value = 1,
  //                  EndDate = DateTime.Now.AddDays(1),
  //                  ValueType = BalanceValueType.Operative,
  //                  StartDate = DateTime.Now
  //              });

  //          new SetConsumptionCommand(Context).Execute(
  //              new SetConsumptionParameterSet
  //              {
  //                  ContractId = contractId,
  //                  GasOwnerId = ownersId,
  //                  GasConsumerId = measStationId,
  //                  Value = 1,
  //                  ValueType = BalanceValueType.Commercial,
  //                  StartDate = DateTime.Now
  //              });

  //          new SetConsumptionCommand(Context).Execute(
  //              new SetConsumptionParameterSet
  //              {
  //                  ContractId = contractId,
  //                  GasOwnerId = ownersId,
  //                  GasConsumerId = measStationId,
  //                  Value = 1,
  //                  EndDate = DateTime.Now.AddDays(1),
  //                  ValueType = BalanceValueType.Irregularity,
  //                  StartDate = DateTime.Now
  //              });

  //          var consumerTypeId = new GetConsumerTypesListQuery(Context).Execute().First().Id;
  //          newGuidEnt = GetEnterpriseId();
  //          newGuidSite = CreateSite(newGuidEnt);
  //          consumerId =
  //              new AddOperConsumerCommand(Context).Execute(new AddEditOperConsumerParameterSet
  //              {
  //                  ConsumerType = consumerTypeId,
  //                  ConsumerName = "qwerty",
  //                  IsDirectConnection = false,
  //                  SiteId = newGuidSite,RegionId = regionId
  //              });
  //          ownersId = new AddGasOwnerCommand(Context).Execute(new AddGasOwnerParameterSet { Name = "testOwnerName", Description = "Desc" });

  //          new AddOperConsumptionCommand(Context).Execute(new AddBalanceRowParameterSet<int>
  //              {
  //                  ContractId = contractId,
  //                  PointId = consumerId,
  //                  GasOwnerId = ownersId,
  //                  Value = 1,
  //                  EndDate = DateTime.Now.AddDays(1),
  //                  StartDate = date,
  //                  ValueType = BalanceValueType.Commercial
  //              });

  //          new AddOperConsumptionCommand(Context).Execute(new AddBalanceRowParameterSet<int>
  //          {
  //              ContractId = contractId,
  //              PointId = consumerId,
  //              GasOwnerId = ownersId,
  //              Value = 1,
  //              EndDate = DateTime.Now.AddDays(1),
  //              StartDate = date,
  //              ValueType = BalanceValueType.Base
  //          });

  //          new AddOperConsumptionCommand(Context).Execute(new AddBalanceRowParameterSet<int>
  //          {
  //              ContractId = contractId,
  //              PointId = consumerId,
  //              GasOwnerId = ownersId,
  //              Value = 1,
  //              EndDate = DateTime.Now.AddDays(1),
  //              StartDate = date,
  //              ValueType = BalanceValueType.Irregularity
  //          });

  //          new AddOperConsumptionCommand(Context).Execute(new AddBalanceRowParameterSet<int>
  //          {
  //              ContractId = contractId,
  //              PointId = consumerId,
  //              GasOwnerId = ownersId,
  //              Value = 1,
  //              EndDate = DateTime.Now.AddDays(1),
  //              StartDate = date,
  //              ValueType = BalanceValueType.Operative
  //          });

  //          newGuidEnt = GetEnterpriseId();
  //          newGuidSite = CreateSite(newGuidEnt);

  //          measStationId =
  //              new AddMeasStationCommand(Context).Execute(new AddMeasStationParameterSet
  //              {
  //                  ParentId = newGuidSite,
  //                  Name = measStationName,
  //                  BalanceSignId = Sign.In
  //              });

  //          ownersId =
  //              new AddGasOwnerCommand(Context).Execute(new AddGasOwnerParameterSet { Name = "testOwnerName3", Description = "Desc" });

  //          new AddIntakeTransitCommand(Context).Execute(new AddBalanceRowParameterSet<Guid>
  //              {
  //                  ContractId = contractId,
  //                  GasOwnerId = ownersId,
  //                  PointId = measStationId,
  //                  Value = 1,
  //                  EndDate = DateTime.Now.AddDays(1),
  //                  ValueType = BalanceValueType.Commercial,
  //                  StartDate = DateTime.Now
  //              });

  //          new AddIntakeTransitCommand(Context).Execute(new AddBalanceRowParameterSet<Guid>
  //          {
  //              ContractId = contractId,
  //              GasOwnerId = ownersId,
  //              PointId = measStationId,
  //              Value = 1,
  //              EndDate = DateTime.Now.AddDays(1),
  //              ValueType = BalanceValueType.Operative,
  //              StartDate = DateTime.Now
  //          });

  //          new AddIntakeTransitCommand(Context).Execute(new AddBalanceRowParameterSet<Guid>
  //          {
  //              ContractId = contractId,
  //              GasOwnerId = ownersId,
  //              PointId = measStationId,
  //              Value = 1,
  //              EndDate = DateTime.Now.AddDays(1),
  //              ValueType = BalanceValueType.Base,
  //              StartDate = DateTime.Now
  //          });

  //          new AddIntakeTransitCommand(Context).Execute(new AddBalanceRowParameterSet<Guid>
  //          {
  //              ContractId = contractId,
  //              GasOwnerId = ownersId,
  //              PointId = measStationId,
  //              Value = 1,
  //              EndDate = DateTime.Now.AddDays(1),
  //              ValueType = BalanceValueType.Irregularity,
  //              StartDate = DateTime.Now
  //          });

  //          measStationId =
  //              new AddMeasStationCommand(Context).Execute(new AddMeasStationParameterSet
  //              {
  //                  ParentId = newGuidSite,
  //                  Name = measStationName,
  //                  BalanceSignId = Sign.Out
  //              });
            
  //          ownersId =
  //              new AddGasOwnerCommand(Context).Execute(new AddGasOwnerParameterSet { Name = "testOwnerName5", Description = "Desc" });

  //          new AddIntakeTransitCommand(Context).Execute(new AddBalanceRowParameterSet<Guid>
  //          {
  //              ContractId = contractId,
  //              GasOwnerId = ownersId,
  //              PointId = measStationId,
  //              Value = 1,
  //              EndDate = DateTime.Now.AddDays(1),
  //              ValueType = BalanceValueType.Commercial,
  //              StartDate = DateTime.Now
  //          });

  //          new AddIntakeTransitCommand(Context).Execute(new AddBalanceRowParameterSet<Guid>
  //          {
  //              ContractId = contractId,
  //              GasOwnerId = ownersId,
  //              PointId = measStationId,
  //              Value = 1,
  //              EndDate = DateTime.Now.AddDays(1),
  //              ValueType = BalanceValueType.Operative,
  //              StartDate = DateTime.Now
  //          });

  //          new AddIntakeTransitCommand(Context).Execute(new AddBalanceRowParameterSet<Guid>
  //          {
  //              ContractId = contractId,
  //              GasOwnerId = ownersId,
  //              PointId = measStationId,
  //              Value = 1,
  //              EndDate = DateTime.Now.AddDays(1),
  //              ValueType = BalanceValueType.Base,
  //              StartDate = DateTime.Now
  //          });

  //          new AddIntakeTransitCommand(Context).Execute(new AddBalanceRowParameterSet<Guid>
  //          {
  //              ContractId = contractId,
  //              GasOwnerId = ownersId,
  //              PointId = measStationId,
  //              Value = 1,
  //              EndDate = DateTime.Now.AddDays(1),
  //              ValueType = BalanceValueType.Irregularity,
  //              StartDate = DateTime.Now
  //          });

  //          var intakeTransitItems = new GetIntakeQuery(Context).Execute(contractId);
  //          Assert.IsTrue(intakeTransitItems.Item1.Any());
  //          Assert.IsTrue(intakeTransitItems.Item2.Any());
  //          Assert.IsTrue(intakeTransitItems.Item3.Any());
  //          Assert.IsTrue(intakeTransitItems.Item4.Any());

  //          var intakeTransitCorrections = new GetTransitQuery(Context).Execute(contractId);
  //          Assert.IsTrue(intakeTransitCorrections.Item1.Any());
  //          Assert.IsTrue(intakeTransitCorrections.Item2.Any());
  //          Assert.IsTrue(intakeTransitCorrections.Item3.Any());
  //          Assert.IsTrue(intakeTransitCorrections.Item4.Any());

  //          var consumptionCorrections = new GetConsumptionQuery(Context).Execute(contractId);
  //          Assert.IsTrue(consumptionCorrections.Item1.Any());
  //          Assert.IsTrue(consumptionCorrections.Item2.Any());
  //          Assert.IsTrue(consumptionCorrections.Item3.Any());
  //          Assert.IsTrue(consumptionCorrections.Item4.Any());

  //          var operConsumptionsItems = new GetOperConsumptionQuery(Context).Execute(contractId);
  //          Assert.IsTrue(operConsumptionsItems.Item1.Any());
  //          Assert.IsTrue(operConsumptionsItems.Item2.Any());
  //          Assert.IsTrue(operConsumptionsItems.Item3.Any());
  //          Assert.IsTrue(operConsumptionsItems.Item4.Any());
		//}
	}
}
