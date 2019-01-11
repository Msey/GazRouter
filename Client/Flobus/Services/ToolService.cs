using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using GazRouter.Flobus.Tools;
using Telerik.Windows.Diagrams.Core;
using DraggingTool = GazRouter.Flobus.Tools.DraggingTool;
using IGraph = GazRouter.Flobus.Model.IGraph;
using IKeyboardListener = GazRouter.Flobus.Tools.IKeyboardListener;
using IMouseListener = GazRouter.Flobus.Tools.IMouseListener;
using ITool = GazRouter.Flobus.Tools.ITool;
using ManipulationTool = GazRouter.Flobus.Tools.ManipulationTool;
using PanningTool = GazRouter.Flobus.Tools.PanningTool;
using PointerTool = GazRouter.Flobus.Tools.PointerTool;
using RectangleSelectionTool = GazRouter.Flobus.Tools.RectangleSelectionTool;
using ToolBase = GazRouter.Flobus.Tools.ToolBase;

namespace GazRouter.Flobus.Services
{
    /// <summary>
    ///     The toolbox, this tells the application which tools are available.
    /// </summary>
    public class ToolService : SchemaServiceBase, IMouseListener, IKeyboardListener
    {
        private readonly ServiceLocator _serviceLocator;
        private string _primaryTool;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Telerik.Windows.Diagrams.Core.ToolService" /> class.
        /// </summary>
        /// <param name="graph">The graph.</param>
        /// <param name="serviceLocator">The service locator.</param>
        public ToolService(IGraph graph, ServiceLocator serviceLocator)
            : base(graph)
        {
            _serviceLocator = serviceLocator;
            ToolList = new ObservableCollection<ITool>();
            ToolList.CollectionChanged += OnToolListCollectionChanged;
            AddDefaultTools();
        }

        /// <summary>
        ///     Gets the active tool.
        /// </summary>
        public ITool ActiveTool
        {
            get { return ToolList.SingleOrDefault(t => t.IsActive); }
        }

        /// <summary>
        ///     Gets the graph.
        /// </summary>
        public new IGraph Graph => base.Graph;

        /// <summary>
        ///     Gets or sets the primary tool.
        /// </summary>
        /// <value>
        ///     The primary tool.
        /// </value>
        public string PrimaryTool
        {
            get { return _primaryTool; }
            set
            {
                if (_primaryTool == value)
                {
                    return;
                }
                _primaryTool = value;
                ActivateTool(_primaryTool);
            }
        }

        /// <summary>
        ///     Gets the tools collection.
        /// </summary>
        public ObservableCollection<ITool> ToolList { get; }

        /// <summary>
        ///     Gets or sets whether the control-key is pressed.
        /// </summary>
        public bool IsControlDown { get; set; }

        /// <summary>
        ///     Gets or sets whether the alt-key is pressed.
        /// </summary>
        public bool IsAltDown { get; set; }

        /// <summary />
        public bool IsMouseDown { get; set; }

        public bool IsShiftDown { get; set; }

        /// <summary>
        ///     Activates the primary tool.
        /// </summary>
        public void ActivatePrimaryTool()
        {
            ActivateTool(_primaryTool);
        }

        /// <summary>
        ///     Returns the tool with the specified name, if it exists.
        /// </summary>
        /// <param name="name" />
        /// <returns />
        public ITool FindTool(string name)
        {
            var tool = ToolList.ToList().FirstOrDefault(i => i.Name == name);
            return tool;
        }

        /// <summary>
        ///     Handles the mouse-down event.
        /// </summary>
        /// <param name="e">The <see cref="T:Telerik.Windows.Diagrams.Core.PointerArgs" /> instance containing the event data.</param>
        /// <returns />
        public bool MouseDown(PointerArgs e)
        {
            IsMouseDown = true;
            return ToolList.OfType<IMouseListener>().Any(tool => tool.MouseDown(e));
        }

        /// <summary>
        ///     Handles the mouse-move event.
        /// </summary>
        /// <param name="e">The <see cref="T:Telerik.Windows.Diagrams.Core.PointerArgs" /> instance containing the event data.</param>
        /// <returns />
        public bool MouseMove(PointerArgs e)
        {
            return ToolList.OfType<IMouseListener>().Any(tool => tool.MouseMove(e));
        }

        /// <summary>
        ///     Handles the mouse-up event.
        /// </summary>
        /// <param name="e">The <see cref="T:Telerik.Windows.Diagrams.Core.PointerArgs" /> instance containing the event data.</param>
        /// <returns />
        public bool MouseUp(PointerArgs e)
        {
            var flag = ToolList.OfType<IMouseListener>().Any(tool => tool.MouseUp(e));
            IsMouseDown = false;
            return flag;
        }

        /// <summary>
        ///     Handles the key down event.
        /// </summary>
        /// <param name="key" />
        /// <returns />
        public bool KeyDown(KeyArgs key)
        {
            return ToolList.OfType<IKeyboardListener>().Any(tool => tool.KeyDown(key));
        }

        /// <summary>
        ///     Handles the key up event.
        /// </summary>
        /// <param name="key" />
        /// <returns />
        public bool KeyUp(KeyArgs key)
        {
            return ToolList.OfType<IKeyboardListener>().Any(tool => tool.KeyUp(key));
        }

        /// <summary>
        ///     Deactivates the given tool.
        /// </summary>
        /// <param name="tool">A registered ITool.</param>
        /// <returns />
        public bool DeactivateTool(ITool tool)
        {
            var flag = false;
            if (tool != null && tool.IsEnabled && tool.IsActive)
            {
                flag = tool.DeactivateTool();
            }
            return flag;
        }

        /// <summary>
        ///     Activates the tool.
        /// </summary>
        /// <param name="toolName">Label of the tool.</param>
        /// <returns />
        public ITool ActivateTool(string toolName)
        {
            var tool = FindTool(toolName);
            ActivateTool(tool);
            return tool;
        }

        internal static bool IsControlKeyDown()
        {
            return (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
        }

        internal static bool IsAltKeyDown()
        {
            return (Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt;
        }

        internal static bool IsShiftKeyDown()
        {
            return (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;
        }

        private void ToolAdded(IEnumerable elements)
        {
            foreach (var tool in elements.Cast<ToolBase>())
            {
                tool.ToolService = this;
                tool.Initialize(_serviceLocator);
            }
        }

        private void OnToolListCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    ToolAdded(e.NewItems);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    ToolAdded(e.NewItems);
                    break;
            }
        }

        /// <summary>
        ///     Activates the given tool.
        /// </summary>
        /// <param name="tool">The tool to activate.</param>
        /// <returns>
        ///     <c>true</c> if the activation was successful, otherwise <c>false</c>.
        /// </returns>
        /// <seealso cref="M:Telerik.Windows.Diagrams.Core.ToolService.DeactivateTool(Telerik.Windows.Diagrams.Core.ITool)" />
        private bool ActivateTool(ITool tool)
        {
            if (tool == null)
            {
                return false;
            }
            ActiveTool?.DeactivateTool();
            return tool.IsEnabled && tool.ActivateTool();
        }

        /// <summary>
        ///     Adds the default set of tools.
        /// </summary>
        private void AddDefaultTools()
        {
            ToolList.Add(new PointerTool());
            /*      this.ToolList.Add((ITool)new PathTool());
            this.ToolList.Add((ITool)new PencilTool());
            this.ToolList.Add((ITool)new TextTool());*/
            ToolList.Add(new PanningTool());
            ToolList.Add(new DraggingTool());
            ToolList.Add(new PipelineManipulationTool());
            ToolList.Add(new RectangleSelectionTool());
            ToolList.Add(new ManipulationTool(ManipulationTool.ToolNameSenw));
            /*   this.ToolList.Add((ITool)new ConnectionTool());
               this.ToolList.Add((ITool)new DraggingTool());
               this.ToolList.Add((ITool)new ConnectionManipulationTool());
               this.ToolList.Add((ITool)new ManipulationTool("Resizing ToolNESW"));

               this.ToolList.Add((ITool)new ManipulationTool("Resizing ToolSENW"));
               this.ToolList.Add((ITool)new ManipulationTool("Resizing ToolSWNE"));
               this.ToolList.Add((ITool)new ManipulationTool("Rotation Tool"));
               this.ToolList.Add((ITool)new RectangleSelectionTool());*/
            PrimaryTool = PointerTool.ToolName;
        }

    }
}