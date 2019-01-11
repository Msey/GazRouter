using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using GazRouter.Flobus.Misc;
using GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Container;
using GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Dialogs;
using GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Model;
using Microsoft.Practices.Prism.Commands;
using Telerik.Windows.Controls;

namespace GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Elements
{

    public class TextElementView : BoxedElementView<TextElementModel>
    {
        private readonly TextBlock _text;
        


        public TextElementView(TextElementModel elementModel, DashboardElementContainer dashboard)
            : base(elementModel, dashboard, true, true, true, true)
        {
            
            _text = new TextBlock
            {
                FontFamily = new FontFamily("Segoe UI"),
                //Foreground = new SolidColorBrush(Colors.White),
                FontSize = 11,
                TextAlignment = TextAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                Width = 100,
                Height = 100,
                Text = elementModel.Text
            };
            Dashboard.Canvas.Children.Add(_text);

            UpdatePosition();

            InitCommands();
        }

        public override void Destroy()
        {
            if (_text != null)
                Dashboard.Canvas.Children.Remove(_text);
            
            base.Destroy();
        }

        private const int _margin = 4;

        public override void UpdatePosition()
        {
            base.UpdatePosition();

            if (IsDraging) return;


            _text.FontSize = ElementModel.FontSize;
            _text.Foreground = new SolidColorBrush(ElementModel.FontColor);
            Canvas.SetLeft(_text, Position.X + _margin);
            Canvas.SetTop(_text, Position.Y + _margin);
            _text.Width = Width - _margin * 2 < 0 ? 0 : Width - _margin * 2;
            _text.Height = Height - _margin * 2 < 0 ? 0 : Height - _margin * 2;
            _text.Text = ElementModel.Text;
            Canvas.SetZIndex(_text, Z + 2);


            _text.FontStyle = ElementModel.FontStyle == MyFontStyle.Italic ||
                              ElementModel.FontStyle == MyFontStyle.BoldItalic
                ? FontStyles.Italic
                : FontStyles.Normal;

            _text.FontWeight = ElementModel.FontStyle == MyFontStyle.Bold ||
                             ElementModel.FontStyle == MyFontStyle.BoldItalic
               ? FontWeights.Bold
               : FontWeights.Normal;
            
        }
        

        public override void StartDrag()
        {
            base.StartDrag();
            _text.Visibility = Visibility.Collapsed;
        }

        public override void EndDrag()
        {
            base.EndDrag();
            _text.Visibility = Visibility.Visible;
            
            UpdatePosition();
        }

        #region CONTEXT_MENU

        public override void FillMenu(RadContextMenu menu, MouseButtonEventArgs e)
        {
            base.FillMenu(menu, e);
            menu.AddCommand("Параметры...", _changeViewSettings,e);
            menu.AddSeparator();
            menu.AddCommand( "Поверх всех", _moveForward,e);
            menu.AddCommand( "В самый низ", _moveBackward,e);
            menu.AddSeparator();
            menu.AddCommand( "Удалить", _removeDashboardElement,e);
        }

        private DelegateCommand<MouseButtonEventArgs> _removeDashboardElement;
        private DelegateCommand<MouseButtonEventArgs> _moveForward;
        private DelegateCommand<MouseButtonEventArgs> _moveBackward;
        private DelegateCommand<MouseButtonEventArgs> _changeViewSettings;

        private void InitCommands()
        {
            //  Инициализация команды удаления элемента с дашборда
            _removeDashboardElement = new DelegateCommand<MouseButtonEventArgs>(eventArg => Dashboard.RemoveElement(this), eventArg => true);

            //  Инициализация команды перемещения элемента на передний план
            _moveForward = new DelegateCommand<MouseButtonEventArgs>(eventArg => Dashboard.MoveElementForward(this), eventArg => true);

            //  Инициализация команды перемещения элемента на задний план
            _moveBackward = new DelegateCommand<MouseButtonEventArgs>(eventArg => Dashboard.MoveElementBackward(this), eventArg => true);

            //  Инициализация команды изменения настроек отображения элемента
            _changeViewSettings = new DelegateCommand<MouseButtonEventArgs>(eventArg =>
            {
                var vm = new TextElementSettingsViewModel(ElementModel);
                var dlg = new TextElementSettingsView { DataContext = vm };
                dlg.Closed += (sender, args) =>
                {
                    if (dlg.DialogResult.HasValue && dlg.DialogResult.Value)
                    {
                        UpdatePosition();
                    }

                };
                dlg.ShowDialog();


            }, eventArg => true);

        }

        #endregion
        
    }
}