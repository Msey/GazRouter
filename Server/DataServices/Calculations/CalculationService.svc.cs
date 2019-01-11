using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.DataServices.Infrastructure.Attributes.Behaviors;
using GazRouter.DataServices.Infrastructure.Services;
using GazRouter.DAL.Calculations;
using GazRouter.DAL.Calculations.Calculation;
using GazRouter.DAL.Calculations.Log;
using GazRouter.DAL.Calculations.Parameter;
using GazRouter.DAL.SeriesData.Series;
using GazRouter.DTO.Calculations;
using GazRouter.DTO.Calculations.Calculation;
using GazRouter.DTO.Calculations.Log;
using GazRouter.DTO.Calculations.Parameter;
using GazRouter.DTO.SeriesData.Series;

namespace GazRouter.DataServices.Calculations
{
    [ErrorHandlerLogger("mainLogger")]
    [Authorization]
    public class CalculationService : ServiceBase, ICalculationService
    {
        public List<CalculationDTO> GetCalculationList(GetCalculationListParameterSet parameters)
        {
            var result =
                ExecuteRead<GetCalculationListQuery, List<CalculationDTO>, GetCalculationListParameterSet>(parameters);
            return result;
        }
        
        public int AddCalculation(AddCalculationParameterSet parameters)
        {
            var result = ExecuteRead<AddCalculationCommand, int, AddCalculationParameterSet>(parameters);
            return result;
        }

        public void EditCalculation(EditCalculationParameterSet parameters)
        {
            ExecuteNonQuery<EditCalculationCommand, EditCalculationParameterSet>(parameters);
        }

        public void DeleteCalculation(int parameters)
        {
            ExecuteNonQuery<DeleteCalculationCommand, int>(parameters);
        }

        public int AddCalculationParameter(AddEditCalculationParameterParameterSet parameters)
        {
            return ExecuteRead<AddCalculationParameterCommand, int, AddEditCalculationParameterParameterSet>(parameters);
        }

        public void EditCalculationParameter(AddEditCalculationParameterParameterSet parameters)
        {
            ExecuteNonQuery<EditCalculationParameterCommand, AddEditCalculationParameterParameterSet>(parameters);
        }

        public void DeleteCalculationParameter(int parameters)
        {
            ExecuteNonQuery<DeleteCalculationParameterCommand, int>(parameters);
        }

        public int GetCalculationParameter(GetCalculationParameterParameterSet parameters)
        {
            return ExecuteRead<GetCalculationParameterCommand, int, GetCalculationParameterParameterSet>(parameters);
        }

        public List<CalculationParameterDTO> GetCalculationParameterById(int parameters)
        {
            return ExecuteRead<GetCalculationParameterListQuery, List<CalculationParameterDTO>, int>(parameters);
        }

        public TestCalcResultDTO TestExecute(TestCalculationParameterSet parameters)
        {
            var result = new TestCalcResultDTO();

            using (var context = OpenDbContext())
            {
                try
                {
                    result.Parameters = new TestCalculationQuery(context).Execute(parameters);
                }
                catch (Exception e)
                {
                    result.Error = e.Message;
                }
                finally
                {
                    var calc = (new GetCalculationListQuery(context).Execute(
                        new GetCalculationListParameterSet { CalculationId = parameters.CalculationId })).First();

                    result.IsInvalid = calc.IsInvalid;
                }
            }
            
            return result;
        }

        public List<SerializableTuple<DateTime, string>> RunCalc(RunCalcParameterSet parameters)
        {
            var endTimeStamp = parameters.EndTimeStamp;
            var startTimeStamp = parameters.StartTimeStamp;
            var periodType = parameters.PeriodType;
            var result = new List<SerializableTuple<DateTime, string>>();
            using (var context = OpenDbContext())
            {
                var series = new GetSeriesListQuery(context).Execute(new GetSeriesListParameterSet
                {
                    PeriodStart = startTimeStamp,
                    PeriodEnd = (DateTime) endTimeStamp,
                    PeriodType = periodType
                });
                foreach (var serie in series)
                {
                    try
                    {
                        new RunCalcSqlCommand(context).Execute(new RunCalcParameterSet
                        {
                            SeriesId = serie.Id
                        });
                        result.Add(new SerializableTuple<DateTime, string>(serie.KeyDate, "Выполнен"));
                    }
                    catch (Exception e)
                    {
                        result.Add(new SerializableTuple<DateTime, string>(serie.KeyDate, "Ошибка"));
                    }
                }

                return result;
            }
        }

        public List<LogCalculationDTO> GetLogs(GetLogListParameterSet parameters)
        {
            return ExecuteRead<GetCalculationLogsQuery, List<LogCalculationDTO>, GetLogListParameterSet>(parameters);
        }
        
    }
}