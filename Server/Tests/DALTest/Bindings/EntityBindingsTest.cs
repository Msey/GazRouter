using System.Linq;
using GazRouter.DAL.Bindings.EntityBindings;
using GazRouter.DAL.Dictionaries.Sources;
using GazRouter.DTO.Bindings.EntityBindings;
using GazRouter.DTO.Dictionaries.EntityTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DALTest.Bindings
{
	[TestClass]
    public class EntityBindingsTest : DalTestBase
	{
		[TestMethod ,TestCategory(Stable)]
		public void FullEntityBindingsTest()
		{
		    var list = new GetSourcesListQuery(Context).Execute();
		    var sourceId = list.First().Id;
		    var bingingsBefore =
                new GetEntityBindingsPagedListQuery(Context).Execute(new GetEntityBindingsPageParameterSet
		            {
		                EntityType = EntityType.Site,
		                SourceId = sourceId,
		                ShowAll = true,
		                PageSize = 30,
		                PageNumber = 0,
		                NamePart = null
		            });

		    var siteId = CreateSite();

		    var bindingsAfter =
                new GetEntityBindingsPagedListQuery(Context).Execute(new GetEntityBindingsPageParameterSet
		            {
		                EntityType = EntityType.Site,
		                SourceId = sourceId,
		                ShowAll = true,
		                PageSize = 30,
		                PageNumber = 0,
		                NamePart = null
		            });
		    Assert.AreEqual(bingingsBefore.TotalCount + 1, bindingsAfter.TotalCount);

		    bingingsBefore =
                new GetEntityBindingsPagedListQuery(Context).Execute(new GetEntityBindingsPageParameterSet
		            {
		                EntityType = EntityType.Site,
		                SourceId = sourceId,
		                ShowAll = false,
		                PageSize = 30,
		                PageNumber = 0,
		                NamePart = null
		            });


		    var entytiBinding =
                new AddEntityBindingCommand(Context).Execute(new EntityBindingParameterSet
		            {
		                SourceId = sourceId,
		                ExtEntityId = "TestSource2",
		                EntityId = siteId
		            });

            new EditEntityBindingCommand(Context).Execute(new EditEntityBindingParameterSet
		        {
		            Id = entytiBinding,
		            SourceId = sourceId,
		            ExtEntityId = "TestSource3",
		            EntityId = siteId
		        });

            

		    bindingsAfter =
                new GetEntityBindingsPagedListQuery(Context).Execute(new GetEntityBindingsPageParameterSet
		            {
		                EntityType = EntityType.Site,
		                SourceId = sourceId,
		                ShowAll = false,
		                PageSize = 30,
		                PageNumber = 0,
		                NamePart = null
		            });

		    Assert.AreEqual(bindingsAfter.TotalCount, bingingsBefore.TotalCount + 1);
            new DeleteEntityBindingCommand(Context).Execute(entytiBinding);
		    //	Assert.IsNotNull(bindingsAfter.Entities.FirstOrDefault(p => p.Id == entytiBinding));
		    //Assert.IsTrue(totalcount < bindingList.TotalCount);
		}
	}
}
