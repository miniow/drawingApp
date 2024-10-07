using drawingApp.Models;
using drawingApp.ViewModels;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
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
        private AdornerLayer adornerLayer;
        private Adorner currentAdorner;
        public MainWindow()
        {
            InitializeComponent();

        }
        private void RemoveCurrentAdorner()
        {
            if (currentAdorner != null && adornerLayer != null)
            {
                adornerLayer.Remove(currentAdorner);
                currentAdorner = null;
            }
        }
        private void AddAdornerToShape(Shape shape)
        {
            if (shape == null) return;

            adornerLayer = AdornerLayer.GetAdornerLayer(myCanvas);
            if (adornerLayer == null)
            {
                MessageBox.Show("Nie można uzyskać AdornerLayer.");
                return;
            }

            // Usuwamy istniejący adorner
            RemoveCurrentAdorner();

            // Tworzymy nowy adorner zależny od rodzaju kształtu
            switch (shape)
            {
                case Rectangle _:
                    currentAdorner = new RectangleAdorner(shape);
                    break;
                case Ellipse _:
                    currentAdorner = new EllipseAdorner(shape);
                    break;
                case Line _:
                    currentAdorner = new LineAdorner(shape);
                    break;
                case Circle _:
                    currentAdorner = new CircleAdorner(shape);
                    break;
            }

            if (currentAdorner != null)
            {
                adornerLayer.Add(currentAdorner);
            }
        }

        private void Shape_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            RemoveCurrentAdorner();

            Shape clickedShape = sender as Shape;
            if (clickedShape != null)
            {
                var vm = DataContext as MainViewModel;
                if (vm == null) return;

                // Przypisz kliknięty kształt do CurrentShape w ViewModel
                vm.CurrentShape = clickedShape;

                // Dodaj adorner do kształtu
                AddAdornerToShape(clickedShape);

                e.Handled = true;
            }
        
        }

        private void DrawShape(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as MainViewModel;
            if (vm == null) return;

            Shape newShape = null;
            switch (vm.CurrentShapeParameters)
            {
                case RectangleParameters rectParams:
                    newShape = new Rectangle
                    {
                        Width = rectParams.Width,
                        Height = rectParams.Height,
                        Stroke = Brushes.Black,
                        StrokeThickness = 2,
                        Fill = Brushes.Transparent,
                        Cursor = Cursors.Hand
                    };
                    Canvas.SetLeft(newShape, rectParams.X);
                    Canvas.SetTop(newShape, rectParams.Y);
                    break;
                case CircleParameters circleParams:
                    newShape = new Circle
                    {
                        Width = circleParams.Radius * 2,
                        Height = circleParams.Radius * 2,
                        Stroke = Brushes.Black,
                        StrokeThickness = 2,
                        Fill = Brushes.Transparent,
                        CenterX = circleParams.CenterX,
                        CenterY = circleParams.CenterY,
                        Radius = circleParams.Radius,
                        Cursor = Cursors.Hand
                    };
                    Canvas.SetLeft(newShape, circleParams.CenterX - circleParams.Radius);
                    Canvas.SetTop(newShape, circleParams.CenterY - circleParams.Radius);
                    break;
                case LineParameters lineParams:
                    newShape = new Line
                    {
                        X1 = lineParams.X1,
                        Y1 = lineParams.Y1,
                        X2 = lineParams.X2,
                        Y2 = lineParams.Y2,
                        Stroke = Brushes.Black,
                        StrokeThickness = 2,
                        Cursor = Cursors.Hand
                    };
                    break;
            }

            if (newShape != null)
            {
                newShape.MouseLeftButtonDown += Shape_MouseLeftButtonDown;
                vm.Shapes.Add(newShape);
                vm.CurrentShape = newShape;
            }
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Usunięcie obecnego adornera przed rozpoczęciem rysowania
            RemoveCurrentAdorner();

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
                        StrokeThickness = 2,
                        Fill = Brushes.Transparent,
                        Cursor = Cursors.Hand
                    };
                    break;

                case ShapeType.Circle:
                    shape = new Circle
                    {
                        Stroke = vm.SelectedColor,
                        StrokeThickness = 2,
                        Fill = Brushes.Transparent,
                        CenterX = startPoint.X,
                        CenterY = startPoint.Y,
                        Radius = 0,
                        Cursor = Cursors.Hand
                    };
                    break;

                case ShapeType.Ellipse:
                    shape = new Ellipse
                    {
                        Stroke = vm.SelectedColor,
                        StrokeThickness = 2,
                        Fill = Brushes.Transparent,
                        Cursor = Cursors.Hand
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
                        Y2 = startPoint.Y,
                        Cursor = Cursors.Hand
                    };
                    break;
            }

            if (shape != null)
            {
                shape.MouseLeftButtonDown += Shape_MouseLeftButtonDown;
                vm.Shapes.Add(shape);
                vm.CurrentShape = shape;
            }
        }

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


        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!isDrawing)
                return;

            isDrawing = false;

            var vm = DataContext as MainViewModel;
            vm.CurrentShape = null;
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RemoveCurrentAdorner();
            if (isDrawing)
            {
                return;
            }

            var vm = DataContext as MainViewModel;
            if (vm == null) return;

            if (vm.CurrentShape != null)
            {
                AddAdornerToShape(vm.CurrentShape);
            }
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.]+");
            e.Handled = regex.IsMatch(e.Text);
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