using drawingApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace drawingApp.ViewModels
{
    public enum ShapeType
    {
        Rectangle,
        Ellipse,
        Line,
        Circle,
        None
    }
    class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private ShapeType _selectedShapeType;
        private Brush _selectedColor;
        private Shape _currentShape;
        public ObservableCollection<Shape> Shapes { get; set; }
        public ObservableCollection<ShapeType> AvailableShapeTypes { get; set; }
        public ObservableCollection<Brush> AvailableColors { get; set; }
        public Shape CurrentShape
        {
            get=> _currentShape;
            set
            {
                _currentShape = value;
                OnPropertyChanged(nameof(CurrentShape));
            }
        }
        public ShapeType SelectedShapeType
        {
            get => _selectedShapeType;
            set
            {
                _selectedShapeType = value;
                OnPropertyChanged(nameof(SelectedShapeType));
                UpdateCurrentShapeParameters();
            }
        }
        private ShapeParametersBase _currentShapeParameters;
        public ShapeParametersBase CurrentShapeParameters
        {
            get => _currentShapeParameters;
            set
            {
                _currentShapeParameters = value;
                OnPropertyChanged(nameof(CurrentShapeParameters));
            }
        }
        private void UpdateCurrentShapeParameters()
        {
            switch (SelectedShapeType)
            {
                case ShapeType.Rectangle:
                    CurrentShapeParameters = new RectangleParameters();
                    break;
                case ShapeType.Circle:
                    CurrentShapeParameters = new CircleParameters();
                    break;
                case ShapeType.Line:
                    CurrentShapeParameters = new LineParameters();
                    break;
                case ShapeType.Ellipse:
                    CurrentShapeParameters = new ElipseParameters();
                    break;
            }
        }

        public Brush SelectedColor
        {
            get => _selectedColor;
            set
            {
                _selectedColor = value;
                OnPropertyChanged(nameof(SelectedColor));
            }
        }

        // Komendy
        public ICommand DrawShapeCommand { get; set; }

        public MainViewModel()
        {
            Shapes = new ObservableCollection<Shape>();
            AvailableShapeTypes = new ObservableCollection<ShapeType>
            {
                ShapeType.Rectangle,
                ShapeType.Ellipse,
                ShapeType.Line,
                ShapeType.Circle,
                ShapeType.None
            };
            AvailableColors = new ObservableCollection<Brush>
            {
                Brushes.Black,
                Brushes.Red,
                Brushes.Green,
                Brushes.Blue,
                Brushes.Yellow
            };
            SelectedShapeType = ShapeType.Rectangle;
            SelectedColor = Brushes.Black;

            DrawShapeCommand = new RelayCommand(DrawShape);

        }

        private void DrawShape(object obj)
        {
            Shape newShape = null;
            switch (CurrentShapeParameters)
            {
                case RectangleParameters rectParams:
                    newShape = new Rectangle
                    {
                        Width = rectParams.Width,
                        Height = rectParams.Height,
                        Stroke = Brushes.Black,
                        StrokeThickness = 2
                    };
                    Canvas.SetLeft(newShape, rectParams.X);
                    Canvas.SetTop(newShape, rectParams.Y);
                    break;
                case CircleParameters circleParams:
                    newShape = new Ellipse
                    {
                        Width = circleParams.Radius * 2,
                        Height = circleParams.Radius * 2,
                        Stroke = Brushes.Black,
                        StrokeThickness = 2
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
                        StrokeThickness = 2
                    };
                    break;
            }

            if (newShape != null)
            {
                Shapes.Add(newShape);
            }
        }

    }
}
