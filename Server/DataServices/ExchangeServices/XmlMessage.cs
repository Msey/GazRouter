using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.DTO.Authorization.User;
using GazRouter.DTO.Dictionaries.StatusTypes;
using GazRouter.DTO.DispatcherTasks;
using GazRouter.DTO.DispatcherTasks.TaskRecords;
using GazRouter.DTO.DispatcherTasks.Tasks;

namespace GazRouter.DataServices.ExchangeServices
{
    public partial class XmlMessage
    {
        private string GetAttributeField(Key key)
        {
            return DTMessage.Attributes.GetAttributeValue(key);
        }

        public MessageStatus Status
        {
            get { return (MessageStatus) Enum.Parse(typeof (MessageStatus), GetAttributeField(Key.Status), true); }
        }

        public string Subject
        {
            get { return GetAttributeField(Key.Subject); }
        }

        public string UserNameCpdd
        {
            get { return GetAttributeField(Key.UserNameCpdd); }
        }

        public string Description
        {
            get { return GetAttributeField(Key.Description); }
        }


        public IEnumerable<TaskRecord> GetTaskRecords()
        {
            var result = from c in DTMessage.Attributes
                         where c.instanceSpecified
                          group c by c.instance
                          into g
                              let prm = new TaskRecord
                              {
                                  TargetValue = g.GetAttributeValue(Key.Parameter),
                                  Description = g.GetAttributeValue(Key.RecordDescription),
                                  CompletionDate = g.GetAttributeDate(Key.CompletionDate),
                                  OrderNo = g.GetAttributeInt(Key.RecordOrder),
                                  UserNameCPDD = UserNameCpdd,
                                  ExtKey = g.GetAttributeValue(Key.ExtKey),
                              }
                          select prm;
            return result.ToList();
        }

        public string GlobalTaskId
        {
            get { return DTMessage.Header.GlobalId; }
        }

        public static XmlMessage Create(UserDTO user, TaskDTO task, IEnumerable<TaskRecordDTO> records)
        {
            var message = new XmlMessage
                {
                    DTMessage = new XmlMessageDTMessage
                        {
                            Header = new XmlMessageDTMessageHeader
                                {
                                    GlobalId = task.GlobalTaskId,
                                    SendDate = task.StatusSetDate,
                                    Sender = user.SiteName,
                                    Receiver = "CPDD",SendDateSpecified = true
                                },
                            Attributes =
                                new List<XmlMessageDTMessageAttribute>
                                    {
                                        new XmlMessageDTMessageAttribute{key = (int) Key.Status, Value = Enum.GetName(typeof(MessageStatus), task.StatusType.ToXmlStatuses())},
                                        new XmlMessageDTMessageAttribute{key = (int) Key.Subject, Value = task.Subject},
                                        new XmlMessageDTMessageAttribute{key = (int) Key.UserNameCpdd, Value = user.UserName},
                                        new XmlMessageDTMessageAttribute{key = (int) Key.Description, Value = task.Description}
                                    }.Union(GetTaskRecordAttributes(records)).ToArray()
                        }
                };
            return message;
        }

        private static IEnumerable<XmlMessageDTMessageAttribute> GetTaskRecordAttributes(IEnumerable<TaskRecordDTO> records)
        {
            sbyte instance = 0;
            foreach (var r in records)
            {
				yield return new XmlMessageDTMessageAttribute { key = 7019, instance = instance, instanceSpecified = true, Value = string.Empty };
				yield return new XmlMessageDTMessageAttribute { key = (int)Key.Parameter, instance = instance, instanceSpecified = true, Value = r.TargetValue };
				yield return new XmlMessageDTMessageAttribute { key = (int)Key.ExtKey, instance = instance, instanceSpecified = true, Value = r.ExtKey };
				yield return new XmlMessageDTMessageAttribute { key = (int)Key.IsSpecialControl, instance = instance, instanceSpecified = true, Value = r.IsSpecialControl.ToString() };
				yield return new XmlMessageDTMessageAttribute { key = (int)Key.CompletionDate, instance = instance, instanceSpecified = true, Value = r.CompletionDate.Value.ToString("s") };
				yield return new XmlMessageDTMessageAttribute { key = (int)Key.RecordOrder, instance = instance, instanceSpecified = true, Value = r.OrderNo.ToString() };
				yield return new XmlMessageDTMessageAttribute { key = (int)Key.RecordDescription, instance = instance, instanceSpecified = true, Value = r.Description };
                instance++;
            }
        }

    }

    public class TaskRecord
    {
        public string ExtKey { get; set; }
        public string TargetValue { get; set; }
        public string Description { get; set; }
        public DateTime CompletionDate { get; set; }
        public int OrderNo { get; set; }
        public string UserNameCPDD { get; set; }
    }

    public enum MessageStatus
    {
        Consultative,
        Confirmed,
        Consultative_received,
        Confirmed_received,
        Corrected,
        Agreed,
        Fullfilled
    }

    public enum Key
    {
        Status = 1,
        Subject = 2,
        UserNameCpdd = 3,
        Description = 7000,
        RecordDescription = 7023,
        RecordOrder = 7030,
        TargetValue = 7018,
        Parameter = 7020,
        IsSpecialControl = 7026,
        CompletionDate = 7025,
        ExtKey = 7017
    }

    public static class StatusTypesEnumExtensions
    {
        public static MessageStatus ToXmlStatuses(this StatusType statusType)
        {
            switch (statusType)
            {
                case StatusType.OnSubmit:
                    return MessageStatus.Confirmed;
                case StatusType.Submitted:
                    return MessageStatus.Agreed;
                case StatusType.Corrected:
                    return MessageStatus.Corrected;
                case StatusType.ApprovedByCpdd:
                    return MessageStatus.Agreed;
                case StatusType.Created:
                    return MessageStatus.Consultative;
                case StatusType.ApprovedForSite:
                    return MessageStatus.Confirmed_received;
                case StatusType.Performed:
                    return MessageStatus.Fullfilled;
                case StatusType.Correcting:
                    return MessageStatus.Consultative;
                default:
                    throw new ArgumentOutOfRangeException("statusType");
            }
        }

        public static string GetAttributeValue(this IEnumerable<XmlMessageDTMessageAttribute> src, Key key)
        {
            return src.Where(a => a.key == (int)key).Select(a => a.Value).SingleOrDefault();
        }
        
        public static IEnumerable<string> GetAttributeValueCollection(this IEnumerable<XmlMessageDTMessageAttribute> src, Key key)
        {
            return src.Where(a => a.key == (int)key).Select(a => a.Value);
        }
        public static DateTime GetAttributeDate(this IEnumerable<XmlMessageDTMessageAttribute> src, Key key)
        {
            var raw = src.Where(a => a.key == (int)key).Select(a => a.Value).SingleOrDefault();
            return Convert.ToDateTime(raw);
        }
        public static int GetAttributeInt(this IEnumerable<XmlMessageDTMessageAttribute> src, Key key)
        {
            var raw = src.Where(a => a.key == (int)key).Select(a => a.Value).SingleOrDefault();
            return Convert.ToInt32(raw);
        }
    }
}