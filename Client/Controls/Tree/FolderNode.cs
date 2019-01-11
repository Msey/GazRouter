using System.Windows.Media;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.ObjectModel;

namespace GazRouter.Controls.Tree
{
    public class FolderNode : NodeBase
    {
        private static readonly ImageSource ImagePath =
           (ImageSource)new ImageSourceConverter().ConvertFromString(@"/Common;component/Images/16x16/folder.png");

        public FolderNode(string name, EntityType entityType)
        {
            Name = name;
            EntityType = entityType;
        }

        public EntityType EntityType { get; }

        public override string Name { get; }

        public override ImageSource ImageSource => ImagePath;

        public override CommonEntityDTO Entity => GetParentEntity(this);

        private CommonEntityDTO GetParentEntity(NodeBase node)
        {
            if (node.Parent is FolderNode)
            {
                return GetParentEntity(node.Parent);
            }
            return node.Parent?.Entity;
        }
    }
}