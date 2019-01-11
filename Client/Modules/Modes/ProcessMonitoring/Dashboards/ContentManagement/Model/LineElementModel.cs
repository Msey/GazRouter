using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Model
{

     
    public class LineElementModel : BoxedElementModel
    {
        public LineElementModel()
        {
            Thickness = 4;
            Color = Colors.Orange;
            IsDotted = false;
            PointList = new List<LineElementPointModel>();
        }

        /// <summary>
        /// Толщина линии
        /// </summary>
        public int Thickness { get; set; }

        /// <summary>
        /// Цвет линии
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// Пунктирная линия?
        /// </summary>
        public bool IsDotted { get; set; }

        /// <summary>
        /// Закруглять углы на сгибах линии
        /// </summary>
        public bool IsJoinRounded { get; set; }

        /// <summary>
        /// Перечень точек
        /// </summary>
        public List<LineElementPointModel> PointList { get; set; }
        

        public void Straighten()
        {
            for (var i = 0; i < PointList.Count - 1; i++)
            {
                var dx = Math.Abs(PointList[i].Position.X - PointList[i+1].Position.X);
                var dy = Math.Abs(PointList[i].Position.Y - PointList[i+1].Position.Y);
                
                var angle = Math.Atan(dx < dy ? dx/dy : dy/dx) * 180/Math.PI;

                if (angle < 4)
                    PointList[i + 1].Position = dx < dy
                        ? new Point(PointList[i].Position.X, PointList[i + 1].Position.Y)
                        : new Point(PointList[i + 1].Position.X, PointList[i].Position.Y);
            }
            RecalculatePosition();
        }

        public override void CopyStyle(ElementModelBase other)
        {
            var e = other as LineElementModel;
            if (e != null)
            {
                Thickness = e.Thickness;
                Color = e.Color;
                IsDotted = e.IsDotted;
                IsJoinRounded = e.IsJoinRounded;
            }
            base.CopyStyle(other);
        }

        public void RecalculatePosition()
        {
            var x = PointList.Min(p => p.Position.X);
            var y = PointList.Min(p => p.Position.Y);
            Position = new Point(x, y);

            Width = PointList.Max(p => p.Position.X) - x;
            Height = PointList.Max(p => p.Position.Y) - y;
        }
        
    }

    public class LineElementPointModel : ElementModelBase
    {
        public bool DeleteAllowed { get; set; }
        
    }
}