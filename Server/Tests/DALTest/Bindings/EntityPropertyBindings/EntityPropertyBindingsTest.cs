using System.Linq;
using GazRouter.DAL.Bindings.EntityBindings;
using GazRouter.DAL.Bindings.EntityPropertyBindings;
using GazRouter.DAL.Dictionaries.EntityTypes;
using GazRouter.DAL.Dictionaries.Sources;
using GazRouter.DTO.Bindings.EntityBindings;
using GazRouter.DTO.Bindings.EntityPropertyBindings;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.Dictionaries.Targets;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DALTest.Bindings.EntityPropertyBindings
{
	[TestClass]
    public class EntityPropertyBindingsTest : DalTestBase
	{
		[TestMethod ,TestCategory(Stable)]
        public void EntityPropertyBindings()
		{

			{
				var sourceId = new GetSourcesListQuery(Context).Execute().First().Id;
			    
                var enterpriseId = GetCompShop(Context);

                var entityPropertyId = new AddEntityPropertyBindingCommand(Context).Execute(new AddEntityPropertyBindingParameterSet
			                                                                      {
                                                                                      EntityId = enterpriseId,
                                                                                      SourceId = sourceId,
                                                                                      ExtKey = "Test1",
                                                                                      PropertyId = PropertyType.PressureInlet,
                                                                                      PeriodTypeId = PeriodType.Twohours
			                                                                      });

			    var entityPropertyDto =
                    new GetEntityPropertyBindingQuery(Context).Execute(new GetEntityPropertyBindingParameterSet
			                                                                 {
                                                                                 ExtKey = "Test1",
                                                                                 SourceId = sourceId
			                                                                 });

                Assert.IsTrue(entityPropertyDto != null);

                new EditEntityPropertyBindingCommand(Context).Execute(new EditEntityPropertyBindingParameterSet
                                                                          {
                                                                              Id = entityPropertyId,
                                                                              EntityId = enterpriseId,
                                                                              SourceId = sourceId,
                                                                              ExtKey = "Test2",
                                                                              PropertyId = PropertyType.PressureInlet,
                                                                              PeriodTypeId = PeriodType.Twohours
                                                                          });
                entityPropertyDto =
                    new GetEntityPropertyBindingQuery(Context).Execute(new GetEntityPropertyBindingParameterSet
                    {
                        ExtKey = "Test2",
                        SourceId = sourceId
                    });

                Assert.IsTrue(entityPropertyDto != null);

                var entityPropertyBindingsPageDto =
                    new GetEntityPropertyBindingPageQuery(Context).Execute(new GetEntityPropertyBindingsParameterSet
                                                                                 {
                                                                                     EntityId = enterpriseId,
                                                                                     SourceId = sourceId,
                                                                                     PeriodTypeId = PeriodType.Twohours,
                                                                                     PageNumber = 0,
                                                                                     PageSize = 20, NamePart = "T"
                                                                                 });

				var entityPropertyBindingsDto =
					new GetEntityPropertyBindingSourceQuery(Context).Execute(new GetEntityPropertyBindingSourceParameterSet
					{
						EntityId = enterpriseId,
						SourceId = sourceId,
						PeriodTypeId = PeriodType.Twohours,
						PropertyTypeId = PropertyType.PressureInlet
                    });
				Assert.IsTrue(entityPropertyBindingsDto != null);

                var entityBindingsList = new GetEntityBindingsListQuery(Context).Execute(new GetEntityBindingsPageParameterSet
			                                                                    {
			                                                                        ShowAll = true,
                                                                                    SourceId = sourceId,
                                                                                    EntityType = EntityType.Pipeline,
                                                                                    PageNumber = 0,
                                                                                    PageSize = 20
			                                                                    });
                Assert.IsTrue(entityBindingsList.Any());

                new DeleteEntityPropertyBindingCommand(Context).Execute(entityPropertyDto.Id);
			}
		}
	}
}
