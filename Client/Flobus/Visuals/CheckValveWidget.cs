using GazRouter.Flobus.FloScheme;
using GazRouter.Flobus.Misc;
using JetBrains.Annotations;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;
using System.Windows;
using System.Windows.Media;
using Telerik.Windows.Controls;
using GazRouter.Flobus.Dialogs;
using GazRouter.Flobus.Interfaces;
using System.Windows.Data;
using GazRouter.Flobus.UiEntities.FloModel;

namespace GazRouter.Flobus.Visuals
{
    public class CheckValveWidget : ShapeWidgetBase, ISupportContextMenu, ICheckValveWidget
    {
        public static readonly DependencyProperty AngleProperty = DependencyProperty.Register(nameof(Angle), typeof(int), typeof(CheckValveWidget), new PropertyMetadata(0));

        public static readonly DependencyProperty TooltipProperty = DependencyProperty.Register(nameof(Tooltip), typeof(string), typeof(CheckValveWidget), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty ValveBrushProperty = DependencyProperty.Register(nameof(ValveBrush), typeof(Brush), typeof(CheckValveWidget), new PropertyMetadata(default(Brush)));
        public CheckValveWidget([NotNull] Schema schema) : base(schema)
        {
            IsManipulationAdornerVisible = false;
        }

        public string Tooltip
        {
            get { return (string)GetValue(TooltipProperty); }
            set { SetValue(TooltipProperty, value); }
        }

        public int Angle
        {
            get { return (int)GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }

        public Brush ValveBrush
        {
            get { return (Brush) GetValue(ValveBrushProperty); }
            set { SetValue(ValveBrushProperty, value); }
        }
        public void FillMenu(RadContextMenu menu, Point mousePosition, Schema schema)
        {
            if (!Schema.IsReadOnly)
            {
                menu.AddCommand("Повернуть клапан", new DelegateCommand(RotateValve));
                menu.AddCommand("Изменить...", new DelegateCommand(EditValve));
                menu.AddCommand("Удалить", new DelegateCommand(
                    () => RadWindow.Confirm(new DialogParameters
                    {
                        Header = "Внимание!",
                        Content = @"Вы уверены, что хотите удалить клапан со схемы?",
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
        private void EditValve()
        {
            new CheckValveEditDialog { DataContext = new CheckValveEditDialogViewModel(this) }.ShowDialog();
        }        

        private void RotateValve()
        {
            Angle = Angle == 270 ? 0 : Angle + 90;
        }
        public override Rect Bounds => new Rect(Position, new Size(10, 10));

    }
}
