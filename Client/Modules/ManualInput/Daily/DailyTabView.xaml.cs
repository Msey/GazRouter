

using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace GazRouter.ManualInput.Daily
{
    public partial class DailyTabView
    {
        public DailyTabView()
        {
            InitializeComponent();
            gridView.KeyboardCommandProvider = new CustomKeyboardCommandProvider(gridView);
        }
    }

    public class CustomKeyboardCommandProvider : DefaultKeyboardCommandProvider
    {
        private GridViewDataControl dataControl;

        public CustomKeyboardCommandProvider(GridViewDataControl dataControl)
            : base(dataControl)
        {
            this.dataControl = dataControl;
        }

        public override IEnumerable<ICommand> ProvideCommandsForKey(Key key)
        {

            if (key != System.Windows.Input.Key.Enter)
                return base.ProvideCommandsForKey(key);

            List<ICommand> commandsToExecute = new List<ICommand>();

            if (this.dataControl.CurrentCell == null)
                return commandsToExecute;

            if (this.dataControl.CurrentCell.IsInEditMode)
            {
                commandsToExecute.Add(RadGridViewCommands.CommitEdit);
                var currentItem = this.dataControl.CurrentCell.ParentRow.Item;

                for (int i = 0; i <= dataControl.Items.Count - 1; i++)
                {
                    if (dataControl.Items[i] == currentItem)
                    {
                        for (int j = i + 1; j <= dataControl.Items.Count - 1; j++)
                        {
                            if ((dataControl.Items[j] as InputItem) != null)
                            {
                                for (int k = j - i; k > 0; k--)
                                {
                                    commandsToExecute.Add(RadGridViewCommands.MoveDown);
                                }
                                commandsToExecute.Add(RadGridViewCommands.BeginEdit);
                                j = i = dataControl.Items.Count;
                            }
                        }
                    }
                }
            }

            else
            {
                commandsToExecute.Add(RadGridViewCommands.ActivateRow);
            }

            return commandsToExecute;
        }
    }
}
