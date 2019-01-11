using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.Flobus.Extensions;
using GazRouter.Flobus.Interfaces;
using GazRouter.Flobus.UiEntities.FloModel;
using Telerik.Windows.Controls;
using GazRouter.Flobus.Misc;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;
using System;

namespace GazRouter.Flobus.Visuals
{
    /// <summary>
    ///     Визуальный компонент для отображения линий замера газа
    /// </summary>
    public class MeasuringLineWidget : PipelineOmElementWidgetBase
    {
        public MeasuringLineWidget(PipelineWidget pw, IPipelineOmElement el) : base(pw, el)
        {
        }

        public override bool Deleteble()
        {
            return true;
        }

        public override bool DeleteCommand()
        {
            return Schema.SchemaSource.RemoveMeasuringLine(this.Id);
        }

        public override EntityType EntityType => EntityType.MeasLine;

        public override void ShowHideKm(bool show)
        {
        }

        protected override void UpdateContainerLocation(Grid container)
        {
            if (container == null)
            {
                return;
            }

            if (Orientation == Orientation.Vertical)
            {
                container.SetLocation(-container.ActualWidth - 5, -container.ActualHeight/2 + Height/2);
            }
            else
            {
                container.SetLocation(ActualWidth, ActualHeight);
            }
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
                    label.SetLocation(15, label.ActualWidth/2 - Height/2);
                }
                else
                {
                    label.SetLocation(15, -(label.ActualHeight/2 - Height/2));
                }
            }
            else
            {
                if (TextAngle == VerticalAngle)
                {
                    label.SetLocation((ActualWidth - label.ActualHeight)/2, -ActualWidth);
                }
                else
                {
                    label.SetLocation((ActualWidth - label.ActualWidth)/2, -label.ActualHeight);
                }
            }
        }        
    }
}