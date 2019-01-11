using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using GazRouter.DAL.Authorization.Action;
using GazRouter.DAL.Core;
using GazRouter.DataServices.Infrastructure.Attributes;
using GazRouter.DataServices.Infrastructure.Sessions;
using GazRouter.Log;

namespace GazRouter.DataServices.Infrastructure.Authorization
{
    public static class ServiceActions
    {
        public static void Init(Assembly assembly, MyLogger logger)
        {
            if (_allActions != null) return;
            
            lock (_locker)
            {
                if (_allActions == null)
                {
                    GetActionsFromAssembly(assembly);
                    using (var context = DbContextHelper.OpenDbContext(SessionManager.SystemUserLogin, logger))
                    {
                        UpdateActionsInDb(context);
                    }
                }
            }
        }

        private static readonly object _locker = new object();
        
        private static List<ServiceAction> _allActions;

        public static IEnumerable<ServiceAction> AllActions
        {
            get { return _allActions; }
        }

        private static void GetActionsFromAssembly(Assembly assembly)
        {
            _allActions = new List<ServiceAction>();

            var types = assembly.GetTypes();

            foreach (var service in types)
            {
                var serviceAttribute =
                    service.GetCustomAttributes(typeof (ServiceAttribute), false).FirstOrDefault() as ServiceAttribute;

                if (serviceAttribute == null)
                    continue;

                var serviceName = service.Name;
                var serviceContract =
                    service.GetCustomAttributes(typeof (ServiceContractAttribute), false).FirstOrDefault() as
                    ServiceContractAttribute;
                if (serviceContract != null && !string.IsNullOrEmpty(serviceContract.Name)) serviceName = serviceContract.Name;

                foreach (var method in service.GetMethods())
                {
                    var serviceActionAttribute =
                        method.GetCustomAttributes(typeof (ServiceActionAttribute), false).FirstOrDefault() as
                        ServiceActionAttribute;

                    if (serviceActionAttribute == null)
                        continue;


                    var actionName = method.Name;
                    var operationContract =
                        method.GetCustomAttributes(typeof (OperationContractAttribute), false).FirstOrDefault() as
                        OperationContractAttribute;
                    if (operationContract != null && !string.IsNullOrEmpty(operationContract.Name))
                        actionName = operationContract.Name;


                    _allActions.Add(new ServiceAction(string.Format("{0}/{1}", serviceName, actionName),
                                                      serviceActionAttribute.AllowedByDefault,
                                                      serviceActionAttribute.ActionDescription,
                                                      serviceAttribute.ServiceDescription));
                }
            }
        }

        private static void UpdateActionsInDb(ExecutionContext context)
        {
            var editCommand = new EditActionCommand(context);
            var addCommand = new AddActionCommand(context);
            var removeCommand = new DeleteActionCommand(context);

            var actionDtos = new GetActionsAllQuery(context).Execute();


            foreach (var action in _allActions)
            {
                var foundAction = actionDtos.FirstOrDefault(item => item.Path == action.ActionPath);

                if (foundAction != null)
                {
                    if (foundAction.Description != action.ActionDescription ||
                        foundAction.ServiceDescription != action.ServiceDescription)
                        editCommand.Execute(new EditActionParameterSet
                            {
                                Id = foundAction.Id,
                                Description = action.ActionDescription,
                                Path = foundAction.Path,
                                ServiceDescription = action.ServiceDescription
                            });
                    actionDtos.Remove(foundAction);
                }
                else
                {
                    addCommand.Execute(new AddActionParameterSet
                        {
                            Description = action.ActionDescription,
                            Path = action.ActionPath,
                            ServiceDescription = action.ServiceDescription
                        });
                }
            }

            //При запуске апп-сервера из под студии старые методы сервисов не удаляются из БД
            //так как это ломает права для уже развернутого приложения.
            //Вариант запуска приложения на машине разработчика без приаттаченного дебаггера здесь не предусмотрен.
            if (!Debugger.IsAttached)
            {
                foreach (var actionDto in actionDtos)
                {
                    removeCommand.Execute(actionDto.Id);
                }
            }
        }
    }
}