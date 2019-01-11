using System;
using System.Collections.Generic;
using GazRouter.DTO.Calculations;
using GazRouter.DTO.Calculations.Log;

namespace DataProviders.Calculations
{
    public class CalculationDataProvider : DataProviderBase<ICalculationService>
    {
        protected override string ServiceUri
        {
            get { return "/Calculations/CalculationService.svc"; }
        }

        public void GetCalculationList(GetCalculationListParameterSet parameters, Func<List<CalculationDTO>, Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginGetCalculationList, channel.EndGetCalculationList, callback, parameters, behavior);
        }

        public void GetCalculationListById(int parameters, Func<CalculationDTO, Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginGetCalculationListById, channel.EndGetCalculationListById, callback, parameters, behavior);
        }

        public void EditCalculation(EditCalculationParameterSet parameters, Func<Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginEditCalculation, channel.EndEditCalculation, callback, parameters, behavior);
        }

        public void DeleteCalculation(int parameters, Func<Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginDeleteCalculation, channel.EndDeleteCalculation, callback, parameters, behavior);
        }

        public void DeleteCalculationParameter(int parameters, Func<Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginDeleteCalculationParameter, channel.EndDeleteCalculationParameter, callback, parameters, behavior);
        }

        public void GetCalculationParameterById(int parameters, Func<List<CalculationParameterDTO>, Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginGetCalculationParameterById, channel.EndGetCalculationParameterById, callback, parameters, behavior);
        }

        public void TestExecute(GetCalcExecuteSqlParameterSet parameters, Func<CalcExecOutput, Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginTestExecute, channel.EndTestExecute, callback, parameters, behavior);
        }

        public void RunCalc(RunCalcParameterSet parameters, Func<List<SerializableTuple<DateTime, string>>, Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginRunCalc, channel.EndRunCalc, callback, parameters, behavior);
        }

        public void GetLogs(GetLogListParameterSet parameters, Func<List<LogCalculationDTO>, Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginGetLogs, channel.EndGetLogs, callback, parameters, behavior);
        }

        public void GetCalculationListByVar(GetCalculationListByVarParameterSet parameters, Func<List<CalculationsByParameterDTO>, Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginGetCalculationListByVar, channel.EndGetCalculationListByVar, callback, parameters, behavior);
        }
    }
}