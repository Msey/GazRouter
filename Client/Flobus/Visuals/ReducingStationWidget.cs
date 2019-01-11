using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.Flobus.Extensions;
using GazRouter.Flobus.Interfaces;
using GazRouter.Flobus.UiEntities.FloModel;

namespace GazRouter.Flobus.Visuals
{
    /// <summary>
    ///     Визуальный компонент для отображения ПРГ
    /// </summary>
    public class ReducingStationWidget : PipelineOmElementWidgetBase
    {
        public ReducingStationWidget(PipelineWidget pw, IPipelineOmElement el) : base(pw, el)
        {
        }

        public override EntityType EntityType => EntityType.ReducingStation;

        public override bool Deleteble()
        {
            return true;
        }
        public override bool DeleteCommand()
        {
            return Schema.SchemaSource.RemoveReducingStation(this.Id);
        }

        public void Update()
        {
            Visibility = Visibility.Visible;
//            UpdateDisplayElement();
        }

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
                    label.SetLocation((ActualWidth - label.ActualHeight) / 2, -ActualHeight);
                }
                else
                {
                    label.SetLocation((ActualWidth - label.ActualWidth) / 2, -ActualHeight);
                }
             
            }
        }

/*
        protected override void UpdateDisplayElement()
        {
//          Position = Data.PipelinePosition.Position;

            if (_figure != null)
            {
                RenderTransform = new TranslateTransform {X = -ActualWidth/2, Y = -ActualHeight/2};
            }

            
/*
            if (_captionTextBlock != null)
            {
                Canvas.SetLeft(_captionTextBlock, (ActualWidth - _captionTextBlock.ActualWidth)/2);
            }
#1#
        }
*/

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
                container.SetLocation(-container.ActualWidth/2 + Width/2, 15);
            }
        }
    }
}