using GazRouter.DTO.ObjectModel;

namespace GazRouter.ObjectModel.Model
{
    public interface ITabItem
    {
        CommonEntityDTO ParentEntity { get; set; }
        string Header { get; }
        void Refresh();
    }
}