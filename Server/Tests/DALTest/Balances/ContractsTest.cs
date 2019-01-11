using System;
using System.Linq;
using GazRouter.DAL.Balances.Contracts;
using GazRouter.DAL.Balances.Docs;
using GazRouter.DAL.Dictionaries.GasTransportSystem;
using GazRouter.DTO.Attachments;
using GazRouter.DTO.Balances.Contracts;
using GazRouter.DTO.Balances.Docs;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.Targets;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;

namespace DALTest.Balances
{
	[TestClass]
    public class ContractsTest : TransactionTestsBase
	{
		[TestMethod ,TestCategory(Stable)]
		public void FullContractsTest()
		{
		    var date = new DateTime(2013, 1, 1);
		    var systemId = new GetGasTransportSystemListQuery(Context).Execute().First().Id;
		    var contractId =
		        new AddContractCommand(Context).Execute(new AddContractParameterSet
		            {
		                ContractDate = date,
		                PeriodTypeId = PeriodType.Year,
		                TargetId = Target.Project,
                        GasTransportSystemId = systemId
		            });
		    var contract =
                new GetContractListQuery(Context).Execute(new GetContractListParameterSet
                {
                    ContractDate = date,
                    PeriodTypeId = PeriodType.Year,
                    TargetId = Target.Project,
                    SystemId = systemId
                }).FirstOrDefault();

            Assert.IsNotNull(contract);

            var data = new byte[1000];
            new Random(Guid.NewGuid().GetHashCode()).NextBytes(data);

		    var docId = new AddDocCommand(Context).Execute(
                new AddDocParameterSet
                {
                    ExternalId = contractId,
                    Data = data,
                    Description = "TestDescription",
                    FileName = "TestFileName.txt"
                });

            var docsList = new GetDocListQuery(Context).Execute(contractId);
            Assert.IsTrue(docsList.Count > 0);
            new DeleteDocCommand(Context).Execute(docId);

            new CleanContractCommand(Context).Execute(contractId);
            new DeleteContractCommand(Context).Execute(contractId);
            
            contract =
                new GetContractListQuery(Context).Execute(new GetContractListParameterSet
                {
                    ContractDate = date,
                    PeriodTypeId = PeriodType.Year,
                    TargetId = Target.Project,
                    SystemId = systemId
                }).FirstOrDefault();

            Assert.IsNull(contract);
        }
	}
}
