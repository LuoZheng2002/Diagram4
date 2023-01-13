using Diagram4.Adorners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Diagram4.ViewModels
{
	internal class StrategyViewModel : ViewModelBase, ISelectable
	{
		private bool _isSelected;
		public bool IsSelected
		{
			get { return _isSelected; }
			set
			{
				if (_isSelected != value)
				{
					_isSelected = value;
					if (_isSelected)
					{
						Background = Brushes.LightGreen;
					}
					else
					{
						Background = Brushes.AliceBlue;
					}
					OnPropertyChanged(nameof(Background));
				}
			}
		}
		private string _text = "";
		public string Text
		{
			get { return _text; }
			set
			{
				if (_text != value)
				{
					_text = value;
					OnPropertyChanged(nameof(Text));
				}
			}
		}
		private Brush _background = Brushes.AliceBlue;
		public Brush Background
		{
			get { return _background; }
			set
			{
				_background = value;
				OnPropertyChanged(nameof(Background));
			}
		}
		public Command DropCommand { get; }
		public Command SelectCommand { get; }
		public Command KeyDownCommand { get; }
		public Command MouseEnterCommand { get; }
		public Command MouseLeaveCommand { get; }
		private readonly NodeAdorner _nodeAdorner;
		public TextBlock TextBlock { get; }
		public event Action? CanvasClicked;
		public event Action<object>? KeyDown;
		public StrategyViewModel(TextBlock textBlock)
		{
			TextBlock = textBlock;
			DropCommand = new Command(OnDrop);
			SelectCommand = new Command(OnSelect);
			KeyDownCommand = new Command(OnKeyDown);
			MouseEnterCommand= new Command(OnMouseEnter);
			MouseLeaveCommand = new Command(OnMouseLeave);
			_nodeAdorner = new NodeAdorner(textBlock);
			_nodeAdorner.MouseLeave += OnMouseLeaveNodeAdorner;
		}

		private void OnMouseLeaveNodeAdorner(object sender, MouseEventArgs e)
		{
			if (!MouseInTextBlock(e))
			{
				HideNodeAdorner();
			}
		}

		public event Action<StrategyViewModel, object>? Dropped;//second arg: dropeventargs
		public event Action<object?>? DeleteChild;
		public void OnDrop(object? obj)
		{
			Dropped?.Invoke(this, obj);
		}
		public void OnDeselect(object? obj)
		{
			IsSelected = false;
		}
		public void OnSelect(object? obj)
		{
			MouseButtonEventArgs e = (obj as MouseButtonEventArgs)!;
			e.Handled = true;
			IsSelected = true;
		}
		public void OnCanvasClicked()
		{
			CanvasClicked?.Invoke();
			OnDeselect(null);
		}
		public void OnKeyDown(object? obj)
		{
			KeyDown?.Invoke(obj);
			KeyEventArgs e = (obj as KeyEventArgs)!;
			if (e.Key == Key.Delete && IsSelected)
			{
				DeleteChild?.Invoke(this);
			}
		}
		void OnMouseEnter(object? obj)
		{
			ShowNodeAdorner();
		}
		void OnMouseLeave(object? obj)
		{
			MouseEventArgs e = (obj as MouseEventArgs)!;
			if (!MouseInTextBlock(e))
			{
				HideNodeAdorner();
			}
		}
		bool MouseInTextBlock(MouseEventArgs e)
		{
			Point pos = e.GetPosition(TextBlock);
			return (pos.X >= 0 && pos.Y >= 0 && pos.X <= TextBlock.Width && pos.Y <= TextBlock.Height);
		}
		void ShowNodeAdorner()
		{
			AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(TextBlock);
			if (adornerLayer.GetAdorners(TextBlock) == null)
			{
				adornerLayer.Add(_nodeAdorner);
			}
			else
			{
				Console.WriteLine("Attempt to add another node adorner.");
			}
		}
		void HideNodeAdorner()
		{
			AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(TextBlock);
			if (adornerLayer.GetAdorners(TextBlock).Count() > 0)
			{
				adornerLayer.Remove(_nodeAdorner);
			}
			else
			{
				Console.WriteLine("Attempt to remove node adorner twice.");
			}
		}
		~StrategyViewModel()
		{
			Console.WriteLine("Strategy Destructed!");
		}
	}
}
