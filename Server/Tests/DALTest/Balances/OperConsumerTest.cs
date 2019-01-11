using System.Linq;
using GazRouter.DAL.Dictionaries.ConsumerTypes;
using GazRouter.DAL.Dictionaries.Regions;
using GazRouter.DAL.ObjectModel.OperConsumers;
using GazRouter.DAL.ObjectModel.Sites;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.OperConsumers;
using GazRouter.DTO.ObjectModel.Sites;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DALTest.Balances
{
    [TestClass]
    public class OperConsumerTest : DalTestBase
    {
        [TestMethod, TestCategory(Stable)]
        public void FullConsumersTest()
        {
            int consumerType = new GetConsumerTypesListQuery(Context).Execute().First().Id;
            var siteId = CreateSite();
			var region = new GetRegionListQuery(Context).Execute().First();
            var consumId =
                new AddOperConsumerCommand(Context).Execute(new AddEditOperConsumerParameterSet
                                                             {
                                                                 ConsumerType = consumerType,
                                                                 ConsumerName = "qwerty",
                                                                 IsDirectConnection = false,
																 SiteId = siteId,
																 RegionId = region.Id
                                                             });

            new EditOperConsumerCommand(Context).Execute(new AddEditOperConsumerParameterSet
                                                             {
                                                                 Id = consumId,
                                                                 ConsumerType = consumerType,
                                                                 ConsumerName = "qwertyNew",
                                                                 IsDirectConnection = true,
                                                                 SiteId = siteId,
																 RegionId = region.Id
                                                             });
            var site = new GetSiteListQuery(Context).Execute(new GetSiteListParameterSet())
                                                      .Single(s => s.Id == siteId);
            var list = new GetOperConsumerListQuery(Context).Execute(null);
            Assert.IsTrue(list.Any());

            new DeleteOperConsumerCommand(Context).Execute(new DeleteEntityParameterSet {Id = consumId});
        }

    }
}