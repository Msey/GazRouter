using System.Linq;
using GazRouter.DAL.Appearance;
using GazRouter.DAL.Appearance.Positions;
using GazRouter.DAL.Appearance.Styles;
using GazRouter.DAL.Appearance.Versions;
using GazRouter.DAL.Authorization.User;
using GazRouter.DAL.Dictionaries.GasTransportSystem;
using GazRouter.DTO.Appearance;
using GazRouter.DTO.Appearance.Versions;
using GazRouter.DTO.Authorization.User;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;

namespace DALTest.Appearance
{
    [TestClass]
    public class SchemeTests : DalTestBase
    {
        [TestMethod, TestCategory(Stable)]
        public void FullTest()
        {
            var entId = GetEnterpriseId();
            

            var user = GetCurrentUser();
            new EditUserCommand(Context).Execute(new EditUserParameterSet
                {
                    Id = user.Id,
                    Description = user.Description,
                    UserName = user.UserName,
                    SiteId = entId
                });

            var systemList = new GetGasTransportSystemListQuery(Context).Execute();
            Assert.IsTrue(systemList.Count > 0);

            var schemeId =
                new AddSchemeCommand(Context).Execute(new SchemeParameterSet
                    {
                        Description = string.Empty,
                        Name = "TestScheme1",
                        SystemId = systemList.First().Id
                    });

            var schemeVersionId =
                new AddSchemeVersionCommand(Context).Execute(new SchemeVersionParameterSet
                    {
                        Content = "test",
                        Description = string.Empty,
                        SchemeId = schemeId
                    });

            var versionItemList = new GetSchemeVersionListQuery(Context).Execute();
         
            AssertHelper.IsNotEmpty(versionItemList);
        
         
            new PublishSchemeVersionCommand(Context).Execute(schemeVersionId);
          var schemeVersionDTO = new GetSchemeVersionByIdQuery(Context).Execute(schemeVersionId);
            Assert.IsTrue(schemeVersionDTO.IsPublished);

            var schemeVersionList = new GetPublishedSchemeVersionListQuery(Context).Execute();
            AssertHelper.IsNotEmpty(schemeVersionList);

            new UnPublishSchemeVersionCommand(Context).Execute(schemeVersionId);
            schemeVersionDTO = new GetSchemeVersionByIdQuery(Context).Execute(schemeVersionId);
            Assert.IsTrue(!schemeVersionDTO.IsPublished);


            new DeleteSchemeVersionCommand(Context).Execute(schemeVersionId);
        }
    }

}