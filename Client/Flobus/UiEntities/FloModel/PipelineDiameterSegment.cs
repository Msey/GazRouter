using System;
using GazRouter.DTO.ObjectModel.Segment;
using Utils.Calculations;

namespace GazRouter.Flobus.UiEntities.FloModel
{
    /// <summary>
    ///     Класс описвает сегмент газопровода
    /// </summary>
    public class PipelineDiameterSegment : IComparable
    {
        public PipelineDiameterSegment()
        {
        }

        public PipelineDiameterSegment(DiameterSegmentDTO segmentDto)
        {
            DiameterId = segmentDto.DiameterId;
            ExternalDiameterId = segmentDto.ExternalDiameterId;
            DiameterName = segmentDto.DiameterName;
            DiameterReal = segmentDto.DiameterReal;
            DiameterConv = segmentDto.DiameterConv;
            KmBegining = segmentDto.KilometerOfStartPoint;
            KmEnd = segmentDto.KilometerOfEndPoint;
            WallThickness = segmentDto.WallThickness;
            ExternalDiameter = segmentDto.ExternalDiameter;
        }

        public int DiameterId { get; set; }

        public int ExternalDiameterId { get; set; }

        /// <summary>
        /// Наружный диаметр, мм
        /// </summary>
        public double ExternalDiameter { get; set; }

        public string DiameterName { get; set; }

        /// <summary>
        ///     Реальный диаметр, мм
        /// </summary>
        public int DiameterReal { get; set; }

        /// <summary>
        ///     Условный диаметр, мм
        /// </summary>
        public int DiameterConv { get; set; }

        /// <summary>
        ///     Толщина стенки, мм
        /// </summary>
        public double WallThickness { get; set; }

        /// <summary>
        ///     Пикетный километр начала
        /// </summary>
        public double KmBegining { get; set; }

        /// <summary>
        ///     Пикетный километр конца
        /// </summary>
        public double KmEnd { get; set; }

        ///// <summary>
        /////     Текстовое представление геометрических параметров сегмента (Lкм. x Dмм.)
        ///// </summary>
        //public string GeometryString => $"{KmEnd - KmBegining:0.##}км х {DiameterName}";

        /// <summary>
        ///     Текстовое представление геометрических параметров сегмента (Lкм. x Dмм.)
        /// </summary>
        public string GeometryString => $"{KmEnd - KmBegining:0.##}км х Dв {ExternalDiameter - 2 * WallThickness}";

        ///// <summary>
        /////     Геометрический объем сегмента, м³
        ///// </summary>
        //public double GeometricVolume
        //    => SupportCalculations.GeometricVolume((double) DiameterConv/1000, (KmEnd - KmBegining)*1000);

        /// <summary>
        ///     Геометрический объем сегмента, м³
        /// </summary>
        public double GeometricVolume
            => SupportCalculations.GeometricVolume((double)(ExternalDiameter - 2* WallThickness) / 1000, (KmEnd - KmBegining) * 1000);

        public int CompareTo(object obj)
        {
            var other = obj as PipelineDiameterSegment;

            if (KmBegining < other.KmBegining)
            {
                return -1;
            }
            if (KmBegining > other.KmBegining)
            {
                return 1;
            }
            return 0;
        }
    }
}