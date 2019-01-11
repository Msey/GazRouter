using System.Collections.Generic;
using System.Windows;
using GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Container;
namespace GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Columns
{
    public class TimestampColumnView : ColumnViewBase
    {
        public TimestampColumnView(DashboardElementContainer dashboard, 
                                   int rowCount, int fontSize = 11)
            :base(dashboard, "", rowCount, fontSize)
        {
            _tsList = new List<ColumnValueField>();
            for (var i = 0; i < RowCount; i++)
                _tsList.Add(new ColumnValueField(Dashboard, i, fontSize, 0)
                                   {
                                       TextAlign = TextAlignment.Right,
                                       Value = "00.00.0000 00:00"
                                   });
            UpdatePosition();
        }

        private readonly List<ColumnValueField> _tsList;
        public override Visibility Visibility
        {
            get
            {
                return base.Visibility;
            }
            set
            {
                _tsList.ForEach(ts => ts.Visibility = value);
                base.Visibility = value;
            }
        }

        public override void Destroy()
        {
            _tsList.ForEach(ts => ts.Destroy());
            base.Destroy();
        }
        public override void UpdateData()
        {
            for (var i = 0; i < _tsList.Count; i++)
            {
                if (Dashboard.Data == null)
                {        
                    // todo:            
                    continue;
                }
                var curTs = Dashboard.Data.KeyDate.AddHours(-2 * i);
                var prevTs = Dashboard.Data.KeyDate.AddHours(-2 * (i - 1));
                if (i == 0 || (i > 0 && curTs.Day != prevTs.Day))
                    _tsList[i].Value = curTs.ToString("dd.MM.yyyy HH:mm");
                else
                    _tsList[i].Value = curTs.ToString("HH:mm");
            }
        }
        public override void UpdatePosition()
        {
            for (var i = 0; i < _tsList.Count; i++)
            {
                //_tsList[i].Position = new Point(Position.X, Position.Y + Height + Height * i);
                _tsList[i].Position = new Point(Position.X, Position.Y + HeaderHeight + Height * i);
                _tsList[i].Width = Width;
                _tsList[i].Height = Height;
                _tsList[i].Z = Z;
            }
            base.UpdatePosition();
        }
    }
}