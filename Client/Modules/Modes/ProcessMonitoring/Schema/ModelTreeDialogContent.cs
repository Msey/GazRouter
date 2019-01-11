using GazRouter.Controls.Tree;

namespace GazRouter.Modes.ProcessMonitoring.Schema
{
    public class ModelTreeDialogContent : BaseDialogContent
    {
        public TreeViewModelPointObjects ModelTree { get; set; }
        public TreeViewModelPipeline PipeLineTree { get; set; }
    }
}