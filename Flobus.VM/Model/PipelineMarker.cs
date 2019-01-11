using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using GazRouter.Common.ViewModel;
using GazRouter.DTO.ObjectModel;
using GazRouter.Flobus.Interfaces;
using GazRouter.Flobus.Model;
using GazRouter.Flobus.UiEntities;
using JetBrains.Annotations;

namespace GazRouter.Flobus.VM.Model
{
    public class PipelineMarker : EntityBase<CommonEntityDTO>
    {
/*
        public PipelineMarker(PipelineSection section, Color color, string descr, object data)
            : base(null)
        {
            Section = section;
            Color = color;
            Description = descr;
            Data = data;
        }
*/

        public PipelineMarker([NotNull] IPipeline pipe, double kmBegining, double kmEnd, Color color, string descr,
            object data)
            : base(null)
        {
            if (pipe == null)
            {
                throw new ArgumentNullException(nameof(pipe));
            }

        /*    pipe.PipelineMarkers.Add(this);
            Section = new PipelineSection(pipe, kmBegining, kmEnd);
            */

            KmBegining = kmBegining;
            KmEnd = kmEnd;
            Color = color;
            Description = descr;
            Data = data;
        }

        /// <summary>
        ///     Газопровод
        /// </summary>
        public PipelineSection Section { get; }

        public double KmBegining { get; set; }
        public double KmEnd { get; set; }

        /// <summary>
        ///     Цвет маркера
        /// </summary>
        public Color Color { get; }

        /// <summary>
        ///     Описание к маркеру
        /// </summary>
        public string Description { get; }

        public object Data { get; }

        public Point AnnotationAnchorPoint => Support2D.BisectingPoint(Section.Points[0], Section.Points[1]);

//        public List<MenuItemCommand> MenuCommands { get; set; }
    }
}