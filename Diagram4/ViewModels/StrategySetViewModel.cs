using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using Diagram4.Views;
using System.Diagnostics;
using System.IO;
using System.Windows.Controls;
using System.Windows.Shapes;
using Diagram4.Adorners;
using System.Windows.Documents;

namespace Diagram4.ViewModels
{
	internal class StrategySetViewModel : ViewModelBase, ISelectable
	{
		public MoveAdorner MoveAdorner { get; set; }
		public StrategySetView View { get; set; }
		public ConnectionLine? Line { get; set; }
		public bool DraggingLine { get; set; }
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
						TextColor = Brushes.LightGreen;
					}
					else
					{
						TextColor = Brushes.AliceBlue;
					}
					OnPropertyChanged(nameof(TextColor));
				}
			}
		}
		private int number = 0;
		public ImageSource? Image { get; set; } = new BitmapImage(new Uri("Images/114514.jpeg", UriKind.Relative));
		public Brush TextColor { get; set; } = Brushes.AliceBlue;
		public Brush BackgroundColor { get; set; } = Brushes.AliceBlue;
		
		private string _imageName = "";
		public string ImageName
		{
			get { return _imageName; }
			set
			{
				if (_imageName != value)
				{
					_imageName = value;
					Image = new BitmapImage(new Uri(_imageName, UriKind.Relative));

					OnPropertyChanged(nameof(Image));
				}
			}
		}
		public string Text { get; set; } = "";
		public ObservableCollection<StrategyView> StrategyViews { get; } = new();
		public List<StrategyViewModel> StrategyViewModels { get; } = new();
		public Command DropCommand { get; }
		public Command SelectCommand { get; }
		public Command MouseLeftButtonUpCommand { get; }
		public Command LoadedCommand { get; }
		public Command MouseEnterCommand { get; }
		public Command MouseLeaveCommand { get; }

		public event Action? CanvasClicked;
		public event Action<KeyEventArgs>? KeyDown;
		public event Action<StrategySetViewModel, StrategyViewModel, FrameworkElement>? DragStarted;
		public event Action<StrategySetViewModel, StrategySetView>? DragEnded;
		public event Action? PositionChanged;
		public event Action<StrategySetViewModel, StrategyViewModel, FrameworkElement>? NotifyStrategyPosition;
		public event Action<StrategySetViewModel, StrategySetView>? NotifyStrategySetPosition;
		public StrategySetViewModel(StrategySetView view)
		{
			DropCommand = new Command(OnDrop);
			SelectCommand = new Command(OnSelect);
			MouseLeftButtonUpCommand = new Command(OnMouseLeftButtonUp);
			LoadedCommand = new Command(OnLoaded);
			MouseEnterCommand = new Command(OnMouseEnter);
			MouseLeaveCommand = new Command(OnMouseLeave);
			View = view;
			MoveAdorner = new MoveAdorner(view);
			MoveAdorner.Drag += OnPositionChanged;
			MoveAdorner.Drag += OnNotifyStrategySetPosition;
		}

		private void OnMouseLeave(object? obj)
		{
			if (BackgroundColor==Brushes.Magenta)
			{
				BackgroundColor = Brushes.AliceBlue;
				OnPropertyChanged(nameof(BackgroundColor));
			}
		}

		private void OnMouseEnter(object? obj)
		{
			if (DraggingLine)
			{
				BackgroundColor = Brushes.Magenta;
				OnPropertyChanged(nameof(BackgroundColor));
			}
		}

		private void OnLoaded(object? obj)
		{
			SetUpAdorner();
		}

		public void SetUpAdorner()
		{
			AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(View);
			if (adornerLayer == null)
			{
				Console.WriteLine("Failed to get adorner layer!");
			}
			else
			{
				adornerLayer.Add(MoveAdorner);
			}
		}
		void OnDragStarted(StrategyViewModel strategyViewModel, FrameworkElement dragSource)
		{
			DragStarted?.Invoke(this, strategyViewModel, dragSource);
		}
		private void AddChild(int index)
		{
			StrategyView strategyView = new StrategyView();
			StrategyViewModel strategyViewModel = new StrategyViewModel(strategyView);
			strategyView.DataContext = strategyViewModel;
			strategyViewModel.Dropped += OnChildDrop;
			strategyViewModel.DragStarted += OnDragStarted;
			strategyViewModel.DeleteChild += OnDeleteChild;
			strategyViewModel.NotifyStrategyPosition += OnNotifyStrategyPosition;
			StrategyViews.Insert(index, strategyView);
			StrategyViewModels.Insert(index, strategyViewModel);

			// strategyViewModel.SetUpDummyAdorner();
			strategyViewModel.Text = "Strategy " + number.ToString();
			CanvasClicked += strategyViewModel.OnCanvasClicked;
			KeyDown += strategyViewModel.OnKeyDown;
			PositionChanged += strategyViewModel.OnPositionChanged;
			number++;
		}

		public void OnDrop(object? obj)
		{
			DragEventArgs e = (obj as DragEventArgs)!;
			AddChild(0);
			e.Handled = true;
		}
		public void OnChildDrop(StrategyViewModel dropped, object? obj)
		{
			int childIndex = StrategyViewModels.FindIndex(x => x == dropped);
			DragEventArgs e = (obj as DragEventArgs)!;
			if (childIndex != -1)
			{
				DisplayTileViewModel displayTile = (e.Data.GetData(typeof(DisplayTileViewModel)) as DisplayTileViewModel)!;
				if (displayTile != null)
				{
					AddChild(childIndex + 1);
				}
				else
				{
					Console.WriteLine("This is not a display tile!");
				}
			}
			e.Handled = true;
		}
		public void OnSelect(object? obj)
		{
			MouseButtonEventArgs e = (obj as MouseButtonEventArgs)!;
			e.Handled = true;
			IsSelected = true;
		}
		public void OnDeselect(object? obj)
		{
			IsSelected = false;
		}
		public void OnCanvasClicked()
		{
			OnDeselect(null);
			CanvasClicked?.Invoke();
		}
		public void OnKeyDown(KeyEventArgs e)
		{
			KeyDown?.Invoke(e);
		}
		public void OnDeleteChild(object? obj)
		{
			StrategyViewModel strategyViewModel = (obj as StrategyViewModel)!;
			int index = StrategyViewModels.FindIndex(x => x == strategyViewModel);
			if (index != -1)
			{
				strategyViewModel.IsSelected = false;
				StrategyViewModels.RemoveAt(index);
				StrategyViews.RemoveAt(index);
			}
			else
			{
				Console.WriteLine("Cannot delete strategy!");
			}
		}
		public void OnMouseLeftButtonUp(object? obj)
		{
			MouseButtonEventArgs e = (obj as MouseButtonEventArgs)!;
			e.Handled = true;
			Console.WriteLine("Mouse left button up triggered in strategy set.");
			DragEnded?.Invoke(this, View);
		}
		public void OnConnectionLineDestroyed(ConnectionLine line)
		{
			if (Line!=line)
			{
				Console.WriteLine("不是一条线？");
			}
			Line = null;
		}
		void OnPositionChanged()
		{
			PositionChanged?.Invoke();
		}
		public void OnNotifyStrategyPosition(StrategyViewModel strategyViewModel,
			FrameworkElement element)
		{
			NotifyStrategyPosition?.Invoke(this, strategyViewModel, element);
		}
		public void OnNotifyStrategySetPosition()
		{
			if (Line != null)
			{
				NotifyStrategySetPosition?.Invoke(this, View);
			}
		}
		public void OnDragLineStarted()
		{
			DraggingLine = true;
		}
		public void OnDragLineEnded()
		{
			DraggingLine = false;
		}
	}
}
