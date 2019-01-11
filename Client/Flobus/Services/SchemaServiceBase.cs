using GazRouter.Flobus.Model;

namespace GazRouter.Flobus.Services
{
    public abstract class SchemaServiceBase
    {
        protected SchemaServiceBase(IGraph graph)
        {
            Graph = graph;
        }


        public IGraph Graph { get; }
    }
}