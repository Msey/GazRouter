using System;
using System.Linq;
using System.Threading.Tasks;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.ObjectModel.Valves;
using GazRouter.Flobus.UiEntities.FloModel;
using GazRouter.Flobus.VM.Model;

namespace GazRouter.Flobus.VM.FloModel
{
    public static class PipelineLoader
    {
        public static async Task<Pipeline> LoadAsync(Guid id)
        {
            var prov = new ObjectModelServiceProxy();

            var pipeline = await prov.GetPipelineByIdAsync(id);
            var segmentList = await prov.GetDiameterSegmentListAsync(id);
            var valveList = await prov.GetValveListAsync(new GetValveListParameterSet {PipelineId = id});

            if (pipeline == null || segmentList == null || valveList == null)
            {
                return null;
            }

            var result = new Pipeline(pipeline)
            {
                DiameterSegments = segmentList.Select(c => new PipelineDiameterSegment(c)).ToList()
            };
            result.DiameterSegments.Sort();
            valveList.ForEach(v => result.AddValve(v));
            /* result.Valves.Sort();*/

            return result;
        }
    }
}