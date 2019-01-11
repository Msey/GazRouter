using System;
using System.Collections.Generic;
using System.ServiceModel;
using GazRouter.DataServices.Infrastructure.Attributes;
using GazRouter.DTO.Calculations;
using GazRouter.DTO.Calculations.Calculation;
using GazRouter.DTO.Calculations.Log;
using GazRouter.DTO.Calculations.Parameter;

namespace GazRouter.DataServices.Calculations
{
    [Service("Нетиповые расчеты")]
    [ServiceContract]
    public interface ICalculationService
    {
        [ServiceAction("Получение списка расчетов")]
        [OperationContract]
        List<CalculationDTO> GetCalculationList(GetCalculationListParameterSet parameters);

        [ServiceAction("Добавление расчета")]
        [OperationContract]
        int AddCalculation(AddCalculationParameterSet parameters);

        [ServiceAction("Редактирование расчета")]
        [OperationContract]
        void EditCalculation(EditCalculationParameterSet parameters);

        [ServiceAction("Удаление расчета")]
        [OperationContract]
        void DeleteCalculation(int parameters);

        [ServiceAction("Добавление параметра расчета")]
        [OperationContract]
        int AddCalculationParameter(AddEditCalculationParameterParameterSet parameters);

        [ServiceAction("Редактирование параметра расчета")]
        [OperationContract]
        void EditCalculationParameter(AddEditCalculationParameterParameterSet parameters);

        [ServiceAction("Удаление параметра расчета")]
        [OperationContract]
        void DeleteCalculationParameter(int parameters);

        [ServiceAction("Получение параметра расчета")]
        [OperationContract]
        int GetCalculationParameter(GetCalculationParameterParameterSet parameters);

        [ServiceAction("Получение параметра по расчету")]
        [OperationContract]
        List<CalculationParameterDTO> GetCalculationParameterById(int parameters);

        [ServiceAction("Тестирование расчета")]
        [OperationContract]
        TestCalcResultDTO TestExecute(TestCalculationParameterSet parameters);

        [ServiceAction("Запуск расчета")]
        [OperationContract]
        List<SerializableTuple<DateTime, string>> RunCalc(RunCalcParameterSet parameters);

        [ServiceAction("Лог расчета")]
        [OperationContract]
        List<LogCalculationDTO> GetLogs(GetLogListParameterSet parameters);
        
    }
}
