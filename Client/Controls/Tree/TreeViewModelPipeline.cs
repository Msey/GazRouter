using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Common.Cache;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PipelineTypes;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.Boilers;
using GazRouter.DTO.ObjectModel.Pipelines;
using GazRouter.DTO.ObjectModel.PowerUnits;
using GazRouter.DTO.ObjectModel.Valves;
using Microsoft.Practices.ServiceLocation;
using Utils.Extensions;
namespace GazRouter.Controls.Tree
{
    public class TreeViewModelPipeline : TreeViewModelBase
    {
        public void FillTree(TreeDataDTO data, Guid? newEntityId, bool isDeleteCommand = false)
        {
            var selectedId = newEntityId;
            if (!selectedId.HasValue)
            {
                if (SelectedNode != null)
                {
                    if (isDeleteCommand) SelectedNode = SelectedNode.Parent;
                    var node = SelectedNode as EntityNode ?? SelectedNode.Parent as EntityNode;
                    if (node != null) selectedId = node.Entity.Id;
                }
            }
            Nodes.Clear();
            var entityNodes = data.Pipelines.ToDictionary(p => p.Id, CreatePipelineNodeInternal);
            var pipelineTypes = new[]
            {
                PipelineType.Main, PipelineType.Distribution, PipelineType.Looping,
                PipelineType.Inlet, PipelineType.CompressorShopBridge
            };
            foreach (var pipelineType in pipelineTypes)
            {
                var folder = CreatePipelineFolderNode(pipelineType);
                Nodes.Add(folder);
                var nodes = entityNodes.Values.Where(p => ((PipelineDTO)p.Entity).Type == pipelineType)
                                              .OrderBy(p1 => p1.SortOrder).ThenBy(p2 => p2.Name).ToList();
                foreach (var entityNode in nodes)
                    folder.BindNode(entityNode);
            }
            var unbindedPipes = new List<PipelineDTO>();
            foreach (var pipe in entityNodes.Values.Select(v=>v.Entity).Cast<PipelineDTO>().Where(p => !pipelineTypes.Contains(p.Type)))
            {
                var parentId = pipe.Type == PipelineType.CompressorShopOutlet
                                   ? pipe.EndEntityId
                                   : pipe.BeginEntityId;
                EntityNode parentNode;// parent = entityNodes.Keys.SingleOrDefault(p => p.Id == parentId);
                if (parentId.HasValue &&  entityNodes.TryGetValue(parentId.Value, out parentNode))
                    BindPipeline(entityNodes[pipe.Id], parentNode);
                else unbindedPipes.Add(pipe);
            }
            if (unbindedPipes.Count > 0)
            {
                var unbindedFolder = new FolderNode("Непривязанные газопроводы", EntityType.Pipeline);
                Nodes.Add(unbindedFolder);
                foreach (var pipelineNode in unbindedPipes.Select(pipe => entityNodes[pipe.Id]))
                    BindPipeline(pipelineNode, unbindedFolder);
            }
            FillChildrenEntities(entityNodes, data);
            if (selectedId.HasValue) SelectNode(selectedId.Value);
        }
        private static void BindPipeline(EntityNode pipeNode, NodeBase node)
        {
            var type = ((PipelineDTO) pipeNode.Entity).Type;
            var folder =
                node.Children.OfType<PipelineFolderNode>().SingleOrDefault(f => f.PipelineType == type);
            if (folder == null)
            {
                folder = CreatePipelineFolderNode(type);
                node.BindNode(folder);
            }

            folder.BindNode(pipeNode);
        }

        private static PipelineFolderNode CreatePipelineFolderNode(PipelineType pipelineType)
        {
            var folderName = ClientCache.DictionaryRepository.PipelineTypes[pipelineType].Name;
            return new PipelineFolderNode(folderName, EntityType.Pipeline, pipelineType);
        }
        private static IClientCache ClientCache => ServiceLocator.Current.GetInstance<IClientCache>();

        protected virtual EntityNode CreatePipelineNodeInternal(PipelineDTO pipeline)
        {
            return new EntityNode(pipeline)
                {
                    TextTooltip = NodeTooptip(pipeline.BeginEntityId.HasValue, pipeline.EndEntityId.HasValue),
                    SortOrder = pipeline.SortOrder
                };
        }

        protected static string NodeTooptip(bool hasBegin, bool hasEnd)
        {
            if (hasBegin)
            {
                return hasEnd ? "Наличие подключений:\n Начало\nКонец" : "Наличие подключений: Начало";
            }
            return hasEnd ? "Наличие подключений:\n Конец" : "Нет подключений";
        }

        private void FillChildrenEntities(Dictionary<Guid, EntityNode> entityNodes, TreeDataDTO data)
	    {
            //var valves = data.LinearValves.Where(v => v.ParentId == pipelineId).ToList();
            Dictionary<Guid, FolderNode> folderNodes = new Dictionary<Guid, FolderNode>();
            foreach (var valve in data.LinearValves.OrderBy(p=>p.ParentId).ThenBy(p => p.Kilometer))
            {
                if (valve.ParentId == null)
                    continue;
                var node = CreateValveNode(valve);
                FolderNode valveFolder;
                if (folderNodes.ContainsKey(valve.ParentId.Value))
                {
                    valveFolder = folderNodes[valve.ParentId.Value];
                }
                else
                {
                    valveFolder = new FolderNode("Краны", EntityType.Valve);
                    entityNodes[valve.ParentId.Value].BindNode(valveFolder);
                    folderNodes[valve.ParentId.Value] = valveFolder;
                }

                valveFolder.BindNode(node);
            }

            folderNodes.Clear();
            foreach (var boiler in data.Boilers.OrderBy(p => p.ParentId).ThenBy(p=>p.Kilometr).ThenBy(p=>p.Name))
            {
                if (boiler.ParentId == null || !entityNodes.ContainsKey(boiler.ParentId.Value))
                    continue;
                var node = CreateBoilerNode(boiler);
                FolderNode folderNode;
                if (folderNodes.ContainsKey(boiler.ParentId.Value))
                {
                    folderNode = folderNodes[boiler.ParentId.Value];
                }
                else
                {
                    folderNode = new FolderNode("Котлы", EntityType.Boiler);
                    entityNodes[boiler.ParentId.Value].BindNode(folderNode);
                    folderNodes[boiler.ParentId.Value] = folderNode;
                }

                folderNode.BindNode(node);
            }


            folderNodes.Clear();
            
            foreach (var power_unit in data.PowerUnits)
            {
                power_unit.Type = ClientCache.DictionaryRepository.PowerUnitTypes.Single(v => v.Id == power_unit.PowerUnitTypeId).Name;
            }

            foreach (var powerUnitDTO in data.PowerUnits.OrderBy(p => p.ParentId).ThenBy(p => p.Kilometr).ThenBy(p => p.Name).ThenBy(p => p.Type))
            {
                if (powerUnitDTO.ParentId == null)
                    continue;
                var node = CreatePowerUnitNode(powerUnitDTO);
                FolderNode folderNode;
                if (folderNodes.ContainsKey(powerUnitDTO.ParentId.Value))
                {
                    folderNode = folderNodes[powerUnitDTO.ParentId.Value];
                }
                else
                {
                    folderNode = new FolderNode("Электроагрегаты", EntityType.PowerUnit);
                    var parentNode = entityNodes.GetOrDefault(powerUnitDTO.ParentId.Value);
                    if (parentNode != null)
                    {
                        parentNode.BindNode(folderNode);
                    }
                    folderNodes[powerUnitDTO.ParentId.Value] = folderNode;
                }

                folderNode.BindNode(node);
            }

	       
	    }

        protected virtual EntityNode CreatePowerUnitNode(PowerUnitDTO powerUnit)
        {
            var node = new EntityNode(powerUnit)
            {
                SortOrder = powerUnit.SortOrder
            };
            return node;
        }

        protected virtual EntityNode CreateBoilerNode(BoilerDTO boiler)
        {
            var node = new EntityNode(boiler)
            {
                SortOrder = boiler.SortOrder
            };
            return node;
        }

        protected virtual EntityNode CreateValveNode(ValveDTO linarValve)
        {
            var node = new EntityNode(linarValve)
            {
                SortOrder = linarValve.SortOrder
            };
            return node;
        }
    }
}