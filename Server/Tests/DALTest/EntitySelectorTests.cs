using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.DAL.EntitySelector;
using GazRouter.DAL.ObjectModel.Entities;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.EntitySelector;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;

namespace DALTest
{
    [TestClass]
    public class EntitySelectorTests : DalTestBase
    {
        [TestMethod ,TestCategory(Stable)]
        public void GetPageTest()
        {
            EntitiesPageDTO res;

            {
                CreateSite();
				res = new GetEntitiesPageQuery(Context).Execute(new GetEntitesPageParameterSet { PageNumber = 0, PageSize = 30, NamePart = "лпу", EntityTypes = new List<EntityType> { EntityType.Site }});
            }
            AssertHelper.IsNotEmpty(res.Entities);
          
        }

        [TestMethod ,TestCategory(Stable)]
        public void GetEntitiesAllTest()
        {
            List<CommonEntityDTO> res;

            {
                CreateSite();
                res = new GetEntityListQuery(Context).Execute(null);
            }
    
            AssertHelper.IsNotEmpty(res);
        }

        [TestMethod ,TestCategory(Stable)]
        public void GetEntityByIdTest()
        {
            CreateSite();
            EntitiesPageDTO res =
                new GetEntitiesPageQuery(Context).Execute(new GetEntitesPageParameterSet
                    {
                        PageNumber = 0,
                        PageSize = 1,
                        EntityTypes = new List<EntityType> {EntityType.Site}
                    });
            Assert.AreEqual(res.Entities.Count, 1);
            var id = res.Entities[0].Id;
            var entity = new GetEntityQuery(Context).Execute(id);
            Assert.IsNotNull(entity);
            Assert.AreEqual(entity.Id, id);
        }


		[TestMethod, TestCategory(Stable)]
		public void GetFavoritesEntityTest()
		{
			var guid = CreateSite();
			var result = new GetEntityListQuery(Context).Execute(new GetEntityListParameterSet {EntityIdList = new List<Guid> {guid}});
			Assert.IsTrue(result.Any(e => e.Id == guid));
		}
    }
}
