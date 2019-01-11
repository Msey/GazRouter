using System;
using System.Collections.Generic;
using GazRouter.DTO.Dictionaries;
using GazRouter.DTO.Dictionaries.InconsistencyTypes;

namespace GazRouter.DataServices.BL
{
    public abstract class RuleBase
    {
        protected RuleBase(DictionaryRepositoryDTO dictionaries)
        {
            Dictionaries = dictionaries;
        }

        protected DictionaryRepositoryDTO Dictionaries { get; private set; }

        public abstract List<Guid> Validate();

        public abstract InconsistencyType InconsistencyType { get; }
    }
}