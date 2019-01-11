using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;

namespace DALTest.Balances
{
	[TestClass]
    public class IntakeTest : TransactionTestsBase
	{
		[TestMethod ,TestCategory(Stable)]
		public void FullIntakeTest()
		{
            //const string measStationName = "newStation";

            //Guid newGuidEnt = GetEnterpriseId();
            //Guid newGuidSite = CreateSite(newGuidEnt);

            //Guid measStationId =
            //    new AddMeasStationCommand(Context).Execute(new AddMeasStationParameterSet
            //        {
            //            ParentId = newGuidSite,
            //            Name = measStationName,
            //            SortOrder = 1,
            //            BalanceSignId = Sign.In
            //        });
            //var date = new DateTime(2013, 1, 1);
            //var contractId =
            //    new AddContractCommand(Context).Execute(new AddContractParameterSet
            //        {
            //            ContractDate = date,
            //            PeriodTypeId = PeriodType.Year,
            //            TargetId = Target.Project
            //        });
            //Assert.AreNotEqual(contractId, default(int));
            //var ownersId =
            //    new AddGasOwnerCommand(Context).Execute(new AddGasOwnerParameterSet { Name = "testOwnerName", Description = "Desc" });

            //var intake =
            //    new AddIntakeTransitCommand(Context).Execute(new AddIntakeParameterSet
            //        {
            //            ContractId = contractId,
            //            GasOwnersId = ownersId,
            //            MeasStationId = measStationId,
            //            Value = 1.0,
            //            EndDate = DateTime.Now.AddDays(1),
            //            IsCorrection = true,
            //            StartDate = DateTime.Now,
            //            PerspectiveId = PerspectiveType.Commercial
            //        });


            //var list = new GetIntakeListCommand(Context).Execute(contractId);
            //Assert.AreNotEqual(list.Count(), 0);

            //list = new GetIntakeListCommand(Context).Execute(contractId);
            //var source = list.SingleOrDefault(p => p.Id == intake);
            //Assert.IsNotNull(source);
            //Assert.AreEqual(source.Id, intake);
            //Assert.AreEqual(source.GasOwnerId, ownersId);
		}

		[TestMethod ,TestCategory(Stable)]
		public void VersionIntakeTest()
		{
            //const string measStationName = "newStation";

            //Guid newGuidEnt =
            //    GetEnterpriseId();
            //Guid newGuidSite = CreateSite(newGuidEnt);

            //Guid measStationId =
            //    new AddMeasStationCommand(Context).Execute(new AddMeasStationParameterSet
            //        {
            //            ParentId = newGuidSite,
            //            Name = measStationName,
            //            SortOrder = 1,
            //            BalanceSignId = Sign.In
            //        });
            //var date = new DateTime(2013, 1, 1);
            //var ContractId =
            //    new AddContractCommand(Context).Execute(new AddContractParameterSet
            //        {
            //            ContractDate = date,
            //            PeriodTypeId = PeriodType.Year,
            //            TargetId = Target.Project
            //        });
            //Assert.AreNotEqual(ContractId, default(int));
            //var rnd = new Random();
            //var ownersId =
            //    new AddGasOwnerCommand(Context).Execute(new AddGasOwnerParameterSet
            //        {
            //            Name = "testName" + rnd.Next(0, 1000),
            //            Description = "Desc"
            //        });

            //var intake =
            //    new AddIntakeTransitCommand(Context).Execute(new AddIntakeParameterSet
            //        {
            //            ContractId = ContractId,
            //            GasOwnersId = ownersId,
            //            MeasStationId = measStationId,
            //            Value = 1.0,EndDate = DateTime.Now.AddDays(1),
            //            IsCorrection = true,
            //            StartDate = DateTime.Now,
            //            PerspectiveId = PerspectiveType.Commercial
            //        });

            //var list = new GetIntakeListCommand(Context).Execute(ContractId);
            //Assert.AreNotEqual(list.Count(), 0);
            //var source = new GetIntakeListCommand(Context).Execute(ContractId).SingleOrDefault(p => p.Id == intake);
            //Assert.IsNotNull(source);

            //new DeleteIntakeByContractIdCommand(Context).Execute(ContractId);
            //source = new GetIntakeListCommand(Context).Execute(ContractId).SingleOrDefault(p => p.Id == intake);
            //Assert.IsNull(source);

            //new DeleteMeasStationCommand(Context).Execute(new DeleteEntityParameterSet
            //    {
            //        EntityType = EntityType.MeasStation,
            //        Id = measStationId
            //    });
            //new DeleteSiteCommand(Context).Execute(new DeleteEntityParameterSet
            //    {
            //        EntityType = EntityType.Site,
            //        Id = newGuidSite
            //    });
            //new DeleteContractCommand(Context).Execute(ContractId);
            //new DeleteGasOwnerCommand(Context).Execute(ownersId);
		}
	}
}
