using System;

namespace GazRouter.Flobus.Model
{
    public class SchemeVersion
    {
        public int VersionId { get;  set; }
        public int SchemeId { get;  set; }
        public string SchemeName { get;  set; }
        public DateTime CreationDate { get;  set; }
        public string Creator { get;  set; }
        public bool IsPublished { get;  set; }

        public override string ToString()
        {
            return $"{SchemeName}\n" + $"версия: {VersionId}\n" + $"создана: {CreationDate}\n" + $"создал: {Creator}\n" +
                   $"опубликована: {(IsPublished ? "да" : "нет")}";
        }
    }
}