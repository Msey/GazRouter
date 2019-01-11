using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace DataExchange.Integro.Summary
{
    public class IntegroExchangeDataSection
    {
        public string Identifier { get; set; }

        public string Value { get; set; }

        public string Dimension { get; set; }

        public string ParameterFullName { get; set; }

        public string ObjectIUSPTP{ get; set; }

        public string ParameterIUSPTP { get; set; }
    }
}
