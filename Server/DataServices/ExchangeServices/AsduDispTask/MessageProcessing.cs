using GazRouter.DAL.DispatcherTasks;
using GazRouter.DataServices.Infrastructure.Attributes.Behaviors;
using GazRouter.DataServices.Infrastructure.Services;
using GazRouter.DTO.DispatcherTasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using GazRouter.DAL.Core;
using GazRouter.DAL.DispatcherTasks.Attachments;
using GazRouter.DTO.DispatcherTasks.Attachments;
using GazRouter.DAL.DispatcherTasks.TaskRecords;
using GazRouter.DTO.DispatcherTasks.TaskRecords;
using GazRouter.DAL.ObjectModel.Sites;
using GazRouter.DTO.ObjectModel.Sites;
using GazRouter.DAL.Dictionaries.PhysicalTypes;
using GazRouter.DAL.Dictionaries.PropertyTypes;
using GazRouter.DTO.Dictionaries.PhisicalTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.ObjectModel;
using GazRouter.DataServices.ObjectModel;
using GazRouter.DAL.ObjectModel.Entities;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DAL.ObjectModel.DistrStations;
using GazRouter.DTO.ObjectModel.DistrStations;
using GazRouter.DAL.ObjectModel.Valves;
using GazRouter.DAL.ObjectModel.MeasLine;
using GazRouter.DTO.ObjectModel.Valves;
using GazRouter.DTO.ObjectModel.MeasLine;
using GazRouter.DAL.ObjectModel.CompStations;
using GazRouter.DAL.ObjectModel.CompShops;
using GazRouter.DTO.ObjectModel.CompShops;
using GazRouter.DTO.ObjectModel.CompStations;
using GazRouter.DAL.ObjectModel.CompUnits;
using GazRouter.DTO.ObjectModel.CompUnits;
using GazRouter.DAL.ObjectModel.ReducingStations;
using GazRouter.DTO.ObjectModel.ReducingStations;
using GazRouter.DAL.ObjectModel.Boilers;
using GazRouter.DTO.ObjectModel.Boilers;
using GazRouter.DAL.ObjectModel.MeasStations;
using GazRouter.DAL.ObjectModel.PowerUnits;
using GazRouter.DTO.ObjectModel.MeasStations;
using GazRouter.DTO.ObjectModel.PowerUnits;
using GazRouter.DAL.ObjectModel.DistrStationOutlets;
using GazRouter.DTO.ObjectModel.DistrStationOutlets;
using GazRouter.DAL.ObjectModel.Pipelines;
using GazRouter.DTO.ObjectModel.Pipelines;
using GazRouter.DAL.ObjectModel.MeasPoint;
using GazRouter.DTO.ObjectModel.MeasPoint;
using GazRouter.DAL.ObjectModel.CoolingStations;
using GazRouter.DTO.ObjectModel.CoolingStations;
using GazRouter.DAL.ObjectModel.BoilerPlants;
using GazRouter.DTO.ObjectModel.BoilerPlants;
using GazRouter.DAL.ObjectModel.CoolingUnit;
using GazRouter.DTO.ObjectModel.CoolingUnit;
using GazRouter.DAL.ObjectModel.PowerPlants;
using GazRouter.DTO.ObjectModel.PowerPlants;
using GazRouter.DAL.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.EntityTypeProperties;
using GazRouter.DAL.DispatcherTasks.TasksExchangeLog;
using GazRouter.DTO.DispatcherTasks.TasksExchangeLog;
using GazRouter.DAL.DispatcherTasks.TaskStatuses;
using GazRouter.DTO.DispatcherTasks.TaskStatuses;
using GazRouter.DAL.DispatcherTasks.TaskRecords.AddTaskRecordCPDD;

namespace GazRouter.DataServices.ExchangeServices.AsduDispTask
{
    [ErrorHandlerLogger("exchangeLogger")]
    [Authorization]
    public class MessageProcessing : ServiceBase
    {
        const string soapBodyOpen = "<soap:Body>";
        const string soapBodyClose = "</soap:Body>";
        const string xmlHeader = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
        int DefaultAddDays = 5;

        public string Login = "";

        List<EntityTypeDTO> EnTypes = null;
        List<PhysicalTypeDTO> PhTypes = null;
        List<PropertyTypeDTO> PrTypes = null;

        ExecutionContextReal dbcontext = null;
        public MessageProcessing(string login, string password) : base()
        {
            var c = Infrastructure.Sessions.SessionManager.GetSession(login);

            Login = login;

            dbcontext = DataServices.Infrastructure.DbContextHelper.OpenDbContext(login, _logger);

            PhTypes = ExecuteRead<GetPhysicalTypeListQuery, List<PhysicalTypeDTO>>();

            PrTypes = ExecuteRead<GetPropertyTypeListQuery, List<PropertyTypeDTO>>();

            EnTypes = ExecuteRead<GetEntityTypeListQuery, List<EntityTypeDTO>>();

            List<EntityTypePropertyDTO> etProps = ExecuteRead<GetEntityTypePropertyListQuery, List<EntityTypePropertyDTO>, EntityType?>(null);
            foreach (var et in EnTypes)
            {
                et.EntityProperties.AddRange(
                    etProps.Where(p => p.EntityType == et.EntityType)
                        .Select(p => PrTypes.Single(pt => pt.PropertyType == p.PropertyType)));
            }
        }

        public Guid? GetSiteIdByEntity(Guid entityId)
        {
            return ExecuteRead<GetEntitySiteIdQuery, Guid?, Guid>(entityId);
        }

        public CommonEntityDTO GetEntityById(Guid parameters)
        {
            //using (var context = OpenDbContext())
            {
                var entity = ExecuteRead<GetEntityQuery, CommonEntityDTO, Guid>(parameters);

                switch (entity?.EntityType)
                {
                    case EntityType.DistrStation:
                        return ExecuteRead < GetDistrStationByIdQuery, DistrStationDTO, Guid>(parameters);
                    case EntityType.Valve:
                        return ExecuteRead < GetValveByIdQuery, ValveDTO, Guid>(parameters);
                    case EntityType.MeasLine:
                        return ExecuteRead <GetMeasLineByIdQuery, MeasLineDTO, Guid>(parameters);
                    case EntityType.Site:
                        return ExecuteRead<GetSiteByIdQuery, SiteDTO, Guid>(parameters);
                    case EntityType.CompStation:
                        return ExecuteRead <GetCompStationByIdQuery, CompStationDTO, Guid>(parameters);
                    case EntityType.CompShop:
                        return ExecuteRead <GetCompShopByIdQuery, CompShopDTO, Guid>(parameters);
                    case EntityType.CompUnit:
                        return ExecuteRead <GetCompUnitByIdQuery, CompUnitDTO, Guid>(parameters);
                    case EntityType.ReducingStation:
                        return ExecuteRead <GetReducingStationByIdQuery, ReducingStationDTO, Guid>(parameters);
                    case EntityType.Boiler:
                        return ExecuteRead <GetBoilerByIdQuery, BoilerDTO, Guid>(parameters);
                    case EntityType.MeasStation:
                        return ExecuteRead <GetMeasStationByIdQuery, MeasStationDTO, Guid>(parameters);
                    case EntityType.PowerUnit:
                        return ExecuteRead <GetPowerUnitByIdQuery, PowerUnitDTO, Guid>(parameters);
                    case EntityType.DistrStationOutlet:
                        return ExecuteRead <GetDistrStationOutletByIdQuery, DistrStationOutletDTO, Guid>(parameters);
                    case EntityType.Pipeline:
                        return ExecuteRead <GetPipelineByIdQuery, PipelineDTO, Guid>(parameters);
                    case EntityType.MeasPoint:
                        return ExecuteRead <GetMeasPointByIdQuery, MeasPointDTO, Guid>(parameters);
                    case EntityType.CoolingStation:
                        return ExecuteRead <GetCoolingStationByIdQuery, CoolingStationDTO, Guid>(parameters);
                    case EntityType.BoilerPlant:
                        return ExecuteRead <GetBoilerPlantByIdQuery, BoilerPlantDTO, Guid>(parameters);
                    case EntityType.CoolingUnit:
                        return ExecuteRead <GetCoolingUnitByIdQuery, CoolingUnitDTO, Guid>(parameters);
                    case EntityType.PowerPlant:
                        return ExecuteRead <GetPowerPlantByIdQuery, PowerPlantDTO, Guid>(parameters);
                }

                return entity;
            }
        }

        public DispatcherMessage ParseInput(byte[] bytes)
        {
            string source = "";
            try {

                source = System.Text.Encoding.UTF8.GetString(bytes);

                if (source.Contains(soapBodyOpen) && source.Contains(soapBodyClose))
                {
                    string body = source.Split(new string[] { soapBodyOpen }, StringSplitOptions.None)[1].Split(new string[] { soapBodyClose }, StringSplitOptions.None)[0];
                    XmlDocument doc = new XmlDocument();

                    doc.LoadXml(xmlHeader + body);

                    XmlSerializer serializer = new XmlSerializer(typeof(DispatcherMessage));

                    System.IO.Stream stream = new System.IO.MemoryStream();
                    doc.Save(stream);

                    stream.Position = 0;

                    return (DispatcherMessage)serializer.Deserialize(stream);
                }
            } catch (Exception ex)
            {
                var id = Guid.NewGuid();
                _logger.WriteIntegrationServiceException(id, ex, source);
            }
            return null;
        }

        protected override ExecutionContextReal OpenDbContext()
        {
            return DataServices.Infrastructure.DbContextHelper.OpenDbContext(Login, _logger);
        }

        private List<SiteDTO> SiteList()
        {
            return ExecuteRead<GetSiteListQuery, List<SiteDTO>, GetSiteListParameterSet>(null); 
        }

        public void AddAttachment(string data, string description, string filename, Guid guid)
        {
            AddAttachment(Convert.FromBase64String(data), description, filename, guid);
                                        //message.Attachment.Content.GetType() == typeof(XmlNode[]) ?
                                        //((XmlNode[])message.Attachment.Content)[0].InnerText : message.Attachment.Content.ToString()))
        }

        public void AddAttachment(byte[] data, string description, string filename, Guid guid)
        {
            ExecuteRead<AddTaskAttachmentCommand, Guid, AddTaskAttachmentParameterSet>(// dts.AddFileAttachment(
                                    new DTO.DispatcherTasks.Attachments.AddTaskAttachmentParameterSet()
                                    {
                                        Data = data,
                                        Description = description,
                                        FileName = filename,
                                        TaskId = guid,
                                    });
        }
        public void ParseMessage(DispatcherMessage message, byte[] source = null)
        {
            if (message != null)
            {
                try
                {
                    #region clean up CPDD tasks, remove later
                    //List<TaskDTO> list = ExecuteRead<GetTaskListQuery, List<TaskDTO>>();
                    //foreach (TaskDTO t in list)
                    //{
                    //    if (t.GlobalTaskId != null && t.GlobalTaskId != "")
                    //    {
                    //        ExecuteNonQuery<DeleteTaskCommand, Guid>(t.Id);
                    //    }
                    //}
                    #endregion

                    // проверка не приходило ли еще такое задание
                    var res = new GetTaskByGlobalTaskId(dbcontext).Execute(message.HeaderSection.Identifier.Value);

                    Guid guidCpddTask;
                    if (res == null)
                    {
                        #region новое сообщение

                        guidCpddTask = ExecuteRead<AddTaskCpddCommand, Guid, AddTaskCpddParameterSet>(new DTO.DispatcherTasks.AddTaskCpddParameterSet()
                        {
                            Description = message.HeaderSection.Comment,
                            GlobalTaskId = message.HeaderSection.Identifier.Value,
                            Subject = message.HeaderSection.Subject,
                            UserNameCpdd = message.HeaderSection.Author,
                            IsAproved = false
                        });

                        if (source != null) AddAttachment(source, "входящее сообщение", "incoming.xml", guidCpddTask);

                        if (message.Attachment != null && message.Attachment.Content != null)
                            AddAttachment(
                                message.Attachment.Content.GetType() == typeof(XmlNode[]) ?
                                  ((XmlNode[])message.Attachment.Content)[0].InnerText : message.Attachment.Content.ToString(),
                                message.Attachment.Comment,
                                message.Attachment.Filename,
                                guidCpddTask
                            );
                        #region
                        //ExecuteRead<AddTaskAttachmentCommand, Guid, AddTaskAttachmentParameterSet>(// dts.AddFileAttachment(
                        //    new DTO.DispatcherTasks.Attachments.AddTaskAttachmentParameterSet()
                        //    {
                        //        Data = Convert.FromBase64String(
                        //        message.Attachment.Content.GetType() == typeof(XmlNode[]) ?
                        //        ((XmlNode[])message.Attachment.Content)[0].InnerText : message.Attachment.Content.ToString()),
                        //        Description = message.Attachment.Comment,
                        //        FileName = message.Attachment.Filename,
                        //        TaskId = guidCpddTask,
                        //                //UserCpddName = message.HeaderSection.Author

                        //            });
                        #endregion

                        string recId = message.HeaderSection.Receiver.id.ToString();

                        if (message.DataSection != null && message.DataSection.Length > 0)
                        {
                            foreach (DataSection ds in message.DataSection)
                            {
                                var ent = GetEntityById(Guid.Parse(ds.Identifier.Value));
                                var entity = EnTypes.Where(et => et.EntityType == ent.EntityType).ToList();
                                PropertyType pt = PropertyType.None;

                                if (entity != null)
                                {
                                    List<PropertyTypeDTO> properties = entity[0].EntityProperties.Where(p => p.ShortName == ds.Dimension || p.SysName == ds.Dimension || p.Name == ds.Dimension || p.PhysicalType.UnitName == ds.Dimension).ToList();

                                    if (properties != null && properties.Count == 1)
                                        pt = properties[0].PropertyType;
                                }

                                Guid? siteId = GetSiteIdByEntity(ent.Id);

                                /*
                                Guid guidTaskRecord = ExecuteRead<AddTaskRecordPDSCommand, Guid, AddTaskRecordPdsParameterSet>(new AddTaskRecordPdsParameterSet
                                {
                                    TaskId = guidCpddTask,
                                    EntityId = Guid.Parse(ds.Identifier.Value),
                                    PropertyTypeId = pt,
                                    PeriodTypeId = Converters.PeriodTypes(ds.Scale),
                                    TargetValue = ds.Value,
                                    Description = ds.Comment,
                                    CompletionDate = ds.DeadlineSpecified ? ds.Deadline : DateTime.Today.AddDays(DefaultAddDays),
                                    //UserNameCpdd = message.HeaderSection.Author,

                                    //SiteId = siteId.HasValue ? siteId.Value : Guid.Empty
                                });
                                */
                                
                                Guid guidTaskRecord = ExecuteRead<AddTaskRecordCPDDCommand, Guid, AddTaskRecordCpddParameterSet>(new AddTaskRecordCpddParameterSet
                                {
                                    TaskId = guidCpddTask,
                                    EntityId = Guid.Parse(ds.Identifier.Value),
                                    PropertyTypeId = pt,
                                    PeriodTypeId = Converters.PeriodTypes(ds.Scale),
                                    TargetValue = ds.Value,
                                    Description = ds.Comment,
                                    CompletionDate = ds.DeadlineSpecified ? ds.Deadline : DateTime.Today.AddDays(DefaultAddDays),
                                    UserNameCpdd = message.HeaderSection.Author,
                                    
                                    //SiteId = siteId.HasValue ? siteId.Value : Guid.Empty
                                });
                                
                                if (ds.SpecialInterestFlagSpecified && ds.SpecialInterestFlag == boolean.Item1) ExecuteNonQuery<SetTaskRecordAckCommand, Guid>(guidTaskRecord);

                                
                            }
                        }

                        #endregion
                    }
                    else
                    {
                        #region уже приходившее сообщение

                        guidCpddTask = res.Id;

                        if (source != null) AddAttachment(source, "входящее сообщение", "incoming.xml", guidCpddTask);

                        if (message.Attachment != null && message.Attachment.Content != null)
                            AddAttachment(
                                message.Attachment.Content.GetType() == typeof(XmlNode[]) ?
                                  ((XmlNode[])message.Attachment.Content)[0].InnerText : message.Attachment.Content.ToString(),
                                message.Attachment.Comment,
                                message.Attachment.Filename,
                                guidCpddTask
                            );


                        List<TaskRecordDTO> records = ExecuteRead<GetTaskRecordsListQuery, List<TaskRecordDTO>, GetTaskRecordsCpddParameterSet>(new GetTaskRecordsCpddParameterSet
                        {
                            IsCpdd = true,
                            TaskVersionId = res.LastVersionId
                        });

                        if (message.DataSection != null && message.DataSection.Length > 0)
                        {
                            List<TaskRecordDTO> todel = new List<TaskRecordDTO>();

                            foreach (DataSection ds in message.DataSection)
                            {
                                var ent = GetEntityById(Guid.Parse(ds.Identifier.Value));
                                var entity = EnTypes.Where(et => et.EntityType == ent.EntityType).ToList();
                                PropertyType pt = PropertyType.None;

                                if (entity != null)
                                {
                                    List<PropertyTypeDTO> properties = entity[0].EntityProperties.Where(p => p.ShortName == ds.Dimension || p.SysName == ds.Dimension || p.Name == ds.Dimension || p.PhysicalType.UnitName == ds.Dimension).ToList();

                                    if (properties != null && properties.Count == 1)
                                        pt = properties[0].PropertyType;
                                }

                                Guid? siteId = GetSiteIdByEntity(ent.Id);

                                try
                                {
                                    TaskRecordDTO trdto = records.Single(o => o.Entity.Id == ent.Id && o.PropertyTypeId == pt);

                                    ExecuteNonQuery<EditTaskRecordCPDDCommand, EditTaskRecordCpddParameterSet>(new EditTaskRecordCpddParameterSet
                                    {
                                        TaskId = guidCpddTask,
                                        EntityId = Guid.Parse(ds.Identifier.Value),
                                        PropertyTypeId = pt,
                                        PeriodTypeId = Converters.PeriodTypes(ds.Scale),
                                        TargetValue = ds.Value,
                                        Description = ds.Comment,
                                        CompletionDate = ds.DeadlineSpecified ? ds.Deadline : DateTime.Today.AddDays(DefaultAddDays),
                                        UserNameCpdd = message.HeaderSection.Author,
                                    });
                                    /*
                                    ExecuteNonQuery<EditTaskRecordCPDDCommand, EditTaskRecordCpddParameterSet>(new EditTaskRecordCpddParameterSet
                                    {
                                        TaskId = guidCpddTask,
                                        EntityId = Guid.Parse(ds.Identifier.Value),
                                        PropertyTypeId = pt,
                                        PeriodTypeId = Converters.PeriodTypes(ds.Scale),
                                        TargetValue = ds.Value,
                                        Description = ds.Comment,
                                        CompletionDate = ds.DeadlineSpecified ? ds.Deadline : DateTime.Today.AddDays(DefaultAddDays),
                                        UserNameCpdd = message.HeaderSection.Author,
                                    });
                                    */

                                    records.Remove(trdto);
                                    //exeq

                                    //Guid guidTaskRecord = ExecuteRead<AddTaskRecordPDSCommand, Guid, AddTaskRecordPdsParameterSet>(new AddTaskRecordPdsParameterSet
                                    //{
                                    //    TaskId = guidCpddTask,
                                    //    EntityId = Guid.Parse(ds.Identifier.Value),
                                    //    PropertyTypeId = pt,
                                    //    PeriodTypeId = Converters.PeriodTypes(ds.Scale),
                                    //    TargetValue = ds.Value,
                                    //    Description = ds.Comment,
                                    //    CompletionDate = ds.DeadlineSpecified ? ds.Deadline : DateTime.Today.AddDays(DefaultAddDays),
                                    //    SiteId = siteId.HasValue ? siteId.Value : Guid.Empty
                                    //});

                                    //if (ds.SpecialInterestFlagSpecified && ds.SpecialInterestFlag == boolean.Item1) ExecuteNonQuery<SetTaskRecordAckCommand, Guid>(guidTaskRecord);

                                    //ExecuteRead<AddTaskExchangeLogCommand, Guid, AddTaskExchangeLogParameterSet>(new AddTaskExchangeLogParameterSet
                                    //{
                                    //    ExchangeDate = DateTime.Now,
                                    //    ExchangeStatus = message.HeaderSection.Status.ToString(),
                                    //    GlobalTaskID = message.HeaderSection.Identifier.Value,
                                    //    TaskId = guidCpddTask
                                    //});
                                } catch (Exception e)
                                {

                                }
                            }
                            if (todel.Count > 0)
                            {
                                foreach (var del in todel)
                                {
                                    ExecuteNonQuery<RemoveTaskRecordCommand, Guid>(del.Id);
                                }
                            }
                        }


                        #endregion
                    }

                    if (guidCpddTask != null && guidCpddTask != Guid.Empty)
                    {
                        ExecuteRead<AddTaskExchangeLogCommand, Guid, AddTaskExchangeLogParameterSet>(new AddTaskExchangeLogParameterSet
                        {
                            ExchangeDate = DateTime.Now,
                            ExchangeStatus = message.HeaderSection.Status.ToString(),
                            GlobalTaskID = message.HeaderSection.Identifier.Value,
                            TaskId = guidCpddTask
                        });

                        switch (message.HeaderSection.Status)
                        {
                            ///совещательное
                            case status.Consultative:
                                ExecuteRead<TaskCorrectingCommand, Guid, SetTaskStatusParameterSet>(new SetTaskStatusParameterSet
                                {
                                    TaskId = guidCpddTask
                                });
                                break;
                            ///совещательное (получено)
                            case status.Consultative_Received: break;
                            ///скорректированное
                            case status.Corrected:

                                break;
                            ///согласованное
                            case status.Agreed:
                                break;
                            ///утвержденное
                            case status.Confirmed:
                                ExecuteRead<TaskApprovedCPDDCommand, Guid, SetTaskStatusParameterSet>(new SetTaskStatusParameterSet
                                {
                                    TaskId = guidCpddTask,
                                    UserNameCpdd = message.HeaderSection.Author
                                });
                                break;
                            ///утвержденное(получено)
                            case status.Confirmed_Received: break;
                            ///таймаут
                            case status.Timeout:
                                break;
                            ///исполнение опознано
                            case status.Fulfilled_Recognized: break;
                            ///исполнение подверждено диспетчером
                            case status.Fulfilled_DO: break;
                            ///подтвержденно на уровне администрации
                            case status.Fulfilled_Final: break;
                            ///просрочено
                            case status.Fulfilled_DO_Expired: break;
                            ///выполнено (просрочено)
                            case status.Fulfilled_Expired: break;
                            ///аннулировано
                            case status.Canceled:
                                ExecuteRead<TaskAnnuledCommand, Guid, SetTaskStatusParameterSet>(new SetTaskStatusParameterSet
                                {
                                    AnnuledReason = DTO.Dictionaries.AnnuledReasons.AnnuledReason.CancelCpdd,
                                    TaskId = guidCpddTask,
                                    StatusType = DTO.Dictionaries.StatusTypes.StatusType.Annuled,
                                    UserNameCpdd = message.HeaderSection.Author
                                });
                                break;
                            ///нарушено
                            case status.Keep_Violation_Recognized: break;
                            ///завершено
                            case status.Keep_Completed: break;

                            default: break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    var id = Guid.NewGuid();
                    _logger.WriteIntegrationServiceException(id, ex, "CPDD message processing error");
                }
            }
        }
    }
}