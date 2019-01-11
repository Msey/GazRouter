using System;
using System.ServiceModel;
using GazRouter.DataServices.Infrastructure.Attributes;
using GazRouter.DTO;

namespace GazRouter.DataServices.Time
{
    [Service("Сервис времени и состояниий модулей")]
    [ServiceContract]
    public interface ITimeService
    {
        [OperationContract]
        [ServiceAction("Получение серверного времени", true)]
        DateTime GetTimeServer();

        [OperationContract]
        [ServiceAction("Получение токена состояния модуля", true)]
        Guid GetServerState(Module parameters);

    }
}
