using System;
using System.Linq;
using GazRouter.DAL.ObjectModel.Entities.Attachments;
using GazRouter.DAL.ObjectModel.Entities.Urls;
using GazRouter.DTO.Attachments;
using GazRouter.DTO.ObjectModel.Entities.Urls;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DALTest.ObjectModel
{
    [TestClass]
    public class EntityTests : DalTestBase
    {
        [TestMethod ,TestCategory(Stable)]
		public void EntityUrlTest()
        {
			var entity = GetCompShop(Context);

            var urlId = new AddEntityUrlCommand(Context).Execute(
                new AddEntityUrlParameterSet
                {
                    EntityId = entity,
                    Description = "Text",
                    Url = "Url"
                });

            var urlList = new GetEntityUrlListQuery(Context).Execute(entity);
            Assert.IsNotNull(urlList);
            var url = urlList.SingleOrDefault(u => u.UrlId == urlId);
            Assert.IsNotNull(url);


            new EditEntityUrlCommand(Context).Execute(
                new EditEntityUrlParameterSet
                {
                    UrlId = urlId,
                    EntityId = entity,
                    Description = "Text01",
                    Url = "Url01"
                });

            new RemoveEntityUrlCommand(Context).Execute(urlId);

            urlList = new GetEntityUrlListQuery(Context).Execute(entity);
            Assert.IsNotNull(urlList);
            url = urlList.SingleOrDefault(u => u.UrlId == urlId);
            Assert.IsNull(url);

        }

        [TestMethod, TestCategory(Stable)]
        public void EntityAttachmentTest()
        {
            var entity = GetCompShop(Context);

            var attachId = new AddEntityAttachmentCommand(Context).Execute(
                new AddAttachmentParameterSet<Guid>
                {
                    ExternalId = entity,
                    Description = "tetx",
                    FileName = "file.name",
                    Data = new byte[] {1,0,0}
                });

            var attachList = new GetEntityAttachmentListQuery(Context).Execute(entity);
            Assert.IsNotNull(attachList);
            var attach = attachList.SingleOrDefault(a => a.Id == attachId);
            Assert.IsNotNull(attach);

            new RemoveEntityAttachmentCommand(Context).Execute(attachId);

            attachList = new GetEntityAttachmentListQuery(Context).Execute(entity);
            Assert.IsNotNull(attachList);
            attach = attachList.SingleOrDefault(a => a.Id == attachId);
            Assert.IsNull(attach);
        }


    }
}
