using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GazRouter.Controls.Trends;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.Flobus.Misc;
using GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Container;
using Microsoft.Practices.Prism.Commands;
using Telerik.Windows.Controls;
namespace GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Columns
{
    public class ValueColumnView : ColumnViewBase
    {
        public ValueColumnView(DashboardElementContainer dashboard,
                               string title, 
                               int rowCount, 
                               Guid entityId, 
                               PropertyTypeDTO propType, 
                               int fontSize=11)
                                : base(dashboard, title, rowCount, fontSize)
                            {
                                InitCommands();
                                _entityId = entityId;
                                _propertyType = propType;
                                MouseRightButtonUp += Dashboard.OnMouseRightButtonUp;
                                // Создание элементов отображения значений
                                _valueList = new List<ColumnValueField>();
                                for (var i = 0; i < RowCount; i++)
                                {
                                    var vf = new ColumnValueField(Dashboard, i, fontSize, _propertyType.PhysicalType.DefaultPrecision);
//                                    vf.MouseEnter += (sender, args) =>
//                                        {
//                                            var pos = args.GetPosition(Dashboard);
//                                            if (Dashboard.Layout.Measurings == null) return;
//                                            if (!Dashboard.Layout.Measurings.ContainsKey(_entity.Id)) return;
//                                            if (!Dashboard.Layout.Measurings[_entity.Id].ContainsKey(_propertyType)) return;
//                                            var measurings = Dashboard.Layout.Measurings[_entity.Id][_propertyType];
//                                            measurings.Sort();
//                                            measurings.Reverse();
//                                            var vals = measurings.OfType<PropertyValueDoubleDTO>().Select(dm => dm.DisplayCurrentValue).ToList();
//                                            Dashboard.TinyTrend.Show((int)pos.X + 10, (int)pos.Y + 10, vals, 11 - ((FieldBase)sender).SerieIndex);
//                                        };
                                    vf.MouseLeave += (sender, args) => Dashboard.TinyTrend.Hide();
                                    vf.MouseRightButtonUp += (sender, args) => NotifyMouseRightButtonUp(this, args);
                                    _valueList.Add(vf);
                                }
                                UpdatePosition();
                            }
        
        private readonly List<ColumnValueField> _valueList;
        private readonly Guid _entityId;
        private readonly PropertyTypeDTO _propertyType;
        public override Visibility Visibility
        {
            get
            {
                return base.Visibility;
            }
            set
            {
                foreach (var e in _valueList) e.Visibility = value;
                base.Visibility = value;
            }
        }
        public override void Destroy()
        {
            MouseRightButtonUp -= Dashboard.OnMouseRightButtonUp;
            base.Destroy();
            _valueList.ForEach(v => v.Destroy());
        }
        public override void UpdatePosition()
        {
            base.UpdatePosition();
            for (var i = 0; i < _valueList.Count; i++)
            {
                _valueList[i].Position = new Point(Position.X, Position.Y + HeaderHeight + Height * i);
                //_valueList[i].Position = new Point(Position.X, Position.Y + Height + Height*i);
                _valueList[i].Width = Width;
                _valueList[i].Height = Height;
                _valueList[i].Z = Z;
            }
        }
        public override void UpdateData()
        {
            if (Dashboard.Data == null) return;
            //
            if (Dashboard.Data.Measurings.All(m => m.Key != _entityId)) return;
            if (Dashboard.Data.Measurings[_entityId].All(m => m.Key != _propertyType.PropertyType)) return;
            var measurings = Dashboard.Data.Measurings[_entityId][_propertyType.PropertyType];
            if (measurings == null) return;
            for (var i = 0; i < _valueList.Count; i++)
            {
                var val = measurings.FirstOrDefault(c => c.Date == Dashboard.Data.KeyDate.AddHours(i * -2));
                if (val != null) _valueList[i].Measuring = val;
            }
        }
        public override void FillMenu(RadContextMenu menu, MouseButtonEventArgs e)
        {
            base.FillMenu(menu, e);
            menu.AddCommand( "На тренд...", _toTrendCommand,e);
        }
        private DelegateCommand<MouseButtonEventArgs> _toTrendCommand;
        private void InitCommands()
        {
            _toTrendCommand = new DelegateCommand<MouseButtonEventArgs>(obj => 
                    TrendsHelper.ShowTrends(_entityId, _propertyType.PropertyType), 
                                            canExecute => true);
        }
    }
}
#region trash
//  Инициализация команды добавления параметра на тренд
//            _toTrendCommand =
//                            new DelegateCommand<MouseButtonEventArgs>(
//                                args =>
//                                Dashboard.ToTrendCommand.Execute(new ToTrendCommandParameter(_entity, _propertyType)),
//                                args =>
//                                Dashboard.ToTrendCommand != null &&
//                                Dashboard.ToTrendCommand.CanExecute(new ToTrendCommandParameter(_entity, _propertyType)));
#endregion

