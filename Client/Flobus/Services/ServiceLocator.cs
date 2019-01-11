namespace GazRouter.Flobus.Services
{
    public class ServiceLocator
    {
        public ServiceLocator(Schema schema)
        {
            HitTestService = new HitTestService(schema);
            SelectionService = new SelectionService();
            ToolService = new ToolService(schema, this);
            DraggingService = new DraggingService(schema);
            ManipulationPointService = new ManipulationPointService(schema);
            AdornerService = new AdornerService(schema);
            VirtualzationService = new VirtualizationService(schema);
            SnappingService = new SnappingService(schema);
            ResizingService = new ResizingService(schema);
        }

        public SelectionService SelectionService { get; }
        public DraggingService DraggingService { get; }
        public ManipulationPointService ManipulationPointService { get; private set; }
        public HitTestService HitTestService { get; private set; }
        public ToolService ToolService { get; private set;  }
        public AdornerService AdornerService { get; private set; }
        public VirtualizationService VirtualzationService { get; private set; }
        public SnappingService SnappingService { get; private set; }
        public ResizingService ResizingService { get;  }

    }
}