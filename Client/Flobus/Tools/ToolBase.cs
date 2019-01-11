using System.Windows;
using System.Windows.Input;
using GazRouter.Flobus.Model;
using GazRouter.Flobus.Services;

namespace GazRouter.Flobus.Tools
{
    /// <summary>
    /// Base class for Schema tools.
    /// 
    /// </summary>
    public abstract class ToolBase : ITool
    {
        private bool _isEnabled = true;
        private Cursor _prevCursor;
        private bool _isActive;

        /// <summary>
        /// Gets or sets the tool service (see <see cref="P:Telerik.Windows.Diagrams.Core.ToolBase.ToolService"/>).
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The tool service.
        /// </value>
        public  ToolService ToolService { get; set; }

        /// <summary>
        /// Gets or sets the graph view (aka surface).
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The surface.
        /// </value>
        public IGraph Graph => ToolService.Graph;

        /// <summary>
        /// Gets or sets the IsEnabled.
        /// 
        /// </summary>
        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                if (!value && IsActive)
                    DeactivateTool();
                _isEnabled = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this tool is active.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// <c>True</c> if this instance is active; otherwise, <c>false</c>.
        /// </value>
        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            private set
            {
                if (_isActive == value)
                    return;
                _isActive = value;
                if (_isActive)
                {
                    OnActivated();
                }
                else
                {
                    DeactivateTool();
                    OnDeactivated();
                }
            }
        }

        /// <summary>
        /// Gets or sets the layer.
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the initial point of the interaction.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The initial point on the surface.
        /// </value>
        protected Point? InitialPoint { get; set; }

        /// <summary>
        /// Gets or sets the cursor.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The cursor.
        /// 
        /// </value>
        protected Cursor Cursor { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Telerik.Windows.Diagrams.Core.ToolBase"/> class.
        /// 
        /// </summary>
        /// <param name="name">The name.</param>
        protected ToolBase(string name = "")
        {
            Name = name;
        }

        /// <summary>
        /// Deactivates the tool.
        /// 
        /// </summary>
        /// 
        /// <returns/>
        public bool DeactivateTool()
        {
            if (!IsActive)
                return false;
            IsActive = false;
            RestoreCursor();
            return true;
        }

        /// <summary>
        /// Activates the tool.
        /// 
        /// </summary>
        /// 
        /// <returns/>
        public bool ActivateTool()
        {
            if (IsEnabled && !IsActive)
            {
                _prevCursor = Graph.Cursor;
                IsActive = true;
                Graph.Cursor = Cursor;
            }
            return IsActive;
        }

        /// <summary>
        /// Initializes this tool.
        /// 
        /// </summary>
        /// <param name="serviceLocator">The locator or controller.</param>
        public virtual void Initialize(ServiceLocator serviceLocator)
        {
        }

        /// <summary>
        /// Called when tool is <see cref="P:Telerik.Windows.Diagrams.Core.ToolBase.IsActive"/> is changed to true.
        /// 
        /// </summary>
        protected virtual void OnActivated()
        {
        }

        /// <summary>
        /// Called when tool is <see cref="P:Telerik.Windows.Diagrams.Core.ToolBase.IsActive"/> is changed to false.
        /// 
        /// </summary>
        protected virtual void OnDeactivated()
        {
        }

        /// <summary>
        /// Restores the cursor.
        /// 
        /// </summary>
        protected void RestoreCursor()
        {
            if (_prevCursor == null)
                return;
            Graph.Cursor = _prevCursor;
            _prevCursor = null;
        }
    }
}
