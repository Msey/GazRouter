using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GazRouter.DTO.ObjectModel;
using GazRouter.Flobus.Model;
using GazRouter.Flobus.UiEntities.FloModel;

namespace GazRouter.Flobus.VM.Model
{
    public class PipelineSection : EntityBase<CommonEntityDTO>
    {
        private double _kmBegining;
        private double _kmEnd;

        public PipelineSection(Pipeline pipe, double kmBegining, double kmEnd)
            : base(null)
        {
            Pipe = pipe;
            _kmBegining = kmBegining;
            _kmEnd = kmEnd;

            EvaluatePoints();
            EvaluateGeometry();
        }

        /// <summary>
        ///     Газопровод
        /// </summary>
        public Pipeline Pipe { get; }

        /// <summary>
        ///     Километр начала
        /// </summary>
        public double KmBegining
        {
            get { return _kmBegining; }
            set
            {
                _kmBegining = value;
                EvaluatePoints();
                EvaluateGeometry();
            }
        }

        /// <summary>
        ///     Километр конца
        /// </summary>
        public double KmEnd
        {
            get { return _kmEnd; }
            set
            {
                _kmEnd = value;
                EvaluatePoints();
                EvaluateGeometry();
            }
        }

        public List<Point> Points { get; } = new List<Point>();

        /// <summary>
        ///     Список сегментов по диаметру
        /// </summary>
        public List<PipelineDiameterSegment> DiameterSegments { get; set; }

        /// <summary>
        ///     Геометрический объем участка (сумма геом. объемов его сегоментов), м³
        /// </summary>
        public double GeometricVolume
        {
            get { return DiameterSegments.Aggregate(0.0, (v, seg) => v + seg.GeometricVolume); }
        }

        /// <summary>
        ///     Текстовое представление геометрических параметров участка (Lкм. x Dмм.)
        /// </summary>
        public string GeometryString
        {
            get
            {
                return DiameterSegments.Aggregate(string.Empty,
                    (current, seg) => current + (current == string.Empty ? string.Empty : ", ") + seg.GeometryString);
            }
        }

        // Находит точки для рисования участка газопровода
        private void EvaluatePoints()
        {
/*
            if (_pipeline == null) return;
            _ptList.Clear();

            var pts = Pipe.IntermediatePoints.FindPoints(_kmBegining, _kmEnd);
            if (pts != null)
            {
             //   pts.Sort();
                // Поиск начальной точки
                if (pts.Count > 0 && pts.First().Km != _kmBegining)
                {
                    var seg = _pipeline.IntermediatePoints.FindSegment(_kmBegining);
                    if (seg != null)
                        _ptList.Add(seg.Km2Point(_kmBegining));
                }

                _ptList.AddRange(pts.Select(point => point.Position));

                // Поиск конечной точки
                if (pts.Count > 0 && pts.Last().Km != _kmEnd)
                {
                    var seg = _pipeline.IntermediatePoints.FindSegment(_kmEnd);
                    if (seg != null)
                        _ptList.Add(seg.Km2Point(_kmEnd));
                }
            }
*/
        }

        // Расчет геометрии участка (на основе сегментации газопровода по диаметру)
        private void EvaluateGeometry()
        {
            if (Pipe == null)
            {
                return;
            }

            DiameterSegments = new List<PipelineDiameterSegment>(
                Pipe.DiameterSegments.Where(s =>
                    (s.KmBegining <= KmBegining && s.KmEnd <= KmEnd && s.KmEnd > KmBegining)
                    || (s.KmBegining <= KmBegining && s.KmEnd >= KmEnd)
                    || (s.KmBegining >= KmBegining && s.KmEnd <= KmEnd)
                    || (s.KmBegining >= KmBegining && s.KmEnd >= KmEnd && s.KmBegining < KmEnd))
                    .Select(s => new PipelineDiameterSegment
                    {
                        DiameterId = s.DiameterId,
                        ExternalDiameterId = s.ExternalDiameterId,
                        ExternalDiameter = s.ExternalDiameter,
                        DiameterName = s.DiameterName,
                        DiameterConv = s.DiameterConv,
                        DiameterReal = s.DiameterReal,
                        WallThickness = s.WallThickness,
                        KmBegining = s.KmBegining < KmBegining ? KmBegining : s.KmBegining,
                        KmEnd = s.KmEnd > KmEnd ? KmEnd : s.KmEnd
                    }));
            DiameterSegments.Sort();
        }
    }
}