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

namespace Diagram4.ViewModels
{
	internal class StrategySetViewModel : ViewModelBase, ISelectable
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
		private int number = 0;
		public ImageSource? Image { get; set; } = new BitmapImage(new Uri("Images/114514.jpeg", UriKind.Relative));
		public Brush Background { get; set; } = Brushes.AliceBlue;
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

		public event Action? CanvasClicked;
		public event Action<object>? KeyDown;
		public event Action<StrategySetViewModel, StrategyViewModel, MouseButtonEventArgs> DragStarted;
		public StrategySetViewModel()
		{
			DropCommand = new Command(OnDrop);
			SelectCommand = new Command(OnSelect);
		}
		void OnDragStarted(StrategyViewModel strategyViewModel, MouseButtonEventArgs e)
		{
			DragStarted?.Invoke(this, strategyViewModel, e);
		}
		private void AddChild(int index)
		{
			StrategyView strategyView = new StrategyView();
			StrategyViewModel strategyViewModel = new StrategyViewModel(strategyView.TextBlock);
			strategyView.DataContext = strategyViewModel;
			strategyViewModel.Dropped += OnChildDrop;
			strategyViewModel.DragStarted += OnDragStarted;
			StrategyViews.Insert(index, strategyView);
			StrategyViewModels.Insert(index, strategyViewModel);
			strategyViewModel.Text = "Strategy " + number.ToString();
			CanvasClicked += strategyViewModel.OnCanvasClicked;
			KeyDown += strategyViewModel.OnKeyDown;
			strategyViewModel.DeleteChild += OnDeleteChild;
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
		public void OnKeyDown(object? obj)
		{
			KeyDown?.Invoke(obj);
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

	}
}
