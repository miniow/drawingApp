using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace drawingApp.Models
{
    public class Circle : Shape, INotifyPropertyChanged
    {
        public static readonly DependencyProperty CenterXProperty =
            DependencyProperty.Register(nameof(CenterX), typeof(double), typeof(Circle),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender, OnCenterXChanged));

        public static readonly DependencyProperty CenterYProperty =
            DependencyProperty.Register(nameof(CenterY), typeof(double), typeof(Circle),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender, OnCenterYChanged));

        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register(nameof(Radius), typeof(double), typeof(Circle),
                new FrameworkPropertyMetadata(50.0, FrameworkPropertyMetadataOptions.AffectsRender, OnRadiusChanged));

        public double CenterX
        {
            get => (double)GetValue(CenterXProperty);
            set
            {
                SetValue(CenterXProperty, value);
                OnPropertyChanged();
            }
        }

        public double CenterY
        {
            get => (double)GetValue(CenterYProperty);
            set
            {
                SetValue(CenterYProperty, value);
                OnPropertyChanged();
            }
        }

        public double Radius
        {
            get => (double)GetValue(RadiusProperty);
            set
            {
                SetValue(RadiusProperty, value);
                OnPropertyChanged();
            }
        }

        protected override Geometry DefiningGeometry
        {
            get
            {
                return new EllipseGeometry(new Point(CenterX, CenterY), Radius, Radius);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private static void OnCenterXChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Circle circle)
            {
                circle.OnPropertyChanged(nameof(CenterX));
            }
        }

        private static void OnCenterYChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Circle circle)
            {
                circle.OnPropertyChanged(nameof(CenterY));
            }
        }

        private static void OnRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Circle circle)
            {
                circle.OnPropertyChanged(nameof(Radius));
            }
        }
    }


}
