using System;

namespace GazRouter.DTO.ASDU
{
   
    public enum LoadedFileStatus
    {
        Loaded = 1,
        Read = 2,
        Applied = 3,
        Error = 4,
        XmlValidated = 5,
        XmlValidationError = 6,
        RequestCreated = 1001,
        RequestNameEdited = 1002,
        RequestXmlCreated = 1003,
        RequestXmlError = 1004,
        RequestDeleted = 1005,
        RequestSent = 1006,
        InDir = 2000
    }
    public class LoadedFile
    {
        public string Key { get; set; }
        public DateTime LoadDate { get; set; }
        public LoadedFileStatus Status { get; set; }
        public string FileName { get; set; }
        public string Name { get; set; }
    }
}