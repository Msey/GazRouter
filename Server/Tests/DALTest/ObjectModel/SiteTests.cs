using System;
using System.Linq;
using GazRouter.DAL.Dictionaries.GasTransportSystem;
using GazRouter.DAL.ObjectModel.Sites;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.Sites;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DALTest.ObjectModel
{
    [TestClass]
    public class SiteTests : DalTestBase
    {
        [TestMethod ,TestCategory(Stable)]
        public void AddEditDeleteSiteTests()
        {
            Guid newGuidEnt = GetEnterpriseId();

			Guid newGuidSite = CreateSite();
			var gastransport = new GetGasTransportSystemListQuery(Context).Execute().First();
            new EditSiteCommand(Context).Execute(new EditSiteParameterSet
            {
                Id = newGuidSite, Name = "TestEditSite", ParentId = newGuidEnt,GasTransportSystemId = gastransport.Id
            });

            new DeleteSiteCommand(Context).Execute(new DeleteEntityParameterSet
            {
                EntityType = EntityType.Site, Id = newGuidSite
            });
        }
    }
}
