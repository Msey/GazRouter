using System;
using System.Text.RegularExpressions;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.Log;

namespace GazRouter.Service.Exchange.Lib.Import
{
    [Serializable]
    public class LoaderConfig
    {
        public string FileNameMask { get; set; }
        public string XslFileName { get; set; }
        public string Separator { get; set; }

        public string DateTimeFormat { get; set; }

        public PeriodType PeriodType { get; set; }

        private int _firstIndex;
        private int _lastIndex;
        private readonly MyLogger _logger = new MyLogger("dataLoader");

        public bool CheckMask(string fileName)
        {
            var pattern = FileNameMask;
            _firstIndex = FileNameMask.IndexOf('#');
            _lastIndex = FileNameMask.LastIndexOf('#');
            if (_firstIndex != -1 && _lastIndex != -1)
            {
                DateTimeFormat = FileNameMask.Substring(_firstIndex + 1, _lastIndex - _firstIndex - 1 );
                var replaceDateMask = DateTimeFormat
                    .Replace('h', '.').Replace('H', '.')
                    .Replace('d', '.').Replace('D', '.')
                    .Replace('m', '.').Replace('M', '.')
                    .Replace('y', '.').Replace('Y', '.');
                pattern = FileNameMask
                    .Replace("#", string.Empty)
                    .Replace(DateTimeFormat, replaceDateMask);
            }
            return Regex.IsMatch(fileName, pattern);
        }




    }
}
