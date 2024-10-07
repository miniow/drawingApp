using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.ComponentModel;

namespace drawingApp.Models
{

    public abstract class ResizeAdorner : Adorner
    {
        protected VisualCollection visualChildren;
        protected Thumb topLeft, topRight, bottomLeft, bottomRight;
        protected Thumb moveThumb;

        protected const double ThumbSize = 10;

        public ResizeAdorner(UIElement adornedElement) : base(adornedElement)
        {
            visualChildren = new VisualCollection(this);

            BuildAdornerCorner(ref topLeft, Cursors.SizeNWSE);
            BuildAdornerCorner(ref topRight, Cursors.SizeNESW);
            BuildAdornerCorner(ref bottomLeft, Cursors.SizeNESW);
            BuildAdornerCorner(ref bottomRight, Cursors.SizeNWSE);

            BuildMoveThumb();

            topLeft.DragDelta += HandleTopLeft;
            topRight.DragDelta += HandleTopRight;
            bottomLeft.DragDelta += HandleBottomLeft;
            bottomRight.DragDelta += HandleBottomRight;
            moveThumb.DragDelta += HandleMove;
        }

        private void BuildAdornerCorner(ref Thumb cornerThumb, Cursor customizedCursor)
        {
            if (cornerThumb != null)
                return;

            cornerThumb = new Thumb
            {
                Cursor = customizedCursor,
                Width = ThumbSize,
                Height = ThumbSize,
                Opacity = 0.5,
                Background = Brushes.White,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1)
            };

            visualChildren.Add(cornerThumb);
        }

        private void BuildMoveThumb()
        {
            moveThumb = new Thumb
            {
                Cursor = Cursors.SizeAll,
                Width = ThumbSize,
                Height = ThumbSize,
                Opacity = 0.5,
                Background = Brushes.LightBlue,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1)
            };

            visualChildren.Add(moveThumb);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double offset = ThumbSize / 2;

            Size adornedElementSize = AdornedElement.RenderSize;

            topLeft.Arrange(new Rect(-offset, -offset, ThumbSize, ThumbSize));
            topRight.Arrange(new Rect(adornedElementSize.Width - offset, -offset, ThumbSize, ThumbSize));
            bottomLeft.Arrange(new Rect(-offset, adornedElementSize.Height - offset, ThumbSize, ThumbSize));
            bottomRight.Arrange(new Rect(adornedElementSize.Width - offset, adornedElementSize.Height - offset, ThumbSize, ThumbSize));

            moveThumb.Arrange(new Rect((adornedElementSize.Width / 2) - (ThumbSize / 2), (adornedElementSize.Height / 2) - (ThumbSize / 2), ThumbSize, ThumbSize));

            return finalSize;
        }

        protected override int VisualChildrenCount => visualChildren.Count;

        protected override Visual GetVisualChild(int index) => visualChildren[index];


        protected abstract void HandleTopLeft(object sender, DragDeltaEventArgs e);
        protected abstract void HandleTopRight(object sender, DragDeltaEventArgs e);
        protected abstract void HandleBottomLeft(object sender, DragDeltaEventArgs e);
        protected abstract void HandleBottomRight(object sender, DragDeltaEventArgs e);

        protected virtual void HandleMove(object sender, DragDeltaEventArgs e)
        {
            if (AdornedElement is Shape shape)
            {
                double left = Canvas.GetLeft(shape);
                double top = Canvas.GetTop(shape);

                Canvas.SetLeft(shape, left + e.HorizontalChange);
                Canvas.SetTop(shape, top + e.VerticalChange);
            }
        }
    }
    public class RectangleAdorner : ResizeAdorner
    {
        private Rectangle rectangle;

        public RectangleAdorner(UIElement adornedElement) : base(adornedElement)
        {
            rectangle = adornedElement as Rectangle;
        }

        protected override void HandleTopLeft(object sender, DragDeltaEventArgs e)
        {
            if (rectangle == null)
                return;

            double oldLeft = Canvas.GetLeft(rectangle);
            double oldTop = Canvas.GetTop(rectangle);
            double oldWidth = rectangle.Width;
            double oldHeight = rectangle.Height;

            double newLeft = oldLeft + e.HorizontalChange;
            double newTop = oldTop + e.VerticalChange;
            double newWidth = oldWidth - e.HorizontalChange;
            double newHeight = oldHeight - e.VerticalChange;

            if (newWidth > 0)
            {
                rectangle.Width = newWidth;
                Canvas.SetLeft(rectangle, newLeft);
            }
            if (newHeight > 0)
            {
                rectangle.Height = newHeight;
                Canvas.SetTop(rectangle, newTop);
            }
        }

        protected override void HandleTopRight(object sender, DragDeltaEventArgs e)
        {
            if (rectangle == null)
                return;

            double oldTop = Canvas.GetTop(rectangle);
            double oldWidth = rectangle.Width;
            double oldHeight = rectangle.Height;

            double newWidth = oldWidth + e.HorizontalChange;
            double newHeight = oldHeight - e.VerticalChange;
            double newTop = oldTop + e.VerticalChange;

            if (newWidth > 0)
                rectangle.Width = newWidth;
            if (newHeight > 0)
            {
                rectangle.Height = newHeight;
                Canvas.SetTop(rectangle, newTop);
            }
        }

        protected override void HandleBottomLeft(object sender, DragDeltaEventArgs e)
        {
            if (rectangle == null)
                return;

            double oldLeft = Canvas.GetLeft(rectangle);
            double oldWidth = rectangle.Width;
            double oldHeight = rectangle.Height;

            double newLeft = oldLeft + e.HorizontalChange;
            double newWidth = oldWidth - e.HorizontalChange;
            double newHeight = oldHeight + e.VerticalChange;

            if (newWidth > 0)
            {
                rectangle.Width = newWidth;
                Canvas.SetLeft(rectangle, newLeft);
            }
            if (newHeight > 0)
                rectangle.Height = newHeight;
        }

        protected override void HandleBottomRight(object sender, DragDeltaEventArgs e)
        {
            if (rectangle == null)
                return;

            double newWidth = rectangle.Width + e.HorizontalChange;
            double newHeight = rectangle.Height + e.VerticalChange;

            if (newWidth > 0)
                rectangle.Width = newWidth;
            if (newHeight > 0)
                rectangle.Height = newHeight;
        }
    }
    public class EllipseAdorner : ResizeAdorner
    {
        private Ellipse ellipse;

        public EllipseAdorner(UIElement adornedElement) : base(adornedElement)
        {
            ellipse = adornedElement as Ellipse;
        }

        protected override void HandleTopLeft(object sender, DragDeltaEventArgs e)
        {
            if (ellipse == null)
                return;

            double oldLeft = Canvas.GetLeft(ellipse);
            double oldTop = Canvas.GetTop(ellipse);
            double oldWidth = ellipse.Width;
            double oldHeight = ellipse.Height;

            double newLeft = oldLeft + e.HorizontalChange;
            double newTop = oldTop + e.VerticalChange;
            double newWidth = oldWidth - e.HorizontalChange;
            double newHeight = oldHeight - e.VerticalChange;

            if (newWidth > 0)
            {
                ellipse.Width = newWidth;
                Canvas.SetLeft(ellipse, newLeft);
            }
            if (newHeight > 0)
            {
                ellipse.Height = newHeight;
                Canvas.SetTop(ellipse, newTop);
            }
        }

        protected override void HandleTopRight(object sender, DragDeltaEventArgs e)
        {
            if (ellipse == null)
                return;

            double oldTop = Canvas.GetTop(ellipse);
            double oldWidth = ellipse.Width;
            double oldHeight = ellipse.Height;

            double newWidth = oldWidth + e.HorizontalChange;
            double newHeight = oldHeight - e.VerticalChange;
            double newTop = oldTop + e.VerticalChange;

            if (newWidth > 0)
                ellipse.Width = newWidth;
            if (newHeight > 0)
            {
                ellipse.Height = newHeight;
                Canvas.SetTop(ellipse, newTop);
            }
        }

        protected override void HandleBottomLeft(object sender, DragDeltaEventArgs e)
        {
            if (ellipse == null)
                return;

            double oldLeft = Canvas.GetLeft(ellipse);
            double oldWidth = ellipse.Width;
            double oldHeight = ellipse.Height;

            double newLeft = oldLeft + e.HorizontalChange;
            double newWidth = oldWidth - e.HorizontalChange;
            double newHeight = oldHeight + e.VerticalChange;

            if (newWidth > 0)
            {
                ellipse.Width = newWidth;
                Canvas.SetLeft(ellipse, newLeft);
            }
            if (newHeight > 0)
                ellipse.Height = newHeight;
        }

        protected override void HandleBottomRight(object sender, DragDeltaEventArgs e)
        {
            if (ellipse == null)
                return;

            double newWidth = ellipse.Width + e.HorizontalChange;
            double newHeight = ellipse.Height + e.VerticalChange;

            if (newWidth > 0)
                ellipse.Width = newWidth;
            if (newHeight > 0)
                ellipse.Height = newHeight;
        }
    }
    public class LineAdorner : ResizeAdorner
    {
        private Line line;

        public LineAdorner(UIElement adornedElement) : base(adornedElement)
        {
            line = adornedElement as Line;
        }

        protected override void HandleTopLeft(object sender, DragDeltaEventArgs e)
        {
            if (line == null)
                return;

            line.X1 += e.HorizontalChange;
            line.Y1 += e.VerticalChange;
        }

        protected override void HandleTopRight(object sender, DragDeltaEventArgs e)
        {
            if (line == null)
                return;

            line.X2 += e.HorizontalChange;
            line.Y2 += e.VerticalChange;
        }

        protected override void HandleBottomLeft(object sender, DragDeltaEventArgs e)
        {
            HandleTopLeft(sender, e);
        }

        protected override void HandleBottomRight(object sender, DragDeltaEventArgs e)
        {
            HandleTopRight(sender, e);
        }

        protected override void HandleMove(object sender, DragDeltaEventArgs e)
        {
            if (line == null)
                return;

            line.X1 += e.HorizontalChange;
            line.Y1 += e.VerticalChange;
            line.X2 += e.HorizontalChange;
            line.Y2 += e.VerticalChange;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double offset = ThumbSize / 2;

            double midX = (line.X1 + line.X2) / 2;
            double midY = (line.Y1 + line.Y2) / 2;

            topLeft.Arrange(new Rect(line.X1 - offset, line.Y1 - offset, ThumbSize, ThumbSize));
            topRight.Arrange(new Rect(line.X2 - offset, line.Y2 - offset, ThumbSize, ThumbSize));
            moveThumb.Arrange(new Rect(midX - offset, midY - offset, ThumbSize, ThumbSize));

            return finalSize;
        }
    }
    public class CircleAdorner : ResizeAdorner
    {
        private Circle circle;

        public CircleAdorner(UIElement adornedElement) : base(adornedElement)
        {
            circle = adornedElement as Circle;
            if (circle == null)
                throw new ArgumentException("Adorned element must be of type Circle", nameof(adornedElement));

            circle.PropertyChanged += Circle_PropertyChanged;
        }

        private void Circle_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Circle.Radius) || e.PropertyName == nameof(Circle.CenterX) || e.PropertyName == nameof(Circle.CenterY))
            {
                InvalidateArrange();
            }
        }

        protected override void HandleTopLeft(object sender, DragDeltaEventArgs e)
        {
            HandleResize(e, -1, -1);
        }

        protected override void HandleTopRight(object sender, DragDeltaEventArgs e)
        {
            HandleResize(e, 1, -1);
        }

        protected override void HandleBottomLeft(object sender, DragDeltaEventArgs e)
        {
            HandleResize(e, -1, 1);
        }

        protected override void HandleBottomRight(object sender, DragDeltaEventArgs e)
        {
            HandleResize(e, 1, 1);
        }

        private void HandleResize(DragDeltaEventArgs e, int horizontalFactor, int verticalFactor)
        {
            if (circle == null)
                return;
            double delta = Math.Max(e.HorizontalChange * horizontalFactor, e.VerticalChange * verticalFactor);
            double newRadius = Math.Abs(circle.Radius + delta);
            if (newRadius > 0)
            {
                circle.Radius = newRadius;
            }
        }

        protected override void HandleMove(object sender, DragDeltaEventArgs e)
        {
            if (circle == null)
                return;

            circle.CenterX += e.HorizontalChange;
            circle.CenterY += e.VerticalChange;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (circle == null)
                return finalSize;
            double offset = ThumbSize / 2;

            double centerX = circle.CenterX;
            double centerY = circle.CenterY;
            topLeft.Arrange(new Rect(centerX - circle.Radius - offset, centerY - circle.Radius - offset, ThumbSize, ThumbSize));
            topRight.Arrange(new Rect(centerX + circle.Radius - offset, centerY - circle.Radius - offset, ThumbSize, ThumbSize));
            bottomLeft.Arrange(new Rect(centerX - circle.Radius - offset, centerY + circle.Radius - offset, ThumbSize, ThumbSize));
            bottomRight.Arrange(new Rect(centerX + circle.Radius - offset, centerY + circle.Radius - offset, ThumbSize, ThumbSize));
            moveThumb.Arrange(new Rect(centerX - offset, centerY - offset, ThumbSize, ThumbSize));

            return finalSize;
        }
    }
}
