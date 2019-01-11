using System;

namespace GazRouter.DTO.Appearance.Versions
{
    public class SchemeVersionItemDTO : BaseDto<int>
    {
        public bool IsPublished { get; set; }
        public DateTime CreateDate { get; set; }
        public string SchemeName { get; set; }
        
        public int SystemId { get; set; }

        public string SystemName { get; set; }
        public string CreatorName { get; set; }

        public string Comment { get; set; }

        public string CommentAuthor { get; set; }
        public DateTime CommentDate { get; set; }

        public  bool IsCommented => !string.IsNullOrEmpty(Comment);
    }
}