using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using GazRouter.DataServices.Infrastructure;
using GazRouter.DAL.DataExchange.DataSource;
using GazRouter.DAL.DataExchange.ExchangeEntity;
using GazRouter.DAL.DataExchange.ExchangeLog;
using GazRouter.DAL.DataExchange.ExchangeProperty;
using GazRouter.DAL.DataExchange.ExchangeTask;
using GazRouter.DataServices.Infrastructure.Attributes.Behaviors;
using GazRouter.DataServices.Infrastructure.Services;
using GazRouter.DAL.DataExchange.Asdu;
using GazRouter.DAL.DataExchange.ASUTPImport;
using GazRouter.DAL.DataExchange.TypicalExchange;
using GazRouter.DAL.Dictionaries.Enterprises;
using GazRouter.DTO.Bindings.ExchangeEntities;
using GazRouter.DTO.DataExchange.Asdu;
using GazRouter.DTO.DataExchange.ASUTPImport;
using GazRouter.DTO.DataExchange.DataSource;
using GazRouter.DTO.DataExchange.ExchangeEntity;
using GazRouter.DTO.DataExchange.ExchangeLog;
using GazRouter.DTO.DataExchange.ExchangeProperty;
using GazRouter.DTO.DataExchange.ExchangeTask;
using GazRouter.DTO.DataExchange.Transformation;
using GazRouter.DTO.Dictionaries.Enterprises;
using GazRouter.DTO.Dictionaries.ExchangeTypes;
using GazRouter.DTO.Exchange.ExchangeSettings;
using GazRouter.DTO.SeriesData.PropertyValues;
using GazRouter.Service.Exchange.Lib;
using GazRouter.Service.Exchange.Lib.Import;
using GazRouter.Service.Exchange.Lib.Import.Astra;
using GazRouter.Service.Exchange.Lib.Run;
using DeleteExchangeEntityCommand = GazRouter.DAL.DataExchange.ExchangeEntity.DeleteExchangeEntityCommand;

namespace GazRouter.DataServices.DataExchange
{
    [ErrorHandlerLogger("mainLogger")]
    [Authorization]
    public class DataExchangeService : ServiceBase, IDataExchangeService
    {
        public List<DataSourceDTO> GetDataSourceList(GetDataSourceListParameterSet parameter)
        {
            return ExecuteRead<GetDataSourceListQuery, List<DataSourceDTO>, GetDataSourceListParameterSet>(parameter);
        }

        public int AddDataSource(AddDataSourceParameterSet parameter)
        {
            return ExecuteRead<AddDataSourceCommand, int, AddDataSourceParameterSet>(parameter);
        }

        public void EditDataSource(EditDataSourceParameterSet parameter)
        {
            ExecuteNonQuery<EditDataSourceCommand, EditDataSourceParameterSet>(parameter);
        }

        public void DeleteDataSource(int parameter)
        {
            ExecuteNonQuery<DeleteDataSourceCommand, int>(parameter);
        }




        public List<ExchangeTaskDTO> GetExchangeTaskList(GetExchangeTaskListParameterSet parameter)
        {
            return ExecuteRead<GetExchangeTaskListQuery, List<ExchangeTaskDTO>, GetExchangeTaskListParameterSet>(parameter);
        }

        public int AddExchangeTask(AddExchangeTaskParameterSet parameter)
        {
            return ExecuteRead<AddExchangeTaskCommand, int, AddExchangeTaskParameterSet>(parameter);
        }

        public void EditExchangeTask(EditExchangeTaskParameterSet parameter)
        {
            ExecuteNonQuery<EditExchangeTaskCommand, EditExchangeTaskParameterSet>(parameter);
        }

        public void DeleteExchangeTask(int parameter)
        {
            ExecuteNonQuery<DeleteExchangeTaskCommand, int>(parameter);
        }


        public void RunExchangeTask(RunExchangeTaskParameterSet parameters)
        {
            ExchangeTaskDTO config;
            if (parameters.Id.HasValue)
            {
                config =
                    ExecuteRead<GetExchangeTaskListQuery, List<ExchangeTaskDTO>, GetExchangeTaskListParameterSet>(
                        new GetExchangeTaskListParameterSet {Id = parameters.Id}).FirstOrDefault();
               ExchangeExportAgent.RunAndTransportConfig(config, parameters.TimeStamp, parameters.PeriodTypeId);
            }
            else
            {
                ExecuteRead<GetExchangeTaskListQuery, List<ExchangeTaskDTO>, GetExchangeTaskListParameterSet>(
                    new GetExchangeTaskListParameterSet {ExchangeTypeId = ExchangeType.Export})
                    .Where(t => t.EnterpriseId.HasValue)
                    .ToList()
                    .ForEach(t => ExchangeExportAgent.RunAndTransportConfig(t, parameters.TimeStamp, parameters.PeriodTypeId));
            }

        }

        public void RunAstra(RunAstaParameterSet parameters)
        {
            //ExchangeExportAgent.RunAstra(parameters.PeriodType, parameters.TimeStamp, 
            //    directory:
            //    ExchangeHelper.EnsureDirectoryExist(AppSettingsManager.ExchangeDirectory, "Astra")
            //    );
        }


        public List<ExchangeEntityDTO> GetExchangeEntityList(GetExchangeEntityListParameterSet parameter)
        {
            return ExecuteRead<GetExchangeEntityListQuery, List<ExchangeEntityDTO>, GetExchangeEntityListParameterSet>(parameter);
        }

        public void AddExchangeEntity(AddEditExchangeEntityParameterSet parameter)
        {
            ExecuteNonQuery<AddExchangeEntityCommand, AddEditExchangeEntityParameterSet>(parameter);
        }
        public void EditTimer(TimerSettingsDTO parameters)
        {
            //TimerSettings.Edit(parameters);
        }
        public void ChangeTimerStatus(TimerSettingsDTO parameters)
        {
            //TimerSettings.Edit(parameters);
        }
        public List<TimerSettingsDTO> GetTimers()
        {
            return new List<TimerSettingsDTO>();
        }

        public void EditExchangeEntity(AddEditExchangeEntityParameterSet parameter)
        {
            ExecuteNonQuery<EditExchangeEntityCommand, AddEditExchangeEntityParameterSet>(parameter);
        }

        public void DeleteExchangeEntity(AddEditExchangeEntityParameterSet parameter)
        {
            ExecuteNonQuery<DeleteExchangeEntityCommand, AddEditExchangeEntityParameterSet>(parameter);
        }

        public void SetExchangeEntity(AddEditExchangeEntityParameterSet parameter)
        {
            ExecuteNonQuery<SetExchangeEntityCommand, AddEditExchangeEntityParameterSet>(parameter);
            
            //using (var context = OpenDbContext())
            //{
            //    var entityList = new GetExchangeEntityListQuery(context).Execute(
            //        new GetExchangeEntityListParameterSet
            //        {
            //            ExchangeTaskIdList = {parameter.ExchangeTaskId}
            //        });
            //    if (entityList.Any(e => e.EntityId == parameter.EntityId))
            //    {
            //        if (string.IsNullOrEmpty(parameter.ExtId))
            //        {
            //            var propList = new GetExchangePropertyListQuery(context).Execute(
            //                new GetExchangeEntityListParameterSet
            //                {
            //                    ExchangeTaskIdList = { parameter.ExchangeTaskId }
            //                });
            //            if (propList.Any(p => p.EntityId == parameter.EntityId))
            //                new EditExchangeEntityCommand(context).Execute(parameter);
            //            else
            //                new DeleteExchangeEntityCommand(context).Execute(parameter);
            //        }
            //        else
            //            new EditExchangeEntityCommand(context).Execute(parameter);
            //    }
            //    else
            //        new AddExchangeEntityCommand(context).Execute(parameter);
            //}
        }





        public List<ExchangePropertyDTO> GetExchangePropertyList(GetExchangeEntityListParameterSet parameter)
        {
            return ExecuteRead<GetExchangePropertyListQuery, List<ExchangePropertyDTO>, GetExchangeEntityListParameterSet>(parameter);
        }

        public void SetExchangeProperty(SetExchangePropertyParameterSet parameter)
        {
            ExecuteNonQuery<SetExchangePropertyCommand, SetExchangePropertyParameterSet>(parameter);
        }




        public List<ExchangeLogDTO> GetExchangeLog(GetExchangeLogParameterSet parameter)
        {
            return ExecuteRead<GetExchangeLogQuery, List<ExchangeLogDTO>, GetExchangeLogParameterSet>(parameter);
        }

        public List<NeighbourEnterpriseExchangeTask> GetTypicalExchangeEnterpriseList()
        {
            var currentEnterpriseId = AppSettingsManager.CurrentEnterpriseId;
            return ExecuteRead<GetTypicalExchangeTaskList, List<NeighbourEnterpriseExchangeTask>, Guid>(currentEnterpriseId);
        }


        public List<EnterpriseDTO> GetEnterpriseExchangeNeighbourList()
        {
            var currentEnterpriseId = AppSettingsManager.CurrentEnterpriseId;
            return ExecuteRead<GetEnterpriseExchangeNeighbourList, List<EnterpriseDTO>, Guid>(currentEnterpriseId);
        }

        public void ImportTypicalExchange(string parameters)
        {
            //var eo = XmlHelper.GetFromString<ExchangeObject<TypicalExchangeData>>(parameters);
            //using (var context = OpenDbContext())
            //{
            //    eo.Sync(context);
            //}
        }
        public string TransformFile(ImportParams parameters)
        {
            try
            {
                var importers = new List<ExchangeImporter> { new AstraExchangeImporter(), new SpecificExchangeImporter() };
                parameters.Text = Regex.Replace(parameters.Text, "\r", "\r\n");

                var importer  = importers.FirstOrDefault(i => i.IsValid(parameters.Task, parameters.FileName));
                Exception e;
                var timeStamp = ((SpecificExchangeImporter)importer).GetTimeStamp(parameters.Task, parameters.FileName, out e);
                parameters.TimeStamp = timeStamp;
                if (importer is AstraExchangeImporter)
                {
                    var eo = ((AstraExchangeImporter) importer).Import<AstraPipeData>(parameters);
                    return XmlHelper.GetString(eo, new XmlWriterSettings() { Indent = true, NewLineChars = "\n" });
                }
                if(importer is SpecificExchangeImporter)
                {
                    var eo = ((SpecificExchangeImporter) importer).Import<ExtData>(parameters);
                    return XmlHelper.GetString(eo, new XmlWriterSettings() { Indent = true, NewLineChars = "\n" });
                }

                return string.Empty;
            }
            catch (Exception e)
            {
                return $"{e.Message}: \n {e.InnerException}";
            }
        }


        public void AsutpImport(ASUTPImportParameterSet parameters)
        {
            ExecuteNonQuery<ASUTPImportCommand, ASUTPImportParameterSet>(parameters);
        }

        public List<AsduPropertyDTO> GetAsduPropertyList(GetAsduEntityListParameterSet parameter)
        {
            return ExecuteRead<GetAsduPropertyListQuery, List<AsduPropertyDTO>, GetAsduEntityListParameterSet>(parameter);
        }

        public void SetAsduProperty(SetAsduPropertyParameterSet parameter)
        {
            ExecuteNonQuery<SetAsduPropertyCommand, SetAsduPropertyParameterSet>(parameter);
        }
        public void SetAsduEntity(SetAsduPropertyParameterSet parameter)
        {
            ExecuteNonQuery<SetAsduEntityCommand, SetAsduPropertyParameterSet>(parameter);
        }
    }

}
