using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.DAL.Balances.Contracts;
using GazRouter.DAL.Balances.GasOwners;
using GazRouter.DAL.Balances.Routes;
using GazRouter.DAL.Balances.Transport;
using GazRouter.DAL.Balances.Values;
using GazRouter.DAL.Core;
using GazRouter.DTO.Balances.Contracts;
using GazRouter.DTO.Balances.Routes;
using GazRouter.DTO.Balances.Transport;
using GazRouter.DTO.Balances.Values;
using GazRouter.DTO.Dictionaries.BalanceItems;
using KingKong;

namespace GazRouter.DataServices.Balances.Algorithms.Transport
{
    public class TransportAlgorithm
    {
        public static bool Run(ExecutionContextReal context, int contractId)
        {
            var coef = 1000.0;
            
            var contract = (new GetContractListQuery(context).Execute(
                new GetContractListParameterSet { ContractId = contractId })).FirstOrDefault();

            if (contract == null) return false;

            var owners = new GetGasOwnerListQuery(context).Execute(contract.SystemId);
            var values =
                new GetBalanceValueListQuery(context).Execute(
                    new GetBalanceValueListParameterSet {ContractId = contract.Id});
            var routeMap = new RouteMap(new GetRouteListQuery(context).Execute(
                new GetRouteListParameterSet {SystemId = contract.SystemId}));

            foreach (var owner in owners)
            {
                // ПОДГОТОВКА ДАННЫХ ///////////////////
                var ownerValues = values.Where(v => v.GasOwnerId == owner.Id).ToList();


                // ОЧИСИТЬ ЗАПАС
                foreach (var value in ownerValues.Where(v => v.BalanceItem == BalanceItem.GasSupply))
                {
                    new SetBalanceValueCommand(context).Execute(
                        new SetBalanceValueParameterSet
                        {
                            ContractId = value.ContractId,
                            BalanceItem = value.BalanceItem,
                            GasOwnerId = value.GasOwnerId,
                            EntityId = value.EntityId,
                            BaseValue = value.BaseValue,
                            Correction = 0
                        });
                }
                
                // Создаем перечень входных точек
                var inlets = ownerValues
                    .Where(v => v.BalanceItem == BalanceItem.Intake && (v.Correction ?? v.BaseValue) > 0)
                    .Select(v => 
                        new InOutPoint
                        {
                            EntityId = v.EntityId,
                            Volume = (long) ((v.Correction ?? v.BaseValue) * coef)
                        }).ToList();

                // Добавляем в объем по входам имеющийся запас газа
                foreach (var value in ownerValues.Where(v => v.BalanceItem == BalanceItem.GasSupply && v.BaseValue > 0))
                {
                    var inlet = inlets.SingleOrDefault(i => i.EntityId == value.EntityId);
                    if (inlet == null)
                    {
                        // Если по точке входа поступления нет, а запас есть, то ее нужно создать
                        inlet = new InOutPoint { EntityId = value.EntityId };
                        inlets.Add(inlet);
                    }
                    inlet.Volume += (long) (value.BaseValue * coef);
                }

                

                if (inlets.Count == 0) continue;

                // Создаем перечень выходов
                var outlets = ownerValues
                    .Where(
                        v =>
                            v.BalanceItem != BalanceItem.Intake && 
                            v.BalanceItem != BalanceItem.GasSupply &&
                            (v.Correction ?? v.BaseValue) > 0)
                    .Select(v =>
                        new InOutPoint
                        {
                            EntityId = v.EntityId,
                            OutEntityId = v.DistrStationId,
                            BalItem = v.BalanceItem,
                            Volume = (long) ((v.Correction ?? v.BaseValue)*coef)
                        }).ToList();

                var delta = inlets.Sum(i => i.Volume) - outlets.Sum(o => o.Volume);
                if (delta < 0) continue;

                // Создаем фиктивную точку, для распределения остатка
                if (delta > 0)
                    outlets.Add(
                        new InOutPoint
                        {
                            BalItem = BalanceItem.PipePlus,
                            Volume = delta
                        });

                if (outlets.Count == 0) continue;


                var vectIn = inlets.Select(i => i.Volume).ToArray();
                var vectOut = outlets.Select(i => i.Volume).ToArray();
                
                var matrCost = new long[inlets.Count, outlets.Count];
                for (var i = 0; i < inlets.Count; i++)
                {
                    for (var j = 0; j < outlets.Count; j++)
                    {
                        if (outlets[j].BalItem == BalanceItem.PipePlus)
                        {
                            matrCost[i,j] = 0;
                            continue;
                        }

                        if (outlets[j].BalItem == BalanceItem.AuxCosts)
                        {
                            matrCost[i, j] = routeMap.GetAvgLen() ?? 0;
                            continue;
                        }

                        matrCost[i, j] =
                            routeMap.GetLen(inlets[i].EntityId, outlets[j].OutEntityId ?? outlets[j].EntityId, owner.Id) ??
                            long.MaxValue;
                    }
                }
                // ПРОВЕРКИ
                var inTestResult = TestIn(vectIn, vectOut, matrCost);
                var outTestResult = TestOut(vectIn, vectOut, matrCost);
                
                // ЗАПУСК АЛГОРИТМА ///////////////////
                var result = new TransportTask(vectIn, vectOut, matrCost).RunOptimization();



                // СОХРАНЕНИЕ РЕЗУЛЬТАТА ///////////////////
                foreach (var inlet in inlets)
                {
                    var i = inlets.IndexOf(inlet);
                    foreach (var outlet in outlets)
                    {
                        var j = outlets.IndexOf(outlet);
                        
                        var vol = result[i, j];
                        if (!vol.HasValue) continue;

                        if (outlet.BalItem == BalanceItem.PipePlus)
                        {
                            // Запас газа на начало месяца
                            var baseValue =
                                ownerValues.SingleOrDefault(
                                    v => v.BalanceItem == BalanceItem.GasSupply && v.EntityId == inlet.EntityId)?.BaseValue ?? 0;

                            // Сохранение запаса на конец месяца
                            new SetBalanceValueCommand(context).Execute(
                                new SetBalanceValueParameterSet
                                {
                                    ContractId = contract.Id,
                                    GasOwnerId = owner.Id,
                                    EntityId = inlet.EntityId,
                                    BalanceItem = BalanceItem.GasSupply,
                                    BaseValue = baseValue,
                                    Correction = vol.Value/coef
                                });
                            continue;
                        }
                        
                        new AddTransportCommand(context).Execute(
                            new AddTransportParameterSet
                            {
                                ContractId = contractId,
                                OwnerId = owner.Id,
                                InletId = inlet.EntityId,
                                OutletId = outlet.EntityId,
                                BalanceItem = outlet.BalItem,
                                Volume = vol.Value / coef,
                                Length = matrCost[i,j] / coef,
                                RouteId = routeMap.GetRouteId(inlet.EntityId, outlet.OutEntityId ?? outlet.EntityId),
                                
                            });
                    }
                }
            }

            
            

            return true;
        }

        public static bool TestIn(long[] vectIn, long[] vectOut, long[,] matrCost)
        {
            for (var i = 0; i < vectIn.Length; i++)
            {
                var sum = vectOut.Where((t, j) => matrCost[i, j] < long.MaxValue).Sum();
                if (vectIn[i] > sum) return false;
            }

            return true;
        }

        public static bool TestOut(long[] vectIn, long[] vectOut, long[,] matrCost)
        {
            for (var j = 0; j < vectOut.Length; j++)
            {
                var sum = vectIn.Where((t, i) => matrCost[i, j] < long.MaxValue).Sum();
                if (vectOut[j] > sum) return false;
            }

            return true;
        }


        public class InOutPoint
        {
            public Guid EntityId { get; set; }

            // Для потребителей, т.к. маршрут строится до ГРС. 
            // Т.е. EntityId будет содержать потребителя (подключение ГРС), 
            // а OutEntityId - ГРС, к которой подключен данный потребитель, для нахождения маршрута.
            public Guid? OutEntityId { get; set; }

            public long Volume { get; set; }

            public BalanceItem BalItem { get; set;}
        }

       
        public class RouteMap : List<RouteDTO>
        {
            public RouteMap(List<RouteDTO> rl)
            {
                AddRange(rl);
            }
            public long? GetLen(Guid srcId, Guid destId, int ownerId)
            {
                var route = this.SingleOrDefault(r => r.InletId == srcId && r.OutletId == destId);
                if (route == null) return null;

                var len = (route.ExceptionList.SingleOrDefault(e => e.OwnerId == ownerId)?.Length ?? route.Length) * 1000;

                return (long?)(len == 0 ? null : len);

            }

            public int? GetRouteId(Guid srcId, Guid destId)
            {
                return this.SingleOrDefault(r => r.InletId == srcId && r.OutletId == destId)?.RouteId;
            }

            public int? GetAvgLen()
            {
                return (int?) this.Average(r => r.Length)*1000;
            }
        }
    }
}

