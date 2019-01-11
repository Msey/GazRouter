using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using GazRouter.Common.Cache;
using GazRouter.DTO.Dictionaries.PhisicalTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.Flobus.Misc;
using GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Columns;
using GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Container;
using GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Dialogs;
using GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Model;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.ServiceLocation;
using Telerik.Windows.Controls;
using GazRouter.Application;

namespace GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Elements
{
    public class EntityElementView : BoxedElementView<EntityElementModel>
    {
        public EntityElementView(EntityElementModel elementModel, DashboardElementContainer dashboard)
            : base(elementModel, dashboard, true, false, true, true)
        {
            _columnList = new List<ColumnViewBase>();
            _titleText = new TextBlock
            {
                FontFamily = new FontFamily("Segoe UI"),
                //Foreground = new SolidColorBrush(Colors.White),
                FontSize = 11,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                Text = elementModel.EntityName
            };
            Dashboard.Canvas.Children.Add(_titleText);

            _titleSeparator = new Line
            {
                StrokeThickness = 0.5,
                Stroke = new SolidColorBrush(Colors.DarkGray),
                Effect = new DropShadowEffect { BlurRadius = 1, Color = Colors.White, Direction = 270, ShadowDepth = 0.5, Opacity = 1 }
            };
            Dashboard.Canvas.Children.Add(_titleSeparator);
            UpdateColumns();
            InitCommands();
        }
        private readonly List<ColumnViewBase> _columnList;
        // Заголовок элемента (название объекта)
        private readonly TextBlock _titleText;
        private readonly Line _titleSeparator;
        private static IClientCache ClientCache => ServiceLocator.Current.GetInstance<IClientCache>();
        public override void Destroy()
        {
            if (_titleText != null)
                Dashboard.Canvas.Children.Remove(_titleText);

            //if (_titleBox != null)
            //    _dashboard.Canvas.Children.Remove(_titleBox);

            if (_titleSeparator != null)
                Dashboard.Canvas.Children.Remove(_titleSeparator);

            _columnList.ForEach(c => c.Destroy());
            base.Destroy();
        }
        public override void UpdateData()
        {
            _columnList.ForEach(c => c.UpdateData());
            _titleText.Text = ElementModel.EntityName;
        }
        public override void UpdatePosition()
        {
            base.UpdatePosition();

            if (IsDraging) return;


            const int margin = 4;
            _titleText.FontSize = ElementModel.FontSize;
            Canvas.SetLeft(_titleText, Position.X + margin);
            Canvas.SetTop(_titleText, Position.Y + margin);
            _titleText.Width = Width - margin * 2;
            _titleText.Height = ElementModel.FontSize * 3;
            Canvas.SetZIndex(_titleText, Z + 2);

            _titleSeparator.X1 = Position.X + margin;
            _titleSeparator.X2 = Position.X + Width - margin;
            _titleSeparator.Y1 = _titleSeparator.Y2 = Position.Y + ElementModel.FontSize * 3 + margin;
            Canvas.SetZIndex(_titleSeparator, Z + 1);

            
            var off = 0;
            foreach (var vc in _columnList)
            {
                vc.Position = new Point(Position.X + off, Position.Y + ElementModel.FontSize * 3 + margin);
                vc.Z = Z;
                off += (int)vc.Width;
            }
         
        }
        /// <summary> Создать перечень столбцов исходя из настроек элемента </summary>
        public void UpdateColumns()
        {
            var rowHeight = ElementModel.FontSize + 4;
            _columnList.ForEach(c => c.Destroy());
            _columnList.Clear();

            if (ElementModel.IsTimestampVisible)
            {
                _columnList.Add(
                    new TimestampColumnView(Dashboard, ElementModel.SerieCount, ElementModel.FontSize)
                    {
                        Width = 16 * ElementModel.FontSize * 0.65,
                        Height = rowHeight,
                    });
            }
            // todo: может падать если в EntityElementModel.GetDefaultPropertyTypeIdList
            // - propertyTypeId не соответствует типам в базе
#region test
            ElementModel.VisiblePropertyTypeList.ForEach(vpt =>
            {
                var a = ClientCache.DictionaryRepository
                    .PropertyTypes.Single(pt => pt.Id == vpt.PropertyTypeId).ShortName;
            });
#endregion
            _columnList.AddRange(ElementModel.VisiblePropertyTypeList.Select(vpt =>
                    new ValueColumnView(
                        Dashboard,
                        GetHeader(vpt),
                        ElementModel.SerieCount,
                        ElementModel.EntityId,
                        vpt.PropertyType,
                        ElementModel.FontSize)
                    {
                        Width = CalculateValueWidth(vpt.PropertyType, ElementModel.FontSize),
                        Height = rowHeight,
                    }));


            Width = _columnList.Count > 0 ? _columnList.Sum(c => c.Width) : _titleText.ActualWidth;
            Height = ElementModel.FontSize * 3 + 4 + ElementModel.FontSize * 3 + rowHeight * (ElementModel.SerieCount);
            
            UpdatePosition();
            UpdateData();
        }
        public override void UpdateContents()
        {
            UpdateColumns();

            base.UpdateContents();
        }

        public string GetHeader(PropertyTypeDisplaySettings ptset)
        {
            return
                $"{ClientCache.DictionaryRepository.PropertyTypes.Single(pt => pt.Id == ptset.PropertyTypeId).ShortName},\n{UserProfile.UserUnitName(ptset.PropertyType.PhysicalTypeId)}";
        }
        public override void StartDrag()
        {
            base.StartDrag();
            _titleText.Visibility = Visibility.Collapsed;
            _titleSeparator.Visibility = Visibility.Collapsed;
            _columnList.ForEach(c => c.Visibility = Visibility.Collapsed);
        }
        public override void EndDrag()
        {
            base.EndDrag();
            _titleText.Visibility = Visibility.Visible;
            _titleSeparator.Visibility = Visibility.Visible;
            _columnList.ForEach(c => c.Visibility = Visibility.Visible);

            UpdatePosition();
        }
#region CONTEXT_MENU
        public override void FillMenu(RadContextMenu menu, MouseButtonEventArgs e)
        {
            base.FillMenu(menu, e);
            menu.AddCommand( "Параметры...", _changeViewSettings,e);
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
                var vm = new EntityElementSettingsViewModel(ElementModel);
                var dlg = new EntityElementSettingsView { DataContext = vm };
                dlg.Closed += (sender, args) =>
                {
                    if (dlg.DialogResult.HasValue && dlg.DialogResult.Value)
                    {
                        //ElementModel.VisiblePropertyTypeList = vm.VisibleProprtyTypeList;
                        UpdateColumns();
                    }
                };
                dlg.ShowDialog();
            }, eventArg => true);
        }
#endregion
        public static double CalculateValueWidth(PropertyTypeDTO pType, int fontSize)
        {
            switch (pType.PhysicalType.PhysicalType)
            {
                case PhysicalType.Timestamp:
                    return 16*fontSize*0.65;

                case PhysicalType.Volume:
                    return (6 + pType.PhysicalType.DefaultPrecision) * fontSize * 0.65;

                default:
                    return (4 + pType.PhysicalType.DefaultPrecision)*fontSize*0.65;
            }
        }
    }
}