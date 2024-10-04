using drawingApp.Models;
using drawingApp.ViewModels;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace drawingApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
 
    public partial class MainWindow : Window
    {
        private bool isDrawing = false;
        private Point startPoint;

        public MainWindow()
        {
            InitializeComponent();

        }

        // Zdarzenie rozpoczęcia rysowania
        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDrawing = true;
            startPoint = e.GetPosition(myCanvas);

            var vm = DataContext as MainViewModel;

            Shape shape = null;

            switch (vm.SelectedShapeType)
            {
                case ShapeType.Rectangle:
                    shape = new Rectangle
                    {
                        Stroke = vm.SelectedColor,
                        StrokeThickness = 2
                    };
                    break;

                case ShapeType.Circle:
                    shape = new Circle
                    {
                        Stroke = vm.SelectedColor,
                        StrokeThickness = 2,
                        CenterX = startPoint.X,
                        CenterY = startPoint.Y,
                        Radius = 0

                    };
                    break;
                case ShapeType.Ellipse:
                    shape = new Ellipse
                    {
                        Stroke = vm.SelectedColor,
                        StrokeThickness = 2
                    };
                    break;

                case ShapeType.Line:
                    shape = new Line
                    {
                        Stroke = vm.SelectedColor,
                        StrokeThickness = 2,
                        X1 = startPoint.X,
                        Y1 = startPoint.Y,
                        X2 = startPoint.X,
                        Y2 = startPoint.Y
                    };
                    break;
            }

            if (shape != null)
            {
                vm.Shapes.Add(shape); // Add to Shapes collection here
                vm.CurrentShape = shape; // Set the current shape
            }
        }




        // Zdarzenie podczas ruchu myszki
        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            var vm = DataContext as MainViewModel;

            if (!isDrawing || vm.CurrentShape == null)
                return;

            Point pos = e.GetPosition(myCanvas);
            var shape = vm.CurrentShape;

            if (shape is Rectangle rect)
            {
                double x = Math.Min(pos.X, startPoint.X);
                double y = Math.Min(pos.Y, startPoint.Y);
                double width = Math.Abs(pos.X - startPoint.X);
                double height = Math.Abs(pos.Y - startPoint.Y);

                rect.Width = width;
                rect.Height = height;

                Canvas.SetLeft(rect, x);
                Canvas.SetTop(rect, y);
            }
            else if (shape is Ellipse ellipse)
            {
                double x = Math.Min(pos.X, startPoint.X);
                double y = Math.Min(pos.Y, startPoint.Y);
                ellipse.Width = Math.Abs(pos.X - startPoint.X);
                ellipse.Height = Math.Abs(pos.Y - startPoint.Y);
                Canvas.SetLeft(ellipse, x);
                Canvas.SetTop(ellipse, y);
            }
            else if (shape is Circle circle)
            {
                double deltaX = pos.X - circle.CenterX;
                double deltaY = pos.Y - circle.CenterY;
                double radius = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
                circle.Radius = radius;
            }
            else if (shape is Line line)
            {
                line.X2 = pos.X;
                line.Y2 = pos.Y;
            }
        }



        // Zdarzenie zakończenia rysowania
        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!isDrawing)
                return;

            isDrawing = false;

            var vm = DataContext as MainViewModel;
            vm.CurrentShape = null; // Reset the current shape
        }


    }

    public class ShapeParametersTemplateSelector : DataTemplateSelector
    {
        public DataTemplate RectangleTemplate { get; set; }
        public DataTemplate CircleTemplate { get; set; }
        public DataTemplate LineTemplate { get; set; }
        public DataTemplate ElipseTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is RectangleParameters)
                return RectangleTemplate;
            else if (item is CircleParameters)
                return CircleTemplate;
            else if (item is ElipseParameters)
                return ElipseTemplate;
            else if (item is LineParameters)
                return LineTemplate;
            else
                return base.SelectTemplate(item, container);
        }
    }

    public class ShapeDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate RectangleTemplate { get; set; }
        public DataTemplate EllipseTemplate { get; set; }
        public DataTemplate LineTemplate { get; set; }
        public DataTemplate CircleTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is Rectangle)
                return RectangleTemplate;
            if (item is Ellipse)
                return EllipseTemplate;
            if (item is Line)
                return LineTemplate;
            if (item is Circle)
                return CircleTemplate;

            return base.SelectTemplate(item, container);
        }
    }
}