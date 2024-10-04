using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;

namespace drawingApp.Models
{
    public class Circle : Shape
    {
        public static readonly DependencyProperty CenterXProperty =
            DependencyProperty.Register(nameof(CenterX), typeof(double), typeof(Circle),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty CenterYProperty =
            DependencyProperty.Register(nameof(CenterY), typeof(double), typeof(Circle),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register(nameof(Radius), typeof(double), typeof(Circle),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public double CenterX
        {
            get => (double)GetValue(CenterXProperty);
            set => SetValue(CenterXProperty, value);
        }

        public double CenterY
        {
            get => (double)GetValue(CenterYProperty);
            set => SetValue(CenterYProperty, value);
        }

        public double Radius
        {
            get => (double)GetValue(RadiusProperty);
            set => SetValue(RadiusProperty, value);
        }

        protected override Geometry DefiningGeometry
        {
            get
            {
                return new EllipseGeometry(new Point(CenterX, CenterY), Radius, Radius);
            }
        }
    }


}
