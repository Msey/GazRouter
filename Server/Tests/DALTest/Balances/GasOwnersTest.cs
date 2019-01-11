using System;
using System.Linq;
using GazRouter.DAL.Balances.GasOwners;
using GazRouter.DTO.Balances.GasOwners;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;

namespace DALTest.Balances
{
	[TestClass]
    public class GasOwnersTest : TransactionTestsBase
	{
		[TestMethod ,TestCategory(Stable)]
		public void FullGasOwnersTest()
		{
		    var rnd = new Random();
		    var sourceId =
		        new AddGasOwnerCommand(Context).Execute(new AddGasOwnerParameterSet
		            {
		                Name = "testName" + rnd.Next(0, 1000),
		                Description = "Desc"
		            });
            var list = new GetGasOwnerListQuery(Context).Execute(null);
		    Assert.AreNotEqual(list.Count(p => p.Id == sourceId), 0);
            new EditGasOwnerCommand(Context).Execute(new EditGasOwnerParameterSet
		        {
		            Description = "Desc2",
		            Id = sourceId,
		            Name = "Name1"
		        });
            var source = new GetGasOwnerListQuery(Context).Execute(null).SingleOrDefault(p => p.Id == sourceId);
		    Assert.IsNotNull(source);
		    Assert.AreEqual(source.Id, sourceId);
		    Assert.AreEqual(source.Description, "Desc2");
		    Assert.AreEqual(source.Name, "Name1");
            new DeleteGasOwnerCommand(Context).Execute(sourceId);
		}



		[TestMethod, TestCategory(Stable)]
		public void OrderGasOwnersTest()
		{
			var rnd = new Random();
			var sourceId =
				new AddGasOwnerCommand(Context).Execute(new AddGasOwnerParameterSet
				{
					Name = "testName" + Guid.NewGuid(),
					Description = "Desc"
				});

			var sourceId2 =
				new AddGasOwnerCommand(Context).Execute(new AddGasOwnerParameterSet
				{
					Name = "testName" + Guid.NewGuid(),
					Description = "Desc"
				});
            var list = new GetGasOwnerListQuery(Context).Execute(null);
			Assert.AreNotEqual(list.Count(p => p.Id == sourceId), 0);
			Assert.AreNotEqual(list.Count(p => p.Id == sourceId2), 0);
			Assert.IsTrue(list.First(p => p.Id == sourceId).SortOrder < list.First(p => p.Id == sourceId2).SortOrder);
			new SetGasOwnerSortOrderCommand(Context).Execute(new SetGasOwnerSortOrderParameterSet { GasOwnerId = sourceId2, UpDown = UpOrDownSortOrder.Up});
            list = new GetGasOwnerListQuery(Context).Execute(null);
			Assert.AreNotEqual(list.Count(p => p.Id == sourceId), 0);
			Assert.AreNotEqual(list.Count(p => p.Id == sourceId2), 0);
			Assert.IsTrue(list.First(p => p.Id == sourceId).SortOrder > list.First(p => p.Id == sourceId2).SortOrder); 
			new DeleteGasOwnerCommand(Context).Execute(sourceId);
			new DeleteGasOwnerCommand(Context).Execute(sourceId2);
		}
	}
}
