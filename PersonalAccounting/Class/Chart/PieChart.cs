using PersonalAccounting.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PersonalAccounting.Class
{
    internal class PieChart
    {
        private float center;
        private float PieRadius { get; set; }
        private List<CategoryChart> Categories { get; set; }
        private ItemsControl DetailsItemsControl { get; set; }
        private Canvas CanvasChart { get; set; }

        public PieChart(Canvas canvasChart, float pieRadius)
        {
            PieRadius = pieRadius;
            CanvasChart = canvasChart;
            center = PieRadius;
            CanvasChart.Width = pieRadius * 2;
            CanvasChart.Height = pieRadius * 2;
        }

        private void ClearPieChart()
        {
            CanvasChart.Children.Clear();
            DetailsItemsControl = null;
        }

        public void DrawPieChart(List<CategoryChart> categories, ItemsControl detailsItemsControl)
        {
            ClearPieChart();
            DetailsItemsControl = detailsItemsControl;
            Categories = categories;
            if (Categories == null)
            {
                Ellipse el = new Ellipse();
                el.Width = PieRadius + center;
                el.Height = PieRadius + center;
                el.Fill = Brushes.LightGray;
                CanvasChart.Children.Add(el);
                DetailsItemsControl.ItemsSource = new List<CategoryChart> {
                    new CategoryChart
                    {
                        ColorBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#A5A5A5")),
                        Title = "Нет расходов"
                    }
                };
                return;
            } else if (Categories.Count == 1)
            {
                Ellipse el = new Ellipse();
                el.Width = PieRadius + center;
                el.Height = PieRadius + center;
                el.Fill = Categories[0].ColorBrush;
                CanvasChart.Children.Add(el);
                DetailsItemsControl.ItemsSource = Categories;
                return;
            }
            DetailsItemsControl.ItemsSource = Categories;
            float angle = 0, prevAngle = 0;
            foreach (var category in Categories)
            {
                double line1X = (PieRadius * Math.Cos(angle * Math.PI / 180)) + center;
                double line1Y = (PieRadius * Math.Sin(angle * Math.PI / 180)) + center;

                angle = category.Percentage * (float)360 / 100 + prevAngle;
                Debug.WriteLine(angle);

                double arcX = (PieRadius * Math.Cos(angle * Math.PI / 180)) + center;
                double arcY = (PieRadius * Math.Sin(angle * Math.PI / 180)) + center;

                var line1Segment = new LineSegment(new Point(line1X, line1Y), false);
                double arcWidth = PieRadius, arcHeight = PieRadius;
                bool isLargeArc = category.Percentage > 50;
                var arcSegment = new ArcSegment()
                {
                    Size = new Size(arcWidth, arcHeight),
                    Point = new Point(arcX, arcY),
                    SweepDirection = SweepDirection.Clockwise,
                    IsLargeArc = isLargeArc,
                };
                var line2Segment = new LineSegment(new Point(center, center), false);

                var pathFigure = new PathFigure(
                    new Point(center, center),
                    new List<PathSegment>()
                    {
                        line1Segment,
                        arcSegment,
                        line2Segment,
                    },
                    true);

                var pathFigures = new List<PathFigure>() { pathFigure, };
                var pathGeometry = new PathGeometry(pathFigures);
                var path = new Path()
                {
                    Fill = category.ColorBrush,
                    Data = pathGeometry,
                };
                CanvasChart.Children.Add(path);

                prevAngle = angle;

                var outline1 = new Line()
                {
                    X1 = center,
                    Y1 = center,
                    X2 = line1Segment.Point.X,
                    Y2 = line1Segment.Point.Y,
                    Stroke = Brushes.White,
                    StrokeThickness = 5,
                };
                var outline2 = new Line()
                {
                    X1 = center,
                    Y1 = center,
                    X2 = arcSegment.Point.X,
                    Y2 = arcSegment.Point.Y,
                    Stroke = Brushes.White,
                    StrokeThickness = 5,
                };

                CanvasChart.Children.Add(outline1);
                CanvasChart.Children.Add(outline2);
            }
        }

    }
}
