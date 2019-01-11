using System.Linq;
using GazRouter.Flobus.FloScheme;
using GazRouter.Flobus.Visuals;
using Telerik.Windows.Diagrams.Core;

namespace GazRouter.Flobus.Tools
{
    /// <summary>
    ///     An abstract base tool for mouse tools.
    /// </summary>
    public abstract class PrimaryMouseTool : PrimaryMouseToolBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Telerik.Windows.Diagrams.Core.PrimaryMouseTool" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        protected PrimaryMouseTool(string name)
            : base(name)
        {
        }

        /// <summary>
        ///     Handles the mouse-move event.
        /// </summary>
        /// <param name="e">The <see cref="T:Telerik.Windows.Diagrams.Core.PointerArgs" /> instance containing the event data.</param>
        public override bool MouseMove(PointerArgs e)
        {
            var baseResult = base.MouseMove(e);
            if (ToolService.IsMouseDown && HitItem != null)
            {
                var selectionMode = Graph.SelectionMode;
                if (selectionMode != SelectionMode.None)
                {
                    var schemaItem = HitItem as ISchemaItem;
                    if (schemaItem != null && !schemaItem.IsSelected)

                    {
                        var addToExistingSelection = selectionMode != SelectionMode.Single &&
                                                     (ToolService.IsControlDown ||
                                                      selectionMode == SelectionMode.Multiple);
                        SelectWithGroups(schemaItem, addToExistingSelection);
                    }

                    var draggingEnabled = SelectionService.SelectedItems.All(d => d.IsDraggingEnabled);
                    if (draggingEnabled && ToolService.IsShiftDown)
                    {
                        ToolService.ActivateTool(DraggingTool.ToolName);
                        return false;
                    }
                }
            }
            if (IsActive)
            {
                ActivateSecondaryTools(e);
            }
            return baseResult;
        }

        /// <summary>
        ///     Handles the mouse-up event.
        /// </summary>
        /// <param name="e">The <see cref="T:Telerik.Windows.Diagrams.Core.PointerArgs" /> instance containing the event data.</param>
        public override bool MouseUp(PointerArgs e)
        {
            var baseResult = base.MouseUp(e);
            var schemaItem = HitItem as ISchemaItem;
            if (IsActive && schemaItem != null && Graph.SelectionMode != SelectionMode.None)
            {
                if (schemaItem.IsSelected && Graph.SelectionMode != SelectionMode.Single
                    && (ToolService.IsControlDown || Graph.SelectionMode == SelectionMode.Multiple))
                {
                    SelectionService.DeselectItem(schemaItem);
                }
                else
                {
                    var addToExistingSelection = Graph.SelectionMode == SelectionMode.Extended &&
                                                 ToolService.IsControlDown ||
                                                 Graph.SelectionMode == SelectionMode.Multiple;
                    SelectWithGroups(schemaItem, addToExistingSelection);
                }
            }
            Graph.IsMouseCaptured = false;
            HitItem = null;
            return baseResult;
        }

        /// <summary>
        ///     Activates a secondary tools.
        /// </summary>
        protected void ActivateSecondaryTools(PointerArgs e)
        {
            if (ToolService.IsMouseDown)
            {
                if ((ToolService.IsControlDown && HitItem == null && Graph.SelectionMode != SelectionMode.Single &&
                     Graph.SelectionMode != SelectionMode.None) || ToolService.IsShiftDown)
                {
                    ToolService.ActivateTool(RectangleSelectionTool.ToolName);
                }
                else if (Graph.IsPanEnabled)
                {
                    ToolService.ActivateTool(PanningTool.ToolName);
                }
            }
            else if (SelectionService.SelectedItemsCount > 0)
            {
                var selectedTextwidgets = SelectionService.SelectedItems.OfType<ShapeWidgetBase>();
                if (selectedTextwidgets.Any() &&
                    AdornerService.BottomRight().AroundPoint(e.TransformedPoint, AdornerService.ResizeActivationRadius))
                {
                    ToolService.ActivateTool(ManipulationTool.ToolNameSenw);
                }
            }
        }

        private void SelectWithGroups(ISchemaItem item, bool addToExistingSelection)
        {
            SelectionService.SelectItems(new[] {item}, addToExistingSelection);
        }
    }
}