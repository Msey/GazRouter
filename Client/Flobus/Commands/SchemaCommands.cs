using Telerik.Windows.Controls;

namespace GazRouter.Flobus.Commands
{
    public static class SchemaCommands
    {
        static SchemaCommands()
        {
            Delete = new RoutedUICommand("Удаляет выбранный элемент со схемы", "Delete", typeof(SchemaCommands));
            GoToTree = new RoutedUICommand("Показывает выбранный  элемент в  объектной модели", "GoToTree", typeof(SchemaCommands));
            AddCompressorShop = new RoutedUICommand("Добавляет компрессорный цех", "AddCompressorShop", typeof(SchemaCommands));
        }

        public static RoutedUICommand AutoFit { get; } = new RoutedUICommand("Масштабирует и скроллирует окно, чтобы показать все элементы схемы", "AutoFit", typeof(SchemaCommands));

        public static RoutedUICommand Delete { get; private set; }
        public static RoutedUICommand GoToTree { get; private set; }
        public static RoutedUICommand AddCompressorShop { get; set; }
    }
}