using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GazRouter.DTO.DispatcherTasks.RecordNotes
{
    public class EditRecordNoteParameterSet
    {
        public Guid RecordNoteId { get; set; }
        public string Note { get; set; }
        public string UserNameCpdd { get; set; }
    }
}
