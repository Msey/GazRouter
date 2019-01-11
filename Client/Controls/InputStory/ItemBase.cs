using System;

namespace GazRouter.Controls.InputStory
{
    /// <summary>
    /// Базовый класс для всех элементов дерева
    /// </summary>
    public class ItemBase
    {
        public ItemBase()
        {
            
        }

        public virtual string Direction { get; set; }

        public virtual string SourceType { get; set; }

        public virtual string SourceName { get; set; }

        public DataStatus Status { get; set; }

        public int? ErrorCount { get; set; }

        public Tuple<int?, bool> NewIncomingErrors { get; set; }

        public DateTime? ChangeDate { get; set; }

        public string ChangeUser { get; set; }

    }
}