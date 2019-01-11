using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using GazRouter.DataServices.Infrastructure;
using GazRouter.DataServices.Infrastructure.Sessions;
using GazRouter.DAL.Authorization.User;
using GazRouter.DAL.Core;
using GazRouter.Log;
using Rest.Services.Helpers;

namespace Rest.Services.Handlers
{
    public class AuthMessageHandler : DelegatingHandler
    {
        private const char Separator = '_';
        private HttpApplication _app;
        private MyLogger _logger;

        public AuthMessageHandler()
        {
            _logger = new MyLogger("mainLogger");
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            DateTime timeStamp;
            string token = HttpContext.Current.Request.Headers["logontoken"];
            var now = DateTime.Now;
            HttpContextBase context = new HttpContextWrapper(HttpContext.Current);
            if (string.IsNullOrEmpty(token))
            {
                if (request.Method == HttpMethod.Post && RouteTable.Routes["Login"].GetRouteData(context)?.RouteHandler != null)
                {
                    try
                    {
                        var content = await request.Content.ReadAsStringAsync();
                        using (var sr = new StringReader(content))
                        {
                            var doc = XDocument.Load(sr);
                            Debug.Assert(doc.Root != null, "doc.Root != null");
                            var userName = doc.Root.Elements("username").Single().Value;
                            var password = doc.Root.Elements("password").Single().Value;
                            token = now.ToString(CultureInfo.InvariantCulture);
                            SignIn(userName, password, out token);
                            var message = $"<exchangemessage><headersection><servername>mii.dev.ga.loc</servername><generatedtime>{now.ToString(CultureInfo.InvariantCulture)}</generatedtime><title>результаты авторизации</title><logontoken>{token}</logontoken> </headersection></exchangemessage> ";

                            return ResponseMessage(HttpStatusCode.OK, message);
                        }
                    }
                    catch (Exception e)
                    {
                        return ErrorMessage(HttpStatusCode.BadRequest, e.Message);
                    }
                }
                else
                {
                    return ErrorMessage(HttpStatusCode.NotFound, "Истек пароль");
                }

            }
            else if (TryParseToken(token, out timeStamp) && timeStamp.AddHours(2) >= now)
            {
                return await base.SendAsync(request, cancellationToken);
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
        }



        private void SignIn(string userName, string password, out string token)
        {
            token = null;
            using (var db = OpenDbContext())
            {
                var user = new CheckUserQuery(db).Execute(new CheckUserParameters() {UserName = userName, Password = password});
                if(user == null) throw new AuthenticationException("Неправильный логин или пароль", null);
                token = GenerateToken(userName);
            }
        }

        private string GenerateToken(string userName)
        {
            string text = $"{userName}{Separator}{DateTime.Now.ToString(CultureInfo.InvariantCulture)}";
            return EncryptionHelper.Encrypt(text);
        }

        private bool TryParseToken(string token, out DateTime timeStamp)
        {
            var text = EncryptionHelper.Decrypt(token);
            timeStamp = new DateTime();
            if (string.IsNullOrEmpty(text)) return false;
            timeStamp = DateTime.Parse(text.Split(Separator)[1], CultureInfo.InvariantCulture);
            return true;
        }

        private ExecutionContextReal OpenDbContext()
        {
            return DbContextHelper.OpenDbContext(SessionManager.SystemUserLogin, _logger);
        }


        private HttpResponseMessage ErrorMessage(HttpStatusCode code, string message)
        {
            return ResponseMessage(code, $"<error_code>{(int)code}</error_code><message>{message}</message></error>");
        }
        private HttpResponseMessage ResponseMessage(HttpStatusCode code, string content, params KeyValuePair<string, string>[] headers)
        {
            var message = new HttpResponseMessage(code)
            {
                Content = new StringContent(content)
            };
            foreach (var pair in headers)
            {
                message.Headers.Add(pair.Key, pair.Value);
            }

            return message;
        }
     }
}