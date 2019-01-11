using System;
using GazRouter.DTO.Dictionaries.PropertyTypes;

namespace GazRouter.DTO.DispatcherTasks.RecordNotes
{
    public class AddRecordNoteParameterSet
	{
        public Guid TaskId { get; set; }
        public Guid EntityId { get; set; }
        public PropertyType PropertyTypeId { get; set; }
        public string Note { get; set; }
        public string UserNameCpdd { get; set; }
	}
}
