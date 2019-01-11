using System.Linq;
using GazRouter.DAL.Authorization.Role;
using GazRouter.DAL.Authorization.User;
using GazRouter.DTO.Authorization.Role;
using GazRouter.DTO.Authorization.User;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;

namespace DALTest.Authorization
{
    [TestClass]
    public class RoleTest : DalTestBase
    {
		[TestMethod, TestCategory(Stable)]
        public void TestModifyRole()
        {
            var id = new AddRoleCommand(Context).Execute(
                new AddRoleParameterSet
                    {
                        Name = "TestSysName",
                        Description = "TestDescription"
                    });

            var role = new GetRoleByIdQuery(Context).Execute(id);

            Assert.IsNotNull(role);

            new EditRoleCommand(Context).Execute(
                new EditRoleParameterSet
                    {
                        Id = id,
                        Name = "TestSysName1",
                        Description = "TestDescription1"
                    });

            role = new GetRoleByIdQuery(Context).Execute(id);

            Assert.AreEqual("TestSysName1", role.Name);

            new DeleteRoleCommand(Context).Execute(id);

            role = new GetRoleByIdQuery(Context).Execute(id);

            Assert.IsNull(role);
        }

		[TestMethod, TestCategory(Stable)]
        public void TestGetAllRoles()
        {
                new AddRoleCommand(Context).Execute(
                    new AddRoleParameterSet
                    {
                        Name = "TestSysName",
                        Description = "TestDescription"
                    });

                var roles = new GetRolesAllQuery(Context).Execute();

                AssertHelper.IsNotEmpty(roles);
            
        }

		[TestMethod, TestCategory(Stable)]
        public void TestUserRoles()
        {
            {
                var siteId =  CreateSite();

                var userId = new AddUserCommand(Context).Execute(
                    new AddUserParameterSet
                        {
                            Login = "TestLogin",
                            Description = "TestDescription",
                            FullName = "TestUser",
                            SiteId = siteId
                        });

                var roleId1 = new AddRoleCommand(Context).Execute(
                    new AddRoleParameterSet
                    {
                        Name = "TestSysName1",
                        Description = "TestDescription1"
                    });

                var roleId2 = new AddRoleCommand(Context).Execute(
                    new AddRoleParameterSet
                    {
                        Name = "TestSysName2",
                        Description = "TestDescription2"
                    });

                new AddUserRoleCommand(Context).Execute(
                    new UserRoleParameterSet
                        {
                            UserId = userId,
                            RoleId = roleId1
                        });

                new AddUserRoleCommand(Context).Execute(
                    new UserRoleParameterSet
                        {
                            UserId = userId,
                            RoleId = roleId2
                        });

                var userRoles = new GetRolesByUserIdQuery(Context).Execute(userId);

                Assert.AreEqual(2, userRoles.Count );

                new RemoveUserRoleCommand(Context).Execute(
                    new UserRoleParameterSet
                        {
                            UserId = userId,
                            RoleId = roleId1
                        });

                userRoles = new GetRolesByUserIdQuery(Context).Execute(userId);

                Assert.AreEqual( 1, userRoles.Count);
                Assert.AreEqual(roleId2, userRoles.Single().Id);

            }

        }

    }
}
