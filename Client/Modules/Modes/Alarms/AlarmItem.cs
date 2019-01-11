using System;
using System.Linq;
using GazRouter.Application;
using GazRouter.Common.Cache;
using GazRouter.DTO;
using GazRouter.DTO.Alarms;
using GazRouter.DTO.Dictionaries.AlarmTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PhisicalTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using Microsoft.Practices.ServiceLocation;


namespace GazRouter.Modes.Alarms
{
    public class AlarmItem
    {
        private readonly AlarmDTO _alarmDto;


        public AlarmItem()
        {
            _alarmDto = new AlarmDTO();
        }

        public AlarmItem(AlarmDTO alarmDto)
        {
            _alarmDto = alarmDto;
        }


        public AlarmDTO Dto
        {
            get { return _alarmDto; }
        }


        /// <summary>
        /// Идентификатор уставки
        /// </summary>
        public int Id
        {
            get { return _alarmDto.Id; }
        }

        /// <summary>
        /// Наименование сущности
        /// </summary>
        public string EntityName
        {
            get { return _alarmDto.EntityName; }
        }

        /// <summary>
        /// Типа свойства
        /// </summary>
        public PropertyTypeDTO PropertyType
        {
            get
            {
                return
                    ClientCache.DictionaryRepository.PropertyTypes.Single(
                        p => p.PropertyType == _alarmDto.PropertyTypeId);
            }
        }
        private static IClientCache ClientCache => ServiceLocator.Current.GetInstance<IClientCache>();
        public PeriodTypeDTO PeriodType
        {
            get
            {
                return
                    ClientCache.DictionaryRepository.PeriodTypes.Single(pt => pt.PeriodType == _alarmDto.PeriodTypeId);
            }
        }
        

        /// <summary>
        /// Наименование типа уставки
        /// </summary>
        public string AlarmTypeName 
        {
            get
            {
                return ClientCache.DictionaryRepository.AlarmTypes.Single(a => a.AlarmType == _alarmDto.AlarmTypeId).Name;
            }
        }


        public string AlarmTypeImage
        {
            get
            {
                var urlBase = "/Common;component/Images/10x10/";

                switch (
                    ClientCache.DictionaryRepository.AlarmTypes.Single(a => a.AlarmType == _alarmDto.AlarmTypeId)
                        .AlarmType)
                {
                    case AlarmType.UpperLimit:
                        return urlBase + "limit_upper.png";

                    case AlarmType.LowerLimit:
                        return urlBase + "limit_lower.png";

                    case AlarmType.Equality:
                        return urlBase + "equality.png";

                    default:
                        return "";
                }
            }
        }


        /// <summary>
        /// Значение уставки
        /// </summary>
        public double Setting
        {
            get
            {
                return Math.Round(UserProfile.ToUserUnits(_alarmDto.Setting, PropertyType.PropertyType),
                    PropertyType.PhysicalType.DefaultPrecision);
            }
        }

        /// <summary>
        /// Наименование единиц измерения
        /// </summary>
        public string UnitsName => UserProfile.UserUnitName(PropertyType.PropertyType);

        /// <summary>
        /// Дата начала действия уставки
        /// </summary>
        public DateTime ActivationDate
        {
            get { return _alarmDto.ActivationDate; }
        }

        /// <summary>
        /// Дата окончания действия уставки
        /// </summary>
        public DateTime ExpirationDate
        {
            get { return _alarmDto.ExpirationDate; }
        }


        public bool IsNotExpired
        {
            get { return _alarmDto.ExpirationDate > DateTime.Now; }
        }


        /// <summary>
        /// Описание уставки
        /// </summary>
        public string Description
        {
            get { return _alarmDto.Description; }
        }

        /// <summary>
        /// Имя пользователя создавшего уставку
        /// </summary>
        public string UserName
        {
            get { return _alarmDto.UserName; }
        }


        /// <summary>
        /// Дата создания уставки
        /// </summary>
        public DateTime CreationDate
        {
            get { return _alarmDto.CreationDate; }
        }

        /// <summary>
        /// Уставка находиться в состоянии тревоги (произошло срабатывание)
        /// </summary>
        public bool IsActive
        {
            get { return _alarmDto.Status == 1; }
        }



    }

}