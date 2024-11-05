using Accessibility;
using drawingApp.Models;
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
using System.Windows.Shapes;

namespace drawingApp.Views
{
    /// <summary>
    /// Logika interakcji dla klasy HistogramWindow.xaml
    /// </summary>
    public partial class HistogramWindow : Window,INotifyPropertyChanged
    {
        private double _threshold;
        private string _selectedMethodName;
        private string _thresholdText;

        public double Threshold
        {
            get => _threshold;
            set
            {
                _threshold = value;
                OnPropertyChanged(nameof(Threshold));
                OnPropertyChanged(nameof(ThresholdText));
            }
        }

        public string SelectedMethodName
        {
            get => _selectedMethodName;
            set
            {
                _selectedMethodName = value;
                OnPropertyChanged(nameof(SelectedMethodName));
            }
        }

        public string ThresholdText
        {
            get => _thresholdText;
            set
            {
                _thresholdText = value;
                OnPropertyChanged(nameof(ThresholdText));
            }
        }
        public BitmapSource BitmapSource {
            get => _bitmapSource;
            set { _bitmapSource = value; OnPropertyChanged(nameof(BitmapSource)); }
        }
        private BitmapSource _bitmapSource; 
        public event EventHandler<BitmapSource> ImageUpdated;
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public HistogramWindow()
        {
            InitializeComponent();
        }

        public HistogramWindow(BitmapSource bitmapSource)
        {
            InitializeComponent();
            BitmapSource = bitmapSource;
            DataContext = this;
        }

        // Obsługa kliknięcia przycisku rozszerzenia histogramu
        private void OnNormalizeHistogramExtensionClicked(object sender, RoutedEventArgs e)
        {
            NormalizeByHistogramExtension();
        }

        // Obsługa kliknięcia przycisku wyrównania histogramu
        private void OnNormalizeHistogramEqualizationClicked(object sender, RoutedEventArgs e)
        {
            NormalizeByHistogramEqualization();
        }

        private void NormalizeByHistogramExtension()
        {
            byte[] pixels = GetPixelData(BitmapSource);
            int width = BitmapSource.PixelWidth;
            int height = BitmapSource.PixelHeight;
            int stride = (width * BitmapSource.Format.BitsPerPixel + 7) / 8;

            byte[] normalizedPixels = NormalizeByHistogramExtensionARGB(pixels, width, height, stride);
            BitmapSource normalizedImage = CreateBitmapSourceFromPixels(normalizedPixels, width, height, stride);

            BitmapSource = normalizedImage;
            ImageUpdated?.Invoke(this, normalizedImage); // Wywołanie zdarzenia o zaktualizowanym obrazie
        }

        private void NormalizeByHistogramEqualization()
        {
            byte[] pixels = GetPixelData(BitmapSource);
            int width = BitmapSource.PixelWidth;
            int height = BitmapSource.PixelHeight;
            int stride = (width * BitmapSource.Format.BitsPerPixel + 7) / 8;

            byte[] equalizedPixels = NormalizeByHistogramEqualizationARGB(pixels, width, height, stride);
            BitmapSource equalizedImage = CreateBitmapSourceFromPixels(equalizedPixels, width, height, stride);

            BitmapSource = equalizedImage;
            ImageUpdated?.Invoke(this, equalizedImage); // Wywołanie zdarzenia o zaktualizowanym obrazie
        }

        private byte[] GetPixelData(BitmapSource bitmap)
        {
            int stride = (bitmap.PixelWidth * bitmap.Format.BitsPerPixel + 7) / 8;
            byte[] pixels = new byte[bitmap.PixelHeight * stride];
            bitmap.CopyPixels(pixels, stride, 0);
            return pixels;
        }

        private BitmapSource CreateBitmapSourceFromPixels(byte[] pixels, int width, int height, int stride)
        {
            return BitmapSource.Create(width, height, 96, 96, PixelFormats.Bgra32, null, pixels, stride);
        }

        private byte[] NormalizeByHistogramExtensionARGB(byte[] pixels, int width, int height, int stride)
        {
            byte minRed = 255, maxRed = 0;
            byte minGreen = 255, maxGreen = 0;
            byte minBlue = 255, maxBlue = 0;

            for (int i = 0; i < pixels.Length; i += 4)
            {
                byte b = pixels[i];
                byte g = pixels[i + 1];
                byte r = pixels[i + 2];

                minRed = Math.Min(minRed, r);
                maxRed = Math.Max(maxRed, r);
                minGreen = Math.Min(minGreen, g);
                maxGreen = Math.Max(maxGreen, g);
                minBlue = Math.Min(minBlue, b);
                maxBlue = Math.Max(maxBlue, b);
            }

            double scaleRed = 255.0 / (maxRed - minRed);
            double scaleGreen = 255.0 / (maxGreen - minGreen);
            double scaleBlue = 255.0 / (maxBlue - minBlue);

            byte[] normalizedPixels = new byte[pixels.Length];
            for (int i = 0; i < pixels.Length; i += 4)
            {
                normalizedPixels[i] = (byte)Math.Clamp(scaleBlue * (pixels[i] - minBlue), 0, 255);
                normalizedPixels[i + 1] = (byte)Math.Clamp(scaleGreen * (pixels[i + 1] - minGreen), 0, 255);
                normalizedPixels[i + 2] = (byte)Math.Clamp(scaleRed * (pixels[i + 2] - minRed), 0, 255);
                normalizedPixels[i + 3] = pixels[i + 3]; // Alpha
            }

            return normalizedPixels;
        }

        private byte[] NormalizeByHistogramEqualizationARGB(byte[] pixels, int width, int height, int stride)
        {
            int[] histogramRed = new int[256];
            int[] histogramGreen = new int[256];
            int[] histogramBlue = new int[256];

            for (int i = 0; i < pixels.Length; i += 4)
            {
                histogramBlue[pixels[i]]++;
                histogramGreen[pixels[i + 1]]++;
                histogramRed[pixels[i + 2]]++;
            }

            int[] cdfRed = new int[256];
            int[] cdfGreen = new int[256];
            int[] cdfBlue = new int[256];

            cdfRed[0] = histogramRed[0];
            cdfGreen[0] = histogramGreen[0];
            cdfBlue[0] = histogramBlue[0];

            for (int i = 1; i < 256; i++)
            {
                cdfRed[i] = cdfRed[i - 1] + histogramRed[i];
                cdfGreen[i] = cdfGreen[i - 1] + histogramGreen[i];
                cdfBlue[i] = cdfBlue[i - 1] + histogramBlue[i];
            }

            int totalPixels = width * height;
            int cdfRedMin = cdfRed.First(value => value > 0);
            int cdfGreenMin = cdfGreen.First(value => value > 0);
            int cdfBlueMin = cdfBlue.First(value => value > 0);

            byte[] equalizedPixels = new byte[pixels.Length];
            for (int i = 0; i < pixels.Length; i += 4)
            {
                equalizedPixels[i] = (byte)Math.Round((cdfBlue[pixels[i]] - cdfBlueMin) / (double)(totalPixels - cdfBlueMin) * 255);
                equalizedPixels[i + 1] = (byte)Math.Round((cdfGreen[pixels[i + 1]] - cdfGreenMin) / (double)(totalPixels - cdfGreenMin) * 255);
                equalizedPixels[i + 2] = (byte)Math.Round((cdfRed[pixels[i + 2]] - cdfRedMin) / (double)(totalPixels - cdfRedMin) * 255);
                equalizedPixels[i + 3] = pixels[i + 3]; // Alpha
            }

            return equalizedPixels;
        }


        private void OnThresholdMethodChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedMethod = (ThresholdMethodSelector.SelectedItem as ComboBoxItem)?.Tag?.ToString();
            SelectedMethodName = (ThresholdMethodSelector.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";

            switch (selectedMethod)
            {
                case "Manual":
                    ThresholdSlider.Minimum = 0;
                    ThresholdSlider.Maximum = 255;
                    ThresholdSlider.Visibility = Visibility.Visible;
                    ThresholdSlider.Value = Threshold;
                    ThresholdText = Threshold.ToString("0");
                    break;

                case "PercentBlack":
                    ThresholdSlider.Minimum = 0;
                    ThresholdSlider.Maximum = 100;
                    ThresholdSlider.Visibility = Visibility.Visible;
                    Threshold = PercentBlackSelection(BitmapSource);
                    ThresholdSlider.Value = Threshold; // Ustawienie progu na podstawie obliczeń
                    ThresholdText = $"{ThresholdSlider.Value:0}%";
                    break;

                default:
                    ThresholdSlider.Visibility = Visibility.Collapsed;
                    Threshold = CalculateAutomaticThreshold(selectedMethod, BitmapSource);
                    ThresholdText = Threshold.ToString("0");
                    ApplyBinarization(Threshold);

                    break;
            }
        }


        private void ApplyBinarization(double threshold)
        {
            byte[] pixels = GetPixelData(BitmapSource);
            int width = BitmapSource.PixelWidth;
            int height = BitmapSource.PixelHeight;
            int stride = (width * BitmapSource.Format.BitsPerPixel + 7) / 8;

            byte[] binarizedPixels = new byte[pixels.Length];
            for (int i = 0; i < pixels.Length; i += 4)
            {
                byte intensity = (byte)(0.299 * pixels[i + 2] + 0.587 * pixels[i + 1] + 0.114 * pixels[i]);
                byte binaryValue = (byte)(intensity >= threshold ? 255 : 0);
                binarizedPixels[i] = binarizedPixels[i + 1] = binarizedPixels[i + 2] = binaryValue;
                binarizedPixels[i + 3] = pixels[i + 3];
            }

            BinarizedImageDisplay.Source = CreateBitmapSourceFromPixels(binarizedPixels, width, height, stride);
        }

        private void OnThresholdSliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Threshold = e.NewValue;

            if (ThresholdMethodSelector.SelectedItem is ComboBoxItem selectedItem && (string)selectedItem.Tag == "PercentBlack")
            {
                ThresholdText = $"{Threshold:0}%";
                Threshold = PercentBlackSelection(BitmapSource); // Aktualizuj próg dla procentowej metody
            }
            else
            {
                ThresholdText = Threshold.ToString("0");
            }

            ApplyBinarization(Threshold);
        }

        private double CalculateAutomaticThreshold(string method, BitmapSource bitmap)
        {
            return method switch
            {
                "MeanIterative" => MeanIterativeSelection(bitmap),
                "Entropy" => EntropySelection(bitmap),
                "MinimumError" => MinimumErrorSelection(bitmap),
                "FuzzyMinimumError" => FuzzyMinimumErrorSelection(bitmap),
                _ => 127
            };

        }


        private int PercentBlackSelection(BitmapSource bitmap)
        {
            int[] histogram = CalculateGrayscaleHistogram(bitmap);
            int totalPixels = bitmap.PixelWidth * bitmap.PixelHeight;

            // Przetłumacz procent na wartość progu na podstawie histogramu
            double targetCount = totalPixels * (ThresholdSlider.Value / 100); 

            int cumulativeCount = 0;
            for (int i = 0; i < histogram.Length; i++)
            {
                cumulativeCount += histogram[i];
                if (cumulativeCount >= targetCount)
                {
                    return i;
                }
            }
            return 127; // Domyślny próg w przypadku nieznalezienia odpowiedniego
        }

        private int MeanIterativeSelection(BitmapSource bitmap)
        {
            // Obliczamy histogram dla obrazu w skali szarości
            int[] histogram = CalculateGrayscaleHistogram(bitmap);

            // Początkowa wartość progu (przykładowo ustawiona na środek skali intensywności)
            int threshold = 127;
            bool hasConverged; // Flaga do sprawdzania, czy iteracje zakończyły się zbieżnością

            do
            {
                double lowerSum = 0, upperSum = 0;
                double lowerCount = 0, upperCount = 0;

                // Obliczamy sumę wartości i liczbę pikseli w części histogramu poniżej obecnego progu
                for (int i = 0; i <= threshold; i++)
                {
                    lowerSum += i * histogram[i]; // Suma iloczynów intensywności i liczności pikseli dla dolnej części
                    lowerCount += histogram[i];   // Całkowita liczba pikseli w dolnej części
                }

                // Obliczamy sumę wartości i liczbę pikseli w części histogramu powyżej obecnego progu
                for (int i = threshold + 1; i < histogram.Length; i++)
                {
                    upperSum += i * histogram[i]; // Suma iloczynów intensywności i liczności pikseli dla górnej części
                    upperCount += histogram[i];   // Całkowita liczba pikseli w górnej części
                }

                // Wyliczamy nowy próg według wzoru, dzieląc każdą część przez 2
                int newThreshold = (int)((lowerSum / (2 * lowerCount)) + (upperSum / (2 * upperCount)));

                // Sprawdzamy, czy iteracja zbiega się do rozwiązania (czy nowy próg jest równy poprzedniemu)
                hasConverged = newThreshold == threshold;

                // Ustawiamy próg na nowo wyliczoną wartość
                threshold = newThreshold;

            } while (!hasConverged); // Kontynuujemy iterację, aż próg nie zmieni się (osiągnięcie zbieżności)

            return threshold; // Zwracamy wartość progu po osiągnięciu zbieżności
        }

        private int EntropySelection(BitmapSource bitmap)
        {
            // Oblicz histogram skali szarości dla bitmapy
            int[] histogram = CalculateGrayscaleHistogram(bitmap);
            int totalPixels = bitmap.PixelWidth * bitmap.PixelHeight;

            double maxEntropy = double.MinValue;
            int threshold = 127; // Domyślny próg na środku zakresu (0-255)

            // Iterujemy przez wszystkie możliwe progi
            for (int t = 1; t < histogram.Length - 1; t++) // Pomiń wartości 0 i 255, bo mogą być ekstremalne
            {
                // Waga dla klasy poniżej progu
                double w0 = histogram.Take(t).Sum() / (double)totalPixels;
                // Waga dla klasy powyżej progu
                double w1 = histogram.Skip(t).Sum() / (double)totalPixels;

                // Jeśli którakolwiek z wag jest zerowa, pomijamy ten próg
                if (w0 == 0 || w1 == 0) continue;

                // Obliczamy entropię dla klasy poniżej progu
                double entropy0 = 0;
                foreach (var h in histogram.Take(t).Where(h => h > 0))
                {
                    double p = h / (double)(histogram.Take(t).Sum()); // Normalizujemy do klasy poniżej progu
                    entropy0 += p * Math.Log(p, 2); // Użycie logarytmu w podstawie 2
                }

                // Obliczamy entropię dla klasy powyżej progu
                double entropy1 = 0;
                foreach (var h in histogram.Skip(t).Where(h => h > 0))
                {
                    double p = h / (double)(histogram.Skip(t).Sum()); // Normalizujemy do klasy powyżej progu
                    entropy1 += p * Math.Log(p, 2); // Użycie logarytmu w podstawie 2
                }

                // Sumaryczna entropia ważona dla danego progu
                double entropy = -w0 * entropy0 - w1 * entropy1;

                // Aktualizacja maksymalnej entropii i odpowiedniego progu
                if (entropy > maxEntropy)
                {
                    maxEntropy = entropy;
                    threshold = t;
                }
            }

            return threshold;
        }
        private int MinimumErrorSelection(BitmapSource bitmap)
        {
            // Oblicz histogram obrazu w skali szarości
            int[] histogram = CalculateGrayscaleHistogram(bitmap);
            int totalPixels = bitmap.PixelWidth * bitmap.PixelHeight;

            double minError = double.MaxValue;
            int threshold = 127;

            // Iterujemy przez wszystkie możliwe progi
            for (int t = 0; t < histogram.Length; t++)
            {
                // Waga dla klasy poniżej progu
                double w0 = histogram.Take(t).Sum() / (double)totalPixels;
                // Waga dla klasy powyżej progu
                double w1 = histogram.Skip(t).Sum() / (double)totalPixels;

                if (w0 == 0 || w1 == 0) continue;

                // Średnia dla klasy poniżej progu
                double mean0 = histogram.Take(t).Select((h, i) => i * h).Sum() / histogram.Take(t).Sum();
                // Średnia dla klasy powyżej progu
                double mean1 = histogram.Skip(t).Select((h, i) => (i + t) * h).Sum() / histogram.Skip(t).Sum();

                // Wariancja dla klasy poniżej progu
                double variance0 = histogram.Take(t).Select((h, i) => Math.Pow(i - mean0, 2) * h).Sum() / histogram.Take(t).Sum();
                // Wariancja dla klasy powyżej progu
                double variance1 = histogram.Skip(t).Select((h, i) => Math.Pow(i + t - mean1, 2) * h).Sum() / histogram.Skip(t).Sum();

                // Obliczenie funkcji błędu zgodnie ze wzorem na p(g)
                double term1 = 1 / (Math.Sqrt(2 * Math.PI * variance0)) * Math.Exp(-Math.Pow(t - mean0, 2) / (2 * variance0));
                double term2 = 1 / (Math.Sqrt(2 * Math.PI * variance1)) * Math.Exp(-Math.Pow(t - mean1, 2) / (2 * variance1));

                double p_g = w0 * term1 + w1 * term2;

                // Obliczenie błędu z wykorzystaniem p(g) i wag w0 oraz w1
                double error = -Math.Log(p_g);

                // Sprawdzenie, czy ten próg daje mniejszy błąd, jeśli tak, to aktualizujemy wartość progu
                if (error < minError)
                {
                    minError = error;
                    threshold = t;
                }
            }

            return threshold;
        }


        private int FuzzyMinimumErrorSelection(BitmapSource bitmap)
        {
            int[] histogram = CalculateGrayscaleHistogram(bitmap);
            int totalPixels = bitmap.PixelWidth * bitmap.PixelHeight;

            double minFuzzyError = double.MaxValue;
            int threshold = 127;

            for (int t = 0; t < histogram.Length; t++)
            {
                double w0 = histogram.Take(t).Sum() / (double)totalPixels;
                double w1 = histogram.Skip(t).Sum() / (double)totalPixels;

                if (w0 == 0 || w1 == 0) continue;

                double mean0 = histogram.Take(t).Select((h, i) => i * h).Sum() / histogram.Take(t).Sum();
                double mean1 = histogram.Skip(t).Select((h, i) => (i + t) * h).Sum() / histogram.Skip(t).Sum();

                double fuzzyError = w0 * w1 * Math.Pow(mean0 - mean1, 2);

                if (fuzzyError < minFuzzyError)
                {
                    minFuzzyError = fuzzyError;
                    threshold = t;
                }
            }

            return threshold;
        }


        private int[] CalculateGrayscaleHistogram(BitmapSource bitmap)
        {
            int[] histogram = new int[256];
            byte[] pixels = GetPixelData(bitmap);

            for (int i = 0; i < pixels.Length; i += 4)
            {
                byte r = pixels[i + 2];
                byte g = pixels[i + 1];
                byte b = pixels[i];
                byte grayscale = (byte)(0.299 * r + 0.587 * g + 0.114 * b);
                histogram[grayscale]++;
            }

            return histogram;
        }
    }
}

