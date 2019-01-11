using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using GazRouter.DataServices.Infrastructure;
using GazRouter.DataServices.Infrastructure.Sessions;
using GazRouter.DAL.Authorization.User;
using GazRouter.DAL.Core;
using GazRouter.Log;

namespace Rest.Services.Controllers
{
    public class AccountController : ApiController
    {
        MyLogger _logger = new MyLogger("mainLogger");


        // GET: api/Account
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Account/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Account
        public HttpResponseMessage Post([FromBody]string login, [FromBody] string password)
        {
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                var message = string.Format("Not found", login, password);
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, message);
            }
            using (var db = OpenDbContext())
            {
                var userDto = new GazRouter.DAL.Authorization.User.CheckUserQuery(db).Execute(new CheckUserParameters() {UserName = login, Password = password});
                var responseMessage = new HttpResponseMessage(HttpStatusCode.OK);
                responseMessage.Content = new StringContent("");
                responseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");

                return responseMessage;
            }
        }

        // PUT: api/Account/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Account/5
        public void Delete(int id)
        {
        }

        private ExecutionContextReal OpenDbContext()
        {
            return DbContextHelper.OpenDbContext(SessionManager.SystemUserLogin, _logger);
        }
    }
}
