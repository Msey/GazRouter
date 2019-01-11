using System.Windows;
using GazRouter.Flobus.Dialogs;
using GazRouter.Flobus.FloScheme;
using GazRouter.Flobus.Misc;
using JetBrains.Annotations;
using Telerik.Windows.Controls;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;

namespace GazRouter.Flobus.Visuals
{
    public class TextWidget : ShapeWidgetBase, ISupportContextMenu, ITextWidget
    {
        public static readonly DependencyProperty TextAngleProperty = DependencyProperty.Register(
            nameof(TextAngle), typeof(int), typeof(TextWidget),
            new PropertyMetadata(0));

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            nameof(Text), typeof(string), typeof(TextWidget), new PropertyMetadata(default(string)));

        public TextWidget([NotNull] Schema schema) : base(schema)
        {
        }

        public bool IsBoxVisible { get; set; }

        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public int TextAngle
        {
            get { return (int) GetValue(TextAngleProperty); }
            set { SetValue(TextAngleProperty, value); }
        }

        public void FillMenu(RadContextMenu menu, Point mousePosition, Schema schema)
        {
            if (!Schema.IsReadOnly)
            {
                menu.AddCommand("Повернуть текст", new DelegateCommand(RotateLabel));
                menu.AddCommand("Изменить...", new DelegateCommand(EditTextBlock));
                menu.AddCommand("Удалить", new DelegateCommand(
                    () => RadWindow.Confirm(new DialogParameters
                    {
                        Header = "Внимание!",
                        Content = @"Вы уверены, что хотите удалить текст со схемы?",
                        Closed = (sender, e1) =>
                        {
                            if (e1.DialogResult.HasValue && e1.DialogResult.Value)
                            {
                                Schema.RemoveTextWidget(this);
                            }
                        }
                    })));
            }
        }

        private void EditTextBlock()
        {
            new TextEditDialog {DataContext = new TextEditDialogViewModel(this)}.ShowDialog();
        }

        private void RotateLabel()
        {
            TextAngle = TextAngle == 270 ? 0 : 270;
        }
    }
}