using System;
using System.Linq;
using GazRouter.Common.Cache;
using GazRouter.Common.ViewModel;
using GazRouter.DTO.Authorization.User;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PhisicalTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using Microsoft.Practices.ServiceLocation;
using Utils.Units;

namespace GazRouter.Application
{
    public class UserProfile : PropertyChangedBase
    {
        private static readonly Lazy<UserProfile> l = new Lazy<UserProfile>(() => new UserProfile());

        private int _id;
        private string _userName;
        private string _login;
        private string _description;
        private UserSettings _userSettings;
        private EntityType? _entityType;

        private Site _site;


        public static UserProfile Current => l.Value;
        private static IClientCache ClientCache => ServiceLocator.Current.GetInstance<IClientCache>();
        

        #region UNIT CONVERSIONS

        public static double ToUserUnits(double val, PropertyType pt)
        {
            var propType = ClientCache.DictionaryRepository.PropertyTypes.Single(t => t.PropertyType == pt);
            return ToUserUnits(val, propType.PhysicalTypeId);
        }

        public static double ToUserUnits(double val, PhysicalType pt)
        {
            switch (pt)
            {
                case PhysicalType.Pressure:
                    return Pressure.FromKgh(val).As(Current.UserSettings.PressureUnit);

                case PhysicalType.Temperature:
                    return Temperature.FromCelsius(val).As(Current.UserSettings.TemperatureUnit);

                case PhysicalType.CombustionHeat:
                    return CombustionHeat.FromKCal(val).As(Current.UserSettings.CombHeatUnit);

                default:
                    return val;
            }
        }


        public static double ToServerUnits(double val, PropertyType pt)
        {
            var propType = ClientCache.DictionaryRepository.PropertyTypes.Single(t => t.PropertyType == pt);
            return ToServerUnits(val, propType.PhysicalTypeId);
        }

        public static double ToServerUnits(double val, PhysicalType pt)
        {
            switch (pt)
            {
                case PhysicalType.Pressure:
                    return Pressure.From(val, Current.UserSettings.PressureUnit).As(PressureUnit.Kgh);

                case PhysicalType.Temperature:
                    return Temperature.From(val, Current.UserSettings.TemperatureUnit).As(TemperatureUnit.Celsius);

                case PhysicalType.CombustionHeat:
                    return CombustionHeat.From(val, Current.UserSettings.CombHeatUnit).As(CombustionHeatUnit.kcal);

                default:
                    return val;
            }
        }
        #endregion

        public static string UserUnitName(PropertyType pt)
        {
            var propType = ClientCache.DictionaryRepository.PropertyTypes.FirstOrDefault(t => t.PropertyType == pt);
            return propType != null ?  UserUnitName(propType.PhysicalType.PhysicalType) : string.Empty;
        }

        public static string UserUnitName(PhysicalType pt)
        {
            switch (pt)
            {
                case PhysicalType.Pressure:
                    return Pressure.GetAbbreviation(Current.UserSettings.PressureUnit);

                case PhysicalType.Temperature:
                    return Temperature.GetAbbreviation(Current.UserSettings.TemperatureUnit);

                case PhysicalType.CombustionHeat:
                    return CombustionHeat.GetAbbreviation(Current.UserSettings.CombHeatUnit);

                default:
                    return
                        ClientCache.DictionaryRepository.PhisicalTypes.SingleOrDefault(t => t.PhysicalType == pt)?
                            .UnitName ?? string.Empty;
            }
        }


        public int Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        public string UserName
        {
            get { return _userName; }
            private set
            {
                if (SetProperty(ref _userName, value))
                    OnPropertyChanged(() => FullName);
            }
        }

	    public string FullName => $"{UserName} ({Site?.Name ?? string.Empty})";
        


        public string Login
        {
            get { return _login; }
            private set { SetProperty(ref _login, value); }
        }

        public string Description
        {
            get { return _description; }
            private set { SetProperty(ref _description, value); }
        }

        public UserSettings UserSettings
        {
            get { return _userSettings; }
            set
            {
                SetProperty(ref _userSettings, value);
                OnPropertyChanged(() => UserSettings);
            }
        }

        public EntityType? EntityType
        {
            get { return _entityType; }
            private set
            {
                _entityType = value;
                OnPropertyChanged(() => EntityType);
            }
        }


        public void SetProfile(UserDTO userDTO)
        {
            Current.Id = userDTO.Id;
            Current.UserName = userDTO.UserName;
            Current.Login = userDTO.Login;
            Current.Description = userDTO.Description;
            Current.UserSettings = userDTO.UserSettings;
            Current.EntityType = userDTO.EntityType;

            if (userDTO.SiteId.HasValue)
            {
                Current.Site = new Site
                {
                    Id = userDTO.SiteId.Value,
                    Name = userDTO.SiteName,
                    IsEnterprise = !Convert.ToBoolean(userDTO.SiteLevel)
                };
            }
            else
            {
                Current.Site = null;
            }
        }


        public Site Site
        {
            get
            {
                return _site;
            }
            set
            {
                _site = value;
                OnPropertyChanged(() => Site);
                OnPropertyChanged(() => FullName);
            }
        }

    }


    public class Site : PropertyChangedBase
    {
        public Guid Id { get; set; }
        public string Name { get; set; }


        private bool _isEnterprise;
        public bool IsEnterprise
        {
            get { return _isEnterprise; }
            set { SetProperty(ref _isEnterprise, value); }
        }
    }
}