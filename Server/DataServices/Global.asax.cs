using System;
using System.Diagnostics;
using System.Reflection;
using System.Web;
using GazRouter.DataServices.BL;
using GazRouter.DataServices.Dictionaries;
using GazRouter.DataServices.Infrastructure;
using GazRouter.DataServices.Infrastructure.Authorization;
using GazRouter.DataServices.Infrastructure.Sessions;
using GazRouter.DAL.ObjectModel;
using GazRouter.Log;

namespace GazRouter.DataServices
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            var logger = new MyLogger("mainLogger");
            ServiceActions.Init(Assembly.GetExecutingAssembly(), logger);
            using (var context = DbContextHelper.OpenDbContext(SessionManager.SystemUserLogin, logger))
            {
                if (!Debugger.IsAttached)
                {
                    new AfterValidateCommand(context).Execute();
                }
                DictionaryRepository.Init(context);
                ObjectModelValidator.Validate(context);
            }
        }


    }

}