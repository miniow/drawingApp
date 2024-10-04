using drawingApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace drawingApp.Models
{
    public class RectangleParameters : ShapeParametersBase
    {
        public override ShapeType ShapeType => ShapeType.Rectangle;
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
    }
    public class ElipseParameters : ShapeParametersBase
    {
        public override ShapeType ShapeType => ShapeType.Rectangle;
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
    }
    public class CircleParameters : ShapeParametersBase
    {
        public override ShapeType ShapeType => ShapeType.Circle;

        public double CenterX { get; set; }
        public double CenterY { get; set; }
        public double Radius { get; set; }
    }
    public class LineParameters : ShapeParametersBase
    {
        public override ShapeType ShapeType => ShapeType.Line;

        public double X1 { get; set; }
        public double Y1 { get; set; }
        public double X2 { get; set; }
        public double Y2 { get; set; }
    }
}
