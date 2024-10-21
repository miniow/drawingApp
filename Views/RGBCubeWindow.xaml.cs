using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using HelixToolkit;
using HelixToolkit.Wpf;
namespace drawingApp.Views
{
    public partial class RGBCubeWindow : Window
    {
        private double zAxisSlice = 0.0;
        private ModelVisual3D sectionPlaneVisual;

        public RGBCubeWindow()
        {
            InitializeComponent();
            CreateRGBCube();
            DrawCubeSection();
        }

        private void CreateRGBCube()
        {
            // Front face
            var frontPositions = new Point3DCollection
    {
        new Point3D(0, 0, 0),   // bottom-left
        new Point3D(1, 0, 0),   // bottom-right
        new Point3D(1, 1, 0),   // top-right
        new Point3D(0, 1, 0)    // top-left
    };
            var frontIndices = new Int32Collection { 0, 1, 2, 0, 2, 3 };
            var frontTextureCoords = new PointCollection { new Point(0, 0), new Point(1, 0), new Point(1, 1), new Point(0, 1) };

            // Back face
            var backPositions = new Point3DCollection
    {
        new Point3D(0, 0, 1),   // bottom-left
        new Point3D(1, 0, 1),   // bottom-right
        new Point3D(1, 1, 1),   // top-right
        new Point3D(0, 1, 1)    // top-left
    };
            var backIndices = new Int32Collection { 0, 1, 2, 0, 2, 3 };
            var backTextureCoords = new PointCollection { new Point(0, 0), new Point(1, 0), new Point(1, 1), new Point(0, 1) };

            // Left face
            var leftPositions = new Point3DCollection
    {
        new Point3D(0, 0, 0),   // bottom-left
        new Point3D(0, 1, 0),   // top-left
        new Point3D(0, 1, 1),   // top-right
        new Point3D(0, 0, 1)    // bottom-right
    };
            var leftIndices = new Int32Collection { 0, 1, 2, 0, 2, 3 };
            var leftTextureCoords = new PointCollection { new Point(0, 0), new Point(0, 1), new Point(1, 1), new Point(1, 0) };

            // Right face
            var rightPositions = new Point3DCollection
    {
        new Point3D(1, 0, 0),   // bottom-left
        new Point3D(1, 1, 0),   // top-left
        new Point3D(1, 1, 1),   // top-right
        new Point3D(1, 0, 1)    // bottom-right
    };
            var rightIndices = new Int32Collection { 0, 1, 2, 0, 2, 3 };
            var rightTextureCoords = new PointCollection { new Point(0, 0), new Point(0, 1), new Point(1, 1), new Point(1, 0) };

            // Top face
            var topPositions = new Point3DCollection
    {
        new Point3D(0, 1, 0),   // bottom-left
        new Point3D(1, 1, 0),   // bottom-right
        new Point3D(1, 1, 1),   // top-right
        new Point3D(0, 1, 1)    // top-left
    };
            var topIndices = new Int32Collection { 0, 1, 2, 0, 2, 3 };
            var topTextureCoords = new PointCollection { new Point(0, 0), new Point(1, 0), new Point(1, 1), new Point(0, 1) };

            // Bottom face
            var bottomPositions = new Point3DCollection
    {
        new Point3D(0, 0, 0),   // bottom-left
        new Point3D(1, 0, 0),   // bottom-right
        new Point3D(1, 0, 1),   // top-right
        new Point3D(0, 0, 1)    // top-left
    };
            var bottomIndices = new Int32Collection { 0, 1, 2, 0, 2, 3 };
            var bottomTextureCoords = new PointCollection { new Point(0, 0), new Point(1, 0), new Point(1, 1), new Point(0, 1) };

            // Tworzenie modeli GeometryModel3D dla każdej ściany z odpowiednimi teksturami
            var frontMesh = new MeshGeometry3D { Positions = frontPositions, TriangleIndices = frontIndices, TextureCoordinates = frontTextureCoords };
            var backMesh = new MeshGeometry3D { Positions = backPositions, TriangleIndices = backIndices, TextureCoordinates = backTextureCoords };
            var leftMesh = new MeshGeometry3D { Positions = leftPositions, TriangleIndices = leftIndices, TextureCoordinates = leftTextureCoords };
            var rightMesh = new MeshGeometry3D { Positions = rightPositions, TriangleIndices = rightIndices, TextureCoordinates = rightTextureCoords };
            var topMesh = new MeshGeometry3D { Positions = topPositions, TriangleIndices = topIndices, TextureCoordinates = topTextureCoords };
            var bottomMesh = new MeshGeometry3D { Positions = bottomPositions, TriangleIndices = bottomIndices, TextureCoordinates = bottomTextureCoords };

            // Przypisywanie tekstur do ścian
            var frontMaterial = new DiffuseMaterial(new ImageBrush(CreateRGBTexture(Colors.Black, Colors.Red, Colors.Green, Colors.Yellow)));
            var backMaterial = new DiffuseMaterial(new ImageBrush(CreateRGBTexture(Colors.Blue, Colors.Magenta, Colors.Cyan, Colors.White)));
            var leftMaterial = new DiffuseMaterial(new ImageBrush(CreateRGBTexture(Colors.Black, Colors.Blue, Colors.Green, Colors.Cyan)));


            var rightMaterial = new DiffuseMaterial(new ImageBrush(CreateRGBTexture(Colors.Red, Colors.Magenta, Colors.Yellow, Colors.White)));





            var topMaterial = new DiffuseMaterial(new ImageBrush(CreateRGBTexture(Colors.Green, Colors.Yellow, Colors.Cyan, Colors.White)));
            var bottomMaterial = new DiffuseMaterial(new ImageBrush(CreateRGBTexture(Colors.Black, Colors.Red, Colors.Blue, Colors.Magenta)));

            // Tworzenie modeli GeometryModel3D dla każdej ściany
            var frontModel = new GeometryModel3D(frontMesh, frontMaterial) { BackMaterial = frontMaterial };
            var backModel = new GeometryModel3D(backMesh, backMaterial) { BackMaterial = backMaterial };
            var leftModel = new GeometryModel3D(leftMesh, leftMaterial) { BackMaterial = leftMaterial };
            var rightModel = new GeometryModel3D(rightMesh, rightMaterial) { BackMaterial = rightMaterial };
            var topModel = new GeometryModel3D(topMesh, topMaterial) { BackMaterial = topMaterial };
            var bottomModel = new GeometryModel3D(bottomMesh, bottomMaterial) { BackMaterial = bottomMaterial };

            // Dodanie modeli do HelixViewport
            helixViewport.Children.Add(new ModelVisual3D { Content = frontModel });
            helixViewport.Children.Add(new ModelVisual3D { Content = backModel });
            helixViewport.Children.Add(new ModelVisual3D { Content = leftModel });
            helixViewport.Children.Add(new ModelVisual3D { Content = rightModel });
            helixViewport.Children.Add(new ModelVisual3D { Content = topModel });
            helixViewport.Children.Add(new ModelVisual3D { Content = bottomModel });

            AddRGBArrows();
            // Dodanie przekroju
            CreateSectionPlane();
        }
        private void AddRGBArrows()
        {
            // Strzałka dla osi R (czerwony, od (0,0,0) do (1,0,0))
            var arrowRed = new LinesVisual3D
            {
                Color = Colors.Red,
                Thickness = 2,
                Points = new Point3DCollection
        {
            new Point3D(0, 0, 0),   // Punkt początkowy (0, 0, 0)
            new Point3D(1, 0, 0)    // Punkt końcowy (1, 0, 0) - wzrost w osi X (czerwony)
        }
            };
            helixViewport.Children.Add(arrowRed);

            // Strzałka dla osi G (zielony, od (0,0,0) do (0,1,0))
            var arrowGreen = new LinesVisual3D
            {
                Color = Colors.Green,
                Thickness = 2,
                Points = new Point3DCollection
        {
            new Point3D(0, 0, 0),   // Punkt początkowy (0, 0, 0)
            new Point3D(0, 1, 0)    // Punkt końcowy (0, 1, 0) - wzrost w osi Y (zielony)
        }
            };
            helixViewport.Children.Add(arrowGreen);

            // Strzałka dla osi B (niebieski, od (0,0,0) do (0,0,1))
            var arrowBlue = new LinesVisual3D
            {
                Color = Colors.Blue,
                Thickness = 2,
                Points = new Point3DCollection
        {
            new Point3D(0, 0, 0),   // Punkt początkowy (0, 0, 0)
            new Point3D(0, 0, 1)    // Punkt końcowy (0, 0, 1) - wzrost w osi Z (niebieski)
        }
            };
            helixViewport.Children.Add(arrowBlue);

            // Opcjonalnie można dodać etykiety tekstowe do oznaczenia osi
            AddAxisLabels();
        }
        private void AddAxisLabels()
        {
            // Etykieta dla osi R (czerwony)
            var redLabel = new BillboardTextVisual3D
            {
                Text = "R",
                Foreground = Brushes.Red,
                Position = new Point3D(1.1, 0, 0)  // Pozycja nieco poza strzałką w osi X
            };
            helixViewport.Children.Add(redLabel);

            // Etykieta dla osi G (zielony)
            var greenLabel = new BillboardTextVisual3D
            {
                Text = "G",
                Foreground = Brushes.Green,
                Position = new Point3D(0, 1.1, 0)  // Pozycja nieco poza strzałką w osi Y
            };
            helixViewport.Children.Add(greenLabel);

            // Etykieta dla osi B (niebieski)
            var blueLabel = new BillboardTextVisual3D
            {
                Text = "B",
                Foreground = Brushes.Blue,
                Position = new Point3D(0, 0, 1.1)  // Pozycja nieco poza strzałką w osi Z
            };
            helixViewport.Children.Add(blueLabel);
        }
        // Funkcja tworzy teksturę, która odwzorowuje kolory RGB w rogach kostki
        private BitmapSource CreateRGBTexture(Color bottomLeft, Color bottomRight, Color topLeft, Color topRight)
        {
            var width = 2;
            var height = 2;
            var stride = width * 4;  // 4 bajty na piksel (Bgra32)
            var pixels = new byte[width * height * 4];  // 2x2 piksele, 4 bajty na piksel (B, G, R, A)

            // Definiowanie kolorów rogów dla każdej ściany
            // bottom-left
            pixels[0] = bottomLeft.B;
            pixels[1] = bottomLeft.G;
            pixels[2] = bottomLeft.R;
            pixels[3] = bottomLeft.A;

            // bottom-right
            pixels[4] = bottomRight.B;
            pixels[5] = bottomRight.G;
            pixels[6] = bottomRight.R;
            pixels[7] = bottomRight.A;

            // top-left
            pixels[8] = topLeft.B;
            pixels[9] = topLeft.G;
            pixels[10] = topLeft.R;
            pixels[11] = topLeft.A;

            // top-right
            pixels[12] = topRight.B;
            pixels[13] = topRight.G;
            pixels[14] = topRight.R;
            pixels[15] = topRight.A;

            // Tworzenie bitmapy z użyciem zdefiniowanych kolorów
            return BitmapSource.Create(width, height, 96, 96, PixelFormats.Bgra32, null, pixels, stride);
        }
        private GeometryModel3D CreateColoredFace(Point3D p0, Point3D p1, Point3D p2, Point3D p3, Color c0, Color c1, Color c2, Color c3)
        {
            var mesh = new MeshGeometry3D
            {
                Positions = new Point3DCollection { p0, p1, p2, p3 },
                TriangleIndices = new Int32Collection { 0, 1, 2, 0, 2, 3 }
            };

            // Definiowanie UV (współrzędne tekstury)
            mesh.TextureCoordinates = new PointCollection
    {
        new Point(0, 0),  // p0
        new Point(1, 0),  // p1
        new Point(1, 1),  // p2
        new Point(0, 1)   // p3
    };

            // Tworzenie mapy kolorów (bitmapy) dla każdego z rogów
            var brush = new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(1, 1),
                GradientStops = new GradientStopCollection
        {
            new GradientStop(c0, 0.0),  // kolor p0
            new GradientStop(c1, 0.33), // kolor p1
            new GradientStop(c2, 0.66), // kolor p2
            new GradientStop(c3, 1.0)   // kolor p3
        }
            };

            var material = new DiffuseMaterial(brush);
            return new GeometryModel3D(mesh, material);
        }



        private void CreateSectionPlane()
        {
            // Create a larger mesh for the cutting plane (extending beyond the cube)
            var sectionMeshBuilder = new MeshBuilder();

            // Extend the cutting plane beyond the cube: -0.5 to 1.5 in X and Y
            sectionMeshBuilder.AddQuad(
                new Point3D(-0.5, -0.5, zAxisSlice),  // bottom-left (extend plane beyond the cube)
                new Point3D(1.5, -0.5, zAxisSlice),   // bottom-right (extend plane beyond the cube)
                new Point3D(1.5, 1.5, zAxisSlice),    // top-right (extend plane beyond the cube)
                new Point3D(-0.5, 1.5, zAxisSlice));  // top-left (extend plane beyond the cube)

            var sectionMesh = sectionMeshBuilder.ToMesh();

            // Set a semi-transparent material for the section plane
            var sectionMaterial = new DiffuseMaterial(new SolidColorBrush(Color.FromArgb(128, 255, 0, 0)));

            // Create the model for the cutting plane
            var sectionPlaneModel = new GeometryModel3D(sectionMesh, sectionMaterial);
            sectionPlaneModel.BackMaterial = sectionMaterial;
            // Store the visual for future updates
            sectionPlaneVisual = new ModelVisual3D { Content = sectionPlaneModel };

            // Add the cutting plane to the viewport
            helixViewport.Children.Add(sectionPlaneVisual);
        }

        private void UpdateSectionPlane()
        {
            // Rebuild the cutting plane mesh based on the current zAxisSlice value, extending beyond the cube
            var sectionMeshBuilder = new MeshBuilder();

            // Extend the cutting plane beyond the cube: -0.5 to 1.5 in X and Y
            sectionMeshBuilder.AddQuad(
                new Point3D(-0.5, -0.5, zAxisSlice),  // bottom-left (extend plane beyond the cube)
                new Point3D(1.5, -0.5, zAxisSlice),   // bottom-right (extend plane beyond the cube)
                new Point3D(1.5, 1.5, zAxisSlice),    // top-right (extend plane beyond the cube)
                new Point3D(-0.5, 1.5, zAxisSlice));  // top-left (extend plane beyond the cube)

            var sectionMesh = sectionMeshBuilder.ToMesh();

            // Update the geometry of the existing cutting plane
            ((GeometryModel3D)sectionPlaneVisual.Content).Geometry = sectionMesh;
        }

        private void zAxisSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Update the Z-axis slice value
            zAxisSlice = e.NewValue;

            // Redraw the 2D section based on the selected Z-axis slice
            DrawCubeSection();

            // Update the position of the section plane in 3D view
            UpdateSectionPlane();
        }

        private void DrawCubeSection()
        {
            // Clear the canvas
            sectionCanvas.Children.Clear();

            // Size of the grid for drawing (e.g., 10x10 squares)
            int gridSize = 10;
            double squareSize = sectionCanvas.Width / gridSize;

            // Loop through each point in the grid to calculate the color and draw squares
            for (int x = 0; x < gridSize; x++)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    // Calculate the color based on the position (x, y) and zAxisSlice
                    Color color = GetColorForPosition((double)x / gridSize, (double)y / gridSize, zAxisSlice);

                    // Draw a square with the calculated color
                    var rect = new System.Windows.Shapes.Rectangle
                    {
                        Width = squareSize,
                        Height = squareSize,
                        Fill = new SolidColorBrush(color),
                        Stroke = Brushes.Black,
                        StrokeThickness = 0.5
                    };

                    // Add the rectangle to the canvas
                    sectionCanvas.Children.Add(rect);
                    Canvas.SetLeft(rect, x * squareSize);
                    Canvas.SetTop(rect, y * squareSize);
                }
            }


        }

        // Get RGB color based on x, y, and z position in the cube
        private Color GetColorForPosition(double x, double y, double z)
        {
            byte red = (byte)(x * 255);   // Red depends on x-axis
            byte green = (byte)(y * 255); // Green depends on y-axis
            byte blue = (byte)(z * 255);  // Blue depends on z-axis (from the slider)

            return Color.FromRgb(red, green, blue);
        }
    }
}