using System.Collections.Generic;
using System.ServiceModel;
using GazRouter.DataServices.Infrastructure.Attributes;
using GazRouter.DTO.SystemVariables;

namespace GazRouter.DataServices.SystemVariables
{
    [Service("Системные переменные")]
    [ServiceContract]
    public interface ISysVarService
    {
        [ServiceAction("Получение списка cистемных переменных")]
        [OperationContract]
        List<IusVariableDTO> GetIusVariableList();

        [ServiceAction("Изменение значения системной переменной")]
        [OperationContract]
        void EditIusVariableValue(IusVariableParameterSet newValue);

        [ServiceAction("Получение относительного пути к файлу с инструкциями")]
        [OperationContract]
        string GetHelpFileName();

        [ServiceAction("Получение относительного пути к файлу с послоедними изменениями")]
        [OperationContract]
        string GetLastChangesFileName();
    }
}
