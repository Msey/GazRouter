using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazRouter.DataServices.Infrastructure
{
    public class SettingDictionary
    {
        public NameValueCollection Dictionary { get; private set; }

        public SettingDictionary(string sectionName)
        {
            Dictionary = (NameValueCollection)ConfigurationManager.GetSection(sectionName);
            if (Dictionary == null) throw new Exception($"Секция конфигурации {sectionName} не найдена.");
        }

        public string Recode(string key, bool useKeyIfNull = true)
        {
            return Dictionary.GetValues(key)?.FirstOrDefault() ?? (useKeyIfNull ? key : null);
        }
    }
}
