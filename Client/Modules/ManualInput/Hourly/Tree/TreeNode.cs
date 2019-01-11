using System.Collections.Generic;
using System.Windows.Media;
using GazRouter.Common.ViewModel;
using GazRouter.Controls.Tree;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.SeriesData.EntityValidationStatus;

namespace GazRouter.ManualInput.Hourly.Tree
{
    public class EntityNode : TreeNode
    {
        private EntityValidationStatus _status;
        private readonly CommonEntityDTO _entityDto;


        public EntityNode(CommonEntityDTO entityDto, EntityValidationStatus status)
        {
            _entityDto = entityDto;
            Status = status;
        }

        public EntityValidationStatus Status
        {
            get { return _status; }
            set { SetProperty(ref _status, value); }
        }

        public override string Name => _entityDto.Name;
    }

    public class GroupNode : TreeNode
    {
        public GroupNode(string name)
        {
            Name = name;
        }
    }


    public class TreeNode : ViewModelBase
    {
        public TreeNode()
        {
            Childs = new List<TreeNode>();
        }

        public virtual string Name { get; set; }

        public List<TreeNode> Childs { get; set; }
    }
}