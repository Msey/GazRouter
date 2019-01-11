using GazRouter.DTO.ObjectModel.Sites;

namespace GazRouter.Controls.InputStory
{
    public class SiteItem : ItemBase
    {
        public SiteItem(SiteDTO dto)
        {
            Dto = dto;
        }

        public SiteDTO Dto { get; set; }

        //public  

        public override string Direction => "Импорт";

        public override string SourceType => "ЛПУ";

        public override string SourceName => Dto.Name;
    }
}