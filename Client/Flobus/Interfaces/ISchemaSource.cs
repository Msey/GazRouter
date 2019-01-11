using System;
using System.Collections.Generic;
using System.Windows;
using GazRouter.DTO.Appearance;
using GazRouter.DTO.ObjectModel.CompShops;
using GazRouter.DTO.ObjectModel.Pipelines;
using GazRouter.Flobus.Model;

namespace GazRouter.Flobus.Interfaces
{
    public interface ISchemaSource
    {
        IList<IPipeline> Pipelines { get; }
        SchemeModelDTO Dto { get; set; }
        IEnumerable<ICompressorShop> CompressorShops { get; }
        IEnumerable<ITextBlock> TextBlocks { get; }
        IEnumerable<IPolyLine> PolyLines { get; }
        IEnumerable<ICheckValve> CheckValves { get; }
        SchemeVersion SchemeInfo { get; }
        IEnumerable<IDistributingStation> DistributingStations { get; }

        [Obsolete]
        IPipeline AddPipeline(PipelineDTO dto, Point position);
        IPipeline AddPipeline(PipelineDTO dto, Point startPoint, Point endPoint);
        void RemovePipeline(IPipeline data);
        void RemoveCheckValve(ICheckValve data);
        void RemoveTextBlock(ITextBlock data);
        void RemovePolyLine(IPolyLine data);
        void RemoveCompressorShops(Guid id);
        bool RemoveMeasuringLine(Guid id);
        bool RemoveReducingStation(Guid id);
        bool RemoveDistributionStation(Guid id);
        Guid RestoreMeasuringLine(Guid id);
        Guid RestoreDistributionStation(Guid id);
        Guid RestoreReducingStation(Guid id);
        bool IsMeasuringLineHidden(Guid id);
        bool IsDistributionStationHidden(Guid id);
        bool IsReducingStationHidden(Guid id);
        Dictionary<IPipelineOmElement, string> GetHiddenMeasuringLines(Guid id);
        Dictionary<IDistrStation, string> GetHiddenDistributionStations(Guid id);
        Dictionary<IPipelineOmElement, string> GetHiddenReducingStations(Guid id);
        ICompressorShop AddCompressorShops(CompShopDTO dto, Point position);
        ITextBlock AddTextBlock(string text, Point position);
        IPolyLine AddPolyLine(Point position);
        ICheckValve AddCheckValve(Point position);
    }
}