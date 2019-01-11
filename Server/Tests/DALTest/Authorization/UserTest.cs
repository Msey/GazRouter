using System;
using GazRouter.DAL.Authorization.User;
using GazRouter.DTO.Authorization.User;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;

namespace DALTest.Authorization
{
    [TestClass]
    public class UserTest : DalTestBase
    {
		[TestMethod, TestCategory(Stable)]
        public void TestModifyUser()
        {
            var siteId = CreateSite();

            var userId = CreateUser(siteId);

            var user = new GetUserByIdQuery(Context).Execute(userId);

            Assert.IsNotNull(user);
            Assert.AreEqual(userId, user.Id);

            new EditUserCommand(Context).Execute(
                new EditUserParameterSet
                    {
                        Id = userId,
                        Description = "TestDescription1",
                        UserName = "TestUser1",
                        SiteId = siteId
                    });

            user = new GetUserByIdQuery(Context).Execute(userId);

            Assert.AreEqual("TestUser1",user.UserName);

            new EditUserSettingsCommand(Context).Execute(
                new EditUserSettingsParameterSet
                    {
                        Id = userId,
                        SettingsUser = new UserSettings {EventLogArchivingDelay = 5}
                    });


            user = new GetUserByIdQuery(Context).Execute(userId);
            Assert.AreEqual(5, user.UserSettings.EventLogArchivingDelay);
            new DeleteUserCommand(Context).Execute(userId);

            user = new GetUserByIdQuery(Context).Execute(userId);

            Assert.IsNull(user);
            
        }

        private int CreateUser(Guid siteId)
        {
            var userId = new AddUserCommand(Context).Execute(
                new AddUserParameterSet
                    {
                        Login = "TestLogin",
                        Description = "TestDescription",
                        FullName = "TestUser",
                        SiteId = siteId,
                        SettingsUser = new UserSettings()
                    });
            return userId;
        }

		[TestMethod, TestCategory(Stable)]
        public void TestGetAllUsers()
        {
            var siteId =
                CreateSite();
            CreateUser(siteId);

            var users = new GetUsersAllQuery(Context).Execute();

            AssertHelper.IsNotEmpty(users);
        }
    }
}
