using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace drawingApp.Models
{
    public abstract class Filter
    {
        public abstract BitmapSource Apply(BitmapSource source);
        public override string ToString()
        {
            return GetType().Name;
        }
    }

    public abstract class PointTransformFilter : Filter
    {
        public Func<byte, byte, byte, (byte R, byte G, byte B)> TransformFunction { get; set; }

        protected PointTransformFilter(Func<byte, byte, byte, (byte R, byte G, byte B)> transformFunction)
        {
            TransformFunction = transformFunction;
        }

        public override BitmapSource Apply(BitmapSource source)
        {
            var writableBitmap = new WriteableBitmap(source);
            int width = writableBitmap.PixelWidth;
            int height = writableBitmap.PixelHeight;
            int stride = width * ((writableBitmap.Format.BitsPerPixel + 7) / 8);
            byte[] pixels = new byte[height * stride];
            writableBitmap.CopyPixels(pixels, stride, 0);

            for (int i = 0; i < pixels.Length; i += 4)
            {
                var (newR, newG, newB) = TransformFunction(pixels[i + 2], pixels[i + 1], pixels[i]);
                pixels[i] = ClampToByte(newB);     // B
                pixels[i + 1] = ClampToByte(newG); // G
                pixels[i + 2] = ClampToByte(newR); // R
            }

            writableBitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);
            return writableBitmap;
        }

        protected static byte ClampToByte(int value) => (byte)Math.Max(0, Math.Min(255, value));
    }

    public class AddFilter : PointTransformFilter
    {
        public int RedAdjustment { get; set; }
        public int GreenAdjustment { get; set; }
        public int BlueAdjustment { get; set; }

        public AddFilter(int redAdjustment, int greenAdjustment, int blueAdjustment)
            : base((r, g, b) => (0, 0, 0)) // Placeholder
        {
            RedAdjustment = redAdjustment;
            GreenAdjustment = greenAdjustment;
            BlueAdjustment = blueAdjustment;

            // Updated TransformFunction to use properties
            TransformFunction = (r, g, b) => (ClampToByte(r + RedAdjustment),
                                              ClampToByte(g + GreenAdjustment),
                                              ClampToByte(b + BlueAdjustment));
        }
        public override string ToString() => "Add Filter";
    }

    public class SubtractFilter : PointTransformFilter
    {
        public int RedAdjustment { get; set; }
        public int GreenAdjustment { get; set; }
        public int BlueAdjustment { get; set; }

        public SubtractFilter(int redAdjustment, int greenAdjustment, int blueAdjustment)
            : base((r, g, b) => (ClampToByte(r - redAdjustment), ClampToByte(g - greenAdjustment), ClampToByte(b - blueAdjustment)))
        {
            RedAdjustment = redAdjustment;
            GreenAdjustment = greenAdjustment;
            BlueAdjustment = blueAdjustment;

            TransformFunction = (r, g, b) => (ClampToByte(r - RedAdjustment),
                                          ClampToByte(g - GreenAdjustment),
                                          ClampToByte(b - BlueAdjustment));
        }
        public override string ToString() => "Subtract Filter";
    }

    public class MultiplyFilter : PointTransformFilter
    {
        public double RedFactor { get; set; }
        public double GreenFactor { get; set; }
        public double BlueFactor { get; set; }

        public MultiplyFilter(double redFactor, double greenFactor, double blueFactor)
            : base((r, g, b) => (ClampToByte((int)(r * redFactor)), ClampToByte((int)(g * greenFactor)), ClampToByte((int)(b * blueFactor))))
        {
            RedFactor = redFactor;
            GreenFactor = greenFactor;
            BlueFactor = blueFactor;

            TransformFunction = (r, g, b) => (ClampToByte((int)Math.Round(r * RedFactor)),
                                          ClampToByte((int)Math.Round(g * GreenFactor)),
                                          ClampToByte((int)Math.Round(b * BlueFactor)));

        }
        public override string ToString() => "Multiply Filter";
    }

    public class DivideFilter : PointTransformFilter
    {
        public double RedDivisor { get; set; }
        public double GreenDivisor { get; set; }
        public double BlueDivisor { get; set; }

        public DivideFilter(double redDivisor, double greenDivisor, double blueDivisor)
            : base((r, g, b) => (ClampToByte((int)(r / redDivisor)), ClampToByte((int)(g / greenDivisor)), ClampToByte((int)(b / blueDivisor))))
        {
            RedDivisor = redDivisor;
            GreenDivisor = greenDivisor;
            BlueDivisor = blueDivisor;

            TransformFunction = (r, g, b) => (
            ClampToByte((int)Math.Round(r / Math.Max(RedDivisor, 0.01))),
            ClampToByte((int)Math.Round(g / Math.Max(GreenDivisor, 0.01))),
            ClampToByte((int)Math.Round(b / Math.Max(BlueDivisor, 0.01)))
            );

        }
        public override string ToString() => "Divide Filter";
    }
    public class BrightnessFilter : Filter
    {
        public int BrightnessAdjustment { get; set; }

        public BrightnessFilter(int brightnessAdjustment)
        {
            BrightnessAdjustment = brightnessAdjustment;
        }

        public override BitmapSource Apply(BitmapSource source)
        {
            var writableBitmap = new WriteableBitmap(source);
            int width = writableBitmap.PixelWidth;
            int height = writableBitmap.PixelHeight;
            int stride = width * ((writableBitmap.Format.BitsPerPixel + 7) / 8);
            byte[] pixels = new byte[height * stride];
            writableBitmap.CopyPixels(pixels, stride, 0);

            for (int i = 0; i < pixels.Length; i += 4)
            {
                pixels[i] = ClampToByte(pixels[i] + BrightnessAdjustment);     // B
                pixels[i + 1] = ClampToByte(pixels[i + 1] + BrightnessAdjustment); // G
                pixels[i + 2] = ClampToByte(pixels[i + 2] + BrightnessAdjustment); // R
            }

            writableBitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);
            return writableBitmap;
        }

        private byte ClampToByte(int value) => (byte)Math.Max(0, Math.Min(255, value));

        public override string ToString() => "Brightness Filter";
    }
    public class GrayscaleAverageFilter : Filter
    {
        public override BitmapSource Apply(BitmapSource source)
        {
            var writableBitmap = new WriteableBitmap(source);
            int width = writableBitmap.PixelWidth;
            int height = writableBitmap.PixelHeight;
            int stride = width * ((writableBitmap.Format.BitsPerPixel + 7) / 8);
            byte[] pixels = new byte[height * stride];
            writableBitmap.CopyPixels(pixels, stride, 0);

            for (int i = 0; i < pixels.Length; i += 4)
            {
                byte gray = (byte)((pixels[i] + pixels[i + 1] + pixels[i + 2]) / 3);
                pixels[i] = gray;
                pixels[i + 1] = gray;
                pixels[i + 2] = gray;
            }

            writableBitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);
            return writableBitmap;
        }
        public override string ToString() => "Grayscale Average Filter";
    }
    public class GrayscaleWeightedFilter : Filter
    {
        public override BitmapSource Apply(BitmapSource source)
        {
            var writableBitmap = new WriteableBitmap(source);
            int width = writableBitmap.PixelWidth;
            int height = writableBitmap.PixelHeight;
            int stride = width * ((writableBitmap.Format.BitsPerPixel + 7) / 8);
            byte[] pixels = new byte[height * stride];
            writableBitmap.CopyPixels(pixels, stride, 0);

            for (int i = 0; i < pixels.Length; i += 4)
            {
                byte gray = (byte)(0.3 * pixels[i + 2] + 0.59 * pixels[i + 1] + 0.11 * pixels[i]);
                pixels[i] = gray;
                pixels[i + 1] = gray;
                pixels[i + 2] = gray;
            }

            writableBitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);
            return writableBitmap;
        }
        public override string ToString() => "Grayscale Weighted Filter";
    }
    public abstract class ConvolutionFilter : Filter
    {
        public double[,] Mask { get; set; }
        public ObservableCollection<double> FlattenedMask { get; set; }
        private int size;
        public int Size
        {
            get => size;
            set
            {
                if (size != value)
                {
                    size = value;
                    UpdateMask();
                    RefreshMask();
                }
            }
        }

        public ConvolutionFilter()
        {
            Mask = new double[,] { { } };
            FlattenedMask = new ObservableCollection<double>();
        }
        public void RefreshMask()
        {
            FlattenedMask.Clear();
            foreach (var value in Mask.Cast<double>())
            {
                FlattenedMask.Add(value);
            }
        }
        
        protected abstract void UpdateMask();
        public override BitmapSource Apply(BitmapSource source)
        {
            int maskSize = Mask.GetLength(0);
            var writableBitmap = new WriteableBitmap(source);
            int width = writableBitmap.PixelWidth;
            int height = writableBitmap.PixelHeight;
            int stride = width * ((writableBitmap.Format.BitsPerPixel + 7) / 8);
            byte[] pixels = new byte[height * stride];
            writableBitmap.CopyPixels(pixels, stride, 0);

            int offset = maskSize / 2;
            byte[] paddedPixels = PadImageWithMirror(pixels, width, height, stride, offset);
            byte[] resultPixels = new byte[pixels.Length];

            for (int y = offset; y < height + offset; y++)
            {
                for (int x = offset; x < width + offset; x++)
                {
                    double[] newColor = new double[3];
                    for (int ky = -offset; ky <= offset; ky++)
                    {
                        for (int kx = -offset; kx <= offset; kx++)
                        {
                            int pixelIndex = ((y + ky) * (stride + 2 * offset * 4)) + ((x + kx) * 4);
                            double weight = Mask[ky + offset, kx + offset];

                            newColor[0] += paddedPixels[pixelIndex] * weight;
                            newColor[1] += paddedPixels[pixelIndex + 1] * weight;
                            newColor[2] += paddedPixels[pixelIndex + 2] * weight;
                        }
                    }

                    int resultIndex = ((y - offset) * stride) + ((x - offset) * 4);
                    resultPixels[resultIndex] = ClampToByte(newColor[0]);
                    resultPixels[resultIndex + 1] = ClampToByte(newColor[1]);
                    resultPixels[resultIndex + 2] = ClampToByte(newColor[2]);
                    resultPixels[resultIndex + 3] = pixels[resultIndex + 3];
                }
            }

            writableBitmap.WritePixels(new Int32Rect(0, 0, width, height), resultPixels, stride, 0);
            return writableBitmap;
        }

        protected static byte ClampToByte(double value) => (byte)Math.Max(0, Math.Min(255, value));

        private byte[] PadImageWithMirror(byte[] pixels, int width, int height, int stride, int offset)
        {
            int paddedWidth = width + 2 * offset;
            int paddedHeight = height + 2 * offset;
            int paddedStride = paddedWidth * 4;
            byte[] paddedPixels = new byte[paddedHeight * paddedStride];

            // Copy the central region
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int srcIndex = (y * stride) + (x * 4);
                    int dstIndex = ((y + offset) * paddedStride) + ((x + offset) * 4);
                    Array.Copy(pixels, srcIndex, paddedPixels, dstIndex, 4);
                }
            }

            // Mirror the edges
            // Top and bottom padding
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < offset; y++)
                {
                    int srcIndexTop = ((offset - y) * paddedStride) + ((x + offset) * 4);
                    int dstIndexTop = (y * paddedStride) + ((x + offset) * 4);
                    int srcIndexBottom = ((height - 1 - y + offset) * paddedStride) + ((x + offset) * 4);
                    int dstIndexBottom = ((height + offset + y) * paddedStride) + ((x + offset) * 4);

                    Array.Copy(paddedPixels, srcIndexTop, paddedPixels, dstIndexTop, 4);
                    Array.Copy(paddedPixels, srcIndexBottom, paddedPixels, dstIndexBottom, 4);
                }
            }

            // Left and right padding
            for (int y = 0; y < paddedHeight; y++)
            {
                for (int x = 0; x < offset; x++)
                {
                    int srcIndexLeft = (y * paddedStride) + ((offset - x) * 4);
                    int dstIndexLeft = (y * paddedStride) + (x * 4);
                    int srcIndexRight = (y * paddedStride) + ((width - 1 + offset - x) * 4);
                    int dstIndexRight = (y * paddedStride) + ((width + offset + x) * 4);

                    Array.Copy(paddedPixels, srcIndexLeft, paddedPixels, dstIndexLeft, 4);
                    Array.Copy(paddedPixels, srcIndexRight, paddedPixels, dstIndexRight, 4);
                }
            }

            return paddedPixels;
        }
    }
    public class SmoothingFilter : ConvolutionFilter
    {
        public SmoothingFilter(int size = 3)
        {
            Size = size;
            UpdateMask();
        }

        protected override void UpdateMask()
        {
            Mask = new double[Size, Size];
            for (int i = 0; i < Size; i++)
                for (int j = 0; j < Size; j++)
                    Mask[i, j] = 1.0;

            double sum = Mask.Cast<double>().Sum();
            for (int i = 0; i < Size; i++)
                for (int j = 0; j < Size; j++)
                    Mask[i, j] /= sum;

            RefreshMask();
        }

        public override string ToString() => "Smoothing Filter";
    }
    public class SobelHorizontalFilter : ConvolutionFilter
    {
        public SobelHorizontalFilter(int size = 3)
        {
            Size = size;
            UpdateMask();
        }

        protected override void UpdateMask()
        {
            Mask = new double[Size, Size];
            int center = Size / 2;

            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    if (i == center) Mask[i, j] = 0;
                    else Mask[i, j] = (i < center ? -1 : 1) * (j == center ? 2 : 1);
                }
            }

            RefreshMask();
        }

        public override string ToString() => "Sobel Horizontal Filter";
    }
    public class SobelVerticalFilter : ConvolutionFilter
    {
        public SobelVerticalFilter(int size = 3)
        {
            Size = size;
            UpdateMask();
        }

        protected override void UpdateMask()
        {
            Mask = new double[Size, Size];
            int center = Size / 2;

            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    if (j == center) Mask[i, j] = 0;
                    else Mask[i, j] = (j < center ? -1 : 1) * (i == center ? 2 : 1);
                }
            }

            RefreshMask();
        }

        public override string ToString() => "Sobel Vertical Filter";
    }
    public class MedianFilter : Filter
    {
        public int Size { get; set; }
        public MedianFilter(int maskSize = 3)
        {
            Size = maskSize;
        }
        public override string ToString() => "Median Filter";
        public override BitmapSource Apply(BitmapSource source)
        {
            var writableBitmap = new WriteableBitmap(source);
            int width = writableBitmap.PixelWidth;
            int height = writableBitmap.PixelHeight;
            int stride = width * ((writableBitmap.Format.BitsPerPixel + 7) / 8);
            byte[] pixels = new byte[height * stride];
            writableBitmap.CopyPixels(pixels, stride, 0);
            byte[] resultPixels = new byte[pixels.Length];

            int offset = Size / 2;

            for (int y = offset; y < height - offset; y++)
            {
                for (int x = offset; x < width - offset; x++)
                {
                    List<byte> neighborhoodR = new List<byte>();
                    List<byte> neighborhoodG = new List<byte>();
                    List<byte> neighborhoodB = new List<byte>();

                    for (int ky = -offset; ky <= offset; ky++)
                    {
                        for (int kx = -offset; kx <= offset; kx++)
                        {
                            int index = ((y + ky) * stride) + ((x + kx) * 4);
                            neighborhoodB.Add(pixels[index]);
                            neighborhoodG.Add(pixels[index + 1]);
                            neighborhoodR.Add(pixels[index + 2]);
                        }
                    }

                    resultPixels[(y * stride) + (x * 4)] = Median(neighborhoodB);
                    resultPixels[(y * stride) + (x * 4) + 1] = Median(neighborhoodG);
                    resultPixels[(y * stride) + (x * 4) + 2] = Median(neighborhoodR);
                    resultPixels[(y * stride) + (x * 4) + 3] = pixels[(y * stride) + (x * 4) + 3]; // Alpha
                }
            }

            writableBitmap.WritePixels(new Int32Rect(0, 0, width, height), resultPixels, stride, 0);
            return writableBitmap;
        }

        private byte Median(List<byte> values)
        {
            values.Sort();
            return values[values.Count / 2];
        }
    }
    public class HighPassFilter : ConvolutionFilter
    {
        private int centerValue;
        public int CenterValue
        {
            get => centerValue;
            set
            {
                if (centerValue != value)
                {
                    centerValue = value;
                    UpdateMask();
                }
            }
        }

        public HighPassFilter(int size = 3, int centerValue = 9)
        {
            Size = size;
            CenterValue = centerValue;
            UpdateMask();

        }

        protected override void UpdateMask()
        {
            Mask = new double[Size, Size];
            int center = Size / 2;

            for (int i = 0; i < Size; i++)
                for (int j = 0; j < Size; j++)
                    Mask[i, j] = -1;

            Mask[center, center] = CenterValue;

            base.RefreshMask();
        }

        public override string ToString() => "High Pass Filter";
    }
    public enum LowPassFilterMode
    {
        Uniform,    // Wszystkie wartości 1
        CenterWeighted,  // Centrum = A, reszta = 1
        SideWeighted // B na bokach, B^2 w centrum, reszta = 1
    }
    public class LowPassFilter : ConvolutionFilter
    {
        private double parameter;
        private LowPassFilterMode filterMode;

        public double Parameter
        {
            get => parameter;
            set
            {
                if (parameter != value)
                {
                    parameter = value;
                    UpdateMask();
                }
            }
        }

        public LowPassFilterMode FilterMode
        {
            get => filterMode;
            set
            {
                if (filterMode != value)
                {
                    filterMode = value;
                    UpdateMask();
                }
            }
        }

        public LowPassFilter(int size = 3, double parameter = 1, LowPassFilterMode mode = LowPassFilterMode.Uniform)
        {
            Size = size;
            Parameter = parameter;
            FilterMode = mode;
            UpdateMask();
        }

        protected override void UpdateMask()
        {
            Mask = new double[Size, Size];
            int center = Size / 2;

            switch (FilterMode)
            {
                case LowPassFilterMode.Uniform:
                    // Wszystkie wartości 1
                    for (int i = 0; i < Size; i++)
                    {
                        for (int j = 0; j < Size; j++)
                        {
                            Mask[i, j] = 1;
                        }
                    }
                    break;

                case LowPassFilterMode.CenterWeighted:
                    // Centrum = Parameter, reszta = 1
                    for (int i = 0; i < Size; i++)
                    {
                        for (int j = 0; j < Size; j++)
                        {
                            Mask[i, j] = (i == center && j == center) ? Parameter : 1;
                        }
                    }
                    break;

                case LowPassFilterMode.SideWeighted:
                    // B na bokach, B^2 w centrum, reszta = 1
                    for (int i = 0; i < Size; i++)
                    {
                        for (int j = 0; j < Size; j++)
                        {
                            if (i == center && j == center)
                                Mask[i, j] = Parameter * Parameter;
                            else if (i == center || j == center)
                                Mask[i, j] = Parameter;
                            else
                                Mask[i, j] = 1;
                        }
                    }
                    break;
            }

            // Normalizacja maski
            double sum = Mask.Cast<double>().Sum();
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    Mask[i, j] /= sum;
                }
            }

            RefreshMask();
        }

        public override string ToString() => "Low Pass Filter";
    }
    public class GaussianBlurFilter : ConvolutionFilter
    {
        private double sigma;
        public double Sigma
        {
            get => sigma;
            set
            {
                if (sigma != value)
                {
                    sigma = value;
                    UpdateMask();

                }
            }
        }

        public GaussianBlurFilter(int size = 5, double sigma = 1.0)
        {
            Size = size;
            Sigma = sigma;
            UpdateMask();
        }

        protected override void UpdateMask()
        {
            Mask = new double[Size, Size];
            int radius = Size / 2;
            double sum = 0;

            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    int x = i - radius;
                    int y = j - radius;

                    // Gaussian function
                    double value = Math.Exp(-(x * x + y * y) / (2 * Sigma * Sigma)) / (2 * Math.PI * Sigma * Sigma);
                    Mask[i, j] = value;
                    sum += value;
                }
            }

            // Normalize the mask values so they sum to 1
            for (int i = 0; i < Size; i++)
                for (int j = 0; j < Size; j++)
                    Mask[i, j] /= sum;

            RefreshMask();
        }

        public override string ToString() => "Gaussian Blur Filter";
    }
    public class CustomFilter : ConvolutionFilter
    {
        public ObservableCollection<MatrixElement> FlattenedMask { get; set; }

        public CustomFilter(int size = 3)
        {
            FlattenedMask = new ObservableCollection<MatrixElement>();
            Size = size;
            InitializeCustomMask();
        }

        private void InitializeCustomMask()
        {
            // Wyczyść poprzednią zawartość i ponownie inicjalizuj `FlattenedMask` zgodnie z `Size`
            FlattenedMask.Clear();

            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    var element = new MatrixElement(i, j, 1.0); // Domyślna wartość 1.0
                    element.PropertyChanged += (s, e) => SyncMaskWithFlattenedMask();
                    FlattenedMask.Add(element);
                }
            }
            UpdateMask(); // Zaktualizuj `Mask` po inicjalizacji `FlattenedMask`
        }

        protected override void UpdateMask()
        {
            // Zaktualizuj dwuwymiarową macierz `Mask` na podstawie `FlattenedMask`
            Mask = new double[Size, Size];

            foreach (var element in FlattenedMask)
            {
                if (element.Row < Size && element.Column < Size)
                {
                    Mask[element.Row, element.Column] = element.Value;
                }
            }

            RefreshMask();
        }

        private void SyncMaskWithFlattenedMask()
        {
            // Aktualizuje `Mask` na podstawie aktualnych wartości w `FlattenedMask`
            foreach (var element in FlattenedMask)
            {
                if (element.Row < Size && element.Column < Size)
                {
                    Mask[element.Row, element.Column] = element.Value;
                }
            }
        }

        public override string ToString() => "Custom Filter";

        public new int Size
        {
            get => base.Size;
            set
            {
                if (base.Size != value)
                {
                    base.Size = value;
                    InitializeCustomMask(); // Resetuje `FlattenedMask` na podstawie nowego rozmiaru
                }
            }
        }
    }

    public class MatrixElement : INotifyPropertyChanged
    {
        private double value;
        public int Row { get; }
        public int Column { get; }

        public double Value
        {
            get => value;
            set
            {
                if (this.value != value)
                {
                    this.value = value;
                    OnPropertyChanged(nameof(Value));
                }
            }
        }

        public MatrixElement(int row, int column, double initialValue)
        {
            Row = row;
            Column = column;
            Value = initialValue;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
