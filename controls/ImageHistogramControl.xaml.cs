using OxyPlot.Series;
using OxyPlot;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using OxyPlot.Axes;

namespace drawingApp.Controls
{
    public partial class ImageHistogramControl : UserControl
    {
        public static readonly DependencyProperty BitmapSourceProperty =
              DependencyProperty.Register(
                  nameof(BitmapSource),
                  typeof(BitmapSource),
                  typeof(ImageHistogramControl),
                  new PropertyMetadata(null, OnBitmapSourceChanged));

        public static readonly DependencyProperty ThresholdProperty =
      DependencyProperty.Register(
          nameof(Threshold),
          typeof(double),
          typeof(ImageHistogramControl),
          new PropertyMetadata(0.0, OnThresholdChanged));

        public BitmapSource BitmapSource
        {
            get => (BitmapSource)GetValue(BitmapSourceProperty);
            set => SetValue(BitmapSourceProperty, value);
        }
        private LineSeries _thresholdLine;
        public double Threshold
        {
            get => (double)GetValue(ThresholdProperty);
            set => SetValue(ThresholdProperty, value);
        }
        public PlotModel HistogramModel { get; private set; }

        public ImageHistogramControl()
        {
            InitializeComponent();
            InitializeHistogramModel();
            ChannelSelector.SelectedIndex = 0;
            DataContext = this;
        }

        private void InitializeHistogramModel()
        {
            HistogramModel = new PlotModel { Title = "Image Histogram" };

            // Oś X dla intensywności (0-255)
            HistogramModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Minimum = 0,
                Maximum = 255,
                Title = "Intensity (0-255)",
                IsPanEnabled = false, // Blokowanie przesuwania osi X
                IsZoomEnabled = false // Blokowanie zmiany skali osi X
            });

            // Oś Y dla liczby pikseli
            HistogramModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Pixel Count",
                Minimum = 0,
                IsPanEnabled = false, // Blokowanie przesuwania osi Y
                IsZoomEnabled = false // Blokowanie zmiany skali osi Y
            });
        }
        // Metoda wywoływana przy zmianie źródła obrazu
        private static void OnBitmapSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImageHistogramControl control && e.NewValue is BitmapSource bitmap)
            {
                control.DrawHistogram(bitmap);
            }
        }

        // Metoda wywoływana przy zmianie progu
        private static void OnThresholdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImageHistogramControl control)
            {
                control.DrawThresholdLine((double)e.NewValue);
            }
        }

        // Rysowanie histogramu
        private void DrawHistogram(BitmapSource bitmap)
        {
            HistogramModel.Series.Clear();
            string selectedChannel = (ChannelSelector.SelectedItem as ComboBoxItem)?.Tag?.ToString();

            if (selectedChannel == "Grayscale")
            {
                AddGrayscaleHistogram(bitmap);
            }
            else if (selectedChannel == "RGB")
            {
                AddRgbHistogram(bitmap);
            }
            

            HistogramModel.InvalidatePlot(true);
        }

        private void AddGrayscaleHistogram(BitmapSource bitmap)
        {
            int[] grayscaleHistogram = new int[256];
            var pixels = new byte[bitmap.PixelWidth * bitmap.PixelHeight * 4];
            bitmap.CopyPixels(pixels, bitmap.PixelWidth * 4, 0);

            for (int i = 0; i < pixels.Length; i += 4)
            {
                byte r = pixels[i + 2];
                byte g = pixels[i + 1];
                byte b = pixels[i];
                byte grayscale = (byte)(0.299 * r + 0.587 * g + 0.114 * b);
                grayscaleHistogram[grayscale]++;
            }

            var series = new LineSeries
            {
                Title = "Grayscale",
                Color = OxyColors.Gray
            };

            for (int i = 0; i < 256; i++)
            {
                series.Points.Add(new DataPoint(i, grayscaleHistogram[i]));
            }

            HistogramModel.Series.Add(series);
        }

        private void AddRgbHistogram(BitmapSource bitmap)
        {
            int[] redHistogram = new int[256];
            int[] greenHistogram = new int[256];
            int[] blueHistogram = new int[256];
            var pixels = new byte[bitmap.PixelWidth * bitmap.PixelHeight * 4];
            bitmap.CopyPixels(pixels, bitmap.PixelWidth * 4, 0);

            for (int i = 0; i < pixels.Length; i += 4)
            {
                byte b = pixels[i];
                byte g = pixels[i + 1];
                byte r = pixels[i + 2];

                redHistogram[r]++;
                greenHistogram[g]++;
                blueHistogram[b]++;
            }

            var redSeries = new LineSeries
            {
                Title = "Red Channel",
                Color = OxyColors.Red
            };
            var greenSeries = new LineSeries
            {
                Title = "Green Channel",
                Color = OxyColors.Green
            };
            var blueSeries = new LineSeries
            {
                Title = "Blue Channel",
                Color = OxyColors.Blue
            };

            for (int i = 0; i < 256; i++)
            {
                redSeries.Points.Add(new DataPoint(i, redHistogram[i]));
                greenSeries.Points.Add(new DataPoint(i, greenHistogram[i]));
                blueSeries.Points.Add(new DataPoint(i, blueHistogram[i]));
            }

            HistogramModel.Series.Add(redSeries);
            HistogramModel.Series.Add(greenSeries);
            HistogramModel.Series.Add(blueSeries);
        }

        private void DrawThresholdLine(double threshold)
        {
            if (_thresholdLine == null)
            {
                // Dodajemy linię progową tylko raz
                _thresholdLine = new LineSeries
                {
                    Title = "Threshold Line",
                    Color = OxyColors.Red,
                    LineStyle = LineStyle.Dash,
                    StrokeThickness = 2
                };
                _thresholdLine.Points.Add(new DataPoint(threshold, 0));
                _thresholdLine.Points.Add(new DataPoint(threshold, HistogramModel.Axes[1].ActualMaximum));
                HistogramModel.Series.Add(_thresholdLine);
            }
            else
            {
                // Aktualizujemy położenie linii
                _thresholdLine.Points[0] = new DataPoint(threshold, 0);
                _thresholdLine.Points[1] = new DataPoint(threshold, _thresholdLine.Points[1].Y);
            }

            HistogramModel.InvalidatePlot(true); // Odświeżenie wykresu
        }

        private void OnChannelSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BitmapSource != null)
            {
                DrawHistogram(BitmapSource);
            }
        }
    }
}