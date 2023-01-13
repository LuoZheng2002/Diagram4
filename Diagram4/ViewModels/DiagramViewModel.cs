using Diagram4.Adorners;
using Diagram4.Models;
using Diagram4.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    internal class DragInfo
    {
        public StrategySetViewModel? StrategySetFrom { get; set; }
        public StrategySetViewModel? StrategySetTo { get; set;}
        public StrategyViewModel? StrategyFrom { get; set; }
        public bool Dragging { get; set; }
        public Point StartPos { get; set; }
        public void Clear()
        {
            StrategySetFrom = null;
            StrategySetTo = null;
            StrategyFrom = null;
            Dragging = false;
        }
    }
    internal class DiagramViewModel:ViewModelBase
    {
        private Model _model;
        private readonly DragInfo _dragInfo = new DragInfo();
        public Canvas? Canvas { get; set; }
        public ObservableCollection<FrameworkElement> DiagramItems { get; } = new();
        public List<ViewModelBase> DiagramItemViewModels { get; } = new();
        public DisplayTileViewModel DisplayTile0 { get; }
        public DisplayTileViewModel DisplayTile1 { get; }
        public DisplayTileViewModel DisplayTile2 { get; }
        public DisplayTileViewModel DisplayTile3 { get; }
        public Command DropCommand { get; }
        public Command ClickCommand { get; }
        public Command MouseLeftButtonUpCommand { get; }
        public Command CanvasLoadedCommand { get; }
        public event Action? CanvasClicked;
        public event Action<KeyEventArgs>? KeyDown;
        public event Action? DragLineStarted;
        public event Action? DragLineEnded;

        public DiagramViewModel(Model model)
        {
            _model = model;
            DisplayTile0 = new DisplayTileViewModel() { Text = "HS", ImageName = "../Images/114514.jpeg" };
            DisplayTile1 = new DisplayTileViewModel() { Text = "PS", ImageName = "../Images/fufu.jpg" };
            DisplayTile2 = new DisplayTileViewModel() { Text = "H", ImageName = "../Images/53.jpg" };
            DisplayTile3 = new DisplayTileViewModel() { Text = "P", ImageName = "../Images/gua.jpg" };
            DropCommand = new Command(OnDrop);
            ClickCommand = new Command(OnClick);
            MouseLeftButtonUpCommand= new Command(OnMouseLeftButtonUp);
            CanvasLoadedCommand = new Command(OnCanvasLoaded);
        }
        public void OnDrop(object? obj)
        {
            DragEventArgs e = (obj as DragEventArgs)!;
            DisplayTileViewModel displayTile = (e.Data.GetData(typeof(DisplayTileViewModel)) as DisplayTileViewModel)!;
            if (displayTile != null)
            {
                if (Canvas != null)
                {
                    Point pos = e.GetPosition(Canvas);
                    switch(displayTile.Text)
                    {
                        default:
                            {
                                StrategySetView strategySetView = new StrategySetView();
                                StrategySetViewModel strategySetViewModel = new StrategySetViewModel(strategySetView);
                                strategySetViewModel.Text = "Strategy Set Name Here!";
                                strategySetViewModel.ImageName = "../Images/114514.jpeg";
                                strategySetViewModel.DragStarted += OnDragStarted;
                                strategySetViewModel.DragEnded += OnDragEnded;
                                strategySetViewModel.NotifyStrategyPosition += OnNotifyStrategyPosition;
                                strategySetViewModel.NotifyStrategySetPosition += OnNotifyStrategySetPosition;
                                CanvasClicked += strategySetViewModel.OnCanvasClicked;
                                KeyDown += strategySetViewModel.OnKeyDown;
                                DragLineStarted += strategySetViewModel.OnDragLineStarted;
                                DragLineEnded += strategySetViewModel.OnDragLineEnded;
                                strategySetView.DataContext= strategySetViewModel;
                                DiagramItems.Add(strategySetView);
                                DiagramItemViewModels.Add(strategySetViewModel);
                                Canvas.SetLeft(strategySetView, pos.X);
                                Canvas.SetTop(strategySetView, pos.Y);
                                break;
                            }
                    }
                }
                else
                {
                    Console.WriteLine("The item doesn't drop on canvas!");
                }
            }
        }

		private void DiagramViewModel_DragLineEnded()
		{
			throw new NotImplementedException();
		}

		public void OnClick(object? obj)
        {
            Console.WriteLine("Clicked on canvas!");
            CanvasClicked?.Invoke();
        }
        public void OnKeyDown(KeyEventArgs e)
        {
            KeyDown?.Invoke(e);
        }
        void OnMouseLeftButtonUp(object? obj)
        {
            Console.WriteLine("Mouse released, drag interrupted");
            _dragInfo.Clear();
            DragLineEnded?.Invoke();
        }
        void OnDragStarted(StrategySetViewModel strategySetViewModel,
            StrategyViewModel strategyViewModel,
            FrameworkElement dragSource)
        {
            _dragInfo.Dragging = true;
            _dragInfo.StrategySetFrom = strategySetViewModel;
            _dragInfo.StrategyFrom = strategyViewModel;
            _dragInfo.StartPos = GetStartPosition(dragSource);
			Console.WriteLine($"Dragged at ({_dragInfo.StartPos.X}, {_dragInfo.StartPos.Y})");
            DragLineStarted?.Invoke();
        }
        void OnDragEnded(StrategySetViewModel strategySetViewModel,
            StrategySetView strategySetView)
        {
            if (_dragInfo.Dragging)
            {
                _dragInfo.StrategySetTo = strategySetViewModel;
                if (_dragInfo.StrategyFrom!.Line == null 
                    && _dragInfo.StrategySetTo.Line == null
                    && _dragInfo.StrategySetFrom != _dragInfo.StrategySetTo)
                {
                    Point endPos = GetEndPosition(strategySetView);
                    ConnectionLine line = new ConnectionLine(new Line
                    {
                        X1 = _dragInfo.StartPos.X,
                        X2 = endPos.X,
                        Y1 = _dragInfo.StartPos.Y,
                        Y2 = endPos.Y,
                        Stroke = Brushes.Black,
                        StrokeThickness = 5
                    },
                    _dragInfo.StrategyFrom,
                    _dragInfo.StrategySetTo);
                    KeyDown += line.OnKeyDown;
                    CanvasClicked += line.OnDeselect;
                    line.Destroyed += OnConnectionLineDestroyed;
                    DiagramItems.Add(line.Line);
                    _dragInfo.StrategyFrom!.Line = line;
                    _dragInfo.StrategySetTo!.Line = line;
                    line.Destroyed += _dragInfo.StrategyFrom!.OnConnectionLineDestroyed;
                    line.Destroyed += _dragInfo.StrategySetTo!.OnConnectionLineDestroyed;
                    _dragInfo.StrategyFrom.StrategySetConnectedTo = _dragInfo.StrategySetTo;
                    _dragInfo.Clear();
                }
                else if (_dragInfo.StrategySetFrom == _dragInfo.StrategySetTo)
                {
                    Console.WriteLine("不能连接自己！");
                }
                else
                {
                    Console.WriteLine("你已经连接过一条线了！");
                }
            }
            DragLineEnded?.Invoke();
		}
        Point GetStartPosition(FrameworkElement element)
        {
            Point pos = element.TranslatePoint(new Point(0, 0), Canvas);
            pos.X += element.Width / 2;
            pos.Y += element.Height / 2;
            return pos;
        }
        Point GetEndPosition(StrategySetView strategySetView)
        {
            Point pos = strategySetView.TranslatePoint(new Point(0, 0), Canvas);
            pos.Y += 10;
            return pos;
        }
        void OnConnectionLineDestroyed(ConnectionLine line)
        {
            DiagramItems.Remove(line.Line);
        }
        void OnNotifyStrategyPosition(StrategySetViewModel strategySetViewModel,
            StrategyViewModel strategyViewModel,
            FrameworkElement element)
        {
            if (strategyViewModel.Line != null)
            {
                strategyViewModel.Line!.OnStartPosChanged(GetStartPosition(element));
            }
            else
            {
                Console.WriteLine("Strategy doesn't have a line.");
            }
        }
        
        void OnNotifyStrategySetPosition(StrategySetViewModel strategySetViewModel,
            StrategySetView strategySetView)
        {
            if (strategySetViewModel.Line != null)
            {
                strategySetViewModel.Line!.OnEndPosChanged(GetEndPosition(strategySetView));
            }
            else
            {
				Console.WriteLine("Strategy set doesn't have a line.");
			}
        }
        //Retrieve Canvas from View
        void OnCanvasLoaded(object? obj)
        {
            RoutedEventArgs e = (obj as RoutedEventArgs)!;
            Canvas = e.Source as Canvas;
        }
    }
}
