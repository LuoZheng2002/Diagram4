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
using System.Windows.Shapes;

namespace Diagram4.Adorners
{
	internal class NodeDummyAdorner : Adorner
	{
		VisualCollection AdornerVisuals { get; }
		static readonly int WIDTH = 10;
		static readonly int HEIGHT = 10;
		private readonly Rectangle _rectangle;
		public event Action<FrameworkElement>? NotifyStrategyPosition;
		public NodeDummyAdorner(UIElement adornedElement) : base(adornedElement)
		{
			AdornerVisuals = new VisualCollection(this);
			_rectangle = new Rectangle { Width = WIDTH, Height = HEIGHT };
			_rectangle.Fill = Brushes.Transparent; 
			AdornerVisuals.Add(_rectangle);
		}
		protected override Visual GetVisualChild(int index)
		{
			return AdornerVisuals[index];
		}
		protected override Size ArrangeOverride(Size finalSize)
		{
			_rectangle.Arrange(new Rect(AdornedElement.DesiredSize.Width - WIDTH / 2, (AdornedElement.DesiredSize.Height - HEIGHT) / 2, WIDTH, HEIGHT));
			return base.ArrangeOverride(finalSize);
		}
		protected override int VisualChildrenCount => AdornerVisuals.Count;
		public void OnPositionChanged()
		{
			NotifyStrategyPosition?.Invoke(_rectangle);
		}
	}
}
