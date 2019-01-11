using System;
using GazRouter.DTO.Dictionaries.PropertyTypes;

namespace GazRouter.DTO.DispatcherTasks.RecordNotes
{
    public class GetRecordNoteListParameterSet
	{
        public Guid TaskId { get; set; }
        public Guid EntityId { get; set; }
        public PropertyType PropertyTypeId { get; set; }
	}
}
