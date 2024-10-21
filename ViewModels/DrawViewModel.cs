using drawingApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.IO;

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
    class DrawViewModel: ViewModelBase
    {
        private ShapeType _selectedShapeType;
        private Brush _selectedColor;
        private Shape _currentShape;
        public ObservableCollection<Shape> Shapes { get; set; }
        public ObservableCollection<ShapeType> AvailableShapeTypes { get; set; }

        public Shape CurrentShape
        {
            get => _currentShape;
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
                UpdateCurrentShapeColor();
            }
        }

        public ICommand DeleteShapeCommand { get; }
        public ICommand SaveShapesCommand { get; }
        public ICommand LoadShapesCommand { get; }
        public DrawViewModel()
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
            
            SelectedShapeType = ShapeType.Rectangle;
            SelectedColor = Brushes.Black;

            DeleteShapeCommand = new RelayCommand(
       execute: _ => DeleteShape(),
       canExecute: _ => CurrentShape != null
   );


            SaveShapesCommand = new RelayCommand(
                execute: _ =>
                {
                    var dialog = new Microsoft.Win32.SaveFileDialog
                    {
                        Filter = "JSON Files (*.json)|*.json",
                        DefaultExt = "json"
                    };
                    if (dialog.ShowDialog() == true)
                    {
                        SaveShapesToFile(dialog.FileName);
                    }
                },
                canExecute: _ => Shapes.Count > 0
            );

            LoadShapesCommand = new RelayCommand(
                execute: _ =>
                {
                    var dialog = new Microsoft.Win32.OpenFileDialog
                    {
                        Filter = "JSON Files (*.json)|*.json",
                        DefaultExt = "json"
                    };
                    if (dialog.ShowDialog() == true)
                    {
                        LoadShapesFromFile(dialog.FileName);
                    }
                }
            );

            PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(Shapes) || args.PropertyName == "Count")
                {
                    (SaveShapesCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
                if (args.PropertyName == nameof(CurrentShape))
                {
                    (DeleteShapeCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            };
        }

        private void SaveShapesToFile(string fileName)
        {
            var shapesData = new List<ShapeData>();
            foreach (var shape in Shapes)
            {
                if (shape is Rectangle rect)
                {
                    shapesData.Add(new ShapeData
                    {
                        Type = ShapeType.Rectangle,
                        X = double.IsNaN(Canvas.GetLeft(rect)) ? 0 : Canvas.GetLeft(rect),
                        Y = double.IsNaN(Canvas.GetTop(rect)) ? 0 : Canvas.GetTop(rect),
                        Width = rect.Width,
                        Height = rect.Height,
                        Color = ((SolidColorBrush)rect.Stroke).Color.ToString()
                    });
                }
                else if (shape is Ellipse ellipse)
                {
                    shapesData.Add(new ShapeData
                    {
                        Type = ShapeType.Ellipse,
                        X = double.IsNaN(Canvas.GetLeft(ellipse)) ? 0 : Canvas.GetLeft(ellipse),
                        Y = double.IsNaN(Canvas.GetTop(ellipse)) ? 0 : Canvas.GetTop(ellipse),
                        Width = ellipse.Width,
                        Height = ellipse.Height,
                        Color = ((SolidColorBrush)ellipse.Stroke).Color.ToString()
                    });
                }
                else if (shape is Line line)
                {
                    shapesData.Add(new ShapeData
                    {
                        Type = ShapeType.Line,
                        X1 = line.X1,
                        Y1 = line.Y1,
                        X2 = line.X2,
                        Y2 = line.Y2,
                        Color = ((SolidColorBrush)line.Stroke).Color.ToString()
                    });
                }
                else if (shape is Circle circle)
                {
                    shapesData.Add(new ShapeData
                    {
                        Type = ShapeType.Circle,
                        CenterX = circle.CenterX,
                        CenterY = circle.CenterY,
                        Radius = circle.Radius,
                        Color = ((SolidColorBrush)circle.Stroke).Color.ToString()
                    });
                }
            }

            var json = JsonSerializer.Serialize(shapesData, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(fileName, json);
        }

        private void LoadShapesFromFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                MessageBox.Show("Plik nie istnieje lub nie można go otworzyć.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var json = File.ReadAllText(fileName);
            var shapesData = JsonSerializer.Deserialize<List<ShapeData>>(json);

            Shapes.Clear();

            foreach (var shapeData in shapesData)
            {
                Shape shape = null;
                switch (shapeData.Type)
                {
                    case ShapeType.Rectangle:
                        shape = new Rectangle
                        {
                            Width = shapeData.Width,
                            Height = shapeData.Height,
                            Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(shapeData.Color)) ?? Brushes.Black,
                            StrokeThickness = 2,
                            Fill = Brushes.Transparent
                        };
                        Canvas.SetLeft(shape, shapeData.X);
                        Canvas.SetTop(shape, shapeData.Y);
                        break;
                    case ShapeType.Ellipse:
                        shape = new Ellipse
                        {
                            Width = shapeData.Width,
                            Height = shapeData.Height,
                            Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(shapeData.Color)),
                            StrokeThickness = 2,
                            Fill = Brushes.Transparent
                        };
                        Canvas.SetLeft(shape, shapeData.X);
                        Canvas.SetTop(shape, shapeData.Y);
                        break;
                    case ShapeType.Line:
                        shape = new Line
                        {
                            X1 = shapeData.X1,
                            Y1 = shapeData.Y1,
                            X2 = shapeData.X2,
                            Y2 = shapeData.Y2,
                            Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(shapeData.Color)),
                            StrokeThickness = 2
                        };
                        break;
                    case ShapeType.Circle:
                        shape = new Circle
                        {
                            CenterX = shapeData.CenterX,
                            CenterY = shapeData.CenterY,
                            Radius = shapeData.Radius,
                            Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(shapeData.Color)),
                            StrokeThickness = 2,
                            Fill = Brushes.Transparent
                        };
                        break;
                }

                if (shape != null)
                {
                    shape.MouseLeftButtonDown += (sender, e) =>
                    {
                        CurrentShape = shape;
                        e.Handled = true;
                    };
                    Shapes.Add(shape);
                }
            }
        }

        private void DeleteShape()
        {
            if (CurrentShape != null)
            {


                if (CurrentShape.Parent is Canvas canvas)
                {
                    canvas.Children.Remove(CurrentShape);
                }
                Shapes.Remove(CurrentShape);
                CurrentShape = null;
            }
        }

        private bool CanDeleteShape()
        {
            return CurrentShape != null;
        }
        private void UpdateCurrentShapeColor()
        {
            if (_currentShape != null)
            {
                _currentShape.Stroke = _selectedColor;
            }
        }


    }
    public class ShapeData
    {
        public ShapeType Type { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double X1 { get; set; }
        public double Y1 { get; set; }
        public double X2 { get; set; }
        public double Y2 { get; set; }
        public double CenterX { get; set; }
        public double CenterY { get; set; }
        public double Radius { get; set; }
        public string Color { get; set; }
    }
}

