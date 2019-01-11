using System.Windows.Input;
using Telerik.Windows.Diagrams.Core;

namespace GazRouter.Flobus.Tools
{
    /// <summary>
    /// The default diagram pointer tool.
    /// 
    /// </summary>
    public class PointerTool : PrimaryMouseTool
    {
        /// <summary>
        /// The name of the Pointer tool.
        /// 
        /// </summary>
        public const string ToolName = "Pointer Tool";

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Telerik.Windows.Diagrams.Core.PointerTool"/> class.
        /// 
        /// </summary>
        public PointerTool()
            : base(ToolName)
        {
            Cursor = DiagramCursors.Pointer;
        }

        /// <inheritdoc/>
        public override bool MouseDown(PointerArgs e)
        {
            if (!IsActive)
                return false;
            return base.MouseDown(e);
        }


        /// <summary>
        /// Handles the mouse-move event.
        /// 
        /// </summary>
        /// <param name="e">The <see cref="T:Telerik.Windows.Diagrams.Core.PointerArgs"/> instance containing the event data.</param>
        public override bool MouseMove(PointerArgs e)
        {
            if (!IsActive)
                return false;
            base.MouseMove(e);
            return false;
        }

        /// <summary>
        /// Handles the mouse-up event.
        /// 
        /// </summary>
        /// <param name="e">The <see cref="T:Telerik.Windows.Diagrams.Core.PointerArgs"/> instance containing the event data.</param>
        public override bool MouseUp(PointerArgs e)
        {
            if (IsActive && HitItem == null && ToolService.IsMouseDown)
                SelectionService.ClearSelection();
            return base.MouseUp(e);
        }

        /// <summary>
        /// Handles the key down event.
        /// 
        /// </summary>
        /// <param name="key"/>
        /// <returns/>
        public override bool KeyDown(KeyArgs key)
        {
            bool flag = base.KeyDown(key);
            if (!IsActive || key.Key != Key.Escape || SelectionService.SelectedItemsCount == 0)
                return flag;
            SelectionService.ClearSelection();
            return true;
        }
    }
}
