using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using Diagram4.ViewModels;

namespace Diagram4
{
	internal class ConnectionLine : ISelectable
	{
		public bool Dead { get; set; }
		public Line Line { get; set; }
		public StrategyViewModel StrategyFrom { get; }
		public StrategySetViewModel StrategySetTo { get; }
		public ConnectionLine(Line line, StrategyViewModel strategyFrom, 
			StrategySetViewModel strategySetTo)
		{
			Line = line;
			Line.MouseLeftButtonDown += OnMouseLeftButtonDown;
			StrategyFrom = strategyFrom;
			StrategySetTo = strategySetTo;
		}

		private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (!Dead)
			{
				IsSelected = true;
				e.Handled = true;
			}
			
		}

		private bool _isSelected;
		public bool IsSelected {
			get { return _isSelected; }
			set
			{
				if (_isSelected != value)
				{
					_isSelected = value;
					if (_isSelected)
					{
						Line.Stroke = Brushes.Orange;
					}
					else
					{
						Line.Stroke = Brushes.Green;
					}
				}
			}
		}

		public void OnStartPosChanged(Point point)
		{
			Line.X1= point.X;
			Line.Y1= point.Y;
		}
		public void OnEndPosChanged(Point point)
		{
			Line.X2= point.X;
			Line.Y2= point.Y;
		}	
		public event Action<ConnectionLine>? Destroyed;
		public void OnDeselect()
		{
			if (!Dead)
			{
				IsSelected = false;
			}
		}
		void OnDeletePressed()
		{
			Destroyed?.Invoke(this);
			Dead = true;
		}
		public void OnKeyDown(KeyEventArgs e)
		{
			if (!Dead)
			{
				if (e.Key == Key.Delete && IsSelected)
				{
					OnDeletePressed();
				}
			}
		}
	}
}
