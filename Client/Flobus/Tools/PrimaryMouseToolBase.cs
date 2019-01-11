using GazRouter.Flobus.FloScheme;
using GazRouter.Flobus.Services;
using KeyArgs = Telerik.Windows.Diagrams.Core.KeyArgs;
using PointerArgs = Telerik.Windows.Diagrams.Core.PointerArgs;
using TextTool = Telerik.Windows.Diagrams.Core.TextTool;

namespace GazRouter.Flobus.Tools
{
    /// <summary>
    ///     An abstract base tool for mouse tools.
    /// </summary>
    public abstract class PrimaryMouseToolBase : ToolBase, IMouseListener, IKeyboardListener
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Telerik.Windows.Diagrams.Core.PrimaryMouseToolBase" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        protected PrimaryMouseToolBase(string name)
            : base(name)
        {
        }

        protected AdornerService AdornerService => ServiceLocator.AdornerService;

        protected SelectionService SelectionService => ServiceLocator.SelectionService;

        protected HitTestService HitTestService => ServiceLocator.HitTestService;

        protected ServiceLocator ServiceLocator { get; set; }

        /// <summary>
        ///     Gets the hit item.
        /// </summary>
        protected IWidget HitItem { get; set; }

        /// <inheritdoc />
        public override void Initialize(ServiceLocator serviceLocator)
        {
            ServiceLocator = serviceLocator;
        }

        /// <summary>
        ///     Handles the key down event.
        /// </summary>
        /// <param name="key" />
        public virtual bool KeyDown(KeyArgs key)
        {
            QueryControlKey();
            return false;
        }

        /// <summary>
        ///     Handles the key up event.
        /// </summary>
        /// <param name="key" />
        public virtual bool KeyUp(KeyArgs key)
        {
            QueryControlKey();
            return false;
        }

        /// <summary>
        ///     Handles the mouse-down event.
        /// </summary>
        /// <param name="e">The <see cref="T:Telerik.Windows.Diagrams.Core.PointerArgs" /> instance containing the event data.</param>
        public virtual bool MouseDown(PointerArgs e)
        {
            QueryControlKey();
            if (IsActive)
            {
                InitialPoint = e.Point;

                //Получаем элемент под курсором
                HitItem = HitTestService.ItemUnderMouse /* ??
                          HitTestService.GetItemsNearPoint(e.TransformedPoint, DiagramConstants.SelectionHitTestRadius)
                    .Where(i => i is IPipelineWidget)
                    .OrderByDescending(x => x.ZIndex).FirstOrDefault()*/;

                var pipelineWidget = HitItem as IPipelineWidget;
                if (pipelineWidget == null || Name == PipelineManipulationTool.ToolName || Name == TextTool.ToolName)
                {
                    return false;
                }

                var pipelineManipulationTool =
                    ToolService.FindTool(PipelineManipulationTool.ToolName) as PipelineManipulationTool;

                if (ToolService.IsControlDown && pipelineWidget.IsSelected && SelectionService.SelectedItemsCount == 1 ||
                    pipelineManipulationTool?.ActiveManipulationPoint != null)
                {
                    SelectionService.SelectItem(pipelineWidget);

                    ToolService.ActivateTool(PipelineManipulationTool.ToolName);
                }
            }
            return false;
        }

        /// <summary>
        ///     Handles the mouse-move event.
        /// </summary>
        /// <param name="e">The <see cref="T:Telerik.Windows.Diagrams.Core.PointerArgs" /> instance containing the event data.</param>
        public virtual bool MouseMove(PointerArgs e)
        {
            return false;
        }

        /// <summary>
        ///     Handles the mouse-up event.
        /// </summary>
        /// <param name="e">The <see cref="T:Telerik.Windows.Diagrams.Core.PointerArgs" /> instance containing the event data.</param>
        public virtual bool MouseUp(PointerArgs e)
        {
            return false;
        }

        private void QueryControlKey()
        {
            ToolService.IsControlDown = ToolService.IsControlKeyDown();
            ToolService.IsAltDown = ToolService.IsAltKeyDown();
            ToolService.IsShiftDown = ToolService.IsShiftKeyDown();
        }
    }
}