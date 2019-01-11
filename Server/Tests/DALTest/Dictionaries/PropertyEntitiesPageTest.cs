using System.Linq;
using GazRouter.DAL.Bindings.PropertyBindings;
using GazRouter.DAL.Dictionaries.Sources;
using GazRouter.DTO;
using GazRouter.DTO.Bindings.EntityBindings;
using GazRouter.DTO.Bindings.PropertyBindings;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;

namespace DALTest.Dictionaries
{
    [TestClass]
    public class PropertyBindingsTest : TransactionTestsBase
    {
        [TestMethod ,TestCategory(Stable)]
        public void PropertyBindings()
        {
            var sourceId = new GetSourcesListQuery(Context).Execute().First().Id;

            
            var propertyBindingsId =
                new AddPropertyBindingCommand(Context).Execute(new AddPropertyBindingParameterSet
                    {
                        SourceId = sourceId,
                        PropertyId = PropertyType.PressureInlet,
                        ExtEntityId = "TestSource"
                    });

            new EditPropertyBindingCommand(Context).Execute(new EditPropertyBindingParameterSet
                {
                    Id = propertyBindingsId,
                    SourceId = sourceId,
                    ExtEntityId = "TestSource",
                    PropertyId = PropertyType.PressureInlet
            });

            var list =
                new GetPropertyEntitiesPageQuery(Context).Execute(new GetPropertyBindingsParameterSet
                    {
                        NamePart = string.Empty,
                        ShowAll = true,
                        SortBy = SortBy.Name,
                        SortOrder = SortOrder.Desc,
                        SourceId = sourceId
                    });

            AssertHelper.IsNotEmpty(list);

            new DeletePropertyBindingCommand(Context).Execute(propertyBindingsId);
        }
    }
}