using drawingApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Ink;
using System.Windows.Media;

namespace drawingApp.Models
{
    public abstract class ShapeParametersBase 
    {
        public abstract ShapeType ShapeType { get;}
        public  Brush StrokeBrush { get; set; }
    }
}
