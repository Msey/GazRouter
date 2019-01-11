using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PipelineTypes;

namespace GazRouter.Controls.Tree
{
    public class PipelineFolderNode : FolderNode
    {
        private readonly PipelineType _pipelineType;

        public PipelineFolderNode(string name, EntityType entityType, PipelineType pipelineType)
            : base(name, entityType)
        {
            _pipelineType = pipelineType;
        }

        public PipelineType PipelineType
        {
            get { return _pipelineType; }
        }
    }
}         