using GazRouter.Flobus.Commands;
using GazRouter.Flobus.Visuals;
using Telerik.Windows.Controls;

namespace GazRouter.Flobus
{
    public partial class Schema
    {
        private static void RegisterCommands()
        {
            CommandManager.RegisterClassCommandBinding(typeof (Schema),
                new CommandBinding(SchemaCommands.Delete, OnDeleteCommandExecuted, OnCanDeleteCommandExecute));
            CommandManager.RegisterClassCommandBinding(typeof (Schema),
                new CommandBinding(SchemaCommands.GoToTree, OnGoToTreeCommandExected, OnGoToTreeCanCommandExecuted));

            CommandManager.RegisterClassCommandBinding(typeof (Schema),
                new CommandBinding(SchemaCommands.AutoFit, OnAutoFitCommandExecuted, OnCanAutoFitCommandExecuted));
        }

        private static void OnAutoFitCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var schema = sender as Schema;
            schema?.AutoFit();
        }

        private static void OnCanAutoFitCommandExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            var schema = sender as Schema;
            if (schema != null)
            {
                e.CanExecute = true;
            }
        }

        private static void OnGoToTreeCanCommandExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            var scheme = sender as Schema;

            var i = scheme?.SelectedWidget as CompressorShopWidget;
            if (i == null)
            {
                return;
            }
            e.CanExecute = true;
        }

        private static void OnGoToTreeCommandExected(object sender, ExecutedRoutedEventArgs e)
        {
            var scheme = sender as Schema;

            if (scheme == null)
            {
                return;
            }

            var i = scheme.SelectedWidget as CompressorShopWidget;
            if (i == null)
            {
                return;
            }
            scheme.GotoTreeCommand.Execute(i.Id);
        }
    }
}