using Diagram4.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Diagram4.Adorners
{
    internal class MoveAdorner : Adorner
    {
        VisualCollection AdornerVisuals { get; }
        public Thumb thumb;
        static readonly int THUMB_WIDTH = 30;
        static readonly int THUMB_HEIGHT = 30;
        public MoveAdorner(UIElement adornedElement) : base(adornedElement)
        {
            AdornerVisuals = new VisualCollection(this);
            thumb = new Thumb() { Background = Brushes.Transparent, Height = THUMB_HEIGHT, Width = THUMB_WIDTH };
            thumb.DragDelta += Thumb_DragDelta;
            AdornerVisuals.Add(thumb);
            AdornerStyle adornerStyle = new AdornerStyle();
            thumb.Style = adornerStyle.FindResource("Transparent") as Style;
        }

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Canvas.SetLeft(AdornedElement, Canvas.GetLeft(AdornedElement) + e.HorizontalChange);
            Canvas.SetTop(AdornedElement, Canvas.GetTop(AdornedElement) + e.VerticalChange);
        }

        protected override Visual GetVisualChild(int index)
        {
            return AdornerVisuals[index];
        }
        protected override Size ArrangeOverride(Size finalSize)
        {
            thumb.Arrange(new Rect(0, 0, THUMB_WIDTH, THUMB_HEIGHT));
            return base.ArrangeOverride(finalSize);
        }
        protected override int VisualChildrenCount => AdornerVisuals.Count;
    }
}

