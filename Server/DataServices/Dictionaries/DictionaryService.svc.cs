using GazRouter.DAL.ObjectModel;
using GazRouter.DataServices.BL;
using GazRouter.DataServices.Infrastructure;
using GazRouter.DataServices.Infrastructure.Attributes.Behaviors;
using GazRouter.DataServices.Infrastructure.Authorization;
using GazRouter.DataServices.Infrastructure.Services;
using GazRouter.DataServices.Infrastructure.Sessions;
using GazRouter.DTO.Dictionaries;
using GazRouter.Log;
using System.Diagnostics;

namespace GazRouter.DataServices.Dictionaries
{
    [ErrorHandlerLogger("mainLogger")]
    [Authorization]
    public class DictionaryService : ServiceBase, IDictionaryService
    {
        private object _lock = new object();
        public DictionaryRepositoryDTO GetDictionaryRepository(bool force)
        {
            lock (_lock)
            {
                if (force)
                {
                    var logger = new MyLogger("mainLogger");
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
                return DictionaryRepository.Dictionaries;
            }
        }
    }
}
