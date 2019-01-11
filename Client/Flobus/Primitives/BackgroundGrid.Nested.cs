using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GazRouter.Flobus.Primitives
{
    partial class BackgroundGrid
    {
        protected struct LineInfo
        {
            internal double Position { get; set; }
            internal Orientation Orientation { get; set; }

            public bool Equals(LineInfo other)
            {
                return Position.Equals(other.Position) && Orientation == other.Orientation;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is LineInfo && Equals((LineInfo)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (Position.GetHashCode() * 397) ^ (int)Orientation;
                }
            }

        
        }

        struct LineDescriptor
        {
    
            internal Brush LineStroke { get; set; }
            internal Orientation Orientation { get; set; }

            internal double LineStart { get; set; }
            internal double LineEnd { get; set; }
            internal double Interval { get; set; }

            internal double Zoom { get; set; }
            internal double Min { get; set; }
            internal double Max { get; set; }
            
        }


        private class LineContainerRecycler
        {
            private readonly Panel _panel;
            private readonly BackgroundGrid _host;
            private readonly List<Line> _realizedContainers = new List<Line>();
            private readonly List<Line> _markedContainers = new List<Line>();
            private readonly List<Line> _virtualizedContainers = new List<Line>();

            private HashSet<LineInfo> _newItems = new HashSet<LineInfo>();


            public LineContainerRecycler(Panel panel, BackgroundGrid host)
            {
                _panel = panel;
                _host = host;
            }

            protected internal void Update(IEnumerable<LineInfo> items )
            {
                _markedContainers.Clear();

                var realizedContainersCopy = _realizedContainers.ToList();
				_newItems = new HashSet<LineInfo>(items.ToList());

                foreach (var container in realizedContainersCopy)
                {
                    var item = (LineInfo) container.Tag;
                    if (_newItems.Remove(item) == false)
                    {
                        _realizedContainers.Remove(container);
                        _markedContainers.Add(container);
                    }
                }

                foreach (var container in _realizedContainers)
                {
                    _host.PrepareLine(container, (LineInfo) container.Tag);
                }

                foreach (var item in _newItems)
                {
                    if (_markedContainers.Count > 0)
                    {
                        var container = _markedContainers[0];
						_host.PrepareLine(container,item);
						_realizedContainers.Add(container);
						_markedContainers.RemoveAt(0);
                    } else if (_virtualizedContainers.Count > 0)
                    {
                        var container = _virtualizedContainers[0];
						_host.PrepareLine(container, item);
						_realizedContainers.Add(container);
						_virtualizedContainers.RemoveAt(0);
						container.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        var container = new Line();
						_host.PrepareLine(container, item);
						_realizedContainers.Add(container);
						_panel.Children.Add(container);
                    }
                }

                var count = _markedContainers.Count;
                for (int i = 0; i < count; i++)
                {
                    var container = _markedContainers[0];
					_markedContainers.RemoveAt(0);
					_virtualizedContainers.Add(container);
					container.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}