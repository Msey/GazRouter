using System;
using System.Globalization;
using System.Windows.Data;
using GazRouter.DTO.Authorization.User;
namespace GazRouter.ActionsRolesUsers.Dialog.AddUserDialog
{
    public class AdUserConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var a = (AdUserDTO)value;
            return a == null ? "Выберите логин..." : $"{a.DisplayName}, {a.Login}";
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
