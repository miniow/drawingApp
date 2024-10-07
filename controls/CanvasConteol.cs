using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace drawingApp.Controls
{
    public class CanvasItemsControl : ItemsControl
    {
        protected override bool IsItemItsOwnContainerOverride(object item)
        {

            return item is UIElement;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {

            return null;
        }
    }
}
