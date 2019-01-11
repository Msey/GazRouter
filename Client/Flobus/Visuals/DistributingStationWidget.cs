using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.Flobus.Extensions;
using GazRouter.Flobus.Interfaces;
using GazRouter.Flobus.UiEntities.FloModel;
using Telerik.Windows.Controls;

using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;
using GazRouter.Flobus.Misc;

namespace GazRouter.Flobus.Visuals
{
    /// <summary>
    ///     Визуальный компонент для ГРС
    /// </summary>
    public class DistributingStationWidget : PipelineOmElementWidgetBase
    {
        private bool _isSelected;

        public DistributingStationWidget(PipelineWidget pipelineWidget, IDistrStation ds) : base(pipelineWidget, ds)
        {
        }

        public override EntityType EntityType => EntityType.DistrStation;

        public override void ShowHideKm(bool show)
        {
        }

        protected override void UpdateLabelLocation(FrameworkElement label)
        {
            if (label == null)
            {
                return;
            }

            if (Orientation == Orientation.Vertical)
            {
                if (TextAngle == VerticalAngle)
                {
                    label.SetLocation(20, label.ActualWidth / 2 + (Angle == 90 ? 10 : 0));
                }
                else
                {
                    label.SetLocation(15, 10 - label.ActualHeight/2 + (Angle == 270 ? -10 : 0));
                }
            }
            else
            {
                if (TextAngle == VerticalAngle)
                {
                    label.SetLocation(10 - label.ActualHeight / 2 + (Angle == 180 ? -10 : 0), -5);
                }
                else
                {
                    label.SetLocation(-label.ActualWidth/2  + (Angle == 0 ? 10 : 0), -label.ActualHeight - 5);
                }
            }


        }

        protected override void CreateBindings()
        {
            base.CreateBindings();
            //            SetBinding(ContentProperty, new Binding(nameof(IDistrStation.Data)) {Mode = BindingMode.OneWay});
        }

        protected override void UpdateContainerLocation(Grid container)
        {
            if (container == null)
            {
                return;
            }
            if (Orientation == Orientation.Vertical)
            {
                container.SetLocation(-container.ActualWidth - 5, -container.ActualHeight / 2 + MinHeight / 2);
            }
            else
            {
                container.SetLocation(-container.ActualWidth / 2 + MinWidth / 2, 15);
            }
        }

        private void UpdateVisualStates()
        {
            VisualStateManager.GoToState(this, _isSelected ? VisualStates.Selected : VisualStates.Unselected, true);
        }

        public override bool Deleteble()
        {
            return true;
        }

        public override bool DeleteCommand()
        {
            return Schema.SchemaSource.RemoveDistributionStation(this.Id);
        }

        public override void FillMenu(RadContextMenu menu, Point mousePosition, Schema schema)
        {
            base.FillMenu(menu, mousePosition, schema);
           

            if (Schema.IsRepair)
            {

                menu.AddCommand("Запланировать ремонт...",
                new DelegateCommand(
                    () =>
                    {
                        Schema.InvokeDialogWindowCall(Id);
                    }));
            }
        }
    }
}