using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using GazRouter.Application;
using GazRouter.Common.Cache;
using GazRouter.DTO.SeriesData.PropertyValues;
using GazRouter.Flobus.Misc;
using GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Container;
using GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Dialogs;
using GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Model;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.ServiceLocation;
using Telerik.Windows.Controls;

namespace GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Elements
{
    public class FunctionElementView : BoxedElementView<FunctionElementModel>
    {
        private readonly TextBlock _value;
        private readonly int _precision;
        private Path _badDataBox;
        private const int _margin = 4;
        private bool IsState;
        private bool IsAdd;

        public FunctionElementView(FunctionElementModel elementModel, DashboardElementContainer dashboard)
            : base(elementModel, dashboard, true, false, true, true)
        {
            IsState = ElementModel.PropertyType == DTO.Dictionaries.PropertyTypes.PropertyType.CompressorUnitState;
            _value = new TextBlock
            {
                FontFamily = new FontFamily("Segoe UI"),
                FontSize = 11,
                Foreground = new SolidColorBrush(Colors.Black),
                TextAlignment = TextAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                Width = 100,
                Height = 100,
                Text = "x"
            };
 
            FunctionParameters(true);

            //UpdatePosition();

            InitCommands();
        }

        public void FunctionParameters(bool IsAdd)
        {
            var vm = new FunctionElementSettingsViewModel(ElementModel, IsAdd);
            var dlg = new FunctionElementSettingsView { DataContext = vm };
            dlg.Closed += (sender, args) =>
            {
                if (dlg.DialogResult.HasValue && dlg.DialogResult.Value)
                {
                    if (IsAdd) Dashboard.Canvas.Children.Add(_value);
                    UpdateSize();
                    UpdatePosition();
                }
            };
            dlg.ShowDialog();
        }

        public override void Destroy()
        {
            if (_value != null)
                Dashboard.Canvas.Children.Remove(_value);

            base.Destroy();
        }

        public override void UpdatePosition()
        {
            base.UpdatePosition();

            if (IsDraging) return;

            _value.FontSize = ElementModel.FontSize;
            _value.Foreground = new SolidColorBrush(ElementModel.FontColor);
            Canvas.SetLeft(_value, Position.X + _margin);
            Canvas.SetTop(_value, Position.Y + _margin);
            Canvas.SetZIndex(_value, Z + 2);

            ToolTipService.SetToolTip(_value,
                new TextBlock
                {
                    Text = ElementModel.Comment,
                    FontSize = 11,
                    MaxWidth = 200,
                    TextWrapping = TextWrapping.Wrap
                });


        }
        private static IClientCache ClientCache => ServiceLocator.Current.GetInstance<IClientCache>();
        public void UpdateSize()
        {
            var propType = ClientCache.DictionaryRepository.PropertyTypes.Single(p => p.Id == (int)ElementModel.PropertyType);

            _value.Width = ElementModel.ShowTitle
                ? propType.ShortName.Length * ElementModel.FontSize * 0.65 + EntityElementView.CalculateValueWidth(propType, ElementModel.FontSize)
                : EntityElementView.CalculateValueWidth(propType, ElementModel.FontSize);

            Width = _value.Width + 2 * _margin;
            Height = _value.ActualHeight + 2 * _margin;


            Width = 150 + 2 * _margin;
            Height = 30 + 2 * _margin;
        }

        public override void UpdateData()
        {
            // Показывать наименование свойства
            _value.Text = ElementModel.ShowTitle ?
                    string.Format("{0} {1} ", ClientCache.DictionaryRepository.PropertyTypes.Single(p => p.Id == (int)ElementModel.PropertyType).ShortName, IsState ? ": " : "=") : "";


            if (Dashboard.Data != null
                && Dashboard.Data.Measurings.ContainsKey(ElementModel.EntityId)
                && Dashboard.Data.Measurings[ElementModel.EntityId].ContainsKey(ElementModel.PropertyType))
            {
                var meas = Dashboard.Data.Measurings[ElementModel.EntityId][ElementModel.PropertyType]
                    .SingleOrDefault(m => m.Date == Dashboard.Data.KeyDate);
                if (meas == null) return;


                if (meas is PropertyValueDoubleDTO)
                {
                    var dbl = meas as PropertyValueDoubleDTO;
                    _value.Text += ElementModel.PropertyType.ToString().Contains("State") ? ClientCache.DictionaryRepository.CompUnitStates.Single(s => s.Id == (int)dbl.Value).Name : dbl.Value.ToString("n" + _precision);

                    if (dbl.QualityCode != QualityCode.Good)
                    {
                        _badDataBox = new Path
                        {
                            Fill = new SolidColorBrush(Colors.Orange),
                            Data = new RectangleGeometry
                            {
                                Rect = new Rect(Position.X, Position.Y, _value.Width, _value.Height)
                            }
                        };
                        Dashboard.Canvas.Children.Add(_badDataBox);
                        Canvas.SetZIndex(_badDataBox, Z);
                    }
                    else
                    {
                        if (_badDataBox != null)
                            Dashboard.Canvas.Children.Remove(_badDataBox);
                    }
                }
                else if (meas is PropertyValueStringDTO)
                {
                    var v = meas as PropertyValueStringDTO;
                    _value.Text += v.Value;
                }
                else
                {
                    _value.Text += "x";
                }
            }
            else
            {
                _value.Text += "x";
            }
        }


        public override void StartDrag()
        {
            base.StartDrag();
            _value.Visibility = Visibility.Collapsed;
        }

        public override void EndDrag()
        {
            base.EndDrag();
            _value.Visibility = Visibility.Visible;

            UpdatePosition();
        }

        #region CONTEXT_MENU

        public override void FillMenu(RadContextMenu menu, MouseButtonEventArgs e)
        {
            base.FillMenu(menu, e);
            menu.AddCommand("Параметры...", _changeViewSettings, e);
            menu.AddSeparator();
            menu.AddCommand("Поверх всех", _moveForward, e);
            menu.AddCommand("В самый низ", _moveBackward, e);
            menu.AddSeparator();
            menu.AddCommand("Удалить", _removeDashboardElement, e);
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
            _changeViewSettings = new DelegateCommand<MouseButtonEventArgs>(eventArg => FunctionParameters(false), eventArg => true);

        }

        #endregion

    }
}
