using System.Linq;
using Telerik.Windows.Controls;
using Telerik.Windows.Rendering;

namespace GazRouter.Repair.Plan
{
	public partial class PlanGanttView
    {
		public PlanGanttView()
		{
			InitializeComponent();

            Gantt.Loaded += OnGanttLoaded;
        }

        private void OnGanttLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var panels = Gantt.ChildrenOfType<LogicalCanvasPanel>().ToList();
            var visibleArea = panels.Where(p => p.Name == "EventsPanel").First().ViewportWidth;
            var vm = Gantt.DataContext as PlanViewModel;
            if (vm != null)
                vm.VisibleArea = (long) visibleArea;
        }
    }
}
