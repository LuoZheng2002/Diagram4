using Diagram4.Adorners;
using Diagram4.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Diagram4.ViewModels
{
	internal class StrategyViewModel : ViewModelBase, ISelectable
	{
		public StrategyView View { get; set; }
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
		public ConnectionLine? Line { get; set; }
		public StrategySetViewModel? StrategySetConnectedTo { get; set; }
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
		public Command MouseEnterCommand { get; }
		public Command MouseLeaveCommand { get; }
		public Command LoadedCommand { get; }
		private readonly NodeAdorner _nodeAdorner;
		private readonly NodeDummyAdorner _nodeDummyAdorner;
		public TextBlock TextBlock { get; }
		public event Action? CanvasClicked;
		public event Action<KeyEventArgs>? KeyDown;
		public event Action<StrategyViewModel, FrameworkElement>? DragStarted;
		public event Action? PositionChanged;

		public event Action<StrategyViewModel, FrameworkElement>? NotifyStrategyPosition;
		public StrategyViewModel(StrategyView view)
		{
			View = view;
			TextBlock = View.TextBlock;
			DropCommand = new Command(OnDrop);
			SelectCommand = new Command(OnSelect);
			MouseEnterCommand= new Command(OnMouseEnter);
			MouseLeaveCommand = new Command(OnMouseLeave);
			LoadedCommand = new Command(OnLoaded);
			_nodeAdorner = new NodeAdorner(TextBlock);
			_nodeAdorner.MouseLeave += OnMouseLeaveNodeAdorner;
			_nodeAdorner.DragStarted += OnDragStarted;
			_nodeDummyAdorner = new NodeDummyAdorner(TextBlock);
			_nodeDummyAdorner.NotifyStrategyPosition += OnNotifyStrategyPosition;
			PositionChanged += _nodeDummyAdorner.OnPositionChanged;
		}
		public void SetUpDummyAdorner()
		{
			AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(TextBlock);
			adornerLayer.Add(_nodeDummyAdorner);
			
		}
		void OnDragStarted(FrameworkElement dragSource)
		{
			DragStarted?.Invoke(this, dragSource);
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
		public void OnKeyDown(KeyEventArgs e)
		{
			KeyDown?.Invoke(e);
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
			Adorner[] adorners = adornerLayer.GetAdorners(TextBlock);
			if (adorners == null)
			{
				Console.WriteLine("Dummy adorner not successfully added.");
			}
			else if (!adorners.Contains(_nodeAdorner))
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
			Adorner[] adorners = adornerLayer.GetAdorners(TextBlock);
			if (adorners == null)
			{
				Console.WriteLine("There is no adorners.");
			}
			else if (adorners.Contains(_nodeAdorner))
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
		public void OnPositionChanged()
		{
			PositionChanged?.Invoke();
		}
		public void OnConnectionLineDestroyed(ConnectionLine line)
		{
			if (Line != line)
			{
				Console.WriteLine("不是一条线？");
			}
			Line = null;
			StrategySetConnectedTo = null;
		}
		public void OnNotifyStrategyPosition(FrameworkElement element)
		{
			if (Line != null)
			{
				NotifyStrategyPosition?.Invoke(this, element);
			}
		}
		public void OnLoaded(object? obj)
		{
			SetUpDummyAdorner();
		}
	}
}
