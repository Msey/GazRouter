using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GazRouter.Application;
using GazRouter.DataProviders.Balances;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Balances.Routes;
using GazRouter.DTO.Dictionaries.BalanceSigns;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.DistrStations;
using GazRouter.DTO.ObjectModel.MeasStations;
using GazRouter.DTO.ObjectModel.OperConsumers;
using GazRouter.DTO.ObjectModel.Sites;

namespace GazRouter.Balances.Routes
{
    public class RoutesData
    {
        public List<SiteDTO> Sites { get; set; }

        public List<MeasStationDTO> MeasStations { get; set; }

        public List<DistrStationDTO> DistrStations { get; set; }

        public List<OperConsumerDTO> OperConsumers { get; set; }

        public List<RouteDTO> Routes { get; set; }

        private int _systemId;
        public static async Task<RoutesData> GetData(int systemId)
        {
            var data = new RoutesData();

            data._systemId = systemId;

            data.Sites = await new ObjectModelServiceProxy().GetSiteListAsync(
                new GetSiteListParameterSet
                {
                    SystemId = systemId,
                    EnterpriseId = UserProfile.Current.Site.Id
                });

            data.MeasStations = await new ObjectModelServiceProxy().GetMeasStationListAsync(
                new GetMeasStationListParameterSet
                {
                    SystemId = systemId
                });

            data.DistrStations = await new ObjectModelServiceProxy().GetDistrStationListAsync(
                new GetDistrStationListParameterSet
                {
                    SystemId = systemId,
                    UseInBalance = true
                });

            data.OperConsumers = await new ObjectModelServiceProxy().GetOperConsumersAsync(
                new GetOperConsumerListParameterSet
                {
                    SystemId = systemId
                });

            data.Routes = await new BalancesServiceProxy().GetRoutesListAsync(
                new GetRouteListParameterSet
                {
                    SystemId = systemId
                });

            return data;
        }


        public List<EntityDTO> GetInletList()
        {
            return
                MeasStations.Where(s => !s.IsIntermediate && s.BalanceSignId == Sign.In)
                    .OfType<EntityDTO>()
                    .ToList();
        }

        public List<EntityDTO> GetOutletList(EntityType eType)
        {
            switch (eType)
            {
                case EntityType.MeasStation:
                    return
                        MeasStations.Where(s => !s.IsIntermediate && s.BalanceSignId == Sign.Out)
                            .OfType<EntityDTO>()
                            .ToList();
                case EntityType.DistrStation:
                    return DistrStations.Where(s => s.UseInBalance).OfType<EntityDTO>().ToList();

                case EntityType.OperConsumer:
                    return OperConsumers.OfType<EntityDTO>().ToList();

                default:
                    return null;
            }
        }

        public int GetRouteCount(Guid pointId)
        {
            return Routes.Count(r => r.InletId == pointId || r.OutletId == pointId);
        }

        public RouteDTO GetRoute(Guid onePt, Guid twoPt)
        {
            return Routes.SingleOrDefault(r => r.InletId == onePt && r.OutletId == twoPt) ??
                   Routes.SingleOrDefault(r => r.InletId == twoPt && r.OutletId == onePt);
        }

        public async Task SetRoute(Guid inletId, Guid outletId, double? len)
        {
            if (len == 0 && !(GetRoute(inletId, outletId)?.HasExceptions ?? false))
                len = null;

            await new BalancesServiceProxy().SetRouteAsync(
                new SetRouteParameterSet
                {
                    InletId = inletId,
                    Length = len,
                    OutletId = outletId
                });

            Routes = await new BalancesServiceProxy().GetRoutesListAsync(
                new GetRouteListParameterSet
                {
                    SystemId = _systemId
                });
        }

        public async Task ReloadRoutes()
        {
            Routes = await new BalancesServiceProxy().GetRoutesListAsync(
                new GetRouteListParameterSet
                {
                    SystemId = _systemId
                });
        }

    }
}