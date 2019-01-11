using System.Linq;
using GazRouter.DAL.Dictionaries.EntityTypes;
using GazRouter.DAL.Dictionaries.PhysicalTypes;
using GazRouter.DAL.Dictionaries.PropertyTypes;
using GazRouter.DAL.Dictionaries.States;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.Dictionaries.StatesModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;

namespace DALTest.Dictionaries
{
    [TestClass]
    public class EntityTypeListTests : TransactionTestsBase
    {
        [TestMethod ,TestCategory(Stable)]
		public void EntityTypeList()
        {
            var entityTypes = new GetEntityTypeListQuery(Context).Execute();
            AssertHelper.CheckDictionary(entityTypes, dto => dto.EntityType);

            var propTypes = new GetPropertyTypeListQuery(Context).Execute();
            AssertHelper.CheckDictionary(propTypes, dto => dto.PropertyType);


            var phisicalTypes = new GetPhysicalTypeListQuery(Context).Execute();
            AssertHelper.IsNotEmpty(phisicalTypes);

        }
    }
}