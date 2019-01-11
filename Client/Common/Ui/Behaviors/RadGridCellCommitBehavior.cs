using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
namespace GazRouter.Common.Ui.Behaviors
{
    public class RadGridCellCommitBehavior : Behavior<UIElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            var treeList = AssociatedObject as RadTreeListView;
            if (treeList == null) return;
            treeList.KeyboardCommandProvider = new CommitCellKeyBoardCommandProvider(treeList);
        }
    }
    public class CommitCellKeyBoardCommandProvider : DefaultKeyboardCommandProvider
    {
        public CommitCellKeyBoardCommandProvider(GridViewDataControl dataControl)
            : base(dataControl)
        {
            _dataControl = dataControl;
        }

        readonly GridViewDataControl _dataControl;

        public override IEnumerable<ICommand> ProvideCommandsForKey(Key key)
        {
            var commandsToExecute = base.ProvideCommandsForKey(key).ToList();
            if (key == Key.Enter)
            {
                if (_dataControl.CurrentCell.IsInEditMode)
                {
                    commandsToExecute.Clear();
                    commandsToExecute.Add(RadGridViewCommands.CommitCellEdit);
                }
            }
            if (key == Key.Escape)
            {
                commandsToExecute.Clear();
                if (_dataControl.CurrentCell.IsInEditMode)
                {
                    commandsToExecute.Add(RadGridViewCommands.CancelCellEdit);
                }
            }
            return commandsToExecute;
        }
    }
}
