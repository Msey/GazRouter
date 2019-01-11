using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using GazRouter.Flobus.FloScheme;
using Telerik.Windows.Diagrams.Core;

namespace GazRouter.Flobus.Model
{
    public interface IGraph
    {
        Point Position { get; set; }
        IEnumerable<IWidget> Widgets { get; }
        Cursor Cursor { get; set; }
        SelectionMode SelectionMode { get;  }
        bool IsMouseCaptured { get; set; }
        bool IsPanEnabled { get; set; }
        bool IsSnapToGridEnabled { get; }
        bool IsReadOnly { get; set; }
        int Snap { get; }
        void Pan(Point newPosition);
        bool PublishDiagramEvent(DiagramEvent diagramEvent, object args);
        void UpdateRectSelection(Rect bounds);
        bool BringIntoView(Rect rectangle);
        IAdornerPartResolver GetAdornerPartResolver();
    }
}