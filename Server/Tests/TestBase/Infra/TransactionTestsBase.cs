using System;
using GazRouter.DAL.Authorization.User;
using GazRouter.DTO.Authorization.User;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestBase.Infra
{
    [TestClass]
    public abstract class TransactionTestsBase : TestBase
    {
          public new ExecutionContextTest OpenDbContext()
          {
              throw new Exception("Не надо самим открывать context");
          }

        protected ExecutionContextTest Context;

        [TestInitialize]
        public void Init()
        {
            Context = base.OpenDbContext();
        }

        [TestCleanup]
        public void Cleanup()
        {
            Context.Dispose();
        }



          protected UserDTO GetCurrentUser()
          {
              return new GetUserByLoginQuery(Context).Execute(Context.UserIdentifier);
          }
    }
}