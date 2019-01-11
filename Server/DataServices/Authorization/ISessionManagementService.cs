using System.Collections.Generic;
using System.ServiceModel;
using GazRouter.DataServices.Infrastructure.Attributes;
using GazRouter.DTO.Authorization.User;

namespace GazRouter.DataServices.Authorization
{
    [ServiceContract]
    [Service("Сервис управления сессиями")]
    public interface ISessionManagementService
    {
        [ServiceAction("Получение списка активных сессий")]
        [OperationContract]
        List<UserSessionDTO> GetActiveSessions();
    }
}
