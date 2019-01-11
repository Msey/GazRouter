using System.Linq;
using GazRouter.DAL.Authorization.Action;
using GazRouter.DAL.Authorization.Role;
using GazRouter.DAL.Authorization.User;
using GazRouter.DTO.Authorization.Role;
using GazRouter.DTO.Authorization.User;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;

namespace DALTest.Authorization
{
    [TestClass]
    public class ActionTest : DalTestBase
    {
        [TestMethod ,TestCategory(Stable)]
        public void TestModifyAction()
        {
            {
                var id = new AddActionCommand(Context).Execute(
                    new AddActionParameterSet
                {
                    Path = "TestPath",
                    Description = "TestDescription"
                });

                var action = new GetActionByIdQuery(Context).Execute(id);

                Assert.IsNotNull(action);

                new EditActionCommand(Context).Execute(
                    new EditActionParameterSet
                    {
                        Id = id,
                        Path = "TestPath1",
                        Description = "TestDescription1"
                    });

                action = new GetActionByIdQuery(Context).Execute(id);

                Assert.AreEqual("TestPath1", action.Path);

                new DeleteActionCommand(Context).Execute(id);

                action = new GetActionByIdQuery(Context).Execute(id);

                Assert.IsNull(action);

            }
        }

		[TestMethod, TestCategory(Stable)]
        public void TestGetAllActions()
        {
            {
                new AddActionCommand(Context).Execute(
                    new AddActionParameterSet
                    {
                        Path = "TestPath",
                        Description = "TestDescription"
                    });

                var actions = new GetActionsAllQuery(Context).Execute();

                AssertHelper.IsNotEmpty(actions);
            }
        }

		[TestMethod, TestCategory(Stable)]
        public void TestUserRoleActions()
        {
            {
                var siteId = CreateSite();

                var userId = new AddUserCommand(Context).Execute(
                    new AddUserParameterSet
                        {
                            Login = "TestLogin",
                            Description = "TestDescription",
                            FullName = "TestUser",
                            SiteId = siteId
                        });

                var roleId = new AddRoleCommand(Context).Execute(
                    new AddRoleParameterSet
                    {
                        Name = "TestSysName",
                        Description = "TestDescription"
                    });

                new AddUserRoleCommand(Context).Execute(new UserRoleParameterSet { RoleId = roleId, UserId = userId });

                var actionId1 = new AddActionCommand(Context).Execute(
                    new AddActionParameterSet
                    {
                        Path = "TestPath",
                        Description = "TestDescription"
                    });

                var actionId2 = new AddActionCommand(Context).Execute(
                    new AddActionParameterSet
                    {
                        Path = "TestPath2",
                        Description = "TestDescription2"
                    });

                new AddRoleActionCommand(Context).Execute(
                    new RoleActionParameterSet
                        {
                            RoleId = roleId,
                            ActionId = actionId1
                        });

                new AddRoleActionCommand(Context).Execute(
                    new RoleActionParameterSet
                        {
                            RoleId = roleId,
                            ActionId = actionId2
                        });

                var roleActions = new GetActionsByRoleIdQuery(Context).Execute(roleId);

                Assert.AreEqual(2, roleActions.Count);

                new RemoveRoleActionCommand(Context).Execute(
                    new RoleActionParameterSet
                        {
                            RoleId = roleId,
                            ActionId = actionId1
                        });

                roleActions = new GetActionsByRoleIdQuery(Context).Execute(roleId);
                Assert.AreEqual(1,roleActions.Count);
                Assert.AreEqual(actionId2, roleActions.Single().Id);

                var userActions = new GetActionsByUserIdQuery(Context).Execute(userId);
                Assert.AreEqual(1, userActions.Count);
                Assert.AreEqual(actionId2, userActions.Single().Id);

            }
        }
    }
}
