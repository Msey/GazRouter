using System.Linq;
using GazRouter.DAL.Authorization.User;
using GazRouter.DAL.EventLog.TextTemplates;
using GazRouter.DTO.Authorization.User;
using GazRouter.DTO.EventLog.EventTextTemplates;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DALTest.Event
{
    [TestClass]
    public class EventTextTemplateTest : EventsTestBase
    {
        [TestMethod ,TestCategory(Stable)]
        public void Test()
        {
            var enterpriseId = GetEnterpriseId();

            var user = GetCurrentUser();
            new EditUserCommand(Context).Execute(new EditUserParameterSet
                {
                    Id = user.Id,
                    Description = user.Description,
                    UserName = user.UserName,
                    SiteId = enterpriseId
                });

            var templId = 
            new AddEventTextTemplateCommand(Context).Execute(
                new AddEventTextTemplateParameterSet
                {
                    Name = "Name",
                    Text = "Text"
                });
            
            var templList = new GetEventTextTemplateListQuery(Context).Execute(enterpriseId);
            Assert.IsTrue(templList.Any(t => t.Id == templId));
            var templ = templList.First(t => t.Id == templId);
            Assert.AreEqual(templ.Name, "Name");
            Assert.AreEqual(templ.Text, "Text");


            new EditEventTextTemplateCommand(Context).Execute(
                new EditEventTextTemplateParameterSet
                {
                    Id = templId,
                    Name = "emaN",
                    Text = "txeT"
                });

            templList = new GetEventTextTemplateListQuery(Context).Execute(enterpriseId);
            templ = templList.First(t => t.Id == templId);
            Assert.AreEqual(templ.Name, "emaN");
            Assert.AreEqual(templ.Text, "txeT");

            
            new DeleteEventTextTemplateCommand(Context).Execute(templId);

            templList = new GetEventTextTemplateListQuery(Context).Execute(enterpriseId);
            Assert.IsFalse(templList.Any(t => t.Id == templId));
        }
    }
}
