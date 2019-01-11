using System;
using GazRouter.DTO.Dictionaries.PeriodTypes;

namespace GazRouter.DTO.DataExchange.Transformation
{
    public class TransformFileParameterSet
    {
        public string FileName { get; set; }
        public string Transformation { get; set; }
        public string InputContent { get; set; }
    }
}