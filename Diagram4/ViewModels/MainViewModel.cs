using Diagram4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diagram4.ViewModels
{
	internal class MainViewModel : ViewModelBase
	{
		private Model _model;
		private ViewModelBase? _currentViewModel;
		public ViewModelBase? CurrentViewModel
		{
			get { return _currentViewModel; }
			set
			{
				if (_currentViewModel != value)
				{
					_currentViewModel = value;
					OnPropertyChanged(nameof(CurrentViewModel));
				}
			}
		}
		public Command KeyDownCommand { get; }
		public event Action<object>? KeyDown;
		public void NavigateToDiagram()
		{
			DiagramViewModel diagramViewModel = new DiagramViewModel(_model);
			KeyDown += diagramViewModel.OnKeyDown;
			CurrentViewModel = diagramViewModel;
		}
		public MainViewModel(Model model)
		{
			_model = model;
			NavigateToDiagram();
			KeyDownCommand = new Command(OnKeyDown);
		}
		public void OnKeyDown(object? obj)
		{
			KeyDown?.Invoke(obj);
		}
	}
}
