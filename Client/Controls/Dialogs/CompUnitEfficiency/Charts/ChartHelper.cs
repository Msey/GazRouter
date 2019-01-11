using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using GazRouter.Controls.Dialogs.CompUnitEfficiency.Model;

namespace GazRouter.Controls.Dialogs.CompUnitEfficiency.Charts
{
    public static class ChartHelper
    {
        private const int _pointCount = 6;


        public static List<ChartLine> CreateMainChartSource(CompUnitModel model)
        {
            var lineList = new List<ChartLine>();


            //Первая линия
            var firstLine = new ChartLine
            {
                Rpm = 1,
                LineColor = Colors.Gray,
            };

            var qmin = model.Qmin;
            var qmax = model.Qmax;

            for (var i = 0; i <= _pointCount; i++)
            {
                var qi = qmin + i*(qmax - qmin)/_pointCount;
                var ei = model.ERated(qi);
                var kpdi = model.NRated(qi);
                var ni = model.NRorated(qi);

                var point = new ChartPoint
                {
                    Pumping = qi,
                    CompressionRatio = ei,
                    Efficiency = kpdi,
                    Power = ni
                };
                firstLine.Points.Add(point);
            }
            lineList.Add(firstLine);


            //Остальные линии на основе первой
            for (var ni = (decimal) 0.7; ni <= (decimal) 1.1; ni += (decimal) 0.05)
            {
                if (ni == 1) continue;

                var line = new ChartLine
                {
                    Rpm = (double) ni,
                    LineColor = Colors.Gray,
                };


                if (ni == (decimal) 1.1)
                    line.IsEfficiencyLabel = true;


                for (var i = 0; i <= _pointCount; i++)
                {
                    var point = new ChartPoint();

                    if (firstLine.Points[i].Pumping.HasValue)
                        point.Pumping = (double) ni*firstLine.Points[i].Pumping.Value;

                    if (firstLine.Points[i].CompressionRatio.HasValue && firstLine.Points[i].Efficiency.HasValue)
                    {
                        var e = firstLine.Points[i].CompressionRatio.Value;
                        var kpd = firstLine.Points[i].Efficiency.Value;
                        point.CompressionRatio = model.Ei((double) ni, e, kpd);
                    }

                    point.Efficiency = firstLine.Points[i].Efficiency;

                    if (firstLine.Points[i].Power.HasValue)
                        point.Power = model.NRoi((double) ni, firstLine.Points[i].Power.Value);

                    line.Points.Add(point);
                }
                lineList.Add(line);
            }
            var mainLineList = lineList.OrderBy(p => p.Rpm).ToList();


            //Соединительные линии
            var lineCount = mainLineList.Count;
            for (var pi = 0; pi <= _pointCount; pi++)
            {
                var line = new ChartLine {LineColor = Colors.Gray};


                for (var li = 0; li < lineCount; li++)
                {
                    line.Points.Add(mainLineList[li].Points[pi]);
                    line.Points[li].Rpm = mainLineList[li].Rpm;
                }

                if (pi == _pointCount)
                {
                    line.IsNNomLabel = true;
                }

                lineList.Add(line);
            }


            //Кривая для N/Rpm = RPMRelative
            var ratedLine = new ChartLine
            {
                Rpm = model.Results.RpmRelative,
                LineColor = Colors.Gray,
                IsRatedLine = true
            };

            for (var i = 0; i <= _pointCount; i++)
            {
                var point = new ChartPoint();

                if (firstLine.Points[i].Pumping.HasValue)
                    point.Pumping = model.Results.RpmRelative*firstLine.Points[i].Pumping.Value;

                if (firstLine.Points[i].CompressionRatio.HasValue && firstLine.Points[i].Efficiency.HasValue)
                {
                    var e = firstLine.Points[i].CompressionRatio.Value;
                    var kpd = firstLine.Points[i].Efficiency.Value;
                    point.CompressionRatio = model.Ei(model.Results.RpmRelative, e, kpd);
                }

                point.Efficiency = firstLine.Points[i].Efficiency;

                if (firstLine.Points[i].Power.HasValue)
                    point.Power = model.NRoi(model.Results.RpmRelative, firstLine.Points[i].Power.Value);

                ratedLine.Points.Add(point);
            }
            ratedLine.Points[_pointCount].Rpm = ratedLine.Rpm;
            if (model.Results.RpmRelative < 1.1 && model.Results.RpmRelative > 0.7)
                lineList.Add(ratedLine);


            //точка на ней
            var pointLine = new ChartLine
            {
                Rpm = model.Results.RpmRelative,
                PointSize = 10,
                LineColor = Colors.Blue,
                PointColor = Colors.Blue,
                Points = new List<ChartPoint>
                {
                    new ChartPoint
                    {
                        Pumping = model.Results.PumpingCalculated,
                        CompressionRatio = model.Results.CompressionRatio
                    }
                }
            };
            lineList.Add(pointLine);


            //Красная линия
            var redLine = new ChartLine
            {
                LineColor = Colors.Red,
                IsRedLine = true
            };
            mainLineList.ForEach(l => redLine.Points.Add(l.Points[0]));
            var firstRedPoint = new ChartPoint {Pumping = 1.1*firstLine.Points[0].Pumping};
            if (firstRedPoint.Pumping != null)
            {
                firstRedPoint.CompressionRatio = model.ERated(firstRedPoint.Pumping.Value);
                firstRedPoint.Efficiency = model.NRated(firstRedPoint.Pumping.Value);

                if (firstRedPoint.CompressionRatio != null && firstRedPoint.Efficiency != null)
                {
                    for (var nNom = (decimal) 1.1; nNom >= (decimal) 0.7; nNom -= (decimal) 0.05)
                    {
                        if (nNom == 1)
                        {
                            redLine.Points.Add(firstRedPoint);
                            continue;
                        }

                        var basePoint = mainLineList.Single(p => p.Rpm == (double) nNom).Points[0];
                        var point = new ChartPoint
                        {
                            Efficiency = basePoint.Efficiency,
                            Pumping = (double) nNom*firstRedPoint.Pumping,
                            CompressionRatio =
                                model.Ei((double) nNom, firstRedPoint.CompressionRatio.Value,
                                    firstRedPoint.Efficiency.Value)
                        };

                        redLine.Points.Add(point);
                    }
                }
            }
            redLine.Points.Add(mainLineList.First().Points[0]);
            lineList.Add(redLine);


            return lineList;
        }


        private static List<ChartPoint> PreparePointsList(CompUnitModel model)
        {
            var pointList = new List<ChartPoint>();

            for (var i = 0; i <= _pointCount; i++)
            {
                var qi = model.Qmin + i*(model.Qmax - model.Qmin)/_pointCount;

                var point = new ChartPoint
                {
                    Pumping = qi,
                    CompressionRatio = model.ERated(qi),
                    Efficiency = model.NRated(qi),
                    Power = model.NRorated(qi)
                };
                pointList.Add(point);
            }

            var rpmRelative = model.Results.RpmRelative;

            return pointList.Select(pointRated =>
                new ChartPoint
                {
                    Pumping = rpmRelative*pointRated.Pumping,
                    Efficiency = pointRated.Efficiency,
                    CompressionRatio =
                        model.Ei(rpmRelative, pointRated.CompressionRatio.Value, pointRated.Efficiency.Value),
                    Power = pointRated.Power.HasValue ? model.NRoi(rpmRelative, pointRated.Power.Value) : (double?) null
                }).ToList();
        }


        public static List<ChartLine> CreateCompRatioChartSource(CompUnitModel model)
        {
            var lineList = new List<ChartLine>();

            var pointList = PreparePointsList(model);
            var line = new ChartLine
            {
                LineColor = Colors.Gray,
                Points = pointList
            };

            lineList.Add(line);

            //Отображение точки
            lineList.Add(
                new ChartLine
                {
                    LineColor = Colors.Blue,
                    PointColor = Colors.Blue,
                    Points = new List<ChartPoint>
                    {
                        new ChartPoint
                        {
                            CompressionRatio = model.Results.CompressionRatio,
                            Pumping = model.Results.PumpingCalculated
                        }
                    }
                });

            return lineList;
        }


        public static List<ChartLine> CreateEfficiencyChartSource(CompUnitModel model)
        {
            var lineList = new List<ChartLine>();
            var pointList = PreparePointsList(model);

            var line = new ChartLine
            {
                YValue = "Efficiency",
                LineColor = Colors.Gray,
                Points = pointList
            };
            lineList.Add(line);

            //Отображение точки
            lineList.Add(
                new ChartLine
                {
                    YValue = "Efficiency",
                    PointColor = Colors.Blue,
                    Points = new List<ChartPoint>
                    {
                        new ChartPoint
                        {
                            Efficiency = model.Results.PolytropicEfficiency,
                            Pumping = model.Results.PumpingCalculated
                        }
                    }
                });

            return lineList;
        }


        public static List<ChartLine> CreateCompUnitPowerChartSource(CompUnitModel model)
        {
            var lineList = new List<ChartLine>();
            var pointList = PreparePointsList(model);

            var line = new ChartLine
            {
                YValue = "Power",
                LineColor = Colors.Gray
            };

            foreach (var pt in pointList)
            {
                if (pt.Pumping != null && pt.Power != null)
                    line.Points.Add(
                        new ChartPoint
                        {
                            Pumping = pt.Pumping,
                            Power = pt.Power*(100*model.Measurings.PIn.Mpa/9.80665)
                            // * model.Results.DensitySuperchargerInlet
                        });
            }

            if (line.Points.Any()) lineList.Add(line);

            //Отображение точки
            if (lineList.Any())
                lineList.Add(
                    new ChartLine
                    {
                        YValue = "Power",
                        LineColor = Colors.Blue,
                        PointColor = Colors.Blue,
                        Points = new List<ChartPoint>
                        {
                            new ChartPoint
                            {
                                Power = model.Results.PowerIn,
                                Pumping = model.Results.PumpingCalculated
                            }
                        }
                    });

            return lineList;
        }


        public static void SmoothRange(ref double min, ref double max)
        {
            if (max <= min) return;

            var delta = max - min;
            var digit = 0.001;
            while (Math.Round(delta/digit) > 0) digit *= 10;
            digit /= 10;

            min = Math.Round(min/digit)*digit;
            max = Math.Round(max/digit)*digit;

            delta = (max - min);

            min -= delta;
            max += delta;
        }

        public static List<ChartLine> CreatePowerFuelGasConsimptionChartSource(CompUnitModel model)
        {
            var lineList = new List<ChartLine>();
            var pointList = PreparePointsList(model);
            var line = new ChartLine
            {
                XValue = "Power",
                YValue = "Pumping",
                LineColor = Colors.Gray
            };
            foreach (var pt in pointList)
            {
                if (pt.Pumping != null && pt.Power != null)
                    line.Points.Add(new ChartPoint
                    {
                        Power =
                            model.GetPowerEffective(
                                pt.Power.Value*(100*(model.Measurings.PIn).Mpa/9.80665),
                                0.958), //model.Results.DensitySuperchargerInlet, 0.985),
                        Pumping =
                            model.GetFuelGasConsumption(
                                pt.Power.Value*(100*model.Measurings.PIn.Mpa/9.80665),
                                // * model.Results.DensitySuperchargerInlet,
                                model.Measurings.TemperatureAir.Kelvins,
                                model.Measurings.PressureAir.Mpa)*1000
                    });
            }
            if (line.Points.Any()) lineList.Add(line);


            //Отображение точки
            if (model.Results.PowerIn != null && lineList.Any())
                lineList.Add(
                    new ChartLine
                    {
                        XValue = "Power",
                        YValue = "Pumping",
                        LineColor = Colors.Blue,
                        PointColor = Colors.Blue,
                        Points = new List<ChartPoint>
                        {
                            new ChartPoint
                            {
                                Power =
                                    model.GetPowerEffective(
                                        model.Results.PowerIn.Value, 0.985),
                                Pumping =
                                    model.GetFuelGasConsumption(model.Results.PowerIn.Value,
                                        model.Measurings.TemperatureAir.Kelvins,
                                        model.Measurings
                                            .PressureAir.Mpa)*1000
                            }
                        }
                    });

            return lineList;
        }
    }
}