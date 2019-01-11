using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Application;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.SeriesData;
using GazRouter.DTO.SeriesData.ValueMessages;
using Microsoft.Practices.Prism.Commands;

namespace GazRouter.ManualInput.Hourly.ObjectForms
{
    public class ManualInputPropertyValue : ValidationViewModel
    {
        public ManualInputPropertyValue()
        {
            _messageList = new List<PropertyValueMessageDTO>();
            AcceptCommand = new DelegateCommand(OnAcceptCommand);
        }

        private double? _value;
        private List<PropertyValueMessageDTO> _messageList;
        
        public double? Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                OnPropertyChanged(() => Value);
                ValidateProperty("DisplayCurrentValue");
            }
        }

        /// <summary>
        /// Список сообщений
        /// </summary>
        public List<PropertyValueMessageDTO> MessageList
        {
            get { return _messageList; }
            set
            {
                _messageList = value;
                OnPropertyChanged(() => ErrorList);
                OnPropertyChanged(() => ErrorStatus);
                OnPropertyChanged(() => AlarmList);
                OnPropertyChanged(() => AlarmStatus);
            }
        }


        /// <summary>
        /// Список ошибок
        /// </summary>
        public List<PropertyValueMessageDTO> ErrorList
        {
            get { return _messageList.Where(m => m.MessageType == PropertyValueMessageType.Error).ToList(); }
        }

        /// <summary>
        /// Значение параметра содержит ошибки
        /// </summary>
        public bool ErrorStatus => ErrorList.Count > 0;

        /// <summary>
        /// Список тревог
        /// </summary>
        public List<PropertyValueMessageDTO> AlarmList
        {
            get { return _messageList.Where(m => m.MessageType == PropertyValueMessageType.Alarm).ToList(); }
        }


        /// <summary>
        /// Значение параметра содержит тревоги
        /// </summary>
        public bool AlarmStatus => ErrorList.Count == 0 && AlarmList.Count > 0;

        public PropertyValidation<ValidationViewModel> GetPropertyValidation()
        {
            return AddValidationFor(() => Value);
        }


        public DelegateCommand AcceptCommand { get; set; }


        private async void OnAcceptCommand()
        {
            var ackDate = DateTime.Now;
            foreach (var alarm in AlarmList)
            {
                await new SeriesDataServiceProxy().AcceptMessageAsync(alarm.Id);
                alarm.AckDate = ackDate;
                alarm.AckUserName = UserProfile.Current.UserName;
                alarm.AckUserSite = UserProfile.Current.Site.Name;
            }
            
            OnPropertyChanged(() => AlarmList);
        }
    }
}