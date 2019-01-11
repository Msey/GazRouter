using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GazRouter.Flobus.FloScheme;
using GazRouter.Flobus.Visuals;
using JetBrains.Annotations;
using Telerik.Windows.Diagrams.Core;
using CommonExtensions = GazRouter.Flobus.Extensions.CommonExtensions;

namespace GazRouter.Flobus.Services
{
    public class SelectionService
    {
        private readonly List<ISchemaItem> _selectedItems = new List<ISchemaItem>();
        private bool _isInternalSelection;

        /// <summary>
        ///     Occurs when [selection ended].
        /// </summary>
        public event EventHandler<DiagramSelectionChangedEventArgs> SelectionChanged;

        public IEnumerable<ISchemaItem> SelectedItems => _selectedItems;

        public IEnumerable<IPipelineWidget> SelectedPipelines => SelectedItems.OfType<IPipelineWidget>();

        public int SelectedItemsCount => _selectedItems.Count;

        public IEnumerable<CompressorShopWidget> SelectedShapes => SelectedItems.OfType<CompressorShopWidget>();

        public void SelectItem(ISchemaItem item, bool addToExistingSelection = false)
        {
            SelectItems(new[] {item}, addToExistingSelection);
        }

        public void DeselectItem(ISchemaItem item)
        {
            DeselectItems(new[] {item});
        }

        public bool IsSingleSelected(ISchemaItem item)
        {
            if (item == null)
            {
                return false;
            }

            return item.IsSelected && SelectedItemsCount == 1;
        }

        public void SelectItems([NotNull] IEnumerable<ISchemaItem> items, bool addToExistingSelection = false)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            if (_isInternalSelection)
            {
                _isInternalSelection = false;
                return;
            }

            var itemsToSelect = items.ToList();

            if (itemsToSelect.Count == 0)
            {
                return;
            }

            var itemsToDeselect = addToExistingSelection
                ? new List<ISchemaItem>()
                : _selectedItems.Except(itemsToSelect).ToList();

            var isSelectionChanged = false;
            if (!addToExistingSelection)
            {
                foreach (var itemToDeselect in itemsToDeselect)
                {
                    _isInternalSelection = true;
                    itemToDeselect.IsSelected = false;
                    if (SyncSelectedItems(itemToDeselect))
                    {
                        isSelectionChanged = true;
                    }
                }
            }

            foreach (var itemToSelect in itemsToSelect.Where(i => i != null))
            {
                _isInternalSelection = true;

                itemToSelect.IsSelected = true;
                if (SyncSelectedItems(itemToSelect))
                {
                    isSelectionChanged = true;
                }
            }

            _isInternalSelection = false;
            if (isSelectionChanged)
            {
                RaiseOnSelectionChanged(itemsToDeselect, itemsToSelect);
            }
        }

        public void ClearSelection()
        {
            DeselectItems(_selectedItems);
        }

        public Rect GetSelectionBounds()
        {
            return CommonExtensions.GetEnclosingBounds(SelectedItems);
        }

        private void DeselectItems(IEnumerable<ISchemaItem> items)
        {
            if (_isInternalSelection)
            {
                _isInternalSelection = false;
                return;
            }

            var itemsToDeselect = items.ToList();
            if (itemsToDeselect.Count == 0)
            {
                return;
            }

            var isSelectionChanged = false;
            foreach (var itemToDeselect in itemsToDeselect)
            {
                _isInternalSelection = true;
                itemToDeselect.IsSelected = false;
                if (SyncSelectedItems(itemToDeselect))
                {
                    isSelectionChanged = true;
                }
            }
            _isInternalSelection = false;

            if (isSelectionChanged)
            {
                RaiseOnSelectionChanged(itemsToDeselect, new List<ISchemaItem>());
            }
        }

        private bool SyncSelectedItems(ISchemaItem model)
        {
            if (model.IsSelected)
            {
                if (!_selectedItems.Contains(model))
                {
                    _selectedItems.Add(model);
                    return true;
                }
            }
            else
            {
                if (_selectedItems.Contains(model))
                {
                    _selectedItems.Remove(model);
                    return true;
                }
            }
            return false;
        }

        private void RaiseOnSelectionChanged(IList removedItems, IList addedItems)
        {
            var handler = SelectionChanged;
            handler?.Invoke(this, new DiagramSelectionChangedEventArgs(removedItems, addedItems));
        }
    }
}