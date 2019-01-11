using System.Threading;
using Telerik.Windows.Controls;

namespace GazRouter.Application.Localization
{
    public sealed class RusLocalizationManager : LocalizationManager
    {
        
        #region Overrides of LocalizationManager

        public override string GetStringOverride(string key)
        {
            string value = ResourceManager.GetString(key, Thread.CurrentThread.CurrentCulture);
            if (value != null)
                return value;
            value = base.GetStringOverride(key);
           // Debugger.Break(); //не локализованная строка. добавьте key в TelerikResources.resx
            return value;
        }

        #endregion
    }
}