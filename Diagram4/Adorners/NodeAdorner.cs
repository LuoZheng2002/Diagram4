using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;

namespace Diagram4.Adorners
{
	internal class NodeAdorner : Adorner
	{
		VisualCollection AdornerVisuals { get; }
		static readonly int THUMB_WIDTH = 10;
		static readonly int THUMB_HEIGHT = 10;
		public Thumb thumb;
		public NodeAdorner(UIElement adornedElement) : base(adornedElement)
		{
			AdornerVisuals = new VisualCollection(this);
			thumb = new Thumb() {Height = THUMB_HEIGHT, Width = THUMB_WIDTH };
			thumb.DragDelta += Thumb_DragDelta;
			AdornerVisuals.Add(thumb);
			AdornerStyle adornerStyle = new AdornerStyle();
			thumb.Style = adornerStyle.FindResource("Node") as Style;
		}
		private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
		{

		}
		protected override Visual GetVisualChild(int index)
		{
			return AdornerVisuals[index];
		}
		protected override Size ArrangeOverride(Size finalSize)
		{
			thumb.Arrange(new Rect(AdornedElement.DesiredSize.Width - THUMB_WIDTH/2 , (AdornedElement.DesiredSize.Height - THUMB_HEIGHT)/2, THUMB_WIDTH, THUMB_HEIGHT));
			return base.ArrangeOverride(finalSize);
		}
		protected override int VisualChildrenCount => AdornerVisuals.Count;
	}
}
