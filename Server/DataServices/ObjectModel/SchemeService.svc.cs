using System.Collections.Generic;
using GazRouter.DataServices.Infrastructure.Attributes.Behaviors;
using GazRouter.DataServices.Infrastructure.Services;
using GazRouter.DAL.Appearance;
using GazRouter.DAL.Appearance.Positions;
using GazRouter.DAL.Appearance.Styles;
using GazRouter.DAL.Appearance.Versions;
using GazRouter.DAL.ObjectModel.CompShops;
using GazRouter.DAL.ObjectModel.CompUnits;
using GazRouter.DAL.ObjectModel.DistrStations;
using GazRouter.DAL.ObjectModel.MeasLine;
using GazRouter.DAL.ObjectModel.PipelineConns;
using GazRouter.DAL.ObjectModel.Pipelines;
using GazRouter.DAL.ObjectModel.ReducingStations;
using GazRouter.DAL.ObjectModel.Valves;
using GazRouter.DTO.Appearance;
using GazRouter.DTO.Appearance.Versions;
using GazRouter.DTO.ObjectModel.CompShops;
using GazRouter.DTO.ObjectModel.CompUnits;
using GazRouter.DTO.ObjectModel.DistrStations;
using GazRouter.DTO.ObjectModel.MeasLine;
using GazRouter.DTO.ObjectModel.PipelineConns;
using GazRouter.DTO.ObjectModel.Pipelines;
using GazRouter.DTO.ObjectModel.ReducingStations;
using GazRouter.DTO.ObjectModel.Valves;
using GazRouter.DAL.ObjectModel.Segment.Diameter;
using GazRouter.DAL.ObjectModel.DistrStationOutlets;
using System.Linq;

namespace GazRouter.DataServices.ObjectModel
{
    [Authorization]
    [ErrorHandlerLogger("mainLogger")]
    public class SchemeService : ServiceBase, ISchemeService
    {
        /// <summary>
        ///     Добавляет схему и версию схемы в БД
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns>Возвращается id версии схемы</returns>
        public int AddScheme(SchemeParameterSet parameters)
        {
            using (var context = OpenDbContext())
            {
                var schemeId = new AddSchemeCommand(context).Execute(parameters);
                var versionId = new AddSchemeVersionCommand(context).Execute(new SchemeVersionParameterSet
                {
                    SchemeId = schemeId,
                    Description = parameters.Description
                });
                return versionId;
            }
        }

        public List<SchemeVersionItemDTO> GetSchemeVersionList()
        {
            return ExecuteRead<GetSchemeVersionListQuery, List<SchemeVersionItemDTO>>();
        }

        public List<SchemeVersionItemDTO> GetPublishedSchemeVersionList()
        {
            return ExecuteRead<GetPublishedSchemeVersionListQuery, List<SchemeVersionItemDTO>>();
        }

        /// <summary>
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public SchemeVersionDTO GetSchemeVersionById(int parameters)
        {
            using (var context = OpenDbContext())
            {
                var scheme = new GetSchemeVersionByIdQuery(context).Execute(parameters);
                if (scheme != null)
                {
                    scheme.Positions = new GetVersionedPositionListQuery(context).Execute(scheme.Id);
                    scheme.Styles = new GetStyleListQuery(context).Execute(scheme.Id);
                }
                return scheme;
            }
        }

        public int AddSchemeVersion(SchemeVersionParameterSet parameters)
        {
            int versionId;
            using (var context = OpenDbContext())
            {
                versionId = new AddSchemeVersionCommand(context).Execute(parameters);
            }
            return versionId;
        }

        public void DeleteSchemeVersion(int parameters)
        {
            ExecuteNonQuery<DeleteSchemeVersionCommand, int>(parameters);
        }

        public void PublishSchemeVersion(int parameters)
        {
            ExecuteNonQuery<PublishSchemeVersionCommand, int>(parameters);
        }

        public void UnPublishSchemeVersion(int parameters)
        {
            ExecuteNonQuery<UnPublishSchemeVersionCommand, int>(parameters);
        }

        public SchemeModelDTO GetFullSchemeModel(int parameters)
        {
            var schemeVersionId = parameters;

            var result = new SchemeModelDTO();
            using (var context = OpenDbContext())
            {
                result.SchemeVersion = new GetSchemeVersionByIdQuery(context).Execute(schemeVersionId);

                if (result.SchemeVersion == null)
                {
                    return result;
                }
                result.SchemeVersion.Positions = new GetVersionedPositionListQuery(context).Execute(result.SchemeVersion.Id);
                result.SchemeVersion.Styles = new GetStyleListQuery(context).Execute(result.SchemeVersion.Id);

                var systemId = result.SchemeVersion.SystemId;
                result.PipelineList =
                    new GetPipelineListQuery(context).Execute(new GetPipelineListParameterSet {SystemId = systemId});
                result.DiameterSegments =
                    new GetDiameterSegmentListQuery(context).Execute(null);
                result.ValveList =
                    new GetValveListQuery(context).Execute(new GetValveListParameterSet {SystemId = systemId});
                result.DistrStationList =
                    new GetDistrStationListQuery(context).Execute(new GetDistrStationListParameterSet
                    {
                        SystemId = systemId
                    });
                var outlets = new GetDistrStationOutletListQuery(context).Execute(new DTO.ObjectModel.DistrStationOutlets.GetDistrStationOutletListParameterSet { SystemId = systemId });
                foreach (var ds in result.DistrStationList)
                    ds.Outlets = outlets.Where(o => o.ParentId == ds.Id).ToList();
                result.MeasLineList =
                    new GetMeasLineListQuery(context).Execute(new GetMeasLineListParameterSet {SystemId = systemId});
                result.CompShopList =
                    new GetCompShopListQuery(context).Execute(new GetCompShopListParameterSet {SystemId = systemId});
                result.ReducingStationList =
                    new GetReducingStationListQuery(context).Execute(new GetReducingStationListParameterSet
                    {
                        SystemId = systemId
                    });
                result.PipelineConnList =
                    new GetPipelineConnListQuery(context).Execute(new GetPipelineConnListParameterSet
                    {
                        GasTrasportSystemId = systemId
                    });
                result.CompUnitList =
                    new GetCompUnitListQuery(context).Execute(new GetCompUnitListParameterSet {SystemId = systemId});
            }
            return result;
        }

        public void CommentSchemeVersion(CommentSchemeVersionParameterSet parameters)
        {
            ExecuteNonQuery<EditSchemeVersionCommentCommand, CommentSchemeVersionParameterSet>(parameters);
        }
    }
}