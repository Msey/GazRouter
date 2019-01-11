using System.Collections.Generic;
using System.Linq;
using GazRouter.Flobus.VM.Model;

namespace GazRouter.Flobus.VM.Serialization
{
    public class SchemeJson
    {
        /// <summary>
        ///     Список компрессорных цехов (КЦ)
        /// </summary>
        public List<CompressorShopJson> CompressorShops { get; set; } = new List<CompressorShopJson>();

        /// <summary>
        ///     Список газопроводов
        /// </summary>
        public List<PipelineJson> Pipelines { get; set; } = new List<PipelineJson>();

        /// <summary>
        ///     Список текстовых элементов
        /// </summary>
        public List<TextBlockJson> TextBlocks { get; set; } = new List<TextBlockJson>();

        public List<PolyLineJson> PolyLines { get; set; } = new List<PolyLineJson>();

        public List<CheckValveJson> CheckValves { get; set; } = new List<CheckValveJson>();

        public List<UiEntities.FloModel.PipelineDiameterSegment> DiameterSegments { get; set; } = new List<UiEntities.FloModel.PipelineDiameterSegment>();

        public static SchemeJson FromModel(SchemeViewModel model)
        {
            var schemeJson = new SchemeJson
            {
                CompressorShops = model.CompressorShops.Select(shop => new CompressorShopJson(shop)).ToList(),
                Pipelines = model.Pipelines.Select(pipeline => new PipelineJson(pipeline as Pipeline)).ToList(),
                TextBlocks = model.TextBlocks.Select(d => new TextBlockJson(d as TextBlock)).ToList(),
                PolyLines = model.PolyLines.Select(p=> new PolyLineJson(p as PolyLine)).ToList(),
                CheckValves = model.CheckValves.Select(p => new CheckValveJson(p as CheckValve)).ToList()
            };
            return schemeJson;
        }
    }
}