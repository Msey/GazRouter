using System;
using System.Collections.Generic;
using GazRouter.DTO.Dictionaries.InconsistencyTypes;

namespace GazRouter.DataServices.BL
{
    public class RuleProcessor
    {
        public List<RuleBase> Rules { get; } = new List<RuleBase>();

        public Dictionary<InconsistencyType, List<Guid>> Run()
        {
            var inconsistencies = new Dictionary<InconsistencyType, List<Guid>>();
            foreach (var rule in Rules)
            {
                var inconsistEntityIds = rule.Validate();
                if (inconsistEntityIds.Count > 0)
                {
                    inconsistencies.Add(rule.InconsistencyType, inconsistEntityIds);
                }
            }
            return inconsistencies;
        }
    }
}